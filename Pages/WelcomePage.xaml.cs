using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Niero.Pages
{
    /// <summary>
    /// Логика взаимодействия для WelcomePage.xaml
    /// </summary>
    public partial class WelcomePage : Page
    {
        public WelcomePage()
        {
            InitializeComponent();
            this.Loaded += WelcomePage_Loaded;
        }

        private void WelcomePage_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation anim = new DoubleAnimation(1 ,new Duration(new TimeSpan(0, 0, 0, 0, 750)));
            this.BeginAnimation(Page.OpacityProperty, anim);
        }

    }
}
