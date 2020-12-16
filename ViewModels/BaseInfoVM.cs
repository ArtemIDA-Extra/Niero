using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Input;
using NieroNetLib;
using NieroNetLib.Types;
using Niero.SupportClasses;
using Niero.Models;


namespace Niero.ViewModels
{
    class BaseInfoVM : BaseViewModel
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

                AdditionalShiftX = AdditionalShiftY = 0;

                if ((borderOwner.RenderTransform as TranslateTransform) != null)
                {
                    AdditionalShiftX = (borderOwner.RenderTransform as TranslateTransform).X; //Canvas.GetLeft(borderOwner);
                    AdditionalShiftY = (borderOwner.RenderTransform as TranslateTransform).Y; //Canvas.GetTop(borderOwner);
                }
                if (!double.IsNaN(Canvas.GetLeft(borderOwner))) AdditionalShiftX = Canvas.GetLeft(borderOwner);
                if (!double.IsNaN(Canvas.GetTop(borderOwner))) AdditionalShiftY = Canvas.GetTop(borderOwner);
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
        Canvas MainCanvas;
        Border MainBorder;

        List<BorderWithPositionInfo> CanvasBorders = new List<BorderWithPositionInfo>();

        ColorAnimation ColorToDark, ColorToBase;
        Storyboard FlashingTextSB;

        //Net data
        NetworkDataHub DataHub;

        private class DGAdapterInfo
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

        //CONSTRUCTOR!!!
        public BaseInfoVM(Page page, NetworkDataHub dataHub) : base(page)
        {
            DataHub = dataHub;

            MainCanvas = (Canvas)m_Page.FindName(nameof(MainCanvas));
            MainBorder = (Border)m_Page.FindName(nameof(MainBorder));

            CanvasBorders.Add(new BorderWithPositionInfo((Border)m_Page.FindName("DeviceInfo")));
            CanvasBorders.Add(new BorderWithPositionInfo((Border)m_Page.FindName("WorkingAdaptersInfo")));
            CanvasBorders.Add(new BorderWithPositionInfo((Border)m_Page.FindName("DisabledAdaptersInfo")));

            DeviceInfoBorderInit();

            StoryboardsInit();

            SubscribeBordersToEvents(CanvasBorders[0].DataOwner);
            SubscribeDGBordersToEvents(CanvasBorders[1].DataOwner, CanvasBorders[2].DataOwner);

            DataUpdateLoop();
        }

        //Elements init functions
        private void DeviceInfoBorderInit()
        {
            Border DeviceInfo = null;
            foreach (BorderWithPositionInfo itBorderInfo in CanvasBorders)
            {
                if (itBorderInfo.DataOwner.Name == nameof(DeviceInfo))
                    DeviceInfo = itBorderInfo.DataOwner;
            }

            TextBox DeviceNameOutput = (TextBox)DeviceInfo.FindName(nameof(DeviceNameOutput));
            StackPanel IPv4Output = (StackPanel)DeviceInfo.FindName(nameof(IPv4Output));

            DeviceNameOutput.Cursor = CustomCursors.Text_Select;
            DeviceNameOutput.Foreground = new SolidColorBrush((Color)m_Page.TryFindResource("DarkTextColor"));

            DeviceNameOutput.MouseEnter += TextBox_MouseEnter;
            DeviceNameOutput.MouseLeave += TextBox_MouseLeave;
        }

        private void StoryboardsInit()
        {
            ColorToDark = new ColorAnimation((Color)m_Page.FindResource("BaseColor"), (Color)m_Page.FindResource("OverlayDarkColor"), new Duration(new TimeSpan(0, 0, 0, 0, 750)));
            ColorToBase = new ColorAnimation((Color)m_Page.FindResource("BaseColor"), new Duration(new TimeSpan(0, 0, 0, 0, 750)));

            FlashingTextSB = new Storyboard();
            PropertyPath PropPath = new PropertyPath("(TextBox.Foreground).(SolidColorBrush.Color)");
            Storyboard.SetTargetProperty(ColorToDark, PropPath);
            FlashingTextSB.Children.Add(ColorToDark);
            FlashingTextSB.AutoReverse = true;
            FlashingTextSB.RepeatBehavior = RepeatBehavior.Forever;
        }

        //Subscribe elements to events
        private void SubscribeBordersToEvents(params Border[] borders)
        {
            foreach (Border border in borders)
            {
                border.MouseLeftButtonDown += AnyBorder_MouseLeftButtonDown;
                border.MouseLeftButtonUp += AnyBorder_MouseLeftButtonUp;
                border.MouseMove += AnyBorder_MouseMove;
                border.MouseEnter += Border_MouseEnter;
                border.MouseLeave += Border_MouseLeave;
            }
        }
        private void SubscribeDGBordersToEvents(params Border[] borders)
        {
            foreach (Border border in borders)
            {
                border.MouseLeftButtonDown += AnyBorder_MouseLeftButtonDown;
                border.MouseLeftButtonUp += AnyBorder_MouseLeftButtonUp;
                border.MouseMove += AnyBorder_MouseMove;
                border.MouseEnter += DGBorder_MouseEnter;
                border.MouseLeave += DGBorder_MouseLeave;

                DataGrid BorderDG = (((border.Child as Grid).Children[1] as Grid).Children[1] as DataGrid);
                BorderDG.SelectedCellsChanged += DataGrid_SelectedCellsChanged;
            }
        }
        private void SubscribeDGCellsToEvents(DataGrid dataGrid)
        {
            List<DependencyObject> DGCells = VisualTreeSearch.SearchChildren(dataGrid, typeof(DataGridCell));

            if (DGCells != null)
            {
                foreach (DataGridCell cell in DGCells)
                {
                    cell.MouseEnter += DGCell_MouseEnter;
                    cell.MouseLeave += DGCell_MouseLeave;
                    cell.MouseDoubleClick += DGCell_MouseDoubleClick;
                }
            }
        }
        private void SubscribeBTextBlocksToEvents(Border infoBorder)
        {
            List<DependencyObject> BorderTBs = VisualTreeSearch.SearchChildren(infoBorder, typeof(TextBox));

            if(BorderTBs != null)
            {
                foreach(TextBox tb in BorderTBs)
                {
                    tb.MouseDoubleClick += TextBox_MouseDoubleClick;
                }
            }
        }

        //Events handlers
        private void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText((sender as TextBox).Text);
            //(sender as TextBox).BeginStoryboard((Storyboard)m_Page.TryFindResource("ForegroundColorToInactiveFlashST"));
        }

        private void DGCell_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as DataGridCell).Background = new SolidColorBrush(((sender as DataGridCell).Background as SolidColorBrush).Color);
            ((sender as DataGridCell).Background as SolidColorBrush).BeginAnimation(SolidColorBrush.ColorProperty, (ColorAnimation)m_Page.TryFindResource("SolidColorBrushColorToActiveAN"));
            ((sender as DataGridCell).Content as TextBlock).Foreground = (SolidColorBrush)m_Page.TryFindResource("OverlayDarkBrush");
        }
        private void DGCell_MouseLeave(object sender, MouseEventArgs e)
        {
            //(sender as DataGridCell).Background = Brushes.Transparent;
            (sender as DataGridCell).Background = new SolidColorBrush(((sender as DataGridCell).Background as SolidColorBrush).Color);
            ((sender as DataGridCell).Background as SolidColorBrush).BeginAnimation(SolidColorBrush.ColorProperty, (ColorAnimation)m_Page.TryFindResource("SolidColorBrushColorToTransparentAN"));
            ((sender as DataGridCell).Content as TextBlock).Foreground = (SolidColorBrush)m_Page.TryFindResource("BaseBrush");
        }
        private void DGCell_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(((sender as DataGridCell).Content as TextBlock).Text);
            (sender as DataGridCell).Background = new SolidColorBrush(((sender as DataGridCell).Background as SolidColorBrush).Color);
            (sender as DataGridCell).BeginStoryboard((Storyboard)m_Page.TryFindResource("BackgroundColorToInactiveFlashST"));
        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            (sender as DataGrid).UnselectAll();  //DISABLE Fu*** ROW SELECT!
        }

        private void AnyBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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

            //DraggableBorder.DataOwner.CaptureMouse();
            ((DraggableBorder.DataOwner.Child as Grid).Children[1] as Grid).CaptureMouse();
        }
        private void AnyBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
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

            //DraggableBorder.DataOwner.ReleaseMouseCapture();
            ((DraggableBorder.DataOwner.Child as Grid).Children[1] as Grid).ReleaseMouseCapture();
            DraggableBorder = null;

            ColorAnimation animColor = new ColorAnimation((Color)m_Page.FindResource("BaseColor"), new Duration(new TimeSpan(0, 0, 0, 0, 500)));
            MainBorder.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, animColor);
        }
        private void AnyBorder_MouseMove(object sender, MouseEventArgs e)
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
            Grid BorderGrid = SenderBorder.Child as Grid;

            foreach (UIElement ChildGridChildren in (BorderGrid.Children[1] as Grid).Children)
            {
                if (ChildGridChildren.GetType() == typeof(TextBlock))
                {
                    (ChildGridChildren as TextBlock).BeginStoryboard((Storyboard)m_Page.TryFindResource("ForegroundColorToActiveST"));
                }
                else if (ChildGridChildren.GetType() == typeof(TextBox))
                {
                    if ((ChildGridChildren as TextBox).Tag == null)
                        (ChildGridChildren as TextBox).BeginStoryboard((Storyboard)m_Page.TryFindResource("ForegroundColorToActiveST"));
                }
                else if (ChildGridChildren.GetType() == typeof(StackPanel))
                {
                    foreach (UIElement StackPanelChild in (ChildGridChildren as StackPanel).Children)
                    {
                        if (StackPanelChild.GetType() == typeof(TextBox))
                        {
                            if ((StackPanelChild as TextBox).Tag == null)
                                (StackPanelChild as TextBox).BeginStoryboard((Storyboard)m_Page.TryFindResource("ForegroundColorToActiveST"));
                        }
                    }
                }
            }
            DoubleAnimation FillAnim = new DoubleAnimation(BorderGrid.ActualWidth, new Duration(new TimeSpan(0, 0, 0, 0, 200)));
            (BorderGrid.Children[0] as Grid).BeginAnimation(Grid.WidthProperty, FillAnim);
        }
        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            Border SenderBorder = sender as Border;
            Grid BorderGrid = SenderBorder.Child as Grid;

            foreach (UIElement ChildGridChildren in (BorderGrid.Children[1] as Grid).Children)
            {
                if (ChildGridChildren.GetType() == typeof(TextBlock))
                {
                    (ChildGridChildren as TextBlock).BeginStoryboard((Storyboard)m_Page.TryFindResource("ForegroundColorToBlackST"));
                }
                else if (ChildGridChildren.GetType() == typeof(TextBox))
                {
                    (ChildGridChildren as TextBox).BeginStoryboard((Storyboard)m_Page.TryFindResource("ForegroundColorToInactiveST"));
                    (ChildGridChildren as TextBox).Tag = null;
                }
                else if (ChildGridChildren.GetType() == typeof(StackPanel))
                {
                    foreach (UIElement StackPanelChild in (ChildGridChildren as StackPanel).Children)
                    {
                        if (StackPanelChild.GetType() == typeof(TextBox))
                        {
                            (StackPanelChild as TextBox).BeginStoryboard((Storyboard)m_Page.TryFindResource("ForegroundColorToInactiveST"));
                            (StackPanelChild as TextBox).Tag = null;
                        }
                    }
                }
            }
            DoubleAnimation FillAnim = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 200)));
            (BorderGrid.Children[0] as Grid).BeginAnimation(Grid.WidthProperty, FillAnim);
            /*Border SenderBorder = sender as Border;
            Grid ChildsGrid = SenderBorder.Child as Grid;

            foreach (UIElement GridChild in ChildsGrid.Children)
            {
                if (GridChild.GetType() == typeof(StackPanel))
                {
                    foreach (UIElement StackPanelChild in (GridChild as StackPanel).Children)
                    {
                        if (StackPanelChild.GetType() == typeof(TextBox))
                        {
                            (StackPanelChild as TextBox).BeginStoryboard((Storyboard)m_Page.TryFindResource("ColorsInversionToInactive"));
                            (StackPanelChild as TextBox).Tag = null;
                        }
                    }
                }
                else if (GridChild.GetType() == typeof(TextBox))
                {
                    (GridChild as TextBox).BeginStoryboard((Storyboard)m_Page.TryFindResource("ColorsInversionToInactive"));
                    (GridChild as TextBox).Tag = null;
                }
            }*/
        }
        private void DGBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            Border SenderBorder = sender as Border;
            Grid BorderGrid = SenderBorder.Child as Grid;

            ((BorderGrid.Children[1] as Grid).Children[0] as TextBlock).BeginStoryboard((Storyboard)m_Page.TryFindResource("ForegroundColorToActiveST"));

            List<DependencyObject> DGtb = VisualTreeSearch.SearchChildren((BorderGrid.Children[1] as Grid).Children[1] as DataGrid, typeof(TextBlock));
            for (int i = 0; i < ((BorderGrid.Children[1] as Grid).Children[1] as DataGrid).Columns.Count; i++)
            {
                (DGtb[i] as TextBlock).Foreground = new SolidColorBrush(((DGtb[i] as TextBlock).Foreground as SolidColorBrush).Color);
                (DGtb[i] as TextBlock).BeginStoryboard((Storyboard)m_Page.TryFindResource("ForegroundColorToActiveST"));
            }
            for (int i = ((BorderGrid.Children[1] as Grid).Children[1] as DataGrid).Columns.Count; i < DGtb.Count; i++)
            {
                (DGtb[i] as TextBlock).Foreground = new SolidColorBrush(((DGtb[i] as TextBlock).Foreground as SolidColorBrush).Color);
                (DGtb[i] as TextBlock).BeginStoryboard((Storyboard)m_Page.TryFindResource("ForegroundColorToActiveST"));
            }

            DoubleAnimation FillAnim = new DoubleAnimation(BorderGrid.ActualWidth, new Duration(new TimeSpan(0, 0, 0, 0, 200)));
            (BorderGrid.Children[0] as Grid).BeginAnimation(Grid.WidthProperty, FillAnim);
        }
        private void DGBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            Border SenderBorder = sender as Border;
            Grid BorderGrid = SenderBorder.Child as Grid;

            ((BorderGrid.Children[1] as Grid).Children[0] as TextBlock).BeginStoryboard((Storyboard)m_Page.TryFindResource("ForegroundColorToBlackST"));

            List<DependencyObject> DGtb = VisualTreeSearch.SearchChildren((BorderGrid.Children[1] as Grid).Children[1] as DataGrid, typeof(TextBlock));
            for(int i = 0; i < ((BorderGrid.Children[1] as Grid).Children[1] as DataGrid).Columns.Count; i++)
            {
                (DGtb[i] as TextBlock).Foreground = new SolidColorBrush(((DGtb[i] as TextBlock).Foreground as SolidColorBrush).Color);
                (DGtb[i] as TextBlock).BeginStoryboard((Storyboard)m_Page.TryFindResource("ForegroundColorToBlackST"));
            }
            for (int i = ((BorderGrid.Children[1] as Grid).Children[1] as DataGrid).Columns.Count; i < DGtb.Count; i++)
            {
                (DGtb[i] as TextBlock).Foreground = new SolidColorBrush(((DGtb[i] as TextBlock).Foreground as SolidColorBrush).Color);
                (DGtb[i] as TextBlock).BeginStoryboard((Storyboard)m_Page.TryFindResource("ForegroundColorToInactiveST"));
            }

            DoubleAnimation FillAnim = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 200)));
            (BorderGrid.Children[0] as Grid).BeginAnimation(Grid.WidthProperty, FillAnim);
        }

        private async Task DataUpdateLoop()
        {
            while (true){
                if (DraggableBorder?.DataOwner.Name != "DeviceInfo" && CanvasBorders[0].DataOwner.IsMouseOver != true) await UpdateDeviceInfoBorder();
                if (DraggableBorder?.DataOwner.Name != "WorkingAdaptersInfo" && CanvasBorders[1].DataOwner.IsMouseOver != true) await UpdateWorkingAdaptersInfoBorder();
                if (DraggableBorder?.DataOwner.Name != "DisabledAdaptersInfo" && CanvasBorders[2].DataOwner.IsMouseOver != true) await UpdateDisabledAdaptersInfoBorder();
                await Task.Delay(5000);
            }
        }

        //Update info on borders (Based on DataHub)
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

            DeviceNameOutput.Text = DataHub.DeviceName;

            IPv4Output.Children.Clear();

            foreach (IPAddress ip in DataHub.AviableIPv4)
            {
                TextBox IpAdressTB = new TextBox();

                IpAdressTB.Cursor = CustomCursors.Text_Select;
                IpAdressTB.Style = (Style)m_Page.TryFindResource("MoveTextBox");

                IpAdressTB.Text = ip.ToString();

                IPv4Output.Children.Add(IpAdressTB);

                IpAdressTB.MouseEnter += TextBox_MouseEnter;
                IpAdressTB.MouseLeave += TextBox_MouseLeave;
            }

            await AnimateBorderUpdate_Finish(DeviceInfo);

            SubscribeBTextBlocksToEvents(DeviceInfo);
        }
        private async Task UpdateWorkingAdaptersInfoBorder()
        {
            Border WorkingAdaptersInfo = null;
            foreach (BorderWithPositionInfo itBorderInfo in CanvasBorders)
            {
                if (itBorderInfo.DataOwner.Name == nameof(WorkingAdaptersInfo))
                    WorkingAdaptersInfo = itBorderInfo.DataOwner;
            }

            await AnimateBorderUpdate_Start(WorkingAdaptersInfo);

            DataGrid WorkingAdaptersDG = (DataGrid)m_Page.FindName(nameof(WorkingAdaptersDG));

            WorkingAdaptersDG.ItemsSource = DataHub.EnabledAdapters;

            await AnimateBorderUpdate_Finish(WorkingAdaptersInfo);

            SubscribeDGCellsToEvents(WorkingAdaptersDG);
        }
        private async Task UpdateDisabledAdaptersInfoBorder()
        {
            Border DisabledAdaptersInfo = null;
            foreach (BorderWithPositionInfo itBorderInfo in CanvasBorders)
            {
                if (itBorderInfo.DataOwner.Name == nameof(DisabledAdaptersInfo))
                    DisabledAdaptersInfo = itBorderInfo.DataOwner;
            }

            await AnimateBorderUpdate_Start(DisabledAdaptersInfo);

            DataGrid DisabledAdaptersDG = (DataGrid)m_Page.FindName(nameof(DisabledAdaptersDG));

            DisabledAdaptersDG.ItemsSource = DataHub.DisabledAdapters;

            await AnimateBorderUpdate_Finish(DisabledAdaptersInfo);

            SubscribeDGCellsToEvents(DisabledAdaptersDG);
        }

        private void TextBox_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBox SenderTB = sender as TextBox;

            SenderTB.Tag = false;                                                                  //another animation locked
            FlashingTextSB.Begin(SenderTB, true);
        }              //     !!!you have bound a lot of objects to these two handlers!!!
        private void TextBox_MouseLeave(object sender, MouseEventArgs e)
        {
            TextBox SenderTB = sender as TextBox;

            FlashingTextSB.Stop(SenderTB);
            SenderTB.BeginStoryboard((Storyboard)m_Page.TryFindResource("ForegroundColorToActiveST"));
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
    }
}
