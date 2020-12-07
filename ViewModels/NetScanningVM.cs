using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Niero.Models;
using NieroNetLib;

namespace Niero.ViewModels
{
    class NetScanningVM : BaseViewModel
    {
        NetInterfaceDataHub DataHub;

        ComboBox GatewayCB;
        TextBox TimeoutTB;
        Button RunButton;

        //CONSTRUCTOR!!!
        public NetScanningVM(Page page, NetInterfaceDataHub dataHub) : base(page)
        {
            //Data connect
            DataHub = dataHub;

            //Elements searching
            GatewayCB = (ComboBox)m_Page.FindName(nameof(GatewayCB));
            TimeoutTB = (TextBox)m_Page.FindName(nameof(TimeoutTB));
            RunButton = (Button)m_Page.FindName(nameof(RunButton));

            //Event subscribing
            RunButton.Click += RunButton_Click;

            //Elemets filling
            CBInit();
        }

        private void CBInit()
        {
            GatewayCB.ItemsSource = DataHub.ActiveGateways;
            GatewayCB.SelectedIndex = 0;
        }

        private void RunButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MessageBox.Show("Подключи!1!");
        }
    }
}
