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
using NieroNetLib.Types;
using Niero.SupportClasses;
using System.Windows.Input;
using System.Windows.Media.Effects;
using System.Text.RegularExpressions;
using System.ComponentModel;

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
                    if (CursPosRelToBorder != null)
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
                RightUp.X = LeftUp.X + DataOwner.ActualWidth;
                RightUp.Y = LeftUp.Y;
                RightDown.X = RightUp.X;
                RightDown.Y = RightUp.Y + DataOwner.ActualHeight;
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

        ColorAnimation ColorToDark, ColorToBase;
        Storyboard FlashingTextSB;

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
            CanvasBorders.Add(new BorderWithPositionInfo((Border)m_Page.FindName("WorkingAdaptersInfo")));

            StoryboardsInit();

            SubscribeBordersToEvents(CanvasBorders[0].DataOwner, CanvasBorders[1].DataOwner);

            UpdateNetModel();

            DataUpdateLoop();
        }

        private void StoryboardsInit()
        {
           
            ColorToDark = new ColorAnimation((Color)m_Page.FindResource("OverlayDarkColor"), new Duration(new TimeSpan(0, 0, 0, 0, 750)));
            ColorToBase = new ColorAnimation((Color)m_Page.FindResource("BaseColor"), new Duration(new TimeSpan(0, 0, 0, 0, 750)));
        }

        private void SubscribeBordersToEvents(params Border[] borders)
        {
            foreach (Border border in borders)
            {
                border.MouseLeftButtonDown += Border_MouseLeftButtonDown;
                border.MouseLeftButtonUp += Border_MouseLeftButtonUp;
                border.MouseMove += Border_MouseMove;
                border.MouseEnter += Border_MouseEnter;
                border.MouseLeave += Border_MouseLeave;
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            m_Page.Cursor = CustomCursors.Move;

            foreach(BorderWithPositionInfo itBorderInfo in CanvasBorders)
            {
                if(itBorderInfo.DataOwner != sender as Border)
                {
                    Panel.SetZIndex(itBorderInfo.DataOwner, 0);
                }
                if(itBorderInfo.DataOwner == sender as Border)
                {
                    DraggableBorder = itBorderInfo; 
                    DraggingActive = true;
                }
            }

            Panel.SetZIndex(DraggableBorder.DataOwner, 1);

            ColorAnimation animColor = new ColorAnimation((Color)m_Page.FindResource("RedColor"), new Duration(new TimeSpan(0, 0, 0, 0, 750)));
            MainBorder.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, animColor);
            
            //save element position before offset   
            DraggableBorder.CursPosRelativeToParent = e.GetPosition(MainCanvas);
            DraggableBorder.CursPosRelativeToBorder = e.GetPosition(DraggableBorder.DataOwner);

            DraggableBorder.DataOwner.CaptureMouse();
        }
        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        { 
            m_Page.Cursor = CustomCursors.Normal_Select;

            foreach (BorderWithPositionInfo itBorderInfo in CanvasBorders)
            {
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

            //save shift info
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
        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (DraggingActive)
            {
                foreach (BorderWithPositionInfo itBorderInfo in CanvasBorders)
                {
                    if (itBorderInfo.DataOwner == sender as Border)
                    {
                        DraggableBorder = itBorderInfo;
                    }
                }
            }

            if (DraggingActive && DraggableBorder != null)
            {
                Vector MoveVector;
                Point CurrentPosition = e.GetPosition(MainCanvas);

                //Calculation of the vector of displacement FROM the last captured(saved) position
                //(first position capture in the event Border_MouseLeftButtonDown)
                MoveVector.X = CurrentPosition.X - DraggableBorder.CursPosRelativeToParent.X;
                MoveVector.Y = CurrentPosition.Y - DraggableBorder.CursPosRelativeToParent.Y;
                
                TranslateTransform Transform = new TranslateTransform();
                DraggableBorder.DataOwner.RenderTransform = Transform;

                Transform.X = MoveVector.X;
                Transform.Y = MoveVector.Y;
                Transform.X += DraggableBorder.AdditionalShiftX;
                Transform.Y += DraggableBorder.AdditionalShiftY;

                //save element position after offset
                DraggableBorder.CursPosRelativeToParent = e.GetPosition(MainCanvas);
                DraggableBorder.CursPosRelativeToBorder = e.GetPosition(DraggableBorder.DataOwner);

                if (Transform != null)
                {
                    DraggableBorder.AdditionalShiftX = Transform.X;
                    DraggableBorder.AdditionalShiftY = Transform.Y;
                }

                //Check for taking out the canvas abroad
                if (DraggableBorder.LeftUp.X < -10 || DraggableBorder.LeftUp.Y < -10 || 
                   DraggableBorder.RightDown.X > MainCanvas.ActualWidth + 10 ||
                   DraggableBorder.RightDown.Y > MainCanvas.ActualHeight + 10)
                {
                    MouseButtonEventArgs args = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);
                    args.RoutedEvent = Border.MouseLeftButtonUpEvent;
                    DraggableBorder.DataOwner.RaiseEvent(args);
                }
            }
        }
        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            Border SenderBorder = sender as Border;
            Grid ChildsGrid = SenderBorder.Child as Grid;

            foreach (UIElement GridChild in ChildsGrid.Children)
            {
                if (GridChild.GetType() == typeof(StackPanel))
                {
                    foreach (UIElement StackPanelChild in (GridChild as StackPanel).Children)
                    {
                        if (StackPanelChild.GetType() == typeof(TextBox))
                        {
                            if ((StackPanelChild as TextBox).Tag == null)
                                (StackPanelChild as TextBox).BeginStoryboard((Storyboard)m_Page.TryFindResource("ColorsInversionToActive"));
                        }
                    }
                }
                else if (GridChild.GetType() == typeof(TextBox))
                {
                    if ((GridChild as TextBox).Tag == null)
                        (GridChild as TextBox).BeginStoryboard((Storyboard)m_Page.TryFindResource("ColorsInversionToActive"));
                }
            }
        }
        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            Border SenderBorder = sender as Border;
            Grid ChildsGrid = SenderBorder.Child as Grid;

            foreach (UIElement GridChild in ChildsGrid.Children)
            {
                if (GridChild.GetType() == typeof(StackPanel))
                {
                    foreach (UIElement StackPanelChild in (GridChild as StackPanel).Children)
                    {
                        if (StackPanelChild.GetType() == typeof(TextBox))
                        {
                            if ((StackPanelChild as TextBox).Tag == null) 
                            {
                                (StackPanelChild as TextBox).BeginStoryboard((Storyboard)m_Page.TryFindResource("ColorsInversionToInactive"));
                            }
                            else 
                            {
                                (StackPanelChild as TextBox).BeginStoryboard((Storyboard)m_Page.TryFindResource("ColorsInversionToInactive"));
                                (StackPanelChild as TextBox).Tag = null;                        //another animation unlocked
                            }
                        }
                    }
                }
                else if (GridChild.GetType() == typeof(TextBox))
                {
                    if ((GridChild as TextBox).Tag == null) 
                    {
                        (GridChild as TextBox).BeginStoryboard((Storyboard)m_Page.TryFindResource("ColorsInversionToInactive"));
                    }
                    else
                    {
                        (GridChild as TextBox).BeginStoryboard((Storyboard)m_Page.TryFindResource("ColorsInversionToInactive"));
                        (GridChild as TextBox).Tag = null;                                       //another animation unlocked
                    }
                }
            }
        }

        private async Task DataUpdateLoop()
        {
            while (true){
                UpdateNetModel();
                if (DraggableBorder?.DataOwner.Name != "DeviceInfo" && CanvasBorders[0].DataOwner.IsMouseOver != true) await UpdateDeviceInfoBorder();
                if (DraggableBorder?.DataOwner.Name != "WorkingAdaptersInfo" && CanvasBorders[1].DataOwner.IsMouseOver != true) await UpdateWorkingAdaptersInfoBorder();
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

            await AnimateBorderUpdate_Start(DeviceInfo);

            TextBox DeviceNameOutput = (TextBox)DeviceInfo.FindName(nameof(DeviceNameOutput));
            StackPanel IPv4Output = (StackPanel)DeviceInfo.FindName(nameof(IPv4Output));

            DeviceNameOutput.Foreground = new SolidColorBrush((Color)m_Page.TryFindResource("DarkTextColor"));

            DeviceNameOutput.MouseEnter += TextBox_MouseEnter;
            DeviceNameOutput.MouseLeave += TextBox_MouseLeave;

            DeviceNameOutput.Text = NetworkTools.GetMyDeviceName();

            IPv4Output.Children.Clear();

            foreach (IPAddress ip in AviableIPv4)
            {
                TextBox IpAdressTB = new TextBox();

                IpAdressTB.Style = (Style)m_Page.TryFindResource("MoveTextBox");

                IpAdressTB.Text = ip.ToString();

                IPv4Output.Children.Add(IpAdressTB);

                IpAdressTB.MouseEnter += TextBox_MouseEnter;
                IpAdressTB.MouseLeave += TextBox_MouseLeave;
            }

            await AnimateBorderUpdate_Finish(DeviceInfo);
        }
        private async Task UpdateWorkingAdaptersInfoBorder()
        {
            Border WorkingAdaptersInfo = null;
            foreach (BorderWithPositionInfo itBorderInfo in CanvasBorders)
            {
                if (itBorderInfo.DataOwner.Name == nameof(WorkingAdaptersInfo))
                    WorkingAdaptersInfo = itBorderInfo.DataOwner;
            }

            WorkingAdaptersInfo.Height = WorkingAdaptersInfo.ActualHeight;
            WorkingAdaptersInfo.Width = WorkingAdaptersInfo.ActualWidth;

            await AnimateBorderUpdate_Start(WorkingAdaptersInfo);

            StackPanel AdaptersNamesOutput = (StackPanel)WorkingAdaptersInfo.FindName(nameof(AdaptersNamesOutput));
            StackPanel IpAdressesOutput = (StackPanel)WorkingAdaptersInfo.FindName(nameof(IpAdressesOutput));
            StackPanel MacAdressesOutput = (StackPanel)WorkingAdaptersInfo.FindName(nameof(MacAdressesOutput));

            AdaptersNamesOutput.Children.Clear();
            IpAdressesOutput.Children.Clear();
            MacAdressesOutput.Children.Clear();

            foreach (NetworkInterface inter in AviableNetworkInterfaces)
            {
                Separator sp1 = new Separator();
                Separator sp2 = new Separator();
                Separator sp3 = new Separator();
                TextBox AdaptersNamesTB = new TextBox();
                TextBox IpAdressTB = new TextBox();
                TextBox MACAdressTB = new TextBox();

                sp1.Background = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                sp2.Background = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                sp3.Background = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                AdaptersNamesTB.Style = (Style)m_Page.TryFindResource("MoveTextBox");
                IpAdressTB.Style = (Style)m_Page.TryFindResource("MoveTextBox");
                MACAdressTB.Style = (Style)m_Page.TryFindResource("MoveTextBox");

                AdaptersNamesTB.MouseEnter += TextBox_MouseEnter;
                AdaptersNamesTB.MouseLeave += TextBox_MouseLeave;
                IpAdressTB.MouseEnter += TextBox_MouseEnter;
                IpAdressTB.MouseLeave += TextBox_MouseLeave;
                MACAdressTB.MouseEnter += TextBox_MouseEnter;
                MACAdressTB.MouseLeave += TextBox_MouseLeave;

                BasicInterfaceInfo interfaceInfo = new BasicInterfaceInfo(inter);
                AdaptersNamesTB.Text += interfaceInfo.Name;
                IpAdressTB.Text += interfaceInfo.IPv4;
                MACAdressTB.Text += interfaceInfo.MacAddress;

                AdaptersNamesOutput.Children.Add(sp1);
                IpAdressesOutput.Children.Add(sp2);
                MacAdressesOutput.Children.Add(sp3);
                AdaptersNamesOutput.Children.Add(AdaptersNamesTB);
                IpAdressesOutput.Children.Add(IpAdressTB);
                MacAdressesOutput.Children.Add(MACAdressTB);

                WorkingAdaptersInfo.Height = IpAdressesOutput.Height;
                WorkingAdaptersInfo.Width = AdaptersNamesOutput.Width + IpAdressesOutput.Width + MacAdressesOutput.Width;
            }

            await AnimateBorderUpdate_Finish(WorkingAdaptersInfo);
        }

        private void TextBox_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBox SenderTB = sender as TextBox;

            FlashingTextSB = new Storyboard();
            PropertyPath PropPath = new PropertyPath("(TextBox.Foreground).(SolidColorBrush.Color)");
            Storyboard.SetTargetProperty(ColorToDark, PropPath);
            FlashingTextSB.Children.Add(ColorToDark);
            FlashingTextSB.AutoReverse = true;
            FlashingTextSB.RepeatBehavior = RepeatBehavior.Forever;

            if ((SenderTB.Foreground as SolidColorBrush).Color != (Color)m_Page.FindResource("BaseColor"))
            {
                SenderTB.Foreground = new SolidColorBrush((Color)m_Page.FindResource("BaseColor"));
            }

            SenderTB.Tag = false;                                                                  //another animation locked
            FlashingTextSB.Begin(SenderTB, true);
        }              //     !!!you have bound a lot of objects to these two methods!!!
        private void TextBox_MouseLeave(object sender, MouseEventArgs e)
        {
            TextBox SenderTB = sender as TextBox;

            FlashingTextSB.Stop(SenderTB);
            SenderTB.BeginStoryboard((Storyboard)m_Page.TryFindResource("ColorsInversionToActive"));
        }              //     !!!the execution sequence may fail!!!

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

            if(border.IsMouseOver == true)
            {
                MouseEventArgs args = new MouseEventArgs(Mouse.PrimaryDevice, 0);
                args.RoutedEvent = Border.MouseEnterEvent;
                border.RaiseEvent(args);
            }
        }

        private void UpdateNetModel()
        {
            AviableNetworkInterfaces = NetworkTools.GetNetworkInterfaces(OperationalStatus.Up, DefaulNetworksInterfacesTypes.ToArray());
            AviableIPv4 = NetworkTools.GetLocalIPv4(OperationalStatus.Up, DefaulNetworksInterfacesTypes.ToArray());
        }
    }
}
