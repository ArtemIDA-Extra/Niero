using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Niero.ViewModels;
using Niero.SupportClasses;

namespace Niero.Pages
{
    /// <summary>
    /// Логика взаимодействия для NetworkInfoPage.xaml
    /// </summary>
    public partial class NetworkInfoPage : Page
    {
        protected bool DraggingActive;
        private Point ClickPositionRelativeToParent;
        private Point ClickPositionRelativeToBorder;
        private Point LeftUp, RightUp, LeftDown, RightDown;
        private double prevX, prevY;

        public NetworkInfoPage()
        {
            InitializeComponent();
            DataContext = new NetInfoVM(this);
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DraggingActive = true;
            Cursor = CustomCursors.Move;
            var DraggableBorder = sender as Border;
            ClickPositionRelativeToParent = e.GetPosition(MainCanvas);
            ClickPositionRelativeToBorder = e.GetPosition(DraggableBorder);

            LeftUp.X = ClickPositionRelativeToParent.X - ClickPositionRelativeToBorder.X;
            LeftUp.Y = ClickPositionRelativeToParent.Y - ClickPositionRelativeToBorder.Y;

            RightUp.X = LeftUp.X + DraggableBorder.Width;
            RightUp.Y = LeftUp.Y;
            RightDown.X = RightUp.X;
            RightDown.Y = RightUp.Y + DraggableBorder.Height;
            LeftDown.X = LeftUp.X;
            LeftDown.Y = RightDown.Y;

            DraggableBorder.CaptureMouse();
        }

        private void Border_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DraggingActive = false;
            Cursor = CustomCursors.Normal_Select;
            var DraggableBorder = sender as Border;

            ClickPositionRelativeToParent = e.GetPosition(MainCanvas);
            ClickPositionRelativeToBorder = e.GetPosition(DraggableBorder);

            LeftUp.X = ClickPositionRelativeToParent.X - ClickPositionRelativeToBorder.X;
            LeftUp.Y = ClickPositionRelativeToParent.Y - ClickPositionRelativeToBorder.Y;

            RightUp.X = LeftUp.X + DraggableBorder.Width;
            RightUp.Y = LeftUp.Y;
            RightDown.X = RightUp.X;
            RightDown.Y = RightUp.Y + DraggableBorder.Height;
            LeftDown.X = LeftUp.X;
            LeftDown.Y = RightDown.Y;

            double addX = 0, addY = 0;

            if (LeftUp.X < 0)
            {
                
                Duration duration = new Duration(new TimeSpan(0, 0, 0, 1, 0));
                DoubleAnimation anim = new DoubleAnimation(10, duration);
                DraggableBorder.RenderTransform.BeginAnimation(TranslateTransform.XProperty, anim);
                addX = 10;
            }
            if (LeftUp.Y < 0)
            {
                
                Duration duration = new Duration(new TimeSpan(0, 0, 0, 1, 0));
                DoubleAnimation anim = new DoubleAnimation(10, duration);
                DraggableBorder.RenderTransform.BeginAnimation(TranslateTransform.YProperty, anim);
                addY = 10;
            }
            if (RightDown.X > MainCanvas.ActualWidth)
            {
                Duration duration = new Duration(new TimeSpan(0, 0, 0, 1, 0));
                DoubleAnimation anim = new DoubleAnimation(MainCanvas.ActualWidth - DraggableBorder.ActualWidth - 10, duration);
                DraggableBorder.RenderTransform.BeginAnimation(TranslateTransform.XProperty, anim);
                addX = MainCanvas.ActualWidth - DraggableBorder.ActualWidth - 10;
            }
            if (RightDown.Y > MainCanvas.ActualHeight)
            {
                Duration duration = new Duration(new TimeSpan(0, 0, 0, 1, 0));
                DoubleAnimation anim = new DoubleAnimation(MainCanvas.ActualHeight - DraggableBorder.ActualHeight - 20, duration);
                DraggableBorder.RenderTransform.BeginAnimation(TranslateTransform.YProperty, anim);
                addY = MainCanvas.ActualHeight - DraggableBorder.ActualHeight - 20;
            }

            var Transform = (DraggableBorder.RenderTransform as TranslateTransform);
            if (Transform != null)
            {
                prevX = Transform.X;
                prevY = Transform.Y;
            }

            if (addX != 0) prevX = addX;
            if (addY != 0) prevY = addY;

            DraggableBorder.ReleaseMouseCapture();
        }

        private void Border_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var DraggableBorder = sender as Border;

            if(DraggingActive && DraggableBorder != null)
            {
                Vector MoveVector;
                Point CurrentPosition = e.GetPosition(MainCanvas);

                MoveVector.X = CurrentPosition.X - ClickPositionRelativeToParent.X;
                MoveVector.Y = CurrentPosition.Y - ClickPositionRelativeToParent.Y;

                TranslateTransform Transform = new TranslateTransform();
                DraggableBorder.RenderTransform = Transform;
                
                Transform.X = MoveVector.X;
                Transform.Y = MoveVector.Y;

                if (prevX > 0)
                {
                    Transform.X += prevX;
                    Transform.Y += prevY;
                }
            }
        }
    }
}
