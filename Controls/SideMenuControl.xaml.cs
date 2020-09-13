using Niero.Windows;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace Niero.Controls
{
    /// <summary>
    /// Логика взаимодействия для SideMenuControl.xaml
    /// </summary>
    public partial class SideMenuControl :  UserControl
    {
        // Required elements from ControlTemplate
        Button homeBtn;
        Border iconGridMask, backBorder;

        //Animations parts
        ThicknessAnimation OpenMoveAnim, CloseMoveAnim, HideMoveAnim;
        bool AnimationPermission;

        public SideMenuControl()
        {
            InitializeComponent();
            DataContext = this;             //не работал биндинг из-за отсуствия DataContex. Может все таки почитаешь об этом?

            ApplyTemplate();                //О ЧЮДООО! Теперь поиск елемента не возвращает null!!! -час, проверяй...

            homeBtn = (Button)GetTemplateChild("HomeButton");
            backBorder = (Border)GetTemplateChild("BackBorder");
            iconGridMask = (Border)GetTemplateChild("IconGridMask");

            OpenMoveAnim = new ThicknessAnimation(new Thickness(160d, 0d, 0d, 0d), new Duration(new TimeSpan(0, 0, 0, 0, 750)));
            CloseMoveAnim = new ThicknessAnimation(new Thickness(0d, 0d, 0d, 0d), new Duration(new TimeSpan(0, 0, 0, 0, 750)));
            HideMoveAnim = new ThicknessAnimation(new Thickness(-40d, 0d, 0d, 0d), new Duration(new TimeSpan(0, 0, 0, 0, 750)));

            HideMoveAnim.Completed += HideMoveAnim_Completed;

            AnimationPermission = true;

            this.HideChangedToTrue += SideMenuControl_HideChangedToTrue;
            this.HideChangedToFalse += SideMenuControl_HideChangedToFalse;
            iconGridMask.MouseEnter += IconGridMask_MouseEnter;
            backBorder.MouseLeave += BackBorder_MouseLeave;
            homeBtn.Click += HomeBtn_Click;
        }

        private void SideMenuControl_HideChangedToTrue(object sender, RoutedEventArgs e)
        {
            BeginAnimation(MarginProperty, HideMoveAnim);
            AnimationPermission = false;
        }

        private void SideMenuControl_HideChangedToFalse(object sender, RoutedEventArgs e)
        {
            if (AnimationPermission)
            {
                BeginAnimation(MarginProperty, CloseMoveAnim);
            }
        }

        private void BackBorder_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (AnimationPermission)
            {
                BeginAnimation(MarginProperty, CloseMoveAnim);
            }
        }

        private void IconGridMask_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (AnimationPermission)
            {
                BeginAnimation(MarginProperty, OpenMoveAnim);
            }
        }

        private void HomeBtn_Click(object sender, RoutedEventArgs e)
        {
            Hide = true;
        }

        private void HideMoveAnim_Completed(object sender, EventArgs e)
        {
            AnimationPermission = true;
        }

        //Dependency Properties 

        public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(
        "Title", typeof(string),
        typeof(SideMenuControl),
        new PropertyMetadata("Empty Title")
        );

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty HideProperty =
        DependencyProperty.Register(
        "Hide", typeof(bool),
        typeof(SideMenuControl),
        new PropertyMetadata(false)
        );

        public bool Hide
        {
            get { return (bool)GetValue(HideProperty); }
            set
            {
                SetValue(HideProperty, value);
                if (value == true) RaiseHideTrueEvent();
                else RaiseHideFalseEvent();
            }
        }

        //Routed Events

        public static readonly RoutedEvent HideTrueEvent = EventManager.RegisterRoutedEvent("HideChangedToTrue", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(SideMenuControl));
        public static readonly RoutedEvent HideFalseEvent = EventManager.RegisterRoutedEvent("HideChangedToFalse", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(SideMenuControl));

        public event RoutedEventHandler HideChangedToTrue {
            add
            {
                AddHandler(HideTrueEvent, value);
            }
            remove
            {
                RemoveHandler(HideFalseEvent, value);
            }
        }
        public event RoutedEventHandler HideChangedToFalse {
            add
            {
                AddHandler(HideFalseEvent, value);
            }
            remove
            {
                RemoveHandler(HideFalseEvent, value);
            }
        }

        protected virtual void RaiseHideTrueEvent()
        {
            RoutedEventArgs args = new RoutedEventArgs(SideMenuControl.HideTrueEvent);
            RaiseEvent(args);
        }
        protected virtual void RaiseHideFalseEvent()
        {
            RoutedEventArgs args = new RoutedEventArgs(SideMenuControl.HideFalseEvent);
            RaiseEvent(args);
        }
    }
}
