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

        protected override async void OnAppearing()
        {
            Console.WriteLine("👀 MainPage appearing...");
            base.OnAppearing();
            await UpdateMealCountAsync();
            Console.WriteLine("📊 Meal count updated");
        }

        private async Task UpdateMealCountAsync()
        {
            try
            {
                await _mealService.RefreshMealsAsync();
                var todaysMeals = _mealService.GetTodaysMeals().Count();
                lblMealCount.Text = todaysMeals == 1
                    ? "1 meal logged today"
                    : $"{todaysMeals} meals logged today";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating meal count: {ex.Message}");
                lblMealCount.Text = "Error loading meal count";
            }
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
