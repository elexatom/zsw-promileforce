namespace RangerFinder
{
    public partial class App : Application
    {
        public App(IServiceProvider services)
        {
            InitializeComponent();
            MainPage = services.GetService<AppShell>();
        }
    }
}
