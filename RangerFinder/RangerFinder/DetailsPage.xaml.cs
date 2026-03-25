using RangerFinder.ViewModels;

namespace RangerFinder
{
    public partial class DetailsPage : ContentPage
    {
        public DetailsPage(MainViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
