using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using GigaIRC.Client.WPF.Annotations;

namespace GigaIRC.Client.WPF.Dialogs
{
    public partial class QuickConnect : INotifyPropertyChanged
    {
        private string _fullName = "GigaIRC User";
        private string _username = "GigaIRC";
        private string _nickname = "GigaIRC";
        private string _altNickname = "GigaIRC2";
        private string _server = "irc.efnet.net";
        private ushort? _port = 6667;
        private ICommand _connectCommand;
        private ICommand _cancelCommand;

        public string FullName
        {
            get { return _fullName; }
            set
            {
                if (value == _fullName) return;
                _fullName = value;
                OnPropertyChanged();
            }
        }

        public string Username
        {
            get { return _username; }
            set
            {
                if (value == _username) return;
                _username = value;
                OnPropertyChanged();
            }
        }

        public string Nickname
        {
            get { return _nickname; }
            set
            {
                if (value == _nickname) return;
                _nickname = value;
                OnPropertyChanged();
            }
        }

        public string AltNickname
        {
            get { return _altNickname; }
            set
            {
                if (value == _altNickname) return;
                _altNickname = value;
                OnPropertyChanged();
            }
        }

        public string Server
        {
            get { return _server; }
            set
            {
                if (value == _server) return;
                _server = value;
                OnPropertyChanged();
            }
        }

        public ushort? Port
        {
            get { return _port; }
            set
            {
                if (value == _port) return;
                _port = value;
                OnPropertyChanged();
            }
        }

        public ICommand ConnectCommand
        {
            get { return _connectCommand; }
            set
            {
                if (Equals(value, _connectCommand)) return;
                _connectCommand = value;
                OnPropertyChanged();
            }
        }

        public ICommand CancelCommand
        {
            get { return _cancelCommand; }
            set
            {
                if (Equals(value, _cancelCommand)) return;
                _cancelCommand = value;
                OnPropertyChanged();
            }
        }

        public QuickConnect()
        {
            _connectCommand = new RelayCommand(_ => { DialogResult = true; Close(); });
            _cancelCommand = new RelayCommand(_ => { DialogResult = false; Close(); });
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
