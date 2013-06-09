using System;

namespace NeuroNet.UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private Action<ViewModelBase> _nextViewModel;

        private ViewModelBase _currentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get { return _currentViewModel; }
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            _nextViewModel = (next) => { CurrentViewModel = next; };
            CurrentViewModel = new StartingViewModel(_nextViewModel);
        }
    }
}