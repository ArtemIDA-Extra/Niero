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
        private DoubleAnimation OpacityTo0, OpacityTo1, OpenOpacityTo1, OpenMove, CloseOpacityTo0, CloseMove;
        private DoubleAnimationUsingKeyFrames TBShaking;

        public ScanSettingsControl()
        {
            Opacity = 0;
            InitializeComponent();

            //Focusable = true;

            //Mouse.AddPreviewMouseDownHandler(ScanInitSettings, ClickOnBorder);

            TimeoutTB.PreviewKeyDown += TimeoutTB_PreviewKeyDown;
            TimeoutTB.PreviewTextInput += TB_PreviewTextInput;
            DataObject.AddPastingHandler(TimeoutTB, TimeoutTB_Paste);             //Disabled WrongText animation
            TimeoutTB.MouseLeave += TimeoutTB_MouseLeave;
            TimeoutTB.MouseEnter += TimeoutTB_MouseEnter;
            TimeoutTB.LostFocus += TimeoutTB_LostFocus;
            TimeoutTB.GotFocus += TimeoutTB_GotFocus;

            RunButton.Click += RunButton_Click;

            AnimationInit();
        }

        private void TimeoutTB_MouseLeave(object sender, MouseEventArgs e)
        {
            ColorAnimation anim = new ColorAnimation((Color)TryFindResource("BaseColor"), new Duration(new TimeSpan(0, 0, 0, 0, 300)));
            if (TimeoutTB.IsFocused != true)
                (TimeoutTB.BorderBrush as SolidColorBrush).BeginAnimation(SolidColorBrush.ColorProperty, anim);
        }                              //part to new TextBox class
        private void TimeoutTB_MouseEnter(object sender, MouseEventArgs e)
        {
            ColorAnimation anim = new ColorAnimation((Color)TryFindResource("DarkTextColor"), new Duration(new TimeSpan(0, 0, 0, 0, 300)));
            (TimeoutTB.BorderBrush as SolidColorBrush).BeginAnimation(SolidColorBrush.ColorProperty, anim);
        }                              //part to new TextBox class
        private void TimeoutTB_LostFocus(object sender, RoutedEventArgs e)
        {
            ColorAnimation anim = new ColorAnimation((Color)TryFindResource("BaseColor"), new Duration(new TimeSpan(0, 0, 0, 0, 300)));
            (TimeoutTB.BorderBrush as SolidColorBrush).BeginAnimation(SolidColorBrush.ColorProperty, anim);
        }                              //part to new TextBox class
        private void TimeoutTB_GotFocus(object sender, RoutedEventArgs e)
        {
            ColorAnimation anim = new ColorAnimation((Color)TryFindResource("DarkTextColor"), new Duration(new TimeSpan(0, 0, 0, 0, 300)));
            (TimeoutTB.BorderBrush as SolidColorBrush).BeginAnimation(SolidColorBrush.ColorProperty, anim);
        }                               //part to new TextBox class

        private void ClickOnBorder(object sender, MouseButtonEventArgs e)
        {
            ScanInitSettings.Focus();
        }

        private void TimeoutTB_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back) BackspacePressed();
            if (e.Key == Key.Enter) EnterPressed();
        }                                    //part to new TextBox class
        private void TB_PreviewTextInput(object sender, TextCompositionEventArgs e)
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

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            if (GatewayCB.SelectedItem != null && IsNumbers(TimeoutTB.Text))
            {
                BeginCloseAnimation();
            }
            if (!IsNumbers(TimeoutTB.Text))
            {
                BeginWrongTextAnimation(TimeoutTB);
            }
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

            OpacityTo0 = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 300)));
            OpacityTo0.Completed += OpacityTo0_Completed;
            OpacityTo1 = new DoubleAnimation(1, new Duration(new TimeSpan(0, 0, 0, 0, 0)));

            CloseOpacityTo0 = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 1000)));
            CloseOpacityTo0.Completed += CloseOpacityTo0_Completed;
            CloseMove = new DoubleAnimation(100, new Duration(new TimeSpan(0, 0, 0, 0, 1000)));

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
            tb.BeginAnimation(TextBox.OpacityProperty, OpacityTo0);
            tb.Background = new SolidColorBrush((tb.Background as SolidColorBrush).Color);
            (tb.Background as SolidColorBrush).BeginAnimation(SolidColorBrush.ColorProperty, ColorToRed);
            (tb.BorderBrush as SolidColorBrush).BeginAnimation(SolidColorBrush.ColorProperty, ColorToRed);

            TranslateTransform t = new TranslateTransform();
            TimeoutTB.RenderTransform = t;

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
        private void OpacityTo0_Completed(object sender, EventArgs e)
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
        private void OpenOpacityTo1_Completed(object sender, EventArgs e)
        {
            OnOpenAnimationCompleted();
        }
        private void CloseOpacityTo0_Completed(object sender, EventArgs e)
        {
            SettingsConfirmedEventArgs args = new SettingsConfirmedEventArgs(GatewayCB.SelectedItem as IPAddress,
                                                                 NetMaskCB.SelectedItem as IPAddress,
                                                                 int.Parse(TimeoutTB.Text));
            OnSettingsConfirmed(args);
            OnCloseAnimationCompleted();
            OnClosed();
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

            public SettingsConfirmedEventArgs(IPAddress gateway, IPAddress netMask, int timeout) 
            {
                Gateway = gateway; NetMask = netMask; Timeout = timeout;
            }
        }
    }
}
