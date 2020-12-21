using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Timers;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Input;
using NieroNetLib;
using NieroNetLib.Types;
using Niero.SupportClasses;
using Niero.Controls;
using Niero.Models;


namespace Niero.ViewModels
{
    class BaseInfoVM : BaseViewModel
    {
        //Net data
        NetworkDataHub NetDataHub;
        SystemDataHub SysDataHub;

        ShortNetInfoControl ShortNetInfo;
        WorkingAdaptersInfoControl WorkingAdaptersInfo;
        DisabledAdaptersInfoControl DisabledAdaptersInfo;

        OtherInfoControl OtherInfo;
        MatherboardInfoControl MatherboardInfo;
        RAMInfoControl RAMInfo;

        BIOSInfoControl BIOSInfo;
        OSInfoControl OSInfo;
        CPUInfoControl CPUInfo;

        private Timer UpdateDataTimer = new Timer { Interval = 10000, AutoReset = true };

        //CONSTRUCTOR!!!
        public BaseInfoVM(Page page, NetworkDataHub netDataHub, SystemDataHub sysDataHub) : base(page)
        {
            NetDataHub = netDataHub;
            SysDataHub = sysDataHub;

            ShortNetInfo = (ShortNetInfoControl)m_Page.FindName(nameof(ShortNetInfo));
            WorkingAdaptersInfo = (WorkingAdaptersInfoControl)m_Page.FindName(nameof(WorkingAdaptersInfo));
            DisabledAdaptersInfo = (DisabledAdaptersInfoControl)m_Page.FindName(nameof(DisabledAdaptersInfo));
            OtherInfo = (OtherInfoControl)m_Page.FindName(nameof(OtherInfo));
            MatherboardInfo = (MatherboardInfoControl)m_Page.FindName(nameof(MatherboardInfo));
            RAMInfo = (RAMInfoControl)m_Page.FindName(nameof(RAMInfo));
            BIOSInfo = (BIOSInfoControl)m_Page.FindName(nameof(BIOSInfo));
            OSInfo = (OSInfoControl)m_Page.FindName(nameof(OSInfo));
            CPUInfo = (CPUInfoControl)m_Page.FindName(nameof(CPUInfo));

            ShortNetInfo.UpdateAnimationCompleted += ShortNetInfo_UpdateAnimationCompleted;
            WorkingAdaptersInfo.UpdateAnimationCompleted += WorkingAdaptersInfo_UpdateAnimationCompleted;
            DisabledAdaptersInfo.UpdateAnimationCompleted += DisabledAdaptersInfo_UpdateAnimationCompleted;
            OtherInfo.UpdateAnimationCompleted += OtherInfo_UpdateAnimationCompleted;
            MatherboardInfo.UpdateAnimationCompleted += MatherboardInfo_UpdateAnimationCompleted;
            RAMInfo.UpdateAnimationCompleted += RAMInfo_UpdateAnimationCompleted;
            BIOSInfo.UpdateAnimationCompleted += BIOSInfo_UpdateAnimationCompleted;
            OSInfo.UpdateAnimationCompleted += OSInfo_UpdateAnimationCompleted;

            UpdateDataTimer.Elapsed += UpdateShortNetInfo;
            UpdateDataTimer.Enabled = true;

            InitShortNetInfo();
        }

        private void InitShortNetInfo()
        {
            ShortNetInfo.DeviceName = NetDataHub.DeviceName;
            for (int i = 0; i < NetDataHub.AviableIPv4.Count; i++)
            {
                if (i == 0) ShortNetInfo.Ipv4_1 = NetDataHub.AviableIPv4[i].ToString();
                if (i == 1) ShortNetInfo.Ipv4_2 = NetDataHub.AviableIPv4[i].ToString();
                if (i == 2) ShortNetInfo.Ipv4_3 = NetDataHub.AviableIPv4[i].ToString();
            }
            ShortNetInfo.BeginUpdateAnimation();
        }
        private void UpdateShortNetInfo(object sender, ElapsedEventArgs args)
        {
            ShortNetInfo.DeviceName = NetDataHub.DeviceName;
            for(int i = 0; i < NetDataHub.AviableIPv4.Count; i++)
            {
                if (i == 0) ShortNetInfo.Ipv4_1 = NetDataHub.AviableIPv4[i].ToString();
                if (i == 1) ShortNetInfo.Ipv4_2 = NetDataHub.AviableIPv4[i].ToString();
                if (i == 2) ShortNetInfo.Ipv4_3 = NetDataHub.AviableIPv4[i].ToString();
            }
            ShortNetInfo.BeginUpdateAnimation();
        }
        private void ShortNetInfo_UpdateAnimationCompleted(object sender)
        {
            WorkingAdaptersInfo.itemsSource = NetDataHub.EnabledAdapters;
            WorkingAdaptersInfo.BeginUpdateAnimation();
        }
        private void WorkingAdaptersInfo_UpdateAnimationCompleted(object sender)
        {
            DisabledAdaptersInfo.itemsSource = NetDataHub.DisabledAdapters;
            DisabledAdaptersInfo.BeginUpdateAnimation();
        }
        private void DisabledAdaptersInfo_UpdateAnimationCompleted(object sender)
        {
            OtherInfo.HDDSerialNo = SysDataHub.HDDSerialNO;
            OtherInfo.CDROMDrive = SysDataHub.CDROMDrive;
            OtherInfo.BeginUpdateAnimation();
        }
        private void OtherInfo_UpdateAnimationCompleted(object sender)
        {
            MatherboardInfo.MatherboardMaker = SysDataHub.MatherboardMaker;
            MatherboardInfo.MatherboardModel = SysDataHub.MatherboardModel;
            MatherboardInfo.BeginUpdateAnimation();
        }
        private void MatherboardInfo_UpdateAnimationCompleted(object sender)
        {
            RAMInfo.MemorySize = SysDataHub.Memory;
            RAMInfo.RamSlots = SysDataHub.RamSlots;
            RAMInfo.BeginUpdateAnimation();
        }
        private void RAMInfo_UpdateAnimationCompleted(object sender)
        {
            BIOSInfo.BiosModel = SysDataHub.BiosModel;
            BIOSInfo.BiosSerialNomer = SysDataHub.BiosSerialNo;
            BIOSInfo.BiosCaption = SysDataHub.BiosCaption;
            BIOSInfo.BeginUpdateAnimation();
        }
        private void BIOSInfo_UpdateAnimationCompleted(object sender)
        {
            OSInfo.OS = SysDataHub.OS;
            OSInfo.CurrentLanguage = SysDataHub.CurrentLanguage;
            OSInfo.AccountName = SysDataHub.AccountName;
            OSInfo.ComputerName = SysDataHub.ComputerName;
            OSInfo.BeginUpdateAnimation();
        }
        private void OSInfo_UpdateAnimationCompleted(object sender)
        {
            CPUInfo.CPUManufacturer = SysDataHub.CPUManufacturer;
            CPUInfo.CPUClockSpeed = SysDataHub.CPUClockSpeed;
            CPUInfo.CPUSpeed = SysDataHub.CPUSpeed;
            CPUInfo.CPUId = SysDataHub.CPUId;
            CPUInfo.CPUInfo = SysDataHub.CPUInfo;
            CPUInfo.BeginUpdateAnimation();
        }
    }
}
