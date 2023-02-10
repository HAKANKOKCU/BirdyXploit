using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WeAreDevs_API;
using Microsoft.Win32;
using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Windows.Threading;
using System.Diagnostics;
using System.Collections;

namespace BirdyXploit
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        string userpath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        ExploitAPI api = new();
        int tabid = 0;
        public Dictionary<string, object> jsondata = null;
        ArrayList autoexecute = new();
        BackgroundWorker injectbw = new();
        public Main(Dictionary<string, object> settings)
        {
            jsondata = settings;
            InitializeComponent();
            injectbw.WorkerReportsProgress = true;
            injectbw.DoWork += (sender,e) =>
            {
                try {
                    api.LaunchExploit();
                    injectbw.ReportProgress(20,"Launched Exploit");
                    System.Threading.Thread.Sleep(2000);
                    var counterv = 0;
                    while (api.isAPIAttached() == false && counterv <= 20)
                    {
                        injectbw.ReportProgress(40, "Exploit wasn't injected. Retrying " + counterv + "/20");
                        System.Threading.Thread.Sleep(2000);
                        counterv += 1;
                    }
                }
                catch (Exception ex) {
                    MessageBox.Show("ERROR (While injecting): " + ex.Message,"BirdyXploit");
                }
            };
            injectbw.ProgressChanged += (object sender, ProgressChangedEventArgs e) =>
            {
                Status.Content = e.UserState;
            };

            injectbw.RunWorkerCompleted += (sender, e) =>
            {
                injectbutton.IsEnabled = true;
                executebutton.IsEnabled = true;
                executecbutton.IsEnabled = true;
                foreach (TabItem tab in autoexecute)
                {
                    TextBox tb = tab.Content as TextBox;
                    api.SendLuaScript(tb.Text);
                }
            };
            newTab();
            if ((string)jsondata["Theme"] == "Light")
            {
                injecticon.Source = new BitmapImage(new Uri("/inject.png", UriKind.Relative));
                playicon.Source = new BitmapImage(new Uri("/play.png", UriKind.Relative));
                playcicon.Source = new BitmapImage(new Uri("/playC.png", UriKind.Relative));
                clearicon.Source = new BitmapImage(new Uri("/delete.png", UriKind.Relative));
                openicon.Source = new BitmapImage(new Uri("/open.png", UriKind.Relative));
                saveicon.Source = new BitmapImage(new Uri("/save.png", UriKind.Relative));
                newtabicon.Source = new BitmapImage(new Uri("/add.png", UriKind.Relative));
                scriptsicon.Source = new BitmapImage(new Uri("/menu.png", UriKind.Relative));
                settingsicon.Source = new BitmapImage(new Uri("/settings.png", UriKind.Relative));
                secInjectIcon.Source = new BitmapImage(new Uri("/event.png", UriKind.Relative));
            }
            else
            {
                injecticon.Source = new BitmapImage(new Uri("/inject_White.png", UriKind.Relative));
                playicon.Source = new BitmapImage(new Uri("/play_White.png", UriKind.Relative));
                playcicon.Source = new BitmapImage(new Uri("/playC_White.png", UriKind.Relative));
                clearicon.Source = new BitmapImage(new Uri("/delete_White.png", UriKind.Relative));
                openicon.Source = new BitmapImage(new Uri("/open_White.png", UriKind.Relative));
                saveicon.Source = new BitmapImage(new Uri("/save_White.png", UriKind.Relative));
                newtabicon.Source = new BitmapImage(new Uri("/add_White.png", UriKind.Relative));
                scriptsicon.Source = new BitmapImage(new Uri("/menu_White.png", UriKind.Relative));
                settingsicon.Source = new BitmapImage(new Uri("/settings_White.png", UriKind.Relative));
                secInjectIcon.Source = new BitmapImage(new Uri("/event_White.png", UriKind.Relative));
                toolbar.Background = Brushes.Black;
                Mainsp.Background = Brushes.Black;
                Status.Foreground = Brushes.White;
            }
            this.Topmost = (bool)jsondata["TopMost"];
            saveSettings();
            var files = Directory.GetFiles(userpath + "\\BirdyXploit\\Scripts");
            foreach (string file in files)
            {
                try
                {
                    Button buttonset = new();
                    buttonset.Style = Resources["OrangeButton"] as Style;
                    buttonset.BorderThickness = new Thickness(0);
                    buttonset.Background = Brushes.Transparent;
                    buttonset.Content = System.IO.Path.GetFileName(file);
                    scriptsList.Children.Add(buttonset);

                    buttonset.Click += (sender, e) =>
                    {
                        TabItem tab = tabs.SelectedItem as TabItem;
                        TextBox tb = tab.Content as TextBox;
                        tb.Text = File.ReadAllText(file);
                    };
                    if ((string)jsondata["Theme"] == "Light")
                    {
                    }
                    else
                    {
                        buttonset.Foreground = Brushes.White;
                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show("Cannot add file: " + file + ":\n" + error.Message);
                }
            }

            if ((bool)jsondata["AutoInject"]) {
                DispatcherTimer dtautoinject = new();
                dtautoinject.Interval = TimeSpan.FromSeconds(2);
                dtautoinject.Tick += (sender, arg) =>
                {
                    if (!injectbw.IsBusy)
                    {
                        if (Process.GetProcessesByName("RobloxPlayerBeta").Length != 0)
                        {
                            if (!api.isAPIAttached())
                            {
                                injectbutton.IsEnabled = false;
                                executebutton.IsEnabled = false;
                                executecbutton.IsEnabled = false;
                                injectbw.RunWorkerAsync();
                            }
                        }
                    }
                };
                dtautoinject.Start();
            }
            DispatcherTimer isinjected = new();
            isinjected.Interval = TimeSpan.FromSeconds(5);
            isinjected.Tick += (sender, arg) =>
            {
                if (!injectbw.IsBusy)
                {
                    Status.Content = api.isAPIAttached() ? "Injected!" : "Not Injected...";
                }
            };
            isinjected.Start();
        }

        void saveSettings()
        {
            var settingsstring = Newtonsoft.Json.JsonConvert.SerializeObject(jsondata);
            File.WriteAllText(userpath + "\\BirdyXploit\\Settings.json", settingsstring);
        }

        void newTab()
        {
            TabItem tab = new();
            StackPanel tabheader = new();
            tabheader.Orientation = Orientation.Horizontal;
            Label title = new();
            title.Content = "Tab " + tabid;
            tabheader.Children.Add(title);
            Button close = new();
            close.Background = Brushes.Transparent;
            close.BorderThickness = new Thickness(0);
            close.Content = "×";
            close.Click += (sender, e) => {
                tabs.Items.Remove(tab);
            };
            tabheader.Children.Add(close);
            tab.Header = tabheader;
            TextBox tb = new();
            tb.AcceptsReturn = true;
            tb.AcceptsTab = true;
            tb.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            tb.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            tb.BorderThickness = new Thickness(0);
            tb.SelectionBrush = Brushes.Orange;
            if ((string)jsondata["Theme"] == "Light")
            {

            }
            else
            {
                title.Foreground = Brushes.White;
                close.Foreground = Brushes.White;
                tb.Background = Brushes.Black;
                tb.Foreground = Brushes.White;
                tb.CaretBrush = Brushes.White;
            }
            tab.Content = tb;
            tabs.Items.Add(tab);
            tabs.SelectedItem = tab;
            tabid += 1;
        }

        private void injectbutton_Click(object sender, RoutedEventArgs e)
        {
            injectbutton.IsEnabled = false;
            executebutton.IsEnabled = false;
            executecbutton.IsEnabled = false;
            injectbw.RunWorkerAsync();
        }

        private void newtab_Click(object sender, RoutedEventArgs e)
        {
            newTab();
        }

        private void executebutton_Click(object sender, RoutedEventArgs e)
        {
            if (api.isAPIAttached()) {
                TabItem tab = tabs.SelectedItem as TabItem;
                TextBox tb = tab.Content as TextBox;
                api.SendLuaScript(tb.Text);
            }else
            {
                MessageBox.Show("You forgot to inject.","BirdyXploit - Error");
            }
        }

        private void scriptslib_Click(object sender, RoutedEventArgs e)
        {
            if (scriptspane.Visibility == Visibility.Collapsed)
            {
                scriptspane.Visibility = Visibility.Visible;
            }
            else
            {
                scriptspane.Visibility = Visibility.Collapsed;
            }
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            TabItem tab = tabs.SelectedItem as TabItem;
            TextBox tb = tab.Content as TextBox;
            tb.Text = "";
        }

        private void open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                TabItem tab = tabs.SelectedItem as TabItem;
                TextBox tb = tab.Content as TextBox;
                tb.Text = File.ReadAllText(openFileDialog.FileName);
            }

        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new();
            saveFileDialog.Filter = "Lua file (*.lua)|*.lua|Text File (*.txt)|*.txt|All Files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                TabItem tab = tabs.SelectedItem as TabItem;
                TextBox tb = tab.Content as TextBox;
                File.WriteAllText(saveFileDialog.FileName, tb.Text);
            }
        }

        private void executecbutton_Click(object sender, RoutedEventArgs e)
        {
            if (api.isAPIAttached())
            {
                TabItem tab = tabs.SelectedItem as TabItem;
                TextBox tb = tab.Content as TextBox;
                api.SendLuaCScript(tb.Text);
            }
            else
            {
                MessageBox.Show("You forgot to inject.", "BirdyXploit - Error");
            }
        }

        private void settings_Click(object sender, RoutedEventArgs e)
        {
            Settings settings = new(jsondata);
            settings.ShowDialog();
        }

        private void tabs_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!(e.Source is TabItem tabItem))
            {
                return;
            }

            if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(tabItem, tabItem, DragDropEffects.All);
            }
        }

        private void tabs_Drop(object sender, DragEventArgs e)
        {
            if (e.Source is TabItem tabItemTarget &&
                e.Data.GetData(typeof(TabItem)) is TabItem tabItemSource &&
                !tabItemTarget.Equals(tabItemSource) &&
                tabItemTarget.Parent is TabControl tabControl)
            {
                int targetIndex = tabControl.Items.IndexOf(tabItemTarget);

                tabControl.Items.Remove(tabItemSource);
                tabControl.Items.Insert(targetIndex, tabItemSource);
                tabItemSource.IsSelected = true;
            }
        }

        private void secInject_Click(object sender, RoutedEventArgs e)
        {
            TabItem currenttab = tabs.SelectedItem as TabItem;
            if (autoexecute.Contains(currenttab))
            {
                autoexecute.Remove(currenttab);
            }
            else
            {
                autoexecute.Add(currenttab);
            }
            if (autoexecute.Contains(currenttab))
            {
                (currenttab.Header as StackPanel).Background = Brushes.Gold;
            }
            else
            {
                (currenttab.Header as StackPanel).Background = null;
            }
        }
    }
}
