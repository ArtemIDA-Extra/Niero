using System.ComponentModel;
using System.Windows.Controls;

namespace Niero.ViewModels
{
    abstract class BaseViewModel : INotifyPropertyChanged
    {
        public Page m_Page;

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
        public void OnPropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public BaseViewModel()
        {

        }

        public BaseViewModel(Page page)
        {
            m_Page = page;
        }
    }
}
