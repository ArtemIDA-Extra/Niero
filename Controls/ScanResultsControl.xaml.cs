using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Niero.Controls
{
    public partial class ScanResultsControl : UserControl
    {
        private DoubleAnimation OpenOpacityTo1, OpenMove, CloseOpacityTo0, CloseMove;
        private DoubleAnimation OpenButtonBarMove, OpacityTo0, OpacityTo1;

        public ScanResultsControl()
        {
            Opacity = 0;
            InitializeComponent();
            AnimationsInit();

            AgainButton.Click += AgainButton_Click;
            IpListButton.Click += IpListButton_Click;
        }

        public void OpenButtonBar()
        {
            ScanResults.BeginAnimation(Border.HeightProperty, OpenButtonBarMove);
        }
        private void AgainButton_Click(object sender, RoutedEventArgs e)
        {
            BeginCloseAnimation();
            OnAgainChosen();
        }
        private void IpListButton_Click(object sender, RoutedEventArgs e)
        {
            BeginCloseAnimation();
            OnIpListChosen();
        }

        private void AnimationsInit()
        {
            OpenOpacityTo1 = new DoubleAnimation(1, new Duration(new TimeSpan(0, 0, 0, 0, 1000)));
            OpenOpacityTo1.Completed += OpenOpacityTo1_Completed;
            OpenMove = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 1000)));

            CloseOpacityTo0 = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 1000)));
            CloseOpacityTo0.Completed += CloseOpacityTo0_Completed;
            CloseMove = new DoubleAnimation(100, new Duration(new TimeSpan(0, 0, 0, 0, 1000)));

            OpenButtonBarMove = new DoubleAnimation(380, new Duration(new TimeSpan(0, 0, 0, 0, 1000)));
            OpenButtonBarMove.Completed += OpenButtonBarMove_Completed;

            OpacityTo0 = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 1000)));
            OpacityTo1 = new DoubleAnimation(1, new Duration(new TimeSpan(0, 0, 0, 0, 1000)));
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
        private void OpenButtonBarMove_Completed(object sender, EventArgs e)
        {
            AgainButton.BeginAnimation(Button.OpacityProperty, OpacityTo1);
            IpListButton.BeginAnimation(Button.OpacityProperty, OpacityTo1);
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
