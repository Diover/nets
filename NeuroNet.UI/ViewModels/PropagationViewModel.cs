using System;
using System.Windows.Input;
using NeuroNet.Model.Net;

namespace NeuroNet.UI.ViewModels
{
    public class PropagationViewModel: AbstractViewModel
    {
        private INet _net;
        private ICommand _propagateSignalCommand;
        private string _lastOutputSignal;

        public PropagationViewModel(Action<ViewModelBase> nextViewModel, string filename)
            : base(nextViewModel)
        {
            _net = BinaryFileSerializer.LoadNetState(filename);
        }

        public ICommand PropagateSignalCommand
        {
            get { return _propagateSignalCommand ?? (_propagateSignalCommand = new Command(PropagateSignal)); }
        }

        public string LastOutputSignal
        {
            get { return _lastOutputSignal; }
            set
            {
                _lastOutputSignal = value;
                OnPropertyChanged();
            }
        }

        private void PropagateSignal()
        {
            //_net.Propagate();
        }
    }
}