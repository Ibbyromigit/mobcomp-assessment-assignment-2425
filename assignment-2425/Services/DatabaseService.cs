using SQLite;
using assignment_2425.Models;

namespace assignment_2425.Services
{
    /// <summary>
    /// Service for handling SQLite database operations
    /// </summary>
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;
        private static DatabaseService _instance;
        private static readonly object _lock = new object();

        private DatabaseService()
        {
        }

        /// <summary>
        /// Singleton instance of DatabaseService
        /// </summary>
        public static DatabaseService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new DatabaseService();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Initialize the database connection and create tables
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                if (_database != null)
                    return;

                var databasePath = Path.Combine(FileSystem.AppDataDirectory, "MealTracker.db");
                _database = new SQLiteAsyncConnection(databasePath);

                // Create tables
                await _database.CreateTableAsync<Meal>();

                System.Diagnostics.Debug.WriteLine($"Database initialized at: {databasePath}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing database: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get all meals from the database
        /// </summary>
        /// <returns>List of all meals</returns>
        public async Task<List<Meal>> GetMealsAsync()
        {
            try
            {
                await InitializeAsync();
                return await _database.Table<Meal>().OrderByDescending(m => m.MealTime).ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting meals: {ex.Message}");
                return new List<Meal>();
            }
        }

        /// <summary>
        /// Get meals for today
        /// </summary>
        /// <returns>List of today's meals</returns>
        public async Task<List<Meal>> GetTodaysMealsAsync()
        {
            try
            {
                await InitializeAsync();
                var today = DateTime.Today;
                var tomorrow = today.AddDays(1);
                
                return await _database.Table<Meal>()
                    .Where(m => m.MealTime >= today && m.MealTime < tomorrow)
                    .OrderBy(m => m.MealTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting today's meals: {ex.Message}");
                return new List<Meal>();
            }
        }

        /// <summary>
        /// Get meals by category
        /// </summary>
        /// <param name="category">Category to filter by</param>
        /// <returns>List of meals in the specified category</returns>
        public async Task<List<Meal>> GetMealsByCategoryAsync(string category)
        {
            try
            {
                await InitializeAsync();
                return await _database.Table<Meal>()
                    .Where(m => m.Category == category)
                    .OrderByDescending(m => m.MealTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting meals by category: {ex.Message}");
                return new List<Meal>();
            }
        }

        /// <summary>
        /// Search meals by name or notes
        /// </summary>
        /// <param name="searchTerm">Term to search for</param>
        /// <returns>List of matching meals</returns>
        public async Task<List<Meal>> SearchMealsAsync(string searchTerm)
        {
            try
            {
                await InitializeAsync();
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return await GetMealsAsync();

                var term = searchTerm.ToLower();
                return await _database.Table<Meal>()
                    .Where(m => m.Name.ToLower().Contains(term) || m.Notes.ToLower().Contains(term))
                    .OrderByDescending(m => m.MealTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error searching meals: {ex.Message}");
                return new List<Meal>();
            }
        }

        /// <summary>
        /// Get a meal by its ID
        /// </summary>
        /// <param name="id">Meal ID</param>
        /// <returns>The meal if found, null otherwise</returns>
        public async Task<Meal?> GetMealAsync(string id)
        {
            try
            {
                await InitializeAsync();
                return await _database.Table<Meal>().Where(m => m.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting meal: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Save a meal to the database
        /// </summary>
        /// <param name="meal">Meal to save</param>
        /// <returns>True if successful</returns>
        public async Task<bool> SaveMealAsync(Meal meal)
        {
            try
            {
                await InitializeAsync();
                
                if (meal == null || string.IsNullOrWhiteSpace(meal.Name))
                    return false;

                var existingMeal = await GetMealAsync(meal.Id);
                if (existingMeal != null)
                {
                    // Update existing meal
                    await _database.UpdateAsync(meal);
                }
                else
                {
                    // Insert new meal
                    await _database.InsertAsync(meal);
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving meal: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Delete a meal from the database
        /// </summary>
        /// <param name="mealId">ID of the meal to delete</param>
        /// <returns>True if successful</returns>
        public async Task<bool> DeleteMealAsync(string mealId)
        {
            try
            {
                await InitializeAsync();
                var meal = await GetMealAsync(mealId);
                if (meal != null)
                {
                    await _database.DeleteAsync(meal);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting meal: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get meal count
        /// </summary>
        /// <returns>Total number of meals</returns>
        public async Task<int> GetMealCountAsync()
        {
            try
            {
                await InitializeAsync();
                return await _database.Table<Meal>().CountAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting meal count: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Clear all meals from the database (for testing)
        /// </summary>
        public async Task ClearAllMealsAsync()
        {
            try
            {
                await InitializeAsync();
                await _database.DeleteAllAsync<Meal>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error clearing meals: {ex.Message}");
            }
        }

        /// <summary>
        /// Load sample data for demonstration
        /// </summary>
        public async Task LoadSampleDataAsync()
        {
            try
            {
                var mealCount = await GetMealCountAsync();
                if (mealCount > 0)
                    return; // Don't load sample data if meals already exist

                var sampleMeals = new List<Meal>
                {
                    new Meal
                    {
                        Name = "Healthy Breakfast",
                        Category = "Breakfast",
                        MealTime = DateTime.Today.AddHours(8),
                        Notes = "Oatmeal with fresh berries and honey",
                        Location = "Home"
                    },
                    new Meal
                    {
                        Name = "Office Lunch",
                        Category = "Lunch",
                        MealTime = DateTime.Today.AddHours(12).AddMinutes(30),
                        Notes = "Grilled chicken salad with mixed vegetables",
                        Location = "Office Cafeteria"
                    },
                    new Meal
                    {
                        Name = "Family Dinner",
                        Category = "Dinner",
                        MealTime = DateTime.Today.AddHours(19),
                        Notes = "Pasta with marinara sauce and garlic bread",
                        Location = "Home"
                    }
                };

                foreach (var meal in sampleMeals)
                {
                    await SaveMealAsync(meal);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading sample data: {ex.Message}");
            }
        }
    }
}
