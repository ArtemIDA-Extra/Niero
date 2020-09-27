using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using NieroNetLib;

namespace Niero.ViewModels
{
    class NetInfoVM : BaseViewModel
    {
        Page m_Page;
        
        TextBlock DeviceNameOutput, IPv4Output;

        List<(NetworkInterfaceType intType, string intName)> AviableToSelectInterfaces = new List<(NetworkInterfaceType, string)>
        {
            (NetworkInterfaceType.Ethernet, "Ethernet"),
            (NetworkInterfaceType.Wireless80211, "Wi-Fi"),
        };
        List<NetworkInterfaceType> SelectedNetworksInterfacesTypes;
        List<NetworkInterfaceType> DefaulNetworksInterfacesTypes = new List<NetworkInterfaceType>
        {
            NetworkInterfaceType.Ethernet,
            NetworkInterfaceType.Wireless80211
        };

        List<IPAddress> AviableIPv4;
        List<NetworkInterface> AviableNetworkInterfaces;

        public NetInfoVM(Page page)
        {
            m_Page = page;

            DeviceNameOutput = (TextBlock)m_Page.FindName(nameof(DeviceNameOutput));
            IPv4Output = (TextBlock)m_Page.FindName(nameof(IPv4Output));

            UpdateNetInfo();

            UpdateDeviceNameOutput();
            UpdateIPv4Output();
        }

        void UpdateDeviceNameOutput()
        {
            DeviceNameOutput.Text = NetworkTools.GetMyDeviceName();
        }
        void UpdateIPv4Output()
        {
            string outputString = string.Empty;
            UpdateNetInfo();
            foreach(IPAddress ip in AviableIPv4)
            {
                outputString += ip.ToString() + '\n';
            }
            IPv4Output.Text = outputString;
        }

        public void UpdateNetInfo()
        {
            if (SelectedNetworksInterfacesTypes != null && SelectedNetworksInterfacesTypes.Count != 0)
            {
                AviableNetworkInterfaces = NetworkTools.GetNetworkInterfaces(OperationalStatus.Up, SelectedNetworksInterfacesTypes.ToArray());
            }
            else
            {
                AviableNetworkInterfaces = NetworkTools.GetNetworkInterfaces(OperationalStatus.Up, DefaulNetworksInterfacesTypes.ToArray());
            }
            AviableIPv4 = NetworkTools.GetLocalIPv4(OperationalStatus.Up, DefaulNetworksInterfacesTypes.ToArray());
        }
    }
}
