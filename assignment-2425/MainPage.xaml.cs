using assignment_2425.Services;

namespace assignment_2425
{
    public partial class MainPage : ContentPage
    {
        private readonly MealService _mealService;

        public MainPage()
        {
            Console.WriteLine("📱 MainPage initializing...");
            InitializeComponent();
            _mealService = MealService.Instance;
            Console.WriteLine("🏠 MainPage loaded with meal service");
        }

        protected override void OnAppearing()
        {
            Console.WriteLine("👀 MainPage appearing...");
            base.OnAppearing();
            UpdateMealCount();
            Console.WriteLine("📊 Meal count updated");
        }

        private void UpdateMealCount()
        {
            var todaysMeals = _mealService.GetTodaysMeals().Count();
            lblMealCount.Text = todaysMeals == 1
                ? "1 meal logged today"
                : $"{todaysMeals} meals logged today";
        }

        private async void OnAddMealClicked(object sender, EventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync("//AddMealPage");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Could not navigate to Add Meal page: {ex.Message}", "OK");
            }
        }

        private async void OnViewMealsClicked(object sender, EventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync("//ViewMealsPage");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Could not navigate to View Meals page: {ex.Message}", "OK");
            }
        }

        private async void OnTakePhotoClicked(object sender, EventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync("//CameraPage");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Could not navigate to Camera page: {ex.Message}", "OK");
            }
        }
    }
}
