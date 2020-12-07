using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Niero.ViewModels;
using Niero.Models;

namespace Niero.Pages
{
    public partial class NetScanningMainPage : Page
    {
        public NetScanningMainPage(NetInterfaceDataHub dataHub)
        {
            InitializeComponent();
            DataContext = new NetScanningVM(this, dataHub);
            this.Loaded += NetScanningMainPage_Loaded;
        }

        private void NetScanningMainPage_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation OpenAnim = new DoubleAnimation(1, new Duration(new TimeSpan(0, 0, 0, 0, 750)));
            this.BeginAnimation(Page.OpacityProperty, OpenAnim);
        }
    }
}
