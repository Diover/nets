using System;

namespace NeuroNet.UI.ViewModels
{
    public abstract class AbstractViewModel : ViewModelBase
    {
        protected readonly Action<ViewModelBase> NextViewModel;

        protected AbstractViewModel(Action<ViewModelBase> nextViewModel)
        {
            NextViewModel = nextViewModel;
        }
    }
}