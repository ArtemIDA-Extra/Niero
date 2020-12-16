using System;
using System.Threading;
using System.Threading.Tasks;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Input;
using Niero.Controls;
using Niero.SupportClasses;
using Niero.Pages;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Linq;
using Niero.Models;

namespace Niero.ViewModels
{
    class MainWindowVM : BaseViewModel
    {
        //Models storage area
        List<NetworkInterfaceType> BasicInterfaceTypes = new List<NetworkInterfaceType>
        {
            NetworkInterfaceType.Ethernet,
            NetworkInterfaceType.Wireless80211
        };
        NetworkDataHub BasicNetInterfaceDataHub;
        //--------------------

        Window loadingWindow, mainWindow;

        Grid titleLine;
        Button maxSizeButton, minSizeButton, closeWindowButton, 
               nextPageButton, prevPageButton, homeButton;
        Frame pagesViewer;
        SideMenuControl sideMenu;
        DropShadowEffect windowNeon;

        WelcomePage welcomePage;

        private bool WelcomePageON;
        private bool RemoveLastPageInHistory = true;

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
        public double CaptionHeightProperty
        {
            get;
            private set;
        } = 24;

        private List<Page> Pages;

        //CONSTRUCTOR!!!
        public MainWindowVM(Window window)
        {
            mainWindow = window;

            mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            mainWindow.WindowState = WindowState.Normal;
            mainWindow.Opacity = 0;
            
            mainWindow.Cursor = CustomCursors.Normal_Select;

            //Init loading window 
            loadingWindow = new LoadingWindow();
            loadingWindow.Show();
            loadingWindow.Closed += LoadingWindow_Closed;

            //Models init
            BasicNetInterfaceDataHub = new NetworkDataHub(BasicInterfaceTypes.ToArray());

            //Searching elements
            mainWindow.ApplyTemplate();
            titleLine = (Grid)mainWindow.Template.FindName("TitleLine", mainWindow);
            windowNeon = (DropShadowEffect)mainWindow.Template.FindName("WindowNeon", mainWindow);
            maxSizeButton = (Button)mainWindow.Template.FindName("MaxSizeButton", mainWindow);
            minSizeButton = (Button)mainWindow.Template.FindName("MinSizeButton", mainWindow);
            closeWindowButton = (Button)mainWindow.Template.FindName("CloseWindowButton", mainWindow);
            pagesViewer = (Frame)mainWindow.FindName("PagesViewer");
            sideMenu = (SideMenuControl)mainWindow.FindName("SideMenu");
            nextPageButton = (Button)sideMenu.Template.FindName("NextButton", sideMenu);
            prevPageButton = (Button)sideMenu.Template.FindName("PrevButton", sideMenu);
            homeButton = (Button)sideMenu.Template.FindName("HomeButton", sideMenu);

            //Menu buttons click events connect
            foreach (Button menuButton in (sideMenu.Content as StackPanel).Children) 
            {
                menuButton.Click += ChangeMenuSelection;
            }

            //Navigate buttons init
            nextPageButton.IsEnabled = false;
            prevPageButton.IsEnabled = false;

            //Commands time!
            CommandsInit();

            //PAGES LIST!!!-------------------------------------
            Pages = new List<Page>()
            {
                new BaseInfoPage(BasicNetInterfaceDataHub),
                new NetScanningMainPage(BasicNetInterfaceDataHub),
                new DefaultPage()
            };
            //--------------------------------------------------

            //PagesViewer init
            DoubleAnimation OpenAnim = new DoubleAnimation(1, new Duration(new TimeSpan(0, 0, 0, 0, 750)));

            welcomePage = new WelcomePage();
            welcomePage.MouseDown += WelcomePage_MouseDown;

            pagesViewer.JournalOwnership = JournalOwnership.OwnsJournal;
            pagesViewer.Content = welcomePage;
            pagesViewer.Navigated += PagesViewer_Navigated;
            welcomePage.BeginAnimation(Page.OpacityProperty, OpenAnim);
            WelcomePageON = true;

            //Window events binding
            mainWindow.StateChanged += MainWindow_StateChanged;
            mainWindow.Loaded += MainWindow_Loaded;
            mainWindow.MouseEnter += MainWindow_MouseEnter;
            mainWindow.MouseLeave += MainWindow_MouseLeave;
        }

        private void WelcomePage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SoundPlayer SP = new SoundPlayer("EmbeddedSounds/MainEnterance.wav");
            SP.Play();
            sideMenu.Hide = false;
            RemoveLastPageInHistory = true;
            SwapPageTo(new DefaultPage());
            ColorAnimation titleRecolorAnim = new ColorAnimation((Color)mainWindow.TryFindResource("SurfaceColor"), new Duration(new TimeSpan(0, 0, 0, 0, 750)));
            DoubleAnimation titleResizeAnim = new DoubleAnimation(25, new Duration(new TimeSpan(0, 0, 0, 0, 750)));
            (titleLine.Background as SolidColorBrush).BeginAnimation(SolidColorBrush.ColorProperty, titleRecolorAnim);
            titleLine.BeginAnimation(Grid.HeightProperty, titleResizeAnim);
            WelcomePageON = false;
            MouseEventArgs args = new MouseEventArgs(Mouse.PrimaryDevice, 0);
            args.RoutedEvent = Mouse.MouseEnterEvent;
            mainWindow.RaiseEvent(args);
        }

        //Additional func
        private void CommandsInit()
        {
            maxSizeButton.Command = SystemCommands.MaximizeWindowCommand;
            minSizeButton.Command = SystemCommands.MinimizeWindowCommand;
            closeWindowButton.Command = SystemCommands.CloseWindowCommand;

            nextPageButton.Command = NavigationCommands.NextPage;
            prevPageButton.Command = NavigationCommands.PreviousPage;
            homeButton.Command = NavigationCommands.BrowseHome;

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

            SMcomBind = new CommandBinding();
            SMcomBind.Command = NavigationCommands.BrowseHome;
            SMcomBind.Executed += homePageCommand_Executed;
            homeButton.CommandBindings.Add(SMcomBind);
        }

        //Navigate handler
        private void PagesViewer_Navigated(object sender, NavigationEventArgs e)
        {
            if (RemoveLastPageInHistory)
            {
                pagesViewer.RemoveBackEntry();
                RemoveLastPageInHistory = false;
            }

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
                DoubleAnimation CloseAnim = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 750)));
                CloseAnim.Completed += PrevCommandAnim_Completed;
                (pagesViewer.Content as Page).BeginAnimation(Page.OpacityProperty, CloseAnim);
            }
        }
        private void nextPageCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (pagesViewer.CanGoForward)
            {
                DoubleAnimation CloseAnim = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 750)));
                CloseAnim.Completed += NextCommandAnim_Completed;
                (pagesViewer.Content as Page).BeginAnimation(Page.OpacityProperty, CloseAnim);
            }
        }
        private void homePageCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            sideMenu.Hide = true;
            pagesViewer.Content = welcomePage;
            MouseEventArgs args = new MouseEventArgs(Mouse.PrimaryDevice, 0);
            args.RoutedEvent = Mouse.MouseLeaveEvent;
            mainWindow.RaiseEvent(args);
            ColorAnimation titleRecolorAnim = new ColorAnimation((Color)mainWindow.TryFindResource("OverflowColor"), new Duration(new TimeSpan(0, 0, 0, 0, 750)));
            DoubleAnimation titleResizeAnim = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 750)));
            (titleLine.Background as SolidColorBrush).BeginAnimation(SolidColorBrush.ColorProperty, titleRecolorAnim);
            titleLine.BeginAnimation(Grid.HeightProperty, titleResizeAnim);
            WelcomePageON = true;
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
            DoubleAnimation CloseWindowAnimation = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 500)));
            CloseWindowAnimation.Completed += CloseWindowAnimation_Completed;
            mainWindow.BeginAnimation(Window.OpacityProperty, CloseWindowAnimation);
        }

        private void CloseWindowAnimation_Completed(object sender, EventArgs e)
        {
            mainWindow.Close();
        }

        private void PrevCommandAnim_Completed(object sender, EventArgs e)
        {
            pagesViewer.GoBack();
        } 
        private void NextCommandAnim_Completed(object sender, EventArgs e)
        {
            pagesViewer.GoForward();
        }

        //Menu changes handler
        private Page swapPage;
        private void ChangeMenuSelection(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Content.ToString())
            {
                case "Info":
                    {
                        if (Pages[0].KeepAlive == false) Pages[0] = new BaseInfoPage(BasicNetInterfaceDataHub);
                        SwapPageTo(Pages[0]); 
                        WelcomePageON = false; 
                        break;
                    }
                case "Network Scan":
                    {
                        if (Pages[1].KeepAlive == false) Pages[1] = new NetScanningMainPage(BasicNetInterfaceDataHub);
                        SwapPageTo(Pages[1]);
                        WelcomePageON = false; 
                        break;
                    }

                case "Default(Test)":
                    {
                        if (Pages[2].KeepAlive == false) Pages[2] = new DefaultPage();
                        SwapPageTo(Pages[2]);
                        WelcomePageON = false; 
                        break;
                    }
            }
        }
        private void SwapPageTo(Page page)
        {
            swapPage = page;
            DoubleAnimation CloseAnim = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 750)));
            CloseAnim.Completed += SwapCloseAnim_Completed;
            (pagesViewer.Content as Page).BeginAnimation(Page.OpacityProperty, CloseAnim);
        }
        private void SwapCloseAnim_Completed(object sender, EventArgs e)
        {
            pagesViewer.Content = swapPage;
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
            await Task.Delay(3200);
            loadingWindow.Close();
        }
        private void MainWindow_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!WelcomePageON)
            {
                ColorAnimation anim = new ColorAnimation((Color)mainWindow.TryFindResource("OverflowColor"), new Duration(new TimeSpan(0, 0, 0, 0, 500)));
                windowNeon.BeginAnimation(DropShadowEffect.ColorProperty, anim);
            }
        }
        private void MainWindow_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!WelcomePageON)
            {
                ColorAnimation anim = new ColorAnimation((Color)mainWindow.TryFindResource("BaseColor"), new Duration(new TimeSpan(0, 0, 0, 0, 500)));
                windowNeon.BeginAnimation(DropShadowEffect.ColorProperty, anim);
            }
        }

        private void LoadingWindow_Closed(object sender, EventArgs e)
        {
            mainWindow.Activate();
            DoubleAnimation anim = new DoubleAnimation(1, new Duration(new TimeSpan(0, 0, 0, 0, 1000)));
            mainWindow.BeginAnimation(Window.OpacityProperty, anim);
        }
    }
}
