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
        private static MealService _instance;
        private static readonly object _lock = new object();

        private MealService()
        {
            _meals = new ObservableCollection<Meal>();
            LoadSampleData();
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
        public bool AddMeal(Meal meal)
        {
            try
            {
                if (meal == null)
                    return false;

                if (string.IsNullOrWhiteSpace(meal.Name))
                    return false;

                _meals.Add(meal);
                return true;
            }
            catch (Exception ex)
            {
                // In a real app, you would log this error
                System.Diagnostics.Debug.WriteLine($"Error adding meal: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Remove a meal from the collection
        /// </summary>
        /// <param name="mealId">ID of the meal to remove</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool RemoveMeal(string mealId)
        {
            try
            {
                var meal = _meals.FirstOrDefault(m => m.Id == mealId);
                if (meal != null)
                {
                    _meals.Remove(meal);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error removing meal: {ex.Message}");
                return false;
            }
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
        /// Load some sample data for demonstration
        /// </summary>
        private void LoadSampleData()
        {
            var sampleMeals = new List<Meal>
            {
                new Meal
                {
                    Name = "Breakfast",
                    MealTime = DateTime.Today.AddHours(8),
                    Notes = "Oatmeal with fruits",
                    Location = "Home"
                },
                new Meal
                {
                    Name = "Lunch",
                    MealTime = DateTime.Today.AddHours(12).AddMinutes(30),
                    Notes = "Grilled chicken salad",
                    Location = "Office"
                }
            };

            foreach (var meal in sampleMeals)
            {
                _meals.Add(meal);
            }
        }
    }
}
