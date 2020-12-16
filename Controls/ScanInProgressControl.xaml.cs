using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;


namespace Niero.Controls
{
    public partial class ScanInProgressControl : UserControl
    {
        private DoubleAnimation OpenOpacityTo1, OpenMove, CloseOpacityTo0, CloseMove;
        private DoubleAnimation OpenButtonBarMove, LeftCloseMove, LeftOpenMove;
        private DoubleAnimation OpenScaleX, OpenScaleY, CloseScaleX, CloseScaleY, OpacityTo0, OpacityTo1;

        bool IsCanceled;
        
        public ScanInProgressControl()
        {
            Opacity = 0;
            InitializeComponent();
            AnimationsInit();

            IsCanceled = false;

            ResultsButton.Visibility = Visibility.Hidden;
            CancelButton.Click += CancelButton_Click;
            ResultsButton.Click += ResultsButton_Click;
        }

        public void OpenButtonBar()
        {
            this.Dispatcher.Invoke(() =>
            {
                ScanInitSettings.BeginAnimation(Border.HeightProperty, OpenButtonBarMove);
                CancelButton.BeginAnimation(Button.OpacityProperty, OpacityTo1);
            });
        }
        public void ChangeButtonOnBar()
        {
            this.Dispatcher.Invoke(() =>
            {
                TransformGroup tg = new TransformGroup();
                ScaleTransform s = new ScaleTransform();
                TranslateTransform t = new TranslateTransform();

                tg.Children.Add(s);
                tg.Children.Add(t);

                CancelButton.RenderTransform = tg;
                s.BeginAnimation(ScaleTransform.ScaleXProperty, CloseScaleX);
                s.BeginAnimation(ScaleTransform.ScaleYProperty, CloseScaleY);
            });
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            OnCanceled();
        }
        private void ResultsButton_Click(object sender, RoutedEventArgs e)
        {
            CloseOpacityTo0.Completed += CloseOpacityTo0_Completed;
            BeginCloseAnimation();
        }

        //Basic Animations part
        private void AnimationsInit()
        {
            OpenOpacityTo1 = new DoubleAnimation(1, new Duration(new TimeSpan(0, 0, 0, 0, 1000)));
            OpenOpacityTo1.Completed += OpenOpacityTo1_Completed;
            OpenMove = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 1000)));

            CloseOpacityTo0 = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 1000)));
            CloseOpacityTo0.Completed += CloseOpacityTo0_Completed;
            CloseMove = new DoubleAnimation(100, new Duration(new TimeSpan(0, 0, 0, 0, 1000)));

            OpenButtonBarMove = new DoubleAnimation(330, new Duration(new TimeSpan(0, 0, 0, 0, 1000)));

            OpacityTo0 = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 1000)));
            OpacityTo1 = new DoubleAnimation(1, new Duration(new TimeSpan(0, 0, 0, 0, 1000)));

            LeftOpenMove = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 500)));
            LeftCloseMove = new DoubleAnimation(70, new Duration(new TimeSpan(0, 0, 0, 0, 500)));
            LeftOpenMove.Completed += LeftOpenMove_Completed;
            LeftCloseMove.Completed += LeftCloseMove_Completed;

            OpenScaleX = new DoubleAnimation(1, new Duration(new TimeSpan(0, 0, 0, 0, 300)));
            OpenScaleY = new DoubleAnimation(1, new Duration(new TimeSpan(0, 0, 0, 0, 300)));

            CloseScaleX = new DoubleAnimation(0.9, new Duration(new TimeSpan(0, 0, 0, 0, 300)));
            CloseScaleY = new DoubleAnimation(0.9, new Duration(new TimeSpan(0, 0, 0, 0, 300)));
            CloseScaleY.Completed += CloseScaleY_Completed;
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
        }
        private void CloseOpacityTo0_Completed(object sender, EventArgs e)
        {
            OnCloseAnimationCompleted();
            if (IsCanceled) OnCanceled();
            else OnClosed();
        }

        private void CloseScaleY_Completed(object sender, EventArgs e)
        {
            CancelButton.BeginAnimation(Button.OpacityProperty, OpacityTo0);
            (CancelButton.RenderTransform as TransformGroup).Children[1] = new TranslateTransform();
            (CancelButton.RenderTransform as TransformGroup).Children[1].BeginAnimation(TranslateTransform.XProperty, LeftCloseMove);
        }
        private void LeftCloseMove_Completed(object sender, EventArgs e)
        {
            CancelButton.Visibility = Visibility.Hidden;
            ResultsButton.Visibility = Visibility.Visible;
            TransformGroup tg = new TransformGroup();
            ScaleTransform s = new ScaleTransform(0.9, 0.9);
            TranslateTransform t = new TranslateTransform(-70, 0);

            tg.Children.Add(s);
            tg.Children.Add(t);

            ResultsButton.RenderTransform = tg;
            ResultsButton.BeginAnimation(Button.OpacityProperty, OpenOpacityTo1);
            t.BeginAnimation(TranslateTransform.XProperty, LeftOpenMove);
        }
        private void LeftOpenMove_Completed(object sender, EventArgs e)
        {
            (ResultsButton.RenderTransform as TransformGroup).Children[0] = new ScaleTransform(0.9, 0.9);
            ((ResultsButton.RenderTransform as TransformGroup).Children[0] as ScaleTransform).BeginAnimation(ScaleTransform.ScaleXProperty, OpenScaleX);
            ((ResultsButton.RenderTransform as TransformGroup).Children[0] as ScaleTransform).BeginAnimation(ScaleTransform.ScaleYProperty, OpenScaleY);
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
        private void OnCanceled()
        {
            Canceled?.Invoke(this);
        }

        public event OpenAnimationCompletedEventHandler OpenAnimationCompleted;
        public event OpenAnimationBegunEventHandler OpenAnimationBegun;
        public event CloseAnimationCompletedEventHandler CloseAnimationCompleted;
        public event CloseAnimationBegunEventHandler CloseAnimationBegun;
        public event ClosedEventHandler Closed;
        public event CanceledEventHandler Canceled;

        public delegate void OpenAnimationCompletedEventHandler(object sender);
        public delegate void OpenAnimationBegunEventHandler(object sender);
        public delegate void CloseAnimationCompletedEventHandler(object sender);
        public delegate void CloseAnimationBegunEventHandler(object sender);
        public delegate void ClosedEventHandler(object sender);
        public delegate void CanceledEventHandler(object sender);
    }
}
