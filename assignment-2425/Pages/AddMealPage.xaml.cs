using assignment_2425.Models;
using assignment_2425.Services;

namespace assignment_2425.Pages
{
    public partial class AddMealPage : ContentPage
    {
        private readonly MealService _mealService;
        private readonly LocationService _locationService;

        public AddMealPage()
        {
            InitializeComponent();
            _mealService = MealService.Instance;
            _locationService = LocationService.Instance;

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
            try
            {
                // Disable button during operation
                btnGetLocation.IsEnabled = false;
                btnGetLocation.Text = "⏳";

                // Check location permission
                bool hasPermission = await _locationService.CheckLocationPermissionAsync();
                if (!hasPermission)
                {
                    await DisplayAlert("Permission Required", "Location permission is required to get your current location.", "OK");
                    return;
                }

                // Check if location services are available
                bool isAvailable = await _locationService.IsLocationAvailableAsync();
                if (!isAvailable)
                {
                    await DisplayAlert("Location Unavailable", "Location services are not available on this device or are disabled.", "OK");
                    return;
                }

                // Get current location with address
                var locationInfo = await _locationService.GetCurrentLocationWithAddressAsync();

                if (locationInfo != null)
                {
                    // Update the location field with formatted address
                    entryLocation.Text = locationInfo.FormattedAddress;

                    await DisplayAlert("Location Found",
                        $"Location captured successfully!\n\n" +
                        $"Address: {locationInfo.FormattedAddress}\n" +
                        $"Coordinates: {locationInfo.CoordinatesText}\n" +
                        $"Accuracy: {locationInfo.Accuracy:F0}m", "OK");
                }
                else
                {
                    await DisplayAlert("Location Error", "Could not determine your current location. Please try again or enter location manually.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to get location: {ex.Message}", "OK");
            }
            finally
            {
                // Re-enable button
                btnGetLocation.IsEnabled = true;
                btnGetLocation.Text = "📍";
            }
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
