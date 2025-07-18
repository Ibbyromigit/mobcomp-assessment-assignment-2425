namespace assignment_2425
{
    public partial class App : Application
    {
        public App()
        {
            Console.WriteLine("🚀 Simple Meal Tracker App Starting...");
            InitializeComponent();

            MainPage = new AppShell();
            Console.WriteLine("✅ App initialized successfully!");
        }
    }
}
