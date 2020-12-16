using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.Timers;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using NieroNetLib;
using NieroNetLib.Types;

namespace Niero.Models
{
    public class NetworkDataHub
    {
        public string DeviceName { get; set; }

        public List<IPAddress> AviableIPv4 { get; set; }
        public List<NetworkInterface> AviableNetworkInterfaces { get; set; }
        public List<NetworkInterface> NonAviableNetworkInterfaces { get; set; }
        
        //Lists of special objects for DataGrid
        public ObservableCollection<DGAdapterInfo> DisabledAdapters { get; set; }
        public ObservableCollection<DGAdapterInfo> EnabledAdapters { get; set; }
        //-------------------------------------

        public List<NetworkInterfaceType> CurrentInterfaceTypes { get; }
        public List<IPAddress> ActiveGateways { get; set; }
        public List<IPAddress> CurrentNetMasks { get; set; }

        public LocalNetScan ActiveScan { get; set; }
        private BackgroundWorker BackScanWorker;
        private Timer SpeedUpdateTimer = new Timer { Interval = 5000, AutoReset = true };

        //CONSTRUCTOR
        public NetworkDataHub(params NetworkInterfaceType[] InterfaceTypes)
        {
            //Fields init
            AviableIPv4 = new List<IPAddress>();
            AviableNetworkInterfaces = new List<NetworkInterface>();
            NonAviableNetworkInterfaces = new List<NetworkInterface>();

            DisabledAdapters = new ObservableCollection<DGAdapterInfo>();
            EnabledAdapters = new ObservableCollection<DGAdapterInfo>();

            CurrentInterfaceTypes = new List<NetworkInterfaceType>(InterfaceTypes);
            ActiveGateways = new List<IPAddress>();
            CurrentNetMasks = new List<IPAddress>()
            {
                IPAddress.Parse("255.255.255.0"),
                IPAddress.Parse("255.255.0.0"),
                IPAddress.Parse("255.0.0.0")
            };

            //Post-initialization fields filling
            GlobalFieldsInit();

            //BackgroundWorker-s init
            BackScanWorker = new BackgroundWorker();
            BackScanWorker.WorkerReportsProgress = true;
            BackScanWorker.WorkerSupportsCancellation = true;

            //Timer setting
            SpeedUpdateTimer.Elapsed += GlobalInfoUpdate;
            SpeedUpdateTimer.Enabled = true;
        }

        //Global update functions
        private void GlobalFieldsInit()
        {
            UpdateDeviceNameInfo();
            UpdateAviableIPv4Info();
            UpdateAviableNetworkInterfacesInfo();
            UpdateNonAviableNetworkInterfacesInfo();
            UpdateEnabledAdaptersList();
            UpdateDisabledAdaptersList();
            UpdateActiveGatawaysInfo();
        }

        private void GlobalInfoUpdate(object sender, ElapsedEventArgs args)
        {
            Task[] UpdateTasks = 
            {
                new Task(() => UpdateDeviceNameInfo()),
                new Task(() => UpdateAviableIPv4Info()),
                new Task(() => UpdateAviableNetworkInterfacesInfo()),
                new Task(() => UpdateNonAviableNetworkInterfacesInfo()),
                new Task(() => UpdateEnabledAdaptersList()),
                new Task(() => UpdateDisabledAdaptersList()),
                new Task(() => UpdateActiveGatawaysInfo())
            };

            foreach (Task task in UpdateTasks)
            {
                task.Start();
            }

            Task.WaitAll(UpdateTasks);
        }

        //Model functions
        public void CreateNewScan(IPAddress gateway, IPAddress netMask, int timeout)
        {
            ActiveScan = new LocalNetScan(gateway, netMask, timeout);
        }
        public void TryStartActiveScan()
        {
            if(ActiveScan.ScanStatus == NetScanStatus.Ready)
            {
                BackScanWorker.DoWork += ActiveScan.StartScanningOnBackground;
                BackScanWorker.RunWorkerAsync();
            }
        }
        public void TryCancelActiveScan()
        {
            if(BackScanWorker.IsBusy == true)
            {
                BackScanWorker.CancelAsync();
            }
        }

        //Partial update functions
        private void UpdateDeviceNameInfo()
        {
            DeviceName = NetworkTools.GetMyDeviceName();
            OnDeviceNameInfoUpdated();
        }
        private void UpdateAviableIPv4Info()
        {
            AviableIPv4 = NetworkTools.GetLocalIPv4(OperationalStatus.Up, CurrentInterfaceTypes.ToArray());
            OnAviableIPv4InfoUpdated();
        }
        private void UpdateAviableNetworkInterfacesInfo()
        {
            AviableNetworkInterfaces = NetworkTools.GetNetworkInterfaces(OperationalStatus.Up, CurrentInterfaceTypes.ToArray());
            OnAviableNetworkInterfacesInfoUpdated();
        }
        private void UpdateNonAviableNetworkInterfacesInfo()
        {
            NonAviableNetworkInterfaces = NetworkTools.GetNetworkInterfaces(OperationalStatus.Down); //need to add network interfaces type
            OnNonAviableNetworkInterfacesInfoUpdated();
        }
        private void UpdateEnabledAdaptersList()
        {
            if (EnabledAdapters != null)
            {
                EnabledAdapters = new ObservableCollection<DGAdapterInfo>();
                foreach (NetworkInterface inter in AviableNetworkInterfaces)
                {
                    EnabledAdapters.Add(new DGAdapterInfo(new BasicInterfaceInfo(inter)));
                }
                OnEnabledAdaptersListUpdated();
            }
            else throw new Exception("EnabledAdapters list not initialized!");
        }
        private void UpdateDisabledAdaptersList()
        {
            if (DisabledAdapters != null)
            {
                DisabledAdapters = new ObservableCollection<DGAdapterInfo>();
                foreach (NetworkInterface inter in NonAviableNetworkInterfaces)
                {
                    DisabledAdapters.Add(new DGAdapterInfo(new BasicInterfaceInfo(inter)));
                }
                OnDisabledAdaptersListUpdated();
            }
            else throw new Exception("DisabledAdapters list not initialized!");
        }
        private void UpdateActiveGatawaysInfo()
        {
            ActiveGateways = NetworkTools.GetNetworkInterfacesGateways(OperationalStatus.Up, CurrentInterfaceTypes.ToArray());
            OnActiveGatawaysInfoUpdated();
        }

        //Invoke-functions
        protected virtual void OnDeviceNameInfoUpdated()
        {
            DeviceNameInfoUpdated?.Invoke(this);
        }
        protected virtual void OnAviableIPv4InfoUpdated()
        {
            AviableIPv4InfoUpdated?.Invoke(this);
        }
        protected virtual void OnAviableNetworkInterfacesInfoUpdated()
        {
            AviableNetworkInterfacesInfoUpdated?.Invoke(this);
        }
        protected virtual void OnNonAviableNetworkInterfacesInfoUpdated()
        {
            NonAviableNetworkInterfacesInfoUpdated?.Invoke(this);
        }
        protected virtual void OnEnabledAdaptersListUpdated()
        {
            EnabledAdaptersListUpdated?.Invoke(this);
        }
        protected virtual void OnDisabledAdaptersListUpdated()
        {
            DisabledAdaptersListUpdated?.Invoke(this);
        }
        protected virtual void OnActiveGatawaysInfoUpdated()
        {
            ActiveGatawaysInfoUpdated?.Invoke(this);
        }

        //Events
        public event InfoUpdatedEventHandler DeviceNameInfoUpdated;
        public event InfoUpdatedEventHandler AviableIPv4InfoUpdated;
        public event InfoUpdatedEventHandler AviableNetworkInterfacesInfoUpdated;
        public event InfoUpdatedEventHandler NonAviableNetworkInterfacesInfoUpdated;
        public event InfoUpdatedEventHandler EnabledAdaptersListUpdated;
        public event InfoUpdatedEventHandler DisabledAdaptersListUpdated;
        public event InfoUpdatedEventHandler ActiveGatawaysInfoUpdated;

        //Events handlers
        public delegate void InfoUpdatedEventHandler(object sender);
    }

    //Support classes
    public class DGAdapterInfo
    {
        public DGAdapterInfo(BasicInterfaceInfo interfaceInfo)
        {
            AdapterName = interfaceInfo.Name;
            IPAddress = interfaceInfo.IPv4.ToString();
            AdapterType = interfaceInfo.Type.ToString();
            MacAddress = interfaceInfo.MacAddress.ToString();
            OperationalStatus = interfaceInfo.Interface.OperationalStatus.ToString();
        }
        public string AdapterName { get; set; }
        public string IPAddress { get; set; }
        public string AdapterType { get; set; }
        public string MacAddress { get; set; }
        public string OperationalStatus { get; set; }
    }
}
