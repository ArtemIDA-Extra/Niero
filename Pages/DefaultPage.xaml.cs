using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Niero.Pages
{
    public partial class DefaultPage : Page
    {
        public DefaultPage()
        {
            InitializeComponent();
            this.Loaded += DefaultPage_Loaded;
        }

        private void DefaultPage_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation OpenAnim = new DoubleAnimation(1, new Duration(new TimeSpan(0, 0, 0, 0, 750)));
            this.BeginAnimation(Page.OpacityProperty, OpenAnim);
        }
    }
}
