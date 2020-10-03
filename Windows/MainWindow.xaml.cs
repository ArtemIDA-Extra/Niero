using System.Windows;
using System.Windows.Controls;
using Niero.ViewModels;

namespace Niero.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowVM(this);
        }
    }
}
