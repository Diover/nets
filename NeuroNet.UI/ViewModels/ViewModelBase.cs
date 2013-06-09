﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using NeuroNet.UI.Annotations;

namespace NeuroNet.UI.ViewModels
{
    public class ViewModelBase: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if(handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}