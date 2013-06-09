using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Input;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace NeuroNet.UI.ViewModels
{
    public class StartingViewModel : AbstractViewModel
    {
        private ICommand _learnNetCommand;
        private ICommand _loadNetCommand;

        public StartingViewModel(Action<ViewModelBase> nextViewModel)
            : base(nextViewModel)
        {
        }

        public ICommand LearnNetCommand
        {
            get { return _learnNetCommand ?? (_learnNetCommand = new Command(LearnNet)); }
        }

        public ICommand LoadNetCommand
        {
            get { return _loadNetCommand ?? (_loadNetCommand = new Command(Loadet)); }
        }

        private void Loadet()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Assembly.GetExecutingAssembly().Location;
            openFileDialog.Filter = "net binary files (*.net)|*.net|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            if(openFileDialog.ShowDialog() == true)
            {
                var filename = openFileDialog.FileName;
                if(File.Exists(filename))
                    NextViewModel(new PropagationViewModel(NextViewModel, filename));
                else
                    MessageBox.Show(@"Cannot open file");
            }
        }

        private void LearnNet()
        {
            
        }
    }
}