using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Niero.Pages
{
    /// <summary>
    /// Логика взаимодействия для NetScanningPage.xaml
    /// </summary>
    public partial class NetScanningMainPage : Page
    {
        public NetScanningMainPage()
        {
            InitializeComponent();

            CommandBinding CB = new CommandBinding(NavigationCommands.NextPage);
            CB.Executed += nextPageCommand_Executed;
            this.CommandBindings.Add(CB);
        }

        private void nextPageCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Hi!");
        }
    }
}
