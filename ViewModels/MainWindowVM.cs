using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Input;
using Niero.Controls;
using Niero.SupportClasses;
using Niero.Pages;

namespace Niero.ViewModels
{
    class MainWindowVM : BaseViewModel
    {
        Window loadingWindow, mainWindow;

        Button maxSizeButton, minSizeButton, closeWindowButton, 
               nextPageButton, prevPageButton;
        Frame pagesViewer;
        SideMenuControl sideMenu;

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

        public MainWindowVM(Window window)
        {
            mainWindow = window;

            mainWindow.WindowState = WindowState.Minimized;
            mainWindow.Cursor = CustomCursors.Normal_Select;

            //Init loading window 
            loadingWindow = new LoadingWindow();
            loadingWindow.Show();
            loadingWindow.Closed += LoadingWindow_Closed;

            //Searching elements
            mainWindow.ApplyTemplate();
            maxSizeButton = (Button)mainWindow.Template.FindName("MaxSizeButton", mainWindow);
            minSizeButton = (Button)mainWindow.Template.FindName("MinSizeButton", mainWindow);
            closeWindowButton = (Button)mainWindow.Template.FindName("CloseWindowButton", mainWindow);
            pagesViewer = (Frame)mainWindow.FindName("PagesViewer");
            sideMenu = (SideMenuControl)mainWindow.FindName("SideMenu");
            nextPageButton = (Button)sideMenu.Template.FindName("NextButton", sideMenu);
            prevPageButton = (Button)sideMenu.Template.FindName("PrevButton", sideMenu);

            //Navigate buttons init
            nextPageButton.IsEnabled = false;
            prevPageButton.IsEnabled = false;

            //Commands time!
            CommandsInit();

            //PagesViewer init
            pagesViewer.JournalOwnership = JournalOwnership.OwnsJournal;
            pagesViewer.Content = new DefaultPage();
            pagesViewer.Navigated += PagesViewer_Navigated;

            //Window events binding
            mainWindow.StateChanged += MainWindow_StateChanged;
            mainWindow.Loaded += MainWindow_Loaded;
        }

        //Additional func
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

        //Navigate handler
        private void PagesViewer_Navigated(object sender, NavigationEventArgs e)
        {
            if (pagesViewer.CanGoBack) prevPageButton.IsEnabled = true;
            else prevPageButton.IsEnabled = false;

            if (pagesViewer.CanGoForward) nextPageButton.IsEnabled = true;
            else nextPageButton.IsEnabled = false;
        }

        //Commands handlers
        private void prevPageCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (pagesViewer.CanGoBack)
            {
                pagesViewer.GoBack();
            }
            else
            {
                MessageBox.Show("Cant go back!");
            }
        }
        private void nextPageCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (pagesViewer.CanGoForward)
            {
                pagesViewer.GoForward();
            }
            else
            {
                MessageBox.Show("Cant go forward!");
            }
        }
        private void maximizeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mainWindow.WindowState = WindowState.Maximized;
        }
        private void minimizeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (mainWindow.WindowState == WindowState.Maximized)
            {
                mainWindow.WindowState = WindowState.Normal;
            }
            else
            {
                mainWindow.WindowState = WindowState.Minimized;
            }
        }
        private void closeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mainWindow.Close();
        }

        //Menu changes handler
        public void ChangeMenuSelection(string select)
        {
            switch (select)
            {
                case "Network Info": pagesViewer.Content = new NetworkInfoPage(); break;
                case "Network Scan": pagesViewer.Content = new NetScanningMainPage(); break;
                case "Default(Test)": pagesViewer.Content = new DefaultPage(); break;
            }
        }

        //Windows handlers
        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            switch (mainWindow.WindowState)
            {
                case WindowState.Normal:
                    OuterWindowMargin = 10;
                    ResizeBorderSize = 11;
                    CaptionHeightProperty = (OuterWindowMargin + 25 - ResizeBorderSize);
                    OnPropertyChanged(nameof(ResizeBorderThickness));
                    OnPropertyChanged(nameof(OuterMarginSizeThickness));
                    OnPropertyChanged(nameof(CaptionHeightProperty));
                    break;
                case WindowState.Maximized:
                    mainWindow.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
                    OuterWindowMargin = 7;
                    ResizeBorderSize = 0;
                    CaptionHeightProperty = (OuterWindowMargin + 25 - ResizeBorderSize);
                    OnPropertyChanged(nameof(ResizeBorderThickness));
                    OnPropertyChanged(nameof(OuterMarginSizeThickness));
                    OnPropertyChanged(nameof(CaptionHeightProperty));
                    break;
                case WindowState.Minimized:
                    OuterWindowMargin = 10;
                    ResizeBorderSize = 11;
                    CaptionHeightProperty = (OuterWindowMargin + 25 - ResizeBorderSize);
                    OnPropertyChanged(nameof(ResizeBorderThickness));
                    OnPropertyChanged(nameof(OuterMarginSizeThickness));
                    OnPropertyChanged(nameof(CaptionHeightProperty));
                    break;
            }
        }
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Factory.StartNew(() => {
                Thread.Sleep(3000);                     //Жопой чую, это МЕГА костылина, и есть куча других способов создать таймер 
            });                                                                        //кдасс таймер у меня не сконал (не смог поставить коллбеком Close()), а из других потоков не удается 
            loadingWindow.Close();                                                     //достучатся до загрузочного окна. В общем, пока так оставлю xDDDD
        }
        private void LoadingWindow_Closed(object sender, EventArgs e)
        {
            mainWindow.WindowState = WindowState.Normal;
        }
    }
}
