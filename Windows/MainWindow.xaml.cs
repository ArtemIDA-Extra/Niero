using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Niero.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LoadingWindow loadingWindow;

        public MainWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Minimized;

            loadingWindow = new LoadingWindow();
            loadingWindow.Show();

            this.Loaded += MainWindow_Loaded;

            loadingWindow.Closed += LoadingWindow_Closed;
        }

        public async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Factory.StartNew(()=> { Thread.Sleep(3000);                     //Жопой чую, это МЕГА костылина, и есть куча других способов создать таймер 
            });                                                                        //кдасс таймер у меня не сконал (не смог поставить коллбеком Close()), а из других потоков не удается 
            loadingWindow.Close();                                                     //достучатся до загрузочного окна. В общем, пока так оставлю xDDDD
        }

        public void LoadingWindow_Closed(object sender, EventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }
    }
}
