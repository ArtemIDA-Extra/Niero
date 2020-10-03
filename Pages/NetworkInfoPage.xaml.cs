using System.Windows.Controls;
using Niero.ViewModels;

namespace Niero.Pages
{
    public partial class NetworkInfoPage : Page
    {
        public NetworkInfoPage()
        {
            InitializeComponent();
            DataContext = new NetInfoVM(this);
        }
    }
}
