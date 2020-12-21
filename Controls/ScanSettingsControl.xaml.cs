using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;

namespace Niero.Controls
{
    public partial class ScanSettingsControl : UserControl
    {
        private ColorAnimation ColorToRed, ColorToBase, ColorToDarkText;
        private DoubleAnimation TimeoutTB_OpacityTo0, NetMaskTB_OpacityTo0, OpacityTo1, OpenOpacityTo1, OpenMove, CloseOpacityTo0, CloseMove;
        private DoubleAnimation SwapOpacityTo0, SwapOpacityTo1;
        private DoubleAnimationUsingKeyFrames TBShaking;

        private int ClickTimes = 0;
        private bool IsTextBoxVisible = false;

        public ScanSettingsControl()
        {
            Opacity = 0;
            InitializeComponent();

            TimeoutTB.PreviewKeyDown += TB_PreviewKeyDown;
            TimeoutTB.PreviewTextInput += TimeoutTB_PreviewTextInput;
            DataObject.AddPastingHandler(TimeoutTB, TimeoutTB_Paste);             
            TimeoutTB.MouseLeave += TB_MouseLeave;
            TimeoutTB.MouseEnter += TB_MouseEnter;
            TimeoutTB.LostFocus += TB_LostFocus;
            TimeoutTB.GotFocus += TB_GotFocus;

            NetMaskCB.MouseDoubleClick += Swap_MouseDoubleClick;
            NetMaskTB.MouseDoubleClick += Swap_MouseDoubleClick;

            NetMaskTB.PreviewKeyDown += TB_PreviewKeyDown;
            NetMaskTB.PreviewTextInput += NetMaskTB_PreviewTextInput;
            DataObject.AddPastingHandler(NetMaskTB, NetMaskTB_Paste);
            NetMaskTB.MouseLeave += TB_MouseLeave;
            NetMaskTB.MouseEnter += TB_MouseEnter;
            NetMaskTB.LostFocus += TB_LostFocus;
            NetMaskTB.GotFocus += TB_GotFocus;

            ModeButton.Click += ModeButton_Click;
            RunButton.Click += RunButton_Click;

            AnimationInit();
        }

        private void TB_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back) BackspacePressed();
            if (e.Key == Key.Enter) EnterPressed();
        }
        private void TB_MouseLeave(object sender, MouseEventArgs e)
        {
            ColorAnimation anim = new ColorAnimation((Color)TryFindResource("BaseColor"), new Duration(new TimeSpan(0, 0, 0, 0, 300)));
            if ((sender as TextBox).IsFocused != true)
                ((sender as TextBox).BorderBrush as SolidColorBrush).BeginAnimation(SolidColorBrush.ColorProperty, anim);
        }                              //part to new TextBox class
        private void TB_MouseEnter(object sender, MouseEventArgs e)
        {
            ColorAnimation anim = new ColorAnimation((Color)TryFindResource("DarkTextColor"), new Duration(new TimeSpan(0, 0, 0, 0, 300)));
            ((sender as TextBox).BorderBrush as SolidColorBrush).BeginAnimation(SolidColorBrush.ColorProperty, anim);
        }                              //part to new TextBox class
        private void TB_LostFocus(object sender, RoutedEventArgs e)
        {
            ColorAnimation anim = new ColorAnimation((Color)TryFindResource("BaseColor"), new Duration(new TimeSpan(0, 0, 0, 0, 300)));
            ((sender as TextBox).BorderBrush as SolidColorBrush).BeginAnimation(SolidColorBrush.ColorProperty, anim);
        }                              //part to new TextBox class
        private void TB_GotFocus(object sender, RoutedEventArgs e)
        {
            ColorAnimation anim = new ColorAnimation((Color)TryFindResource("DarkTextColor"), new Duration(new TimeSpan(0, 0, 0, 0, 300)));
            ((sender as TextBox).BorderBrush as SolidColorBrush).BeginAnimation(SolidColorBrush.ColorProperty, anim);
        }                               //part to new TextBox class

        private void Swap_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!IsTextBoxVisible)
            {
                NetMaskTB.Visibility = Visibility.Visible;
                NetMaskCB.BeginAnimation(ComboBox.OpacityProperty, SwapOpacityTo0);
                NetMaskTB.BeginAnimation(TextBox.OpacityProperty, SwapOpacityTo1);
            }
            else if (IsTextBoxVisible)
            {
                NetMaskCB.Visibility = Visibility.Visible;
                NetMaskCB.BeginAnimation(ComboBox.OpacityProperty, SwapOpacityTo1);
                NetMaskTB.BeginAnimation(TextBox.OpacityProperty, SwapOpacityTo0);
            }
        }

        private void ClickOnBorder(object sender, MouseButtonEventArgs e)
        {
            ScanInitSettings.Focus();
        }

        private void TimeoutTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!IsNumbers(e.Text) == true)
                e.Handled = true;
            else
                e.Handled = false;
        }
        private void TimeoutTB_Paste(object sender, DataObjectPastingEventArgs e)
        {
            string PastText = e.DataObject.GetData(DataFormats.UnicodeText, true) as string;
            if (!IsNumbers(PastText))
            {
                e.CancelCommand();
                BeginWrongTextAnimation(TimeoutTB);
            }
        }

        private void NetMaskTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!IsIpPart(e.Text) == true)
                e.Handled = true;
            else
                e.Handled = false;
        }
        private void NetMaskTB_Paste(object sender, DataObjectPastingEventArgs e)
        {
            string PastText = e.DataObject.GetData(DataFormats.UnicodeText, true) as string;
            if (!IsIp(PastText))
            {
                e.CancelCommand();
                BeginWrongTextAnimation(NetMaskTB);
            }
        }

        private void ModeButton_Click(object sender, RoutedEventArgs e)
        {
            switch (ClickTimes) 
            {
                case 0: ModeButton.Content = "Don't touch, please!"; ModeButton.IsChecked = false; break;
                case 1: ModeButton.Content = "You just missed..."; ModeButton.IsChecked = false; break;
                case 2: ModeButton.Content = "You really?"; ModeButton.IsChecked = false; break;
                case 3: ModeButton.Content = "Its bad idea. Trust me."; ModeButton.IsChecked = false; break;
                case 4: ModeButton.Content = "You"; ModeButton.IsChecked = false; break;
                case 5: ModeButton.Content = "Abnormal"; ModeButton.IsChecked = false; break;
                case 6: ModeButton.Content = "Yes?"; ModeButton.IsChecked = false; break;
                case 7: ModeButton.Content = "A kodilom po boshke?!"; ModeButton.IsChecked = false; break;
                case 8: ModeButton.Content = "I did everything I could..."; ModeButton.IsChecked = false; break;
            }
            if(ClickTimes > 8)
            {
                if (ModeButton.IsChecked == false) ModeButton.Content = "Off";
                if (ModeButton.IsChecked == true) ModeButton.Content = "On";
            }
            ClickTimes++;
        }
        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            if (GatewayCB.SelectedItem != null && IsNumbers(TimeoutTB.Text))
            {
                if (NetMaskTB.Visibility == Visibility.Visible)
                {
                    if (IsIp(NetMaskTB.Text)) BeginCloseAnimation();
                    else BeginWrongTextAnimation(NetMaskTB);
                }
                else
                {
                    BeginCloseAnimation();
                }
            }
            if (!IsNumbers(TimeoutTB.Text)) BeginWrongTextAnimation(TimeoutTB);
        }

        //Support functions
        void EnterPressed()
        { 
        }                                                                             //part to new TextBox class
        void BackspacePressed()
        {
            SoundPlayer SP = new SoundPlayer("EmbeddedSounds/BackspacePress.wav");
            SP.Play();
        }                                                                         //part to new TextBox class

        private static bool IsIp(string text)
        {
            Regex _regex = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
            return _regex.IsMatch(text);
        }
        private static bool IsIpPart(string text)
        {
            Regex _regex = new Regex("^[0-9.]+$");
            return _regex.IsMatch(text);
        }
        private static bool IsNumbers(string text)
        {
            Regex _regex = new Regex("^[0-9]+$");
            return _regex.IsMatch(text);
        }

        //Animation part
        private void AnimationInit()
        {
            ColorToRed = new ColorAnimation((Color)TryFindResource("RedColor"), new Duration(new TimeSpan(0, 0, 0, 0, 100)));
            ColorToBase = new ColorAnimation((Color)TryFindResource("BaseColor"), new Duration(new TimeSpan(0, 0, 0, 0, 100)));
            ColorToDarkText = new ColorAnimation((Color)TryFindResource("DarkTextColor"), new Duration(new TimeSpan(0, 0, 0, 0, 100)));

            OpenOpacityTo1 = new DoubleAnimation(1, new Duration(new TimeSpan(0, 0, 0, 0, 1000)));
            OpenOpacityTo1.Completed += OpenOpacityTo1_Completed;
            OpenMove = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 1000)));

            TimeoutTB_OpacityTo0 = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 300)));
            TimeoutTB_OpacityTo0.Completed += TimeoutTB_OpacityTo0_Completed;
            NetMaskTB_OpacityTo0 = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 300)));
            NetMaskTB_OpacityTo0.Completed += NetMaskTB_OpacityTo0_Completed;
            OpacityTo1 = new DoubleAnimation(1, new Duration(new TimeSpan(0, 0, 0, 0, 0)));

            CloseOpacityTo0 = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 1000)));
            CloseOpacityTo0.Completed += CloseOpacityTo0_Completed;
            CloseMove = new DoubleAnimation(100, new Duration(new TimeSpan(0, 0, 0, 0, 1000)));

            SwapOpacityTo0 = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 500)));
            SwapOpacityTo0.Completed += SwapOpacityTo0_Completed;
            SwapOpacityTo1 = new DoubleAnimation(1, new Duration(new TimeSpan(0, 0, 0, 0, 500)));

            TBShaking = new DoubleAnimationUsingKeyFrames();
            TBShaking.KeyFrames = new DoubleKeyFrameCollection
            {
                new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(new TimeSpan(0,0,0,0,0))),
                new EasingDoubleKeyFrame(-5, KeyTime.FromTimeSpan(new TimeSpan(0,0,0,0,50))),
                new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(new TimeSpan(0,0,0,0,100))),
                new EasingDoubleKeyFrame(5, KeyTime.FromTimeSpan(new TimeSpan(0,0,0,0,150))),
                new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(new TimeSpan(0,0,0,0,300))),
            };
        }

        private void BeginWrongTextAnimation(TextBox tb)
        {
            SoundPlayer SP = new SoundPlayer("EmbeddedSounds/ErrorShort.wav");
            SP.Play();
            if(tb == TimeoutTB) tb.BeginAnimation(TextBox.OpacityProperty, TimeoutTB_OpacityTo0);
            else if (tb == NetMaskTB) tb.BeginAnimation(TextBox.OpacityProperty, NetMaskTB_OpacityTo0);
            tb.Background = new SolidColorBrush((tb.Background as SolidColorBrush).Color);
            (tb.Background as SolidColorBrush).BeginAnimation(SolidColorBrush.ColorProperty, ColorToRed);
            (tb.BorderBrush as SolidColorBrush).BeginAnimation(SolidColorBrush.ColorProperty, ColorToRed);

            TranslateTransform t = new TranslateTransform();
            tb.RenderTransform = t;

            t.BeginAnimation(TranslateTransform.XProperty, TBShaking);
        }
        public void BeginOpenAnimation()
        {
            OnOpenAnimationBegun();

            TranslateTransform t = new TranslateTransform();
            RenderTransform = t;
            t.Y = -100;

            this.BeginAnimation(UserControl.OpacityProperty, OpenOpacityTo1);
            t.BeginAnimation(TranslateTransform.YProperty, OpenMove);
        }
        public void BeginCloseAnimation()
        {
            OnCloseAnimationBegun();

            TranslateTransform t = new TranslateTransform();
            this.RenderTransform = t;

            this.BeginAnimation(UserControl.OpacityProperty, CloseOpacityTo0);
            t.BeginAnimation(TranslateTransform.YProperty, CloseMove);
        }

        //Animations complete handlers
        private void TimeoutTB_OpacityTo0_Completed(object sender, EventArgs e)
        {
            TimeoutTB.Text = null;
            if (TimeoutTB.IsFocused)
            {
                (TimeoutTB.Background as SolidColorBrush).BeginAnimation(SolidColorBrush.ColorProperty, ColorToDarkText);
                (TimeoutTB.BorderBrush as SolidColorBrush).BeginAnimation(SolidColorBrush.ColorProperty, ColorToDarkText);
            }
            else
            {
                if(TimeoutTB.IsMouseOver) (TimeoutTB.BorderBrush as SolidColorBrush).BeginAnimation(SolidColorBrush.ColorProperty, ColorToDarkText);
                else (TimeoutTB.BorderBrush as SolidColorBrush).BeginAnimation(SolidColorBrush.ColorProperty, ColorToBase);
                TimeoutTB.Background= new SolidColorBrush(Colors.Transparent);
            }
            TimeoutTB.BeginAnimation(TextBox.OpacityProperty, OpacityTo1);
        }
        private void NetMaskTB_OpacityTo0_Completed(object sender, EventArgs e)
        {
            NetMaskTB.Text = null;
            if (NetMaskTB.IsFocused)
            {
                (NetMaskTB.Background as SolidColorBrush).BeginAnimation(SolidColorBrush.ColorProperty, ColorToDarkText);
                (NetMaskTB.BorderBrush as SolidColorBrush).BeginAnimation(SolidColorBrush.ColorProperty, ColorToDarkText);
            }
            else
            {
                if (NetMaskTB.IsMouseOver) (NetMaskTB.BorderBrush as SolidColorBrush).BeginAnimation(SolidColorBrush.ColorProperty, ColorToDarkText);
                else (NetMaskTB.BorderBrush as SolidColorBrush).BeginAnimation(SolidColorBrush.ColorProperty, ColorToBase);
                NetMaskTB.Background = new SolidColorBrush(Colors.Transparent);
            }
            NetMaskTB.BeginAnimation(TextBox.OpacityProperty, OpacityTo1);
        }
        private void OpenOpacityTo1_Completed(object sender, EventArgs e)
        {
            OnOpenAnimationCompleted();
        }
        private void CloseOpacityTo0_Completed(object sender, EventArgs e)
        {
            IPAddress NetMask;
            if (NetMaskTB.Visibility == Visibility.Visible) NetMask = IPAddress.Parse(NetMaskTB.Text);
            else NetMask = NetMaskCB.SelectedItem as IPAddress;
            SettingsConfirmedEventArgs args = new SettingsConfirmedEventArgs(GatewayCB.SelectedItem as IPAddress,
                                                                             NetMask,
                                                                             int.Parse(TimeoutTB.Text),
                                                                             ModeButton.IsChecked.Value);
            OnSettingsConfirmed(args);
            OnCloseAnimationCompleted();
            OnClosed();
        }
        private void SwapOpacityTo0_Completed(object sender, EventArgs e)
        {
            if (!IsTextBoxVisible)
            {
                NetMaskCB.Visibility = Visibility.Hidden;
                IsTextBoxVisible = true;
            }
            else if (IsTextBoxVisible)
            {
                NetMaskTB.Visibility = Visibility.Hidden;
                IsTextBoxVisible = false;
            }
        }

        //Invoke-functions
        private void OnSettingsConfirmed(SettingsConfirmedEventArgs args)
        {
            SettingsConfirmed?.Invoke(this, args);
        }
        private void OnOpenAnimationCompleted()
        {
            OpenAnimationCompleted?.Invoke(this);
        }
        private void OnOpenAnimationBegun()
        {
            OpenAnimationBegun?.Invoke(this);
        }
        private void OnCloseAnimationCompleted()
        {
            CloseAnimationCompleted?.Invoke(this);
        }
        private void OnCloseAnimationBegun()
        {
            CloseAnimationBegun?.Invoke(this);
        }
        private void OnClosed()
        {
            Closed?.Invoke(this);
        }

        //Events
        public event SettingsConfirmedEventHandler SettingsConfirmed;
        public event OpenAnimationCompletedEventHandler OpenAnimationCompleted;
        public event OpenAnimationBegunEventHandler OpenAnimationBegun;
        public event CloseAnimationCompletedEventHandler CloseAnimationCompleted;
        public event CloseAnimationBegunEventHandler CloseAnimationBegun;
        public event ClosedEventHandler Closed;

        //Events handlers
        public delegate void SettingsConfirmedEventHandler(object sender, SettingsConfirmedEventArgs e);
        public delegate void OpenAnimationCompletedEventHandler(object sender);
        public delegate void OpenAnimationBegunEventHandler(object sender);
        public delegate void CloseAnimationCompletedEventHandler(object sender);
        public delegate void CloseAnimationBegunEventHandler(object sender);
        public delegate void ClosedEventHandler(object sender);

        //Events args
        public class SettingsConfirmedEventArgs : EventArgs
        {
            public IPAddress Gateway { get; set; }
            public IPAddress NetMask { get; set; }
            public int Timeout { get; set; }
            public bool IsAltModeOn { get; set; }

            public SettingsConfirmedEventArgs(IPAddress gateway, IPAddress netMask, int timeout, bool isAltModeOn) 
            {
                Gateway = gateway; NetMask = netMask; Timeout = timeout; IsAltModeOn = isAltModeOn;
            }
        }
    }
}
