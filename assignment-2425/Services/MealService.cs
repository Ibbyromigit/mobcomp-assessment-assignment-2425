using assignment_2425.Models;
using System.Collections.ObjectModel;

namespace assignment_2425.Services
{
    /// <summary>
    /// Service for managing meal data operations
    /// </summary>
    public class MealService
    {
        private readonly ObservableCollection<Meal> _meals;
        private readonly DatabaseService _databaseService;
        private static MealService _instance;
        private static readonly object _lock = new object();

        private MealService()
        {
            _meals = new ObservableCollection<Meal>();
            _databaseService = DatabaseService.Instance;
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                await _databaseService.InitializeAsync();
                await LoadMealsFromDatabase();
                await _databaseService.LoadSampleDataAsync();
                await LoadMealsFromDatabase(); // Reload after sample data
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing MealService: {ex.Message}");
            }
        }

        /// <summary>
        /// Singleton instance of MealService
        /// </summary>
        public static MealService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new MealService();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Observable collection of all meals
        /// </summary>
        public ObservableCollection<Meal> Meals => _meals;

        /// <summary>
        /// Add a new meal to the collection
        /// </summary>
        /// <param name="meal">The meal to add</param>
        /// <returns>True if successful, false otherwise</returns>
        public async Task<bool> AddMealAsync(Meal meal)
        {
            try
            {
                if (meal == null)
                    return false;

                if (string.IsNullOrWhiteSpace(meal.Name))
                    return false;

                bool success = await _databaseService.SaveMealAsync(meal);
                if (success)
                {
                    _meals.Add(meal);
                }
                return success;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding meal: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Add a new meal to the collection (synchronous wrapper for backward compatibility)
        /// </summary>
        /// <param name="meal">The meal to add</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool AddMeal(Meal meal)
        {
            return AddMealAsync(meal).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Remove a meal from the collection
        /// </summary>
        /// <param name="mealId">ID of the meal to remove</param>
        /// <returns>True if successful, false otherwise</returns>
        public async Task<bool> RemoveMealAsync(string mealId)
        {
            try
            {
                bool success = await _databaseService.DeleteMealAsync(mealId);
                if (success)
                {
                    var meal = _meals.FirstOrDefault(m => m.Id == mealId);
                    if (meal != null)
                    {
                        _meals.Remove(meal);
                    }
                }
                return success;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error removing meal: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Remove a meal from the collection (synchronous wrapper for backward compatibility)
        /// </summary>
        /// <param name="mealId">ID of the meal to remove</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool RemoveMeal(string mealId)
        {
            return RemoveMealAsync(mealId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Get a meal by its ID
        /// </summary>
        /// <param name="mealId">ID of the meal to retrieve</param>
        /// <returns>The meal if found, null otherwise</returns>
        public Meal GetMeal(string mealId)
        {
            return _meals.FirstOrDefault(m => m.Id == mealId);
        }

        /// <summary>
        /// Update an existing meal
        /// </summary>
        /// <param name="meal">The updated meal</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool UpdateMeal(Meal meal)
        {
            try
            {
                var existingMeal = GetMeal(meal.Id);
                if (existingMeal != null)
                {
                    existingMeal.Name = meal.Name;
                    existingMeal.MealTime = meal.MealTime;
                    existingMeal.Notes = meal.Notes;
                    existingMeal.ImagePath = meal.ImagePath;
                    existingMeal.Location = meal.Location;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating meal: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get meals for today
        /// </summary>
        /// <returns>Collection of today's meals</returns>
        public IEnumerable<Meal> GetTodaysMeals()
        {
            var today = DateTime.Today;
            return _meals.Where(m => m.MealTime.Date == today).OrderBy(m => m.MealTime);
        }

        /// <summary>
        /// Get meal count
        /// </summary>
        /// <returns>Total number of meals</returns>
        public int GetMealCount()
        {
            return _meals.Count;
        }

        /// <summary>
        /// Clear all meals (for testing purposes)
        /// </summary>
        public void ClearAllMeals()
        {
            _meals.Clear();
        }

        /// <summary>
        /// Load meals from database into the observable collection
        /// </summary>
        private async Task LoadMealsFromDatabase()
        {
            try
            {
                var meals = await _databaseService.GetMealsAsync();
                _meals.Clear();
                foreach (var meal in meals)
                {
                    _meals.Add(meal);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading meals from database: {ex.Message}");
            }
        }

        /// <summary>
        /// Refresh meals from database
        /// </summary>
        public async Task RefreshMealsAsync()
        {
            await LoadMealsFromDatabase();
        }

        /// <summary>
        /// Get meals by category
        /// </summary>
        /// <param name="category">Category to filter by</param>
        /// <returns>Filtered meals</returns>
        public async Task<IEnumerable<Meal>> GetMealsByCategoryAsync(string category)
        {
            return await _databaseService.GetMealsByCategoryAsync(category);
        }

        /// <summary>
        /// Search meals by name or notes
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>Matching meals</returns>
        public async Task<IEnumerable<Meal>> SearchMealsAsync(string searchTerm)
        {
            return await _databaseService.SearchMealsAsync(searchTerm);
        }
    }
}
