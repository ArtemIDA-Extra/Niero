using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Media;
using Niero.Models;
using Niero.Controls;
using System.Windows.Media;

namespace Niero.ViewModels
{
    class NetScanningVM : BaseViewModel
    {
        //Data part
        NetworkDataHub DataHub;

        //Elements
        ContentControl ContentViewer;

        ScanSettingsControl ScanSettings;
        ScanInProgressControl ScanInProgress;
        ScanInProgressSmallControl ScanInProgressSmall;
        ScanResultsControl ScanResults;

        bool IsSmallScanControl;

        //CONSTRUCTOR!!!
        public NetScanningVM(Page page, NetworkDataHub dataHub) : base(page)
        {
            //Data connect
            DataHub = dataHub;

            //Elements searching
            ContentViewer = (ContentControl)m_Page.FindName(nameof(ContentViewer));

            PrepearScanSettings();
        }

        private void PrepearScanSettings()
        {
            m_Page.KeepAlive = false;

            ScanSettings = new ScanSettingsControl();
            ScanSettings.DataContext = DataHub;
            ScanSettings.SettingsConfirmed += ScanSettings_SettingsConfirmed;

            ContentViewer.Content = ScanSettings;

            ScanSettings.BeginOpenAnimation();
        }
        private void PrepearScanInProgress()
        {
            IsSmallScanControl = false;
            ScanInProgress = new ScanInProgressControl();
            ScanInProgress.DataContext = DataHub.ActiveScan;
            ScanInProgress.Canceled += ScanInProgress_Canceled;
            ScanInProgress.Closed += ScanInProgress_Closed;

            ContentViewer.Content = ScanInProgress;

            ScanInProgress.BeginOpenAnimation();
        }
        private void PrepearSmallScanInProgress()
        {
            IsSmallScanControl = true;
            ScanInProgressSmall = new ScanInProgressSmallControl();
            ScanInProgressSmall.DataContext = DataHub.ActiveScan;
            ScanInProgressSmall.Canceled += ScanInProgress_Canceled;
            ScanInProgressSmall.Closed += ScanInProgress_Closed;

            ContentViewer.Content = ScanInProgressSmall;

            ScanInProgressSmall.BeginOpenAnimation();
        }
        private void PrepearScanResults()
        {
            ScanResults = new ScanResultsControl();
            ScanResults.DataContext = DataHub.ActiveScan;
            ScanResults.IpListChosen += ScanResults_IpListChosen;
            ScanResults.Closed += ScanResults_Closed;

            ContentViewer.Content = ScanResults;

            ScanResults.BeginOpenAnimation();
        }

        private void ScanSettings_SettingsConfirmed(object sender, ScanSettingsControl.SettingsConfirmedEventArgs e)
        {
            DataHub.CreateNewScan(e.Gateway, e.NetMask, e.Timeout);
            DataHub.ActiveScan.Completed += ActiveScan_Completed;
            DataHub.ActiveScan.Canceled += ActiveScan_Canceled;
            DataHub.ActiveScan.ScanStatusUpdated += ActiveScan_ScanStatusUpdated;

            if (e.IsAltModeOn)
            {
                DataHub.TryStartActiveScan();
                PrepearScanInProgress();
            }
            else
            {
                DataHub.TryStartFastActiveScan();
                PrepearSmallScanInProgress();
            }
        }

        private void ActiveScan_Completed(object sender)
        {
            SoundPlayer SP = new SoundPlayer("EmbeddedSounds/Warning.wav");
            SP.Play();
            if(!IsSmallScanControl) ScanInProgress.ChangeButtonOnBar();
            else if(IsSmallScanControl) ScanInProgressSmall.ChangeButtonOnBar();
        }
        private void ActiveScan_Canceled(object sender)
        {
            if (!IsSmallScanControl) ScanInProgress.ChangeButtonOnBar();
            else if (IsSmallScanControl) ScanInProgressSmall.ChangeButtonOnBar();
        }
        private void ActiveScan_ScanStatusUpdated(object sender, NieroNetLib.ScanStatusUpdatedEventArgs e)
        {
            if (e.ActualStatus == NieroNetLib.Types.NetScanStatus.ScanStarted)
            {
                if (!IsSmallScanControl)  ScanInProgress.OpenButtonBar();
                else if (IsSmallScanControl) ScanInProgressSmall.OpenButtonBar();
            }
        }

        private void ScanInProgress_Canceled(object sender)
        {
            DataHub.TryCancelActiveScan();
        }
        private void ScanInProgress_Closed(object sender)
        {
            m_Page.KeepAlive = true;
            PrepearScanResults();
        }

        private void ScanResults_IpListChosen(object sender)
        {
            
        }
        private void ScanResults_Closed(object sender)
        {
            PrepearScanSettings();
        }
    }
}
