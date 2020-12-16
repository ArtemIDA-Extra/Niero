using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
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
        ScanResultsControl ScanResults;

        //CONSTRUCTOR!!!
        public NetScanningVM(Page page, NetworkDataHub dataHub) : base(page)
        {
            //Data connect
            DataHub = dataHub;

            //Elements searching
            ContentViewer = (ContentControl)m_Page.FindName(nameof(ContentViewer));

            PrepearScanSettings();
        }

        private void ScanSettings_SettingsConfirmed(object sender, ScanSettingsControl.SettingsConfirmedEventArgs e)
        {
            m_Page.KeepAlive = true;

            DataHub.CreateNewScan(e.Gateway, e.NetMask, e.Timeout);
            DataHub.ActiveScan.Completed += ActiveScan_Completed;
            DataHub.ActiveScan.Canceled += ActiveScan_Canceled;
            DataHub.ActiveScan.ScanStatusUpdated += ActiveScan_ScanStatusUpdated;
            PrepearScanInProgress();

            DataHub.TryStartActiveScan();
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
            ScanInProgress = new ScanInProgressControl();
            ScanInProgress.DataContext = DataHub.ActiveScan;
            ScanInProgress.Canceled += ScanInProgress_Canceled;
            ScanInProgress.Closed += ScanInProgress_Closed;

            ContentViewer.Content = ScanInProgress;

            ScanInProgress.BeginOpenAnimation();
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

        private void ActiveScan_Completed(object sender)
        {
            ScanInProgress.ChangeButtonOnBar();
        }
        private void ActiveScan_Canceled(object sender)
        {
            ScanInProgress.ChangeButtonOnBar();
        }
        private void ActiveScan_ScanStatusUpdated(object sender, NieroNetLib.ScanStatusUpdatedEventArgs e)
        {
            if(e.ActualStatus == NieroNetLib.Types.NetScanStatus.ScanStarted)
            ScanInProgress.OpenButtonBar();
        }

        private void ScanInProgress_Canceled(object sender)
        {
            DataHub.TryCancelActiveScan();
        }
        private void ScanInProgress_Closed(object sender)
        {
            PrepearScanResults();
        }

        private void ScanResults_IpListChosen(object sender)
        {
            MessageBox.Show("Podkluchi uje etu stranicu blia!");
        }
        private void ScanResults_Closed(object sender)
        {
            PrepearScanSettings();
        }
    }
}
