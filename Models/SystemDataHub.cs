using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Management;

namespace Niero.Models
{
    public class SystemDataHub
    {
        public string HDDSerialNO { get; set; }
        public string CDROMDrive { get; set; }

        public string MatherboardMaker { get; set; }
        public string MatherboardModel { get; set; }

        public string BiosModel { get; set; }
        public string BiosSerialNo { get; set; }
        public string BiosCaption { get; set; }

        public string Memory { get; set; }
        public string RamSlots { get; set; }

        public string CPUManufacturer { get; set; }
        public int CPUClockSpeed { get; set; }
        public string CPUSpeed { get; set; }
        public string CPUId { get; set; }
        public string CPUInfo { get; set; }

        public string OS { get; set; }
        public string CurrentLanguage { get; set; }
        public string AccountName { get; set; }
        public string ComputerName { get; set; }

        private Timer SpeedUpdateTimer = new Timer { Interval = 5000, AutoReset = true };

        public SystemDataHub()
        {
            //Post-initialization fields filling
            GlobalFieldsInit();

            //Timer setting
            SpeedUpdateTimer.Elapsed += GlobalInfoUpdate;
            SpeedUpdateTimer.Enabled = true;
        }

        private void GlobalFieldsInit()
        {
            UpdateHDDSerialNO();
            UpdateCDROMDrive();

            UpdateMatherboardMaker();
            UpdateMatherboardModel();

            UpdateBiosModel();
            UpdateBiosSerialNo();
            UpdateBiosCaption();

            UpdateMemory();
            UpdateRamSlots();

            UpdateCPUManufacturer();
            UpdateCPUClockSpeed();
            UpdateCPUSpeed();
            UpdateCPUId();
            UpdateCPUInfo();

            UpdateOS();
            UpdateCurrentLanguage();
            UpdateAccountName();
            UpdateComputerName();
        }

        private void GlobalInfoUpdate(object sender, ElapsedEventArgs args)
        {
            Task[] UpdateTasks =
            {
                new Task(() => UpdateHDDSerialNO()),
                new Task(() => UpdateCDROMDrive()),

                new Task(() => UpdateMatherboardMaker()),
                new Task(() => UpdateMatherboardModel()),

                new Task(() => UpdateBiosModel()),
                new Task(() => UpdateBiosSerialNo()),
                new Task(() => UpdateBiosCaption()),

                new Task(() => UpdateMemory()),
                new Task(() => UpdateRamSlots()),

                new Task(() => UpdateCPUManufacturer()),
                new Task(() => UpdateCPUClockSpeed()),
                new Task(() => UpdateCPUSpeed()),
                new Task(() => UpdateCPUId()),
                new Task(() => UpdateCPUInfo()),

                new Task(() => UpdateOS()),
                new Task(() => UpdateCurrentLanguage()),
                new Task(() => UpdateAccountName()),
                new Task(() => UpdateComputerName()),
            };

            foreach (Task task in UpdateTasks)
            {
                task.Start();
            }

            //Task.WaitAll(UpdateTasks);
        }

        //Partial Shall-Update functions
        private void UpdateHDDSerialNO()
        {
            HDDSerialNO = GetHDDSerialNo();
        }
        private void UpdateCDROMDrive()
        {
            CDROMDrive = GetCdRomDrive();
        }

        private void UpdateMatherboardMaker()
        {
            MatherboardMaker = GetBoardMaker();
        }
        private void UpdateMatherboardModel()
        {
            MatherboardModel = GetBoardProductId();
        }

        private void UpdateBiosModel()
        {
            BiosModel = GetBIOSmaker();
        }
        private void UpdateBiosSerialNo()
        {
            BiosSerialNo = GetBIOSserNo();
        }
        private void UpdateBiosCaption()
        {
            BiosCaption = GetBIOScaption();
        }

        private void UpdateMemory()
        {
            Memory = GetPhysicalMemory();
        }
        private void UpdateRamSlots()
        {
            RamSlots = GetNoRamSlots();
        }

        private void UpdateCPUManufacturer()
        {
            CPUManufacturer = GetCPUManufacturer();
        }
        private void UpdateCPUClockSpeed()
        {
            CPUClockSpeed = GetCPUCurrentClockSpeed();
        }
        private void UpdateCPUSpeed()
        {
            CPUSpeed = GetCpuSpeedInGHz();
        }
        private void UpdateCPUId()
        {
            CPUId = GetProcessorId();
        }
        private void UpdateCPUInfo()
        {
            CPUInfo = GetProcessorInformation();
        }

        private void UpdateOS()
        {
            OS = GetOSInformation();
        }
        private void UpdateCurrentLanguage()
        {
            CurrentLanguage = GetCurrentLanguage();
        }
        private void UpdateAccountName()
        {
            AccountName = GetAccountName();
        }
        private void UpdateComputerName()
        {
            ComputerName = GetComputerName();
        }

        //Model functions
        private string GetHDDSerialNo()
        {
            ManagementClass mangnmt = new ManagementClass("Win32_LogicalDisk");
            ManagementObjectCollection mcol = mangnmt.GetInstances();
            string result = "";
            foreach (ManagementObject strt in mcol)
            {
                result += Convert.ToString(strt["VolumeSerialNumber"]);
            }
            return result;
        }
        private string GetBoardMaker()
        {

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard");

            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    return wmi.GetPropertyValue("Manufacturer").ToString();
                }

                catch { }

            }

            return "Unknown";

        }
        private string GetBoardProductId()
        {

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard");

            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    return wmi.GetPropertyValue("Product").ToString();

                }

                catch { }

            }

            return "Unknown";

        }
        private string GetCdRomDrive()
        {

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_CDROMDrive");

            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    return wmi.GetPropertyValue("Drive").ToString();

                }

                catch { }

            }

            return "Unknown";

        }
        private string GetBIOSmaker()
        {

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS");

            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    return wmi.GetPropertyValue("Manufacturer").ToString();

                }

                catch { }

            }

            return "Unknown";

        }
        private string GetBIOSserNo()
        {

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS");

            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    return wmi.GetPropertyValue("SerialNumber").ToString();

                }

                catch { }

            }

            return "Unknown";

        }
        private string GetBIOScaption()
        {

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS");

            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    return wmi.GetPropertyValue("Caption").ToString();

                }
                catch { }
            }
            return "BIOS Caption: Unknown";
        }
        private string GetPhysicalMemory()
        {
            ManagementScope oMs = new ManagementScope();
            ObjectQuery oQuery = new ObjectQuery("SELECT Capacity FROM Win32_PhysicalMemory");
            ManagementObjectSearcher oSearcher = new ManagementObjectSearcher(oMs, oQuery);
            ManagementObjectCollection oCollection = oSearcher.Get();

            long MemSize = 0;
            long mCap = 0;

            // In case more than one Memory sticks are installed
            foreach (ManagementObject obj in oCollection)
            {
                mCap = Convert.ToInt64(obj["Capacity"]);
                MemSize += mCap;
            }
            MemSize = (MemSize / 1024) / 1024;
            return MemSize.ToString() + "MB";
        }
        private string GetNoRamSlots()
        {

            int MemSlots = 0;
            ManagementScope oMs = new ManagementScope();
            ObjectQuery oQuery2 = new ObjectQuery("SELECT MemoryDevices FROM Win32_PhysicalMemoryArray");
            ManagementObjectSearcher oSearcher2 = new ManagementObjectSearcher(oMs, oQuery2);
            ManagementObjectCollection oCollection2 = oSearcher2.Get();
            foreach (ManagementObject obj in oCollection2)
            {
                MemSlots = Convert.ToInt32(obj["MemoryDevices"]);

            }
            return MemSlots.ToString();
        }
        private string GetCPUManufacturer()
        {
            string cpuMan = String.Empty;
            //create an instance of the Managemnet class with the
            //Win32_Processor class
            ManagementClass mgmt = new ManagementClass("Win32_Processor");
            //create a ManagementObjectCollection to loop through
            ManagementObjectCollection objCol = mgmt.GetInstances();
            //start our loop for all processors found
            foreach (ManagementObject obj in objCol)
            {
                if (cpuMan == String.Empty)
                {
                    // only return manufacturer from first CPU
                    cpuMan = obj.Properties["Manufacturer"].Value.ToString();
                }
            }
            return cpuMan;
        }
        private int GetCPUCurrentClockSpeed()
        {
            int cpuClockSpeed = 0;
            //create an instance of the Managemnet class with the
            //Win32_Processor class
            ManagementClass mgmt = new ManagementClass("Win32_Processor");
            //create a ManagementObjectCollection to loop through
            ManagementObjectCollection objCol = mgmt.GetInstances();
            //start our loop for all processors found
            foreach (ManagementObject obj in objCol)
            {
                if (cpuClockSpeed == 0)
                {
                    // only return cpuStatus from first CPU
                    cpuClockSpeed = Convert.ToInt32(obj.Properties["CurrentClockSpeed"].Value.ToString());
                }
            }
            //return the status
            return cpuClockSpeed;
        }
        private string GetCpuSpeedInGHz()
        {
            double? GHz = null;
            using (ManagementClass mc = new ManagementClass("Win32_Processor"))
            {
                foreach (ManagementObject mo in mc.GetInstances())
                {
                    GHz = 0.001 * (UInt32)mo.Properties["CurrentClockSpeed"].Value;
                    break;
                }
            }
            return $"{GHz} GHz";
        }
        private string GetProcessorId()
        {

            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();
            String Id = String.Empty;
            foreach (ManagementObject mo in moc)
            {

                Id = mo.Properties["processorID"].Value.ToString();
                break;
            }
            return Id;

        }
        private string GetProcessorInformation()
        {
            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();
            String info = String.Empty;
            foreach (ManagementObject mo in moc)
            {
                string name = (string)mo["Name"];
                name = name.Replace("(TM)", "™").Replace("(tm)", "™").Replace("(R)", "®").Replace("(r)", "®").Replace("(C)", "©").Replace("(c)", "©").Replace("    ", " ").Replace("  ", " ");

                info = name + ", " + (string)mo["Caption"] + ", " + (string)mo["SocketDesignation"];
                //mo.Properties["Name"].Value.ToString();
                //break;
            }
            return info;
        }
        private string GetOSInformation()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    return ((string)wmi["Caption"]).Trim() + ", " + (string)wmi["Version"] + ", " + (string)wmi["OSArchitecture"];
                }
                catch { }
            }
            return "BIOS Maker: Unknown";
        }
        private string GetCurrentLanguage()
        {

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS");

            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    return wmi.GetPropertyValue("CurrentLanguage").ToString();

                }

                catch { }

            }

            return "BIOS Maker: Unknown";

        }
        private string GetAccountName()
        {

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_UserAccount");

            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {

                    return wmi.GetPropertyValue("Name").ToString();
                }
                catch { }
            }
            return "User Account Name: Unknown";

        }
        private string GetComputerName()
        {
            ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
            ManagementObjectCollection moc = mc.GetInstances();
            String info = String.Empty;
            foreach (ManagementObject mo in moc)
            {
                info = (string)mo["Name"];
                //mo.Properties["Name"].Value.ToString();
                //break;
            }
            return info;
        }
    }
}
