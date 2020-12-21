using System;
using System.Media;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows;
using System.Net;
using System.Net.NetworkInformation;
using NieroNetLib;

namespace Niero.Controls
{
    public partial class ScanResultsControl : UserControl
    {
        private DoubleAnimation OpenOpacityTo1, OpenMove, CloseOpacityTo0, CloseMove;
        private DoubleAnimation OpenButtonBarMove, OpenIpListButtonBarMove, CloseButtonBarMove, CloseIpListButtonBarMove, ButtonsOpacityTo0, ButtonsOpacityTo1;
        private DoubleAnimationUsingKeyFrames SwapResultXMove, SwapResultYMove, SwapIpListXMove, SwapIpListYMove;
        private DoubleAnimation SwapOpacityTo0, SwapOpacityTo1;

        private enum MyStates
        {
            ResultsDisplaying,
            IpListDisplaying
        }
        private MyStates State;

        public ScanResultsControl()
        {
            Opacity = 0;
            State = MyStates.ResultsDisplaying;
            InitializeComponent();
            AnimationsInit();

            AgainButton.Click += AgainButton_Click;
            IpListButton.Click += IpListButton_Click;
            BackButton.Click += BackButton_Click;
        }

        private void OpenButtonBar()
        {
            ScanResults.BeginAnimation(Border.HeightProperty, OpenButtonBarMove);
        }
        private void CloseButtonBar()
        {
            AgainButton.BeginAnimation(Button.OpacityProperty, ButtonsOpacityTo0);
            IpListButton.BeginAnimation(Button.OpacityProperty, ButtonsOpacityTo0);
        }
        private void OpenIpListButtonBar()
        {
            IpList.BeginAnimation(Border.HeightProperty, OpenIpListButtonBarMove);
        }
        private void CloseIpListButtonBar()
        {
            BackButton.BeginAnimation(Button.OpacityProperty, ButtonsOpacityTo0);
        }

        private void AgainButton_Click(object sender, RoutedEventArgs e)
        {
            BeginCloseAnimation();
            OnAgainChosen();
        }
        private void IpListButton_Click(object sender, RoutedEventArgs e)
        {
            IpList_MainGrid.Children.Clear();
            foreach ((IPAddress, PingReply) ipRep in (DataContext as LocalNetScan).ScanResult)
            {
                TextBlock tempIpTB = new TextBlock();
                tempIpTB.Style = (Style)TryFindResource("BreakCopyTextBlock");
                tempIpTB.Text = ipRep.Item1.ToString();
                tempIpTB.MouseDown += SpecCopyMouseDown_e;
                IpList_MainGrid.Children.Add(tempIpTB);
            }
            CloseButtonBar();
            OnIpListChosen();
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            CloseIpListButtonBar();
        }

        private void AnimationsInit()
        {
            OpenOpacityTo1 = new DoubleAnimation(1, new Duration(new TimeSpan(0, 0, 0, 0, 1000)));
            OpenOpacityTo1.Completed += OpenOpacityTo1_Completed;
            OpenMove = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 1000)));

            CloseOpacityTo0 = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 1000)));
            CloseOpacityTo0.Completed += CloseOpacityTo0_Completed;
            CloseMove = new DoubleAnimation(100, new Duration(new TimeSpan(0, 0, 0, 0, 1000)));

            OpenButtonBarMove = new DoubleAnimation(400, new Duration(new TimeSpan(0, 0, 0, 0, 1000)));
            OpenButtonBarMove.Completed += OpenButtonBarMove_Completed;
            CloseButtonBarMove = new DoubleAnimation(360, new Duration(new TimeSpan(0, 0, 0, 0, 1000)));
            CloseButtonBarMove.Completed += CloseButtonBarMove_Completed;

            OpenIpListButtonBarMove = new DoubleAnimation(400, new Duration(new TimeSpan(0, 0, 0, 0, 1000)));
            OpenIpListButtonBarMove.Completed += OpenIpListButtonBarMove_Completed;
            CloseIpListButtonBarMove = new DoubleAnimation(360, new Duration(new TimeSpan(0, 0, 0, 0, 1000)));
            CloseIpListButtonBarMove.Completed += CloseIpListButtonBarMove_Completed;

            ButtonsOpacityTo0 = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 1000)));
            ButtonsOpacityTo0.Completed += ButtonsOpacityTo0_Completed;
            ButtonsOpacityTo1 = new DoubleAnimation(1, new Duration(new TimeSpan(0, 0, 0, 0, 1000)));

            SwapResultXMove = new DoubleAnimationUsingKeyFrames();
            SwapResultXMove.KeyFrames.Add(new EasingDoubleKeyFrame(-200, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 500))));
            SwapResultXMove.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 1000))));
            SwapResultYMove = new DoubleAnimationUsingKeyFrames();
            SwapResultYMove.KeyFrames.Add(new EasingDoubleKeyFrame(200, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 500))));
            SwapResultYMove.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 1000))));

            SwapIpListXMove = new DoubleAnimationUsingKeyFrames();
            SwapIpListXMove.KeyFrames.Add(new EasingDoubleKeyFrame(200, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 500))));
            SwapIpListXMove.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 1000))));
            SwapIpListYMove = new DoubleAnimationUsingKeyFrames();
            SwapIpListYMove.KeyFrames.Add(new EasingDoubleKeyFrame(-200, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 500))));
            SwapIpListYMove.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 1000))));
            SwapIpListYMove.Completed += SwapIpListYMove_Completed;

            SwapOpacityTo0 = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 750)));
            SwapOpacityTo1 = new DoubleAnimation(1, new Duration(new TimeSpan(0, 0, 0, 0, 750)));
        }

        //Animations caller-functions
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
        private void BeginSwapAnimation()
        {
            IpList.Visibility = Visibility.Visible;
            TranslateTransform t = new TranslateTransform();
            ScanResults.RenderTransform = t;

            TranslateTransform t2 = new TranslateTransform();
            IpList.RenderTransform = t2;

            IpList.BeginAnimation(Border.OpacityProperty, SwapOpacityTo1);
            ScanResults.BeginAnimation(Border.OpacityProperty, SwapOpacityTo0);

            t.BeginAnimation(TranslateTransform.XProperty, SwapResultXMove);
            t.BeginAnimation(TranslateTransform.YProperty, SwapResultYMove);

            t2.BeginAnimation(TranslateTransform.XProperty, SwapIpListXMove);
            t2.BeginAnimation(TranslateTransform.YProperty, SwapIpListYMove);
        }
        private void BeginBackSwapAnimation()
        {
            ScanResults.Visibility = Visibility.Visible;
            TranslateTransform t = new TranslateTransform();
            ScanResults.RenderTransform = t;

            TranslateTransform t2 = new TranslateTransform();
            IpList.RenderTransform = t2;

            IpList.BeginAnimation(Border.OpacityProperty, SwapOpacityTo0);
            ScanResults.BeginAnimation(Border.OpacityProperty, SwapOpacityTo1);

            t.BeginAnimation(TranslateTransform.XProperty, SwapResultXMove);
            t.BeginAnimation(TranslateTransform.YProperty, SwapResultYMove);

            t2.BeginAnimation(TranslateTransform.XProperty, SwapIpListXMove);
            t2.BeginAnimation(TranslateTransform.YProperty, SwapIpListYMove);
        }

        //Animations complete handlers
        private void OpenOpacityTo1_Completed(object sender, EventArgs e)
        {
            OnOpenAnimationCompleted();
            OpenButtonBar();
        }
        private void CloseOpacityTo0_Completed(object sender, EventArgs e)
        {
            OnCloseAnimationCompleted();
            OnClosed();
        }

        private void SwapIpListYMove_Completed(object sender, EventArgs e)
        {
            if (State == MyStates.ResultsDisplaying)
            {
                ScanResults.Visibility = Visibility.Hidden;
                State = MyStates.IpListDisplaying;
                OpenIpListButtonBar();
            }
            else if (State == MyStates.IpListDisplaying)
            {
                IpList.Visibility = Visibility.Hidden;
                State = MyStates.ResultsDisplaying;
                OpenButtonBar();
            }
        }

        private void OpenButtonBarMove_Completed(object sender, EventArgs e)
        {
            AgainButton.BeginAnimation(Button.OpacityProperty, ButtonsOpacityTo1);
            IpListButton.BeginAnimation(Button.OpacityProperty, ButtonsOpacityTo1);
        }
        private void OpenIpListButtonBarMove_Completed(object sender, EventArgs e)
        {
            BackButton.BeginAnimation(Button.OpacityProperty, ButtonsOpacityTo1);
        }

        private void ButtonsOpacityTo0_Completed(object sender, EventArgs e)
        {
            if (State == MyStates.ResultsDisplaying)
            {
                ScanResults.BeginAnimation(Border.HeightProperty, CloseButtonBarMove);
            }
            else if (State == MyStates.IpListDisplaying)
            {
                IpList.BeginAnimation(Border.HeightProperty, CloseIpListButtonBarMove);
            }
        }

        private void CloseButtonBarMove_Completed(object sender, EventArgs e)
        {
            BeginSwapAnimation();
        }
        private void CloseIpListButtonBarMove_Completed(object sender, EventArgs e)
        {
            BeginBackSwapAnimation();
        }

        //Copy part
        private void CopyMouseDown_e(object sender, MouseButtonEventArgs e)
        {
            SoundPlayer SP = new SoundPlayer("EmbeddedSounds/Copied.wav");
            Clipboard.SetText((sender as TextBlock).Text);
            SP.Play();
            (sender as TextBlock).BeginStoryboard((Storyboard)TryFindResource("ForegroundColorToTransparentFlashST"));
        }
        private void SpecCopyMouseDown_e(object sender, MouseButtonEventArgs e)
        {
            SoundPlayer SP = new SoundPlayer("EmbeddedSounds/Copied.wav");
            Clipboard.SetText((sender as TextBlock).Text);
            SP.Play();
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
        private void OnAgainChosen()
        {
            AgainChosen?.Invoke(this);
        }
        private void OnIpListChosen()
        {
            IpListChosen?.Invoke(this);
        }
        private void OnClosed()
        {
            Closed?.Invoke(this);
        }

        public event OpenAnimationCompletedEventHandler OpenAnimationCompleted;
        public event OpenAnimationBegunEventHandler OpenAnimationBegun;
        public event CloseAnimationCompletedEventHandler CloseAnimationCompleted;
        public event CloseAnimationBegunEventHandler CloseAnimationBegun;
        public event AgainChosenEventHandler AgainChosen;
        public event IpListChosenEventHandler IpListChosen;
        public event ClosedEventHandler Closed;

        public delegate void OpenAnimationCompletedEventHandler(object sender);
        public delegate void OpenAnimationBegunEventHandler(object sender);
        public delegate void CloseAnimationCompletedEventHandler(object sender);
        public delegate void CloseAnimationBegunEventHandler(object sender);
        public delegate void AgainChosenEventHandler(object sender);
        public delegate void IpListChosenEventHandler(object sender);
        public delegate void ClosedEventHandler(object sender);
    }
}
