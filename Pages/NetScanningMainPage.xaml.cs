using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

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
            this.Loaded += NetScanningMainPage_Loaded;
        }

        private void NetScanningMainPage_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation OpenAnim = new DoubleAnimation(1, new Duration(new TimeSpan(0, 0, 0, 0, 750)));
            this.BeginAnimation(Page.OpacityProperty, OpenAnim);
        }

        private void nextPageCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Hi!");
        }
    }
}
