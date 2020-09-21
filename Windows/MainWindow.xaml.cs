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
        Button maxSizeButton, minSizeButton, closeWindowButton;

        public MainWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Minimized;

            //Init loading window 
            loadingWindow = new LoadingWindow();
            loadingWindow.Show();

            //Searching elements
            ApplyTemplate();
            maxSizeButton = (Button)GetTemplateChild("MaxSizeButton");
            minSizeButton = (Button)GetTemplateChild("MinSizeButton");
            closeWindowButton = (Button)GetTemplateChild("CloseWindowButton");

            //Commands time!!!
            CommandsInit();

            //Window events binding
            this.Loaded += MainWindow_Loaded;
            loadingWindow.Closed += LoadingWindow_Closed;
        }

        private void CommandsInit()
        {
            maxSizeButton.Command = SystemCommands.MaximizeWindowCommand;
            minSizeButton.Command = SystemCommands.MinimizeWindowCommand;
            closeWindowButton.Command = SystemCommands.CloseWindowCommand;

            CommandBinding comBild;

            comBild = new CommandBinding();
            comBild.Command = SystemCommands.MaximizeWindowCommand;
            comBild.Executed += maximizeCommand_Executed;
            maxSizeButton.CommandBindings.Add(comBild);

            comBild = new CommandBinding();
            comBild.Command = SystemCommands.MinimizeWindowCommand;
            comBild.Executed += minimizeCommand_Executed;
            minSizeButton.CommandBindings.Add(comBild);

            comBild = new CommandBinding();
            comBild.Command = SystemCommands.CloseWindowCommand;
            comBild.Executed += closeCommand_Executed;
            closeWindowButton.CommandBindings.Add(comBild);
        }

        private void maximizeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }

        private void minimizeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Minimized;
            }
        }

        private void closeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
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
