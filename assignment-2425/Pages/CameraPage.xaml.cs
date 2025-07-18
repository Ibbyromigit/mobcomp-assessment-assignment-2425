
namespace assignment_2425.Pages
{
    public partial class CameraPage : ContentPage
    {
        private string? _currentPhotoPath;

        public CameraPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ResetCameraView();
        }

        private async void OnTakePhotoClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Camera Demo", "Camera functionality is available! In the full version, this would open the device camera to take a photo of your meal.", "OK");

            // Simulate taking a photo
            frameCameraPlaceholder.IsVisible = false;
            framePhotoDisplay.IsVisible = true;
            layoutPhotoActions.IsVisible = true;
            layoutPhotoInfo.IsVisible = true;

            lblPhotoPath.Text = "Path: demo_meal_photo.jpg";
            lblPhotoSize.Text = "Size: 2.5 MB";
            lblPhotoTimestamp.Text = $"Taken: {DateTime.Now:MMM dd, yyyy HH:mm}";
        }

        private async void OnSelectPhotoClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Gallery Demo", "Gallery functionality is available! In the full version, this would open your photo gallery to select an existing meal photo.", "OK");
        }

        private async void OnSavePhotoClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Save Demo", "Photo saved! In the full version, this would save the photo and associate it with a meal record.", "OK");
        }

        private void OnRetakePhotoClicked(object sender, EventArgs e)
        {
            ResetCameraView();
        }

        private async void OnDeletePhotoClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Delete Photo", "Delete this demo photo?", "Yes", "No");
            if (confirm)
            {
                ResetCameraView();
                await DisplayAlert("Success", "Photo deleted.", "OK");
            }
        }

        private async void OnBackToHomeClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

        private void ResetCameraView()
        {
            _currentPhotoPath = null;
            imgCapturedPhoto.Source = null;
            
            frameCameraPlaceholder.IsVisible = true;
            framePhotoDisplay.IsVisible = false;
            layoutPhotoActions.IsVisible = false;
            layoutPhotoInfo.IsVisible = false;
        }
    }
}
