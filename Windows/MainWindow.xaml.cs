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
        MainWindowVM mainWindowVM;

        public MainWindow()
        {
            InitializeComponent();
            mainWindowVM = new MainWindowVM(this);
            DataContext = mainWindowVM;
        }

        //Временно-постоянное решение
        //Как будет время запили кастом команду (и ДВЕРЬ МНЕ ЗАПИЛИ!!! =) ) 
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindowVM.ChangeMenuSelection((sender as Button).Content.ToString());
        }
    }
}
