using System;
using System.Windows.Input;

namespace NeuroNet.UI.ViewModels
{
    public class Command: ICommand
    {
        private readonly Action _command;

        public Command(Action command)
        {
            _command = command;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _command();
        }

        public event EventHandler CanExecuteChanged;
    }
}