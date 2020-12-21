using System;
using System.Collections.Generic;
using System.Windows;
using System.Media;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Collections.ObjectModel;
using Niero.Models;
using Niero.SupportClasses;

namespace Niero.Controls
{
    public partial class DisabledAdaptersInfoControl : UserControl
    {
        private DoubleAnimation OpenOpacityTo1, UpdateOpacityTo1, UpdateOpacityTo0;

        public ObservableCollection<DGAdapterInfo> itemsSource;

        List<DependencyObject> DGTextBlocks;

        public DisabledAdaptersInfoControl()
        {
            Opacity = 0;
            InitializeComponent();
            AnimationsInit();

            Loaded += Loaded_e;
            MouseEnter += MouseEnter_e;
            MouseLeave += MouseLeave_e;
            DisabledAdaptersDG.SelectedCellsChanged += DataGrid_SelectedCellsChanged;
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
            DisabledAdaptersDG.ItemsSource = itemsSource;
        }
        private void SwapTextColorToBase()
        {
            DGTextBlocks = VisualTreeSearch.SearchChildren(DisabledAdaptersDG, typeof(TextBlock));
            TitleTB.BeginStoryboard((Storyboard)TryFindResource("ForegroundColorToActiveST"));
            for (int i = 0; i < (DisabledAdaptersDG).Columns.Count; i++)
            {
                (DGTextBlocks[i] as TextBlock).Foreground = new SolidColorBrush(((DGTextBlocks[i] as TextBlock).Foreground as SolidColorBrush).Color);
                (DGTextBlocks[i] as TextBlock).BeginStoryboard((Storyboard)TryFindResource("ForegroundColorToActiveST"));
            }
            for (int i = (DisabledAdaptersDG).Columns.Count; i < DGTextBlocks.Count; i++)
            {
                (DGTextBlocks[i] as TextBlock).Foreground = new SolidColorBrush(((DGTextBlocks[i] as TextBlock).Foreground as SolidColorBrush).Color);
                (DGTextBlocks[i] as TextBlock).BeginStoryboard((Storyboard)TryFindResource("ForegroundColorToActiveST"));
            }
            if (DGTextBlocks != null)
            {
                for (int i = (DisabledAdaptersDG).Columns.Count; i < DGTextBlocks.Count; i++)
                {
                    (DGTextBlocks[i] as TextBlock).MouseDown += CopyMouseDown_e;
                }
            }
        }
        private void SwapTextColorToDark()
        {
            DGTextBlocks = VisualTreeSearch.SearchChildren(DisabledAdaptersDG, typeof(TextBlock));
            TitleTB.BeginStoryboard((Storyboard)TryFindResource("ForegroundColorToBlackST"));
            for (int i = 0; i < (DisabledAdaptersDG).Columns.Count; i++)
            {
                (DGTextBlocks[i] as TextBlock).Foreground = new SolidColorBrush(((DGTextBlocks[i] as TextBlock).Foreground as SolidColorBrush).Color);
                (DGTextBlocks[i] as TextBlock).BeginStoryboard((Storyboard)TryFindResource("ForegroundColorToBlackST"));
            }
            for (int i = (DisabledAdaptersDG).Columns.Count; i < DGTextBlocks.Count; i++)
            {
                (DGTextBlocks[i] as TextBlock).Foreground = new SolidColorBrush(((DGTextBlocks[i] as TextBlock).Foreground as SolidColorBrush).Color);
                (DGTextBlocks[i] as TextBlock).BeginStoryboard((Storyboard)TryFindResource("ForegroundColorToInactiveST"));
            }
            for (int i = (DisabledAdaptersDG).Columns.Count; i < DGTextBlocks.Count; i++)
            {
                (DGTextBlocks[i] as TextBlock).MouseDown -= CopyMouseDown_e;
            }
        }

        private void ThisIsCalledWhenPropertyIsChanged(object sender, EventArgs e)
        {
            DGTextBlocks = VisualTreeSearch.SearchChildren(DisabledAdaptersDG, typeof(TextBlock));
        }
        private void Loaded_e(object sender, RoutedEventArgs e)
        {
            var dpd = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(DataGrid));
            dpd.AddValueChanged(DisabledAdaptersDG, ThisIsCalledWhenPropertyIsChanged);
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

        private void CopyMouseDown_e(object sender, MouseButtonEventArgs e)
        {
            SoundPlayer SP = new SoundPlayer("EmbeddedSounds/Copied.wav");
            Clipboard.SetText((sender as TextBlock).Text);
            SP.Play();
            (sender as TextBlock).BeginStoryboard((Storyboard)TryFindResource("ForegroundColorToTransparentFlashST"));
        }
        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            (sender as DataGrid).UnselectAll();  //DISABLE Fu*** ROW SELECT!
        }

        //Animations caller-functions
        public void BeginOpenAnimation()
        {
            OnOpenAnimationBegun();

            this.BeginAnimation(UserControl.OpacityProperty, OpenOpacityTo1);
        }
        public void BeginUpdateAnimation()
        {
            if (IsMouseOver != true)
            {
                this.Dispatcher.Invoke(() =>
                {
                    OnUpdateAnimationBegun();
                    this.BeginAnimation(UserControl.OpacityProperty, UpdateOpacityTo0);
                });
            }
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
