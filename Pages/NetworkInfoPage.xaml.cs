﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Niero.ViewModels;
using Niero.Models;

namespace Niero.Pages
{
    public partial class NetworkInfoPage : Page
    {
        public NetworkInfoPage(NetInterfaceDataHub dataHub)
        {
            InitializeComponent();
            DataContext = new NetInfoVM(this, dataHub);
            this.Loaded += NetworkInfoPage_Loaded;
        }

        private void NetworkInfoPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            DoubleAnimation OpenAnim = new DoubleAnimation(1, new Duration(new TimeSpan(0, 0, 0, 0, 750)));
            this.BeginAnimation(Page.OpacityProperty, OpenAnim);
        }
    }
}
