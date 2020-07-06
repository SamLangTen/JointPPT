using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JointPPT
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private ObservableCollection<string> _AddedFilenames;
        public ObservableCollection<string> AddedFilenames
        {
            get => _AddedFilenames;
            set
            {
                _AddedFilenames = value;
                Notify("AddedFilenames");
            }
        }

        private List<ErrorInfo> _Logs;
        public List<ErrorInfo> Logs
        {
            get => _Logs;
            set
            {
                _Logs = value;
                Notify("Logs");
            }
        }

        private bool _UsedWideScreen;
        public bool UsedWideScreen
        {
            get => _UsedWideScreen;
            set
            {
                _UsedWideScreen = value;
                Notify("UsedWideScreen");
            }
        }

    }
}
