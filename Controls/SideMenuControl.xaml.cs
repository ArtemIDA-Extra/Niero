using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Niero.Controls
{
    public partial class SideMenuControl : UserControl
    {
        // Required elements from ControlTemplate
        private Button homeButton;
        private Border iconGridMask, backBorder;

        //Animations parts
        private DoubleAnimation OpenStretchingAnim, CloseStretchingAnim, HideStretchingAnim;

        private bool AnimationPermission;

        public SideMenuControl()
        {
            InitializeComponent();
            DataContext = this;

            ApplyTemplate();

            homeButton = (Button)GetTemplateChild("HomeButton");
            backBorder = (Border)GetTemplateChild("BackBorder");
            iconGridMask = (Border)GetTemplateChild("IconGridMask");

            OpenStretchingAnim = new DoubleAnimation(200, new Duration(new TimeSpan(0, 0, 0, 0, 750)));
            CloseStretchingAnim = new DoubleAnimation(40, new Duration(new TimeSpan(0, 0, 0, 0, 750)));
            HideStretchingAnim = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 750)));

            HideStretchingAnim.Completed += HideStretchingAnim_Completed;

            AnimationPermission = true;

            this.HideChangedToTrue += SideMenuControl_HideChangedToTrue;
            this.HideChangedToFalse += SideMenuControl_HideChangedToFalse;
            this.Loaded += SideMenuControl_Loaded;
            iconGridMask.MouseEnter += IconGridMask_MouseEnter;
            backBorder.MouseLeave += BackBorder_MouseLeave;
            homeButton.Click += HomeBtn_Click;
        }

        private void SideMenuControl_HideChangedToTrue(object sender, RoutedEventArgs e)
        {
            BeginAnimation(WidthProperty, HideStretchingAnim);
            AnimationPermission = false;
        }
        private void SideMenuControl_HideChangedToFalse(object sender, RoutedEventArgs e)
        {
            if (AnimationPermission)
            {
                BeginAnimation(WidthProperty, CloseStretchingAnim);
            }
        }
        private void SideMenuControl_Loaded(object sender, EventArgs e)
        {
            if (Hide == true)
            {
                Opacity = 0;
                Width = 0;
            }
            else if (Hide == false)
            {
                Opacity = 1;
                Width = 40; 
            }

        }

        private void BackBorder_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (AnimationPermission)
            {
                BeginAnimation(WidthProperty, CloseStretchingAnim);
            }
        }
        private void IconGridMask_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (AnimationPermission)
            {
                BeginAnimation(WidthProperty, OpenStretchingAnim);
            }
        }

        private void HomeBtn_Click(object sender, RoutedEventArgs e)
        {
            Hide = true;
        }

        private void HideStretchingAnim_Completed(object sender, EventArgs e)
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

        public event RoutedEventHandler HideChangedToTrue
        {
            add
            {
                AddHandler(HideTrueEvent, value);
            }
            remove
            {
                RemoveHandler(HideFalseEvent, value);
            }
        }

        public event RoutedEventHandler HideChangedToFalse
        {
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