using RangerFinder.ViewModels;

namespace RangerFinder
{
    public partial class DebugPage : ContentPage
    {
        public DebugPage(MainViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
