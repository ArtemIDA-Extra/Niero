using System;
using System.Collections.Generic;
using System.Text;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Niero.SupportClasses;

namespace Niero.Controls
{
    public partial class MatherboardInfoControl : UserControl
    {
        public string MatherboardMaker;
        public string MatherboardModel;

        List<DependencyObject> TextBlocks;

        private DoubleAnimation OpenOpacityTo1, UpdateOpacityTo1, UpdateOpacityTo0;

        public MatherboardInfoControl()
        {
            Opacity = 0;
            InitializeComponent();
            AnimationsInit();

            Loaded += Loaded_e;
            MouseEnter += MouseEnter_e;
            MouseLeave += MouseLeave_e;

            MatherboardMakerOutput.MouseDown += CopyMouseDown_e;
            MatherboardModelOutput.MouseDown += CopyMouseDown_e;
        }

        private void AnimationsInit()
        {
            OpenOpacityTo1 = new DoubleAnimation(1, new Duration(new TimeSpan(0, 0, 0, 0, 750)));
            OpenOpacityTo1.Completed += OpenOpacityTo1_Completed;
            UpdateOpacityTo0 = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 200)));
            UpdateOpacityTo0.Completed += UpdateOpacityTo0_Completed;
            UpdateOpacityTo1 = new DoubleAnimation(1, new Duration(new TimeSpan(0, 0, 0, 0, 200)));
            UpdateOpacityTo1.Completed += UpdateOpacityTo1_Completed;
        }

        private void UpdateData()
        {
            MatherboardMakerOutput.Text = MatherboardMaker;
            MatherboardModelOutput.Text = MatherboardModel;
        }
        private void SwapTextColorToBase()
        {
            foreach (TextBlock TB in TextBlocks)
            {
                TB.BeginStoryboard((Storyboard)TryFindResource("ForegroundColorToActiveST"));
            }
        }
        private void SwapTextColorToDark()
        {
            foreach (TextBlock TB in TextBlocks)
            {
                if ((TB.Tag as string) == "Black") TB.BeginStoryboard((Storyboard)TryFindResource("ForegroundColorToBlackST"));
                else TB.BeginStoryboard((Storyboard)TryFindResource("ForegroundColorToInactiveST"));
            }
        }

        private void Loaded_e(object sender, RoutedEventArgs e)
        {
            TextBlocks = VisualTreeSearch.SearchChildren(ContentPartGrid, typeof(TextBlock));
        }
        //Mouse moves handlers
        private void MouseEnter_e(object sender, MouseEventArgs e)
        {
            DoubleAnimation FillAnim = new DoubleAnimation(ContentPartGrid.ActualWidth, new Duration(new TimeSpan(0, 0, 0, 0, 200)));
            AnimatedPartGrid.BeginAnimation(Grid.WidthProperty, FillAnim);
            SwapTextColorToBase();
        }
        private void MouseLeave_e(object sender, MouseEventArgs e)
        {
            DoubleAnimation UnfillAnim = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 200)));
            AnimatedPartGrid.BeginAnimation(Grid.WidthProperty, UnfillAnim);
            SwapTextColorToDark();
        }

        //Copy To Clip part
        private void CopyMouseDown_e(object sender, MouseButtonEventArgs e)
        {
            SoundPlayer SP = new SoundPlayer("EmbeddedSounds/Copied.wav");
            Clipboard.SetText((sender as TextBlock).Text);
            SP.Play();
            (sender as TextBlock).BeginStoryboard((Storyboard)TryFindResource("ForegroundColorToTransparentFlashST"));
        }

        //Animations caller-functions
        public void BeginOpenAnimation()
        {
            OnOpenAnimationBegun();

            this.BeginAnimation(UserControl.OpacityProperty, OpenOpacityTo1);
        }
        public void BeginUpdateAnimation()
        {
            this.Dispatcher.Invoke(() =>
            {
                OnUpdateAnimationBegun();
                this.BeginAnimation(UserControl.OpacityProperty, UpdateOpacityTo0);
            });
        }

        //Animations complete handlers
        private void OpenOpacityTo1_Completed(object sender, EventArgs e)
        {
            OnOpenAnimationCompleted();
        }
        private void UpdateOpacityTo0_Completed(object sender, EventArgs e)
        {
            UpdateData();
            this.BeginAnimation(UserControl.OpacityProperty, UpdateOpacityTo1);
        }
        private void UpdateOpacityTo1_Completed(object sender, EventArgs e)
        {
            OnUpdateAnimationCompleted();
        }

        private void OnUpdateAnimationCompleted()
        {
            UpdateAnimationCompleted?.Invoke(this);
        }
        private void OnUpdateAnimationBegun()
        {
            UpdateAnimationBegun?.Invoke(this);
        }
        private void OnOpenAnimationCompleted()
        {
            OpenAnimationCompleted?.Invoke(this);
        }
        private void OnOpenAnimationBegun()
        {
            OpenAnimationBegun?.Invoke(this);
        }

        public event UpdateAnimationCompletedEventHandler UpdateAnimationCompleted;
        public event UpdateAnimationBegunEventHandler UpdateAnimationBegun;
        public event OpenAnimationCompletedEventHandler OpenAnimationCompleted;
        public event OpenAnimationBegunEventHandler OpenAnimationBegun;

        public delegate void UpdateAnimationCompletedEventHandler(object sender);
        public delegate void UpdateAnimationBegunEventHandler(object sender);
        public delegate void OpenAnimationCompletedEventHandler(object sender);
        public delegate void OpenAnimationBegunEventHandler(object sender);
    }
}
