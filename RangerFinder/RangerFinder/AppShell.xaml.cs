namespace RangerFinder
{
    public partial class AppShell : Shell
    {
        public AppShell(MainPage mainPage)
        {
            InitializeComponent();

            Items.Add(new FlyoutItem
            {
                Title = "Home",
                Items =
                {
                    new ShellContent
                    {
                        Route = nameof(MainPage),
                        Content = mainPage
                    }
                }
            });

            Routing.RegisterRoute(nameof(DetailsPage), typeof(DetailsPage));
            Routing.RegisterRoute(nameof(DebugPage), typeof(DebugPage));
        }
    }
}
