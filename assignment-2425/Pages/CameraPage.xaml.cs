
using assignment_2425.Services;

namespace assignment_2425.Pages
{
    public partial class CameraPage : ContentPage
    {
        private string? _currentPhotoPath;
        private readonly CameraService _cameraService;
        private PhotoInfo? _currentPhotoInfo;

        public CameraPage()
        {
            InitializeComponent();
            _cameraService = CameraService.Instance;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ResetCameraView();
        }

        private async void OnTakePhotoClicked(object sender, EventArgs e)
        {
            try
            {
                // Disable button during operation
                btnTakePhoto.IsEnabled = false;
                btnTakePhoto.Text = "📷 Taking...";

                // Check camera permission
                bool hasPermission = await _cameraService.CheckCameraPermissionAsync();
                if (!hasPermission)
                {
                    await DisplayAlert("Permission Required", "Camera permission is required to take photos.", "OK");
                    return;
                }

                // Take photo
                _currentPhotoPath = await _cameraService.TakePhotoAsync();

                if (!string.IsNullOrEmpty(_currentPhotoPath))
                {
                    // Display the captured photo
                    await DisplayCapturedPhoto(_currentPhotoPath);
                    await DisplayAlert("Success", "Photo captured successfully!", "OK");
                }
                else
                {
                    await DisplayAlert("Cancelled", "Photo capture was cancelled.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to take photo: {ex.Message}", "OK");
            }
            finally
            {
                // Re-enable button
                btnTakePhoto.IsEnabled = true;
                btnTakePhoto.Text = "📷 Take Photo";
            }
        }

        private async void OnSelectPhotoClicked(object sender, EventArgs e)
        {
            try
            {
                // Disable button during operation
                btnSelectPhoto.IsEnabled = false;
                btnSelectPhoto.Text = "🖼️ Selecting...";

                // Check storage permission
                bool hasPermission = await _cameraService.CheckStoragePermissionAsync();
                if (!hasPermission)
                {
                    await DisplayAlert("Permission Required", "Storage permission is required to access photos.", "OK");
                    return;
                }

                // Pick photo from gallery
                _currentPhotoPath = await _cameraService.PickPhotoAsync();

                if (!string.IsNullOrEmpty(_currentPhotoPath))
                {
                    // Display the selected photo
                    await DisplayCapturedPhoto(_currentPhotoPath);
                    await DisplayAlert("Success", "Photo selected successfully!", "OK");
                }
                else
                {
                    await DisplayAlert("Cancelled", "Photo selection was cancelled.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to select photo: {ex.Message}", "OK");
            }
            finally
            {
                // Re-enable button
                btnSelectPhoto.IsEnabled = true;
                btnSelectPhoto.Text = "🖼️ Gallery";
            }
        }

        private async void OnSavePhotoClicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_currentPhotoPath))
                {
                    await DisplayAlert("Error", "No photo to save.", "OK");
                    return;
                }

                // In a real app, you might want to associate this photo with a meal
                // For now, we'll just confirm it's saved
                await DisplayAlert("Success", "Photo saved successfully! You can now use this photo when adding a meal.", "OK");

                // Navigate to Add Meal page to use the photo
                bool goToAddMeal = await DisplayAlert("Add Meal", "Would you like to add a meal with this photo?", "Yes", "No");
                if (goToAddMeal)
                {
                    await Shell.Current.GoToAsync("//AddMealPage");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to save photo: {ex.Message}", "OK");
            }
        }

        private void OnRetakePhotoClicked(object sender, EventArgs e)
        {
            ResetCameraView();
        }

        private async void OnDeletePhotoClicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_currentPhotoPath))
                {
                    await DisplayAlert("Error", "No photo to delete.", "OK");
                    return;
                }

                bool confirm = await DisplayAlert("Delete Photo", "Are you sure you want to delete this photo?", "Yes", "No");
                if (confirm)
                {
                    bool deleted = _cameraService.DeletePhoto(_currentPhotoPath);
                    if (deleted)
                    {
                        ResetCameraView();
                        await DisplayAlert("Success", "Photo deleted successfully.", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Error", "Failed to delete photo.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to delete photo: {ex.Message}", "OK");
            }
        }

        private async void OnBackToHomeClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

        private void ResetCameraView()
        {
            _currentPhotoPath = null;
            _currentPhotoInfo = null;
            imgCapturedPhoto.Source = null;

            frameCameraPlaceholder.IsVisible = true;
            framePhotoDisplay.IsVisible = false;
            layoutPhotoActions.IsVisible = false;
            layoutPhotoInfo.IsVisible = false;
        }

        private async Task DisplayCapturedPhoto(string photoPath)
        {
            try
            {
                // Get photo info
                _currentPhotoInfo = _cameraService.GetPhotoInfo(photoPath);

                // Display the photo
                imgCapturedPhoto.Source = ImageSource.FromFile(photoPath);

                // Update UI
                frameCameraPlaceholder.IsVisible = false;
                framePhotoDisplay.IsVisible = true;
                layoutPhotoActions.IsVisible = true;
                layoutPhotoInfo.IsVisible = true;

                // Update photo info labels
                if (_currentPhotoInfo != null)
                {
                    lblPhotoPath.Text = $"Path: {_currentPhotoInfo.FileName}";
                    lblPhotoSize.Text = $"Size: {_currentPhotoInfo.FileSizeFormatted}";
                    lblPhotoTimestamp.Text = $"Taken: {_currentPhotoInfo.DateTakenFormatted}";
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to display photo: {ex.Message}", "OK");
            }
        }
    }
}
