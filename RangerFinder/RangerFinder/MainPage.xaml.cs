using RangerFinder.ViewModels;

namespace RangerFinder
{
    public partial class MainPage : ContentPage
    {
        private SensorChartDrawable _chartDrawable;
        private MainViewModel _viewModel;

        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            
            _viewModel = viewModel;
            BindingContext = _viewModel;

            _chartDrawable = new SensorChartDrawable();
            _chartDrawable.History = _viewModel.SensorHistory;
            ChartGraphicsView.Drawable = _chartDrawable;

            _viewModel.PropertyChanged += (s, e) => 
            {
                if (e.PropertyName == nameof(MainViewModel.CurrentData))
                {
                    ChartGraphicsView.Invalidate();
                }
            };
        }
    }
}
