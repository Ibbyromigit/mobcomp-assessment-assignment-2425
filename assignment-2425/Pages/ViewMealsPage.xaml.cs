using assignment_2425.Models;
using assignment_2425.Services;

namespace assignment_2425.Pages
{
    public partial class ViewMealsPage : ContentPage
    {
        private readonly MealService _mealService;
        private List<Meal> _allMeals = new List<Meal>();
        private string _currentSearchTerm = string.Empty;
        private string _currentCategoryFilter = "All";

        public ViewMealsPage()
        {
            InitializeComponent();
            _mealService = MealService.Instance;
            pickerCategoryFilter.SelectedItem = "All";
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadMealsAsync();
        }

        private async Task LoadMealsAsync()
        {
            try
            {
                // Refresh meals from database
                await _mealService.RefreshMealsAsync();
                _allMeals = _mealService.Meals.ToList();

                // Apply current filters
                await ApplyFiltersAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load meals: {ex.Message}", "OK");
            }
        }

        private async Task ApplyFiltersAsync()
        {
            try
            {
                List<Meal> filteredMeals;

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(_currentSearchTerm))
                {
                    var searchResults = await _mealService.SearchMealsAsync(_currentSearchTerm);
                    filteredMeals = searchResults.ToList();
                }
                else
                {
                    filteredMeals = _allMeals.ToList();
                }

                // Apply category filter
                if (_currentCategoryFilter != "All")
                {
                    filteredMeals = filteredMeals.Where(m => m.Category == _currentCategoryFilter).ToList();
                }

                // Sort by meal time (newest first)
                filteredMeals = filteredMeals.OrderByDescending(m => m.MealTime).ToList();

                // Update UI
                DisplayMeals(filteredMeals);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to apply filters: {ex.Message}", "OK");
            }
        }

        private void DisplayMeals(List<Meal> meals)
        {
            try
            {
                // Clear existing meal views
                var mealsToRemove = stackMealsList.Children
                    .Where(child => child is Frame)
                    .ToList();

                foreach (var child in mealsToRemove)
                {
                    stackMealsList.Children.Remove(child);
                }

                // Update summary
                var totalMeals = _allMeals.Count;
                var filteredCount = meals.Count;

                if (totalMeals == filteredCount)
                {
                    lblMealSummary.Text = totalMeals == 1 ? "1 meal logged" : $"{totalMeals} meals logged";
                }
                else
                {
                    lblMealSummary.Text = $"Showing {filteredCount} of {totalMeals} meals";
                }

                // Show/hide empty state
                emptyStateLayout.IsVisible = meals.Count == 0;

                // Add meal cards
                foreach (var meal in meals)
                {
                    var mealCard = CreateMealCard(meal);
                    stackMealsList.Children.Add(mealCard);
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"Failed to display meals: {ex.Message}", "OK");
            }
        }

        private Frame CreateMealCard(Meal meal)
        {
            var frame = new Frame
            {
                BackgroundColor = Colors.White,
                BorderColor = Color.FromArgb("#E0E0E0"),
                CornerRadius = 10,
                Padding = 15,
                Margin = new Thickness(0, 5),
                HasShadow = true
            };

            var mainLayout = new VerticalStackLayout
            {
                Spacing = 8
            };

            // Header with meal name and time
            var headerLayout = new HorizontalStackLayout
            {
                Spacing = 10
            };

            var mealNameLabel = new Label
            {
                Text = $"{GetCategoryEmoji(meal.Category)} {meal.Name}",
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#2E7D32"),
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
            };

            var timeLabel = new Label
            {
                Text = meal.MealTimeFormatted,
                FontSize = 14,
                TextColor = Color.FromArgb("#666666"),
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center
            };

            headerLayout.Children.Add(mealNameLabel);
            headerLayout.Children.Add(timeLabel);

            mainLayout.Children.Add(headerLayout);

            // Notes (if available)
            if (!string.IsNullOrWhiteSpace(meal.Notes))
            {
                var notesLabel = new Label
                {
                    Text = $"📝 {meal.Notes}",
                    FontSize = 14,
                    TextColor = Color.FromArgb("#555555"),
                    LineBreakMode = LineBreakMode.WordWrap
                };
                mainLayout.Children.Add(notesLabel);
            }

            // Location (if available)
            if (!string.IsNullOrWhiteSpace(meal.Location))
            {
                var locationLabel = new Label
                {
                    Text = $"📍 {meal.Location}",
                    FontSize = 14,
                    TextColor = Color.FromArgb("#555555")
                };
                mainLayout.Children.Add(locationLabel);
            }

            // Image indicator (if available)
            if (meal.HasImage)
            {
                var imageLabel = new Label
                {
                    Text = "📷 Photo attached",
                    FontSize = 12,
                    TextColor = Color.FromArgb("#1976D2"),
                    FontAttributes = FontAttributes.Italic
                };
                mainLayout.Children.Add(imageLabel);
            }

            // Action buttons
            var buttonLayout = new HorizontalStackLayout
            {
                Spacing = 10,
                Margin = new Thickness(0, 10, 0, 0)
            };

            var editButton = new Button
            {
                Text = "✏️ Edit",
                FontSize = 12,
                BackgroundColor = Color.FromArgb("#FFC107"),
                TextColor = Colors.White,
                CornerRadius = 5,
                Padding = new Thickness(10, 5),
                HorizontalOptions = LayoutOptions.Start
            };
            editButton.Clicked += (s, e) => OnEditMealClicked(meal);

            var deleteButton = new Button
            {
                Text = "🗑️ Delete",
                FontSize = 12,
                BackgroundColor = Color.FromArgb("#F44336"),
                TextColor = Colors.White,
                CornerRadius = 5,
                Padding = new Thickness(10, 5),
                HorizontalOptions = LayoutOptions.Start
            };
            deleteButton.Clicked += (s, e) => OnDeleteMealClicked(meal);

            buttonLayout.Children.Add(editButton);
            buttonLayout.Children.Add(deleteButton);

            mainLayout.Children.Add(buttonLayout);

            frame.Content = mainLayout;

            return frame;
        }

        private async void OnEditMealClicked(Meal meal)
        {
            await DisplayAlert("Edit Meal", $"Edit functionality for '{meal.Name}' would be implemented here.", "OK");
        }

        private async void OnDeleteMealClicked(Meal meal)
        {
            try
            {
                bool confirm = await DisplayAlert("Delete Meal", 
                    $"Are you sure you want to delete '{meal.Name}'?", 
                    "Yes", "No");

                if (confirm)
                {
                    bool success = await _mealService.RemoveMealAsync(meal.Id);
                    if (success)
                    {
                        await LoadMealsAsync(); // Refresh the list
                        await DisplayAlert("Success", "Meal deleted successfully.", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Error", "Failed to delete meal.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private async void OnRefreshClicked(object sender, EventArgs e)
        {
            await LoadMealsAsync();
            await DisplayAlert("Refreshed", "Meals list has been refreshed.", "OK");
        }

        private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            _currentSearchTerm = e.NewTextValue ?? string.Empty;
            await ApplyFiltersAsync();
        }

        private async void OnCategoryFilterChanged(object sender, EventArgs e)
        {
            _currentCategoryFilter = pickerCategoryFilter.SelectedItem?.ToString() ?? "All";
            await ApplyFiltersAsync();
        }

        private async void OnAddNewMealClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//AddMealPage");
        }

        private async void OnAddFirstMealClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//AddMealPage");
        }

        private string GetCategoryEmoji(string category)
        {
            return category switch
            {
                "Breakfast" => "🌅",
                "Lunch" => "🌞",
                "Dinner" => "🌙",
                "Snack" => "🍿",
                _ => "🍽️"
            };
        }
    }
}
