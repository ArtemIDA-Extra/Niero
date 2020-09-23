using Niero.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        LoadingWindow loadingWindow;
        Button maxSizeButton, minSizeButton, closeWindowButton;
        Button nextPageButton, prevPageButton;

        private int OuterWindowMargin = 10;
        private int ResizeBorderSize = 11;

        public Thickness OuterMarginSizeThickness 
        {
            get 
            { 
                return new Thickness(OuterWindowMargin); 
            } 
        }
        public Thickness ResizeBorderThickness
        {
            get
            {
                return new Thickness(ResizeBorderSize);
            }
        }
        public int CaptionHeightProperty 
        { 
            get; 
            private set; 
        } = 24;

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
            nextPageButton = (Button)SideMenu.Template.FindName("NextButton", SideMenu);
            prevPageButton = (Button)SideMenu.Template.FindName("PrevButton", SideMenu);

            //Navigate buttons init
            nextPageButton.IsEnabled = false;
            prevPageButton.IsEnabled = false;

            //PagesViewer init
            PagesViewer.JournalOwnership = JournalOwnership.OwnsJournal;
            PagesViewer.Content = new DefaultPage();
            PagesViewer.Navigated += PagesViewer_Navigated;

            //Commands time!
            CommandsInit();

            //Window events binding
            this.StateChanged += MainWindow_StateChanged;
            this.Loaded += MainWindow_Loaded;
            loadingWindow.Closed += LoadingWindow_Closed;
        }

        private void PagesViewer_Navigated(object sender, NavigationEventArgs e)
        {
            if (PagesViewer.CanGoBack) prevPageButton.IsEnabled = true;
            else                       prevPageButton.IsEnabled = false;

            if (PagesViewer.CanGoForward) nextPageButton.IsEnabled = true;
            else                          nextPageButton.IsEnabled = false;
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            switch (this.WindowState) 
            {
                case WindowState.Normal :
                    OuterWindowMargin = 10;
                    ResizeBorderSize = 11;
                    CaptionHeightProperty = (OuterWindowMargin + 25 - ResizeBorderSize);
                    OnPropertyChanged(nameof(ResizeBorderThickness));
                    OnPropertyChanged(nameof(OuterMarginSizeThickness));
                    OnPropertyChanged(nameof(CaptionHeightProperty));
                    break;
                case WindowState.Maximized :
                    this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
                    OuterWindowMargin = 7;
                    ResizeBorderSize = 0;
                    CaptionHeightProperty = (OuterWindowMargin + 25 - ResizeBorderSize);
                    OnPropertyChanged(nameof(ResizeBorderThickness));
                    OnPropertyChanged(nameof(OuterMarginSizeThickness));
                    OnPropertyChanged(nameof(CaptionHeightProperty));
                    break;
                case WindowState.Minimized :
                    OuterWindowMargin = 10;
                    ResizeBorderSize = 11;
                    CaptionHeightProperty = (OuterWindowMargin + 25 - ResizeBorderSize);
                    OnPropertyChanged(nameof(ResizeBorderThickness));
                    OnPropertyChanged(nameof(OuterMarginSizeThickness));
                    OnPropertyChanged(nameof(CaptionHeightProperty));
                    break;
            }
        }

        private void CommandsInit()
        {
            maxSizeButton.Command = SystemCommands.MaximizeWindowCommand;
            minSizeButton.Command = SystemCommands.MinimizeWindowCommand;
            closeWindowButton.Command = SystemCommands.CloseWindowCommand;

            nextPageButton.Command = NavigationCommands.NextPage;
            prevPageButton.Command = NavigationCommands.PreviousPage;

            CommandBinding comBind;

            comBind = new CommandBinding();
            comBind.Command = SystemCommands.MaximizeWindowCommand;
            comBind.Executed += maximizeCommand_Executed;
            maxSizeButton.CommandBindings.Add(comBind);

            comBind = new CommandBinding();
            comBind.Command = SystemCommands.MinimizeWindowCommand;
            comBind.Executed += minimizeCommand_Executed;
            minSizeButton.CommandBindings.Add(comBind);

            comBind = new CommandBinding();
            comBind.Command = SystemCommands.CloseWindowCommand;
            comBind.Executed += closeCommand_Executed;
            closeWindowButton.CommandBindings.Add(comBind);

            CommandBinding SMcomBind;

            SMcomBind = new CommandBinding();
            SMcomBind.Command = NavigationCommands.NextPage;
            SMcomBind.Executed += nextPageCommand_Executed;
            nextPageButton.CommandBindings.Add(SMcomBind);

            SMcomBind = new CommandBinding();
            SMcomBind.Command = NavigationCommands.PreviousPage;
            SMcomBind.Executed += prevPageCommand_Executed;
            prevPageButton.CommandBindings.Add(SMcomBind);
        }

        private void prevPageCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (PagesViewer.CanGoBack)
            {
                PagesViewer.GoBack();
            }
            else
            {
                MessageBox.Show("Cant go back!");
            }
        }

        private void nextPageCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (PagesViewer.CanGoForward)
            {
                PagesViewer.GoForward();
            }
            else
            {
                MessageBox.Show("Cant go forward!");
            }
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

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Factory.StartNew(()=> { Thread.Sleep(3000);                     //Жопой чую, это МЕГА костылина, и есть куча других способов создать таймер 
            });                                                                        //кдасс таймер у меня не сконал (не смог поставить коллбеком Close()), а из других потоков не удается 
            loadingWindow.Close();                                                     //достучатся до загрузочного окна. В общем, пока так оставлю xDDDD
        }

        private void MenuSelectionController(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show((sender as Button).Content.ToString());
        }

        private void LoadingWindow_Closed(object sender, EventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
        public void OnPropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
