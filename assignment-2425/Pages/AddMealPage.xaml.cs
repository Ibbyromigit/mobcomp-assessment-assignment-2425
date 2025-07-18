using assignment_2425.Models;
using assignment_2425.Services;

namespace assignment_2425.Pages
{
    public partial class AddMealPage : ContentPage
    {
        private readonly MealService _mealService;

        public AddMealPage()
        {
            InitializeComponent();
            _mealService = MealService.Instance;
            
            // Set default values
            datePicker.Date = DateTime.Today;
            timePicker.Time = DateTime.Now.TimeOfDay;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ClearForm();
        }

        private async void OnSaveMealClicked(object sender, EventArgs e)
        {
            try
            {
                // Validate form
                if (!ValidateForm())
                {
                    return;
                }

                // Create new meal
                var meal = new Meal
                {
                    Name = entryMealName.Text.Trim(),
                    MealTime = datePicker.Date.Add(timePicker.Time),
                    Notes = editorNotes.Text?.Trim() ?? string.Empty,
                    Location = entryLocation.Text?.Trim() ?? string.Empty
                };

                // Save meal
                bool success = _mealService.AddMeal(meal);

                if (success)
                {
                    // Show success message
                    frameSuccessMessage.IsVisible = true;
                    
                    // Clear form
                    ClearForm();
                    
                    // Hide success message after 2 seconds
                    await Task.Delay(2000);
                    frameSuccessMessage.IsVisible = false;
                    
                    // Navigate back to home
                    await Shell.Current.GoToAsync("//MainPage");
                }
                else
                {
                    await DisplayAlert("Error", "Failed to save meal. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred while saving the meal: {ex.Message}", "OK");
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            try
            {
                bool confirm = await DisplayAlert("Cancel", "Are you sure you want to cancel? Any unsaved changes will be lost.", "Yes", "No");
                if (confirm)
                {
                    ClearForm();
                    await Shell.Current.GoToAsync("//MainPage");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private async void OnGetLocationClicked(object sender, EventArgs e)
        {
            btnGetLocation.IsEnabled = false;
            btnGetLocation.Text = "⏳";

            // Simulate getting location
            await Task.Delay(1000);

            entryLocation.Text = "Demo Location: University Campus";
            await DisplayAlert("Location Demo", "Location functionality is available! In the full version, this would get your actual GPS coordinates.", "OK");

            btnGetLocation.IsEnabled = true;
            btnGetLocation.Text = "📍";
        }

        private bool ValidateForm()
        {
            bool isValid = true;

            // Reset error states
            lblMealNameError.IsVisible = false;

            // Validate meal name
            if (string.IsNullOrWhiteSpace(entryMealName.Text))
            {
                lblMealNameError.IsVisible = true;
                isValid = false;
            }

            // Additional validation can be added here
            if (!isValid)
            {
                DisplayAlert("Validation Error", "Please correct the errors and try again.", "OK");
            }

            return isValid;
        }

        private void ClearForm()
        {
            entryMealName.Text = string.Empty;
            datePicker.Date = DateTime.Today;
            timePicker.Time = DateTime.Now.TimeOfDay;
            editorNotes.Text = string.Empty;
            entryLocation.Text = string.Empty;
            
            // Reset error states
            lblMealNameError.IsVisible = false;
            frameSuccessMessage.IsVisible = false;
        }
    }
}
