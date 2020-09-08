using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Niero.Controls
{
    /// <summary>
    /// Логика взаимодействия для SideMenuControl.xaml
    /// </summary>
    public partial class SideMenuControl : UserControl
    {
        public SideMenuControl()
        {
            InitializeComponent();
            DataContext = this;             //не работал биндинг из-за отсуствия DataContex. Может все таки почитаешь об этом?
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
    }
}
