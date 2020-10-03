using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Net;
using System.Net.NetworkInformation;
using NieroNetLib;
using Niero.SupportClasses;

namespace Niero.ViewModels
{
    class NetInfoVM : BaseViewModel
    {
        //Drag-move helpers 
        private bool DraggingActive;
        private class BorderWithPositionInfo
        {
            public Border DataOwner { get; set; }

            private Point CursPosRelToParent, CursPosRelToBorder;

            public Point CursPosRelativeToParent
            {
                set
                {
                    CursPosRelToParent = value;
                    if(CursPosRelToBorder != null)
                    {
                        CalculatePosition();
                    }
                }
                get
                {
                    return CursPosRelToParent;
                }
            }
            public Point CursPosRelativeToBorder
            {
                set
                {
                    CursPosRelToBorder = value;
                    if (CursPosRelToParent != null)
                    {
                        CalculatePosition();
                    }
                }
                get
                {
                    return CursPosRelToBorder;
                }
            }
            public Point LeftUp, RightUp, LeftDown, RightDown;

            //Variables for old element coordinates
            //When shifting, it is necessary to add to the shift vector
            //Because I'm creating a new TranslateTransform for every shift
            public double AdditionalShiftX, AdditionalShiftY;

            public BorderWithPositionInfo(Border borderOwner)
            {
                DataOwner = borderOwner;

                AdditionalShiftX = (borderOwner.RenderTransform as TranslateTransform).X; //Canvas.GetLeft(borderOwner);
                AdditionalShiftY = (borderOwner.RenderTransform as TranslateTransform).Y; //Canvas.GetTop(borderOwner);
            }

            private void CalculatePosition()
            {
                LeftUp.X = CursPosRelToParent.X - CursPosRelToBorder.X;
                LeftUp.Y = CursPosRelToParent.Y - CursPosRelToBorder.Y;
                RightUp.X = LeftUp.X + DataOwner.Width;
                RightUp.Y = LeftUp.Y;
                RightDown.X = RightUp.X;
                RightDown.Y = RightUp.Y + DataOwner.Height;
                LeftDown.X = LeftUp.X;
                LeftDown.Y = RightDown.Y;
            }
        }
        BorderWithPositionInfo DraggableBorder = null;

        //UI elements
        Page m_Page;
        Canvas MainCanvas;
        Border MainBorder;

        List<BorderWithPositionInfo> CanvasBorders = new List<BorderWithPositionInfo>();

        //Net data
        List<(NetworkInterfaceType intType, string intName)> AviableToSelectInterfaces = new List<(NetworkInterfaceType, string)>
        {
            (NetworkInterfaceType.Ethernet, "Ethernet"),
            (NetworkInterfaceType.Wireless80211, "Wi-Fi"),
        };
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

            MainCanvas = (Canvas)m_Page.FindName(nameof(MainCanvas));
            MainBorder = (Border)m_Page.FindName(nameof(MainBorder));

            CanvasBorders.Add(new BorderWithPositionInfo((Border)m_Page.FindName("DeviceInfo")));
            CanvasBorders.Add(new BorderWithPositionInfo((Border)m_Page.FindName("AdaptersInfo")));

            SubscribeToDragMoveEvents(CanvasBorders[0].DataOwner, CanvasBorders[1].DataOwner);

            UpdateNetModel();

            DataUpdateLoop();
        }

        private void SubscribeToDragMoveEvents(params Border[] borders)
        {
            foreach (Border border in borders)
            {
                border.MouseLeftButtonDown += Border_MouseLeftButtonDown;
                border.MouseLeftButtonUp += Border_MouseLeftButtonUp;
                border.MouseMove += Border_MouseMove;
            }
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            m_Page.Cursor = CustomCursors.Move;

            foreach(BorderWithPositionInfo itBorderInfo in CanvasBorders)
            {
                if(itBorderInfo.DataOwner != sender as Border)
                {
                    itBorderInfo.DataOwner.Focusable = false;
                    Panel.SetZIndex(itBorderInfo.DataOwner, 0);
                }
                if(itBorderInfo.DataOwner == sender as Border)
                {
                    DraggableBorder = itBorderInfo; 
                    DraggingActive = true;
                }
            }

            Panel.SetZIndex(DraggableBorder.DataOwner, 1);

            ColorAnimation animColor = new ColorAnimation((Color)m_Page.FindResource("RedDarkColor"), new Duration(new TimeSpan(0, 0, 0, 0, 750)));
            MainBorder.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, animColor);

            DraggableBorder.CursPosRelativeToParent = e.GetPosition(MainCanvas);
            DraggableBorder.CursPosRelativeToBorder = e.GetPosition(DraggableBorder.DataOwner);

            DraggableBorder.DataOwner.CaptureMouse();
        }
        private void Border_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        { 
            m_Page.Cursor = CustomCursors.Normal_Select;

            foreach (BorderWithPositionInfo itBorderInfo in CanvasBorders)
            {
                if (itBorderInfo.DataOwner != sender as Border)
                {
                    itBorderInfo.DataOwner.Focusable = true;
                }
                if (itBorderInfo.DataOwner == sender as Border)
                {
                    DraggableBorder = itBorderInfo;
                    DraggingActive = false;
                }
            }

            DraggableBorder.CursPosRelativeToParent = e.GetPosition(MainCanvas);
            DraggableBorder.CursPosRelativeToBorder = e.GetPosition(DraggableBorder.DataOwner);

            //temp local shift var-s
            double addX = -1, addY = -1;

            if (DraggableBorder.LeftUp.X < 0)
            {
                Duration duration = new Duration(new TimeSpan(0, 0, 0, 1, 0));
                DoubleAnimation anim = new DoubleAnimation(0, duration);
                DraggableBorder.DataOwner.RenderTransform.BeginAnimation(TranslateTransform.XProperty, anim);
                addX = 0;
            }
            if (DraggableBorder.LeftUp.Y < 0)
            {
                Duration duration = new Duration(new TimeSpan(0, 0, 0, 1, 0));
                DoubleAnimation anim = new DoubleAnimation(0, duration);
                DraggableBorder.DataOwner.RenderTransform.BeginAnimation(TranslateTransform.YProperty, anim);
                addY = 0;
            }
            if (DraggableBorder.RightDown.X > MainCanvas.ActualWidth)
            {
                Duration duration = new Duration(new TimeSpan(0, 0, 0, 1, 0));
                DoubleAnimation anim = new DoubleAnimation(MainCanvas.ActualWidth - DraggableBorder.DataOwner.ActualWidth, duration);
                DraggableBorder.DataOwner.RenderTransform.BeginAnimation(TranslateTransform.XProperty, anim);
                addX = MainCanvas.ActualWidth - DraggableBorder.DataOwner.ActualWidth;
            }
            if (DraggableBorder.RightDown.Y > MainCanvas.ActualHeight)
            {
                Duration duration = new Duration(new TimeSpan(0, 0, 0, 1, 0));
                DoubleAnimation anim = new DoubleAnimation(MainCanvas.ActualHeight - DraggableBorder.DataOwner.ActualHeight, duration);
                DraggableBorder.DataOwner.RenderTransform.BeginAnimation(TranslateTransform.YProperty, anim);
                addY = MainCanvas.ActualHeight - DraggableBorder.DataOwner.ActualHeight;
            }

            //save shift data
            var Transform = (DraggableBorder.DataOwner.RenderTransform as TranslateTransform);
            if (Transform != null)
            {
                DraggableBorder.AdditionalShiftX = Transform.X;
                DraggableBorder.AdditionalShiftY = Transform.Y;
            }
            //when border was returned to the field
            if (addX != -1) DraggableBorder.AdditionalShiftX = addX;
            if (addY != -1) DraggableBorder.AdditionalShiftY = addY;

            DraggableBorder.DataOwner.ReleaseMouseCapture();
            DraggableBorder = null;

            ColorAnimation animColor = new ColorAnimation((Color)m_Page.FindResource("BaseColor"), new Duration(new TimeSpan(0, 0, 0, 0, 500)));
            MainBorder.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, animColor);
        }
        private void Border_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            foreach (BorderWithPositionInfo itBorderInfo in CanvasBorders)
            {
                if (itBorderInfo.DataOwner == sender as Border)
                {
                    DraggableBorder = itBorderInfo;
                }
            }

            if (DraggingActive && DraggableBorder != null)
            {
                Vector MoveVector;
                Point CurrentPosition = e.GetPosition(MainCanvas);

                MoveVector.X = CurrentPosition.X - DraggableBorder.CursPosRelativeToParent.X;
                MoveVector.Y = CurrentPosition.Y - DraggableBorder.CursPosRelativeToParent.Y;

                TranslateTransform Transform = new TranslateTransform();
                DraggableBorder.DataOwner.RenderTransform = Transform;

                Transform.X = MoveVector.X;
                Transform.Y = MoveVector.Y;
                Transform.X += DraggableBorder.AdditionalShiftX;
                Transform.Y += DraggableBorder.AdditionalShiftY;
            }
        }

        private async Task DataUpdateLoop()
        {
            while (true){
                UpdateNetModel();
                if (DraggingActive)
                {
                    if(DraggableBorder.DataOwner.Name != "DeviceInfo") await UpdateDeviceInfoBorder();
                }
                else
                {
                    await UpdateDeviceInfoBorder();
                }
                await Task.Delay(10000);
            }
        }

        private async Task UpdateDeviceInfoBorder()
        {
            Border DeviceInfo = null;
            foreach (BorderWithPositionInfo itBorderInfo in CanvasBorders)
            {
                if (itBorderInfo.DataOwner.Name == nameof(DeviceInfo))
                    DeviceInfo = itBorderInfo.DataOwner;
            }

            DeviceInfo.IsEnabled = false;
            await AnimateBorderUpdate_Start(DeviceInfo);

            TextBlock DeviceNameOutput = (TextBlock)DeviceInfo.FindName(nameof(DeviceNameOutput));
            TextBlock IPv4Output = (TextBlock)DeviceInfo.FindName(nameof(IPv4Output));

            DeviceNameOutput.Text = NetworkTools.GetMyDeviceName();

            string outputString = string.Empty;
            foreach (IPAddress ip in AviableIPv4)
            {
                outputString += ip.ToString() + '\n';
            }
            IPv4Output.Text = outputString;

            await AnimateBorderUpdate_Finish(DeviceInfo);
            DeviceInfo.IsEnabled = true;
        }

        async Task AnimateBorderUpdate_Start(Border border)
        {
            DoubleAnimation anim = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 200)));
            border.BeginAnimation(Border.OpacityProperty, anim);
            await Task.Delay(200);
        }
        async Task AnimateBorderUpdate_Finish(Border border)
        {
            DoubleAnimation anim = new DoubleAnimation(1, new Duration(new TimeSpan(0, 0, 0, 0, 200)));
            border.BeginAnimation(Border.OpacityProperty, anim);
            await Task.Delay(200);
        }

        private void UpdateNetModel()
        {
            AviableNetworkInterfaces = NetworkTools.GetNetworkInterfaces(OperationalStatus.Up, DefaulNetworksInterfacesTypes.ToArray());
            AviableIPv4 = NetworkTools.GetLocalIPv4(OperationalStatus.Up, DefaulNetworksInterfacesTypes.ToArray());
        }
    }
}
