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
using System.ComponentModel;
using System.Windows.Threading;
using System.Diagnostics;
using System.Collections;
using System.Windows.Media.Animation;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Windows.Media.Effects;

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
        HttpClient client = new();
		SolidColorBrush backgroundcolor = Brushes.White;
		SolidColorBrush foregroundcolor = Brushes.Black;
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
                scriptbloxicon.Source = new BitmapImage(new Uri("/grid.png", UriKind.Relative));
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
                scriptbloxicon.Source = new BitmapImage(new Uri("/grid_White.png", UriKind.Relative));
                toolbar.Background = Brushes.Black;
				Mainsp.Background = Brushes.Black;
				Status.Foreground = Brushes.White;
                backgroundcolor = Brushes.Black;
                foregroundcolor = Brushes.White;
            }
			this.Topmost = (bool)jsondata["TopMost"];
			saveSettings();
			var files = Directory.GetFiles(userpath + "\\BirdyXploit\\Scripts");
			foreach (string file in files)
			{
				try
				{
					Button buttonset = new();
					buttonset.Style = (Style)Resources["OrangeButton"];
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
			close.Click += (sender, e) => {
				tabs.Items.Remove(tab);
				tb.Text = "";
				if (autoexecute.Contains(tab))
				{
					autoexecute.Remove(tab);
				}
			};
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

		bool showhidetoglesp = true;
		private void scriptslib_Click(object sender, RoutedEventArgs e)
		{
			if (showhidetoglesp)
			{
				scriptspane.Visibility = Visibility.Visible;
                CircleEase easing = new CircleEase();  // or whatever easing class you want
                easing.EasingMode = EasingMode.EaseOut;
                DoubleAnimation ani = new();
				ani.From = 0;
				ani.To = 300;
				ani.EasingFunction = easing;
				ani.Duration = new Duration(TimeSpan.FromSeconds(0.5));
				scriptspane.BeginAnimation(DockPanel.WidthProperty, ani);
            }
			else
			{
                scriptspane.Visibility = Visibility.Visible;
                CircleEase easing = new CircleEase();  // or whatever easing class you want
                easing.EasingMode = EasingMode.EaseOut;
                DoubleAnimation ani = new();
                ani.From = 300;
                ani.To = 0;
                ani.EasingFunction = easing;
                ani.Duration = new Duration(TimeSpan.FromSeconds(0.5));
                scriptspane.BeginAnimation(DockPanel.WidthProperty, ani);
				ani.Completed += (s,e) => { scriptspane.Visibility = Visibility.Collapsed; };
			}
			showhidetoglesp = !showhidetoglesp;
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

		bool isfirstsbopen = true;
        private void scriptblox_Click(object sender, RoutedEventArgs e)
        {
			if (scriptbloxMenu.Visibility == Visibility.Collapsed)
			{
				scriptbloxMenu.Visibility = Visibility.Visible;
				BlurEffect blur = new BlurEffect() { Radius = 8 };
                Mainsp.Effect = blur;
                CircleEase easing = new CircleEase();  // or whatever easing class you want
                easing.EasingMode = EasingMode.EaseOut;
                DoubleAnimation ani = new();
                ani.From = 0;
                ani.To = 8;
                ani.EasingFunction = easing;
                ani.Duration = new Duration(TimeSpan.FromSeconds(0.5));
                blur.BeginAnimation(BlurEffect.RadiusProperty, ani);
                DoubleAnimation anio = new();
                anio.From = 0;
                anio.To = 1;
                anio.EasingFunction = easing;
                anio.Duration = new Duration(TimeSpan.FromSeconds(0.5));
                scriptbloxMenu.BeginAnimation(DockPanel.OpacityProperty, anio);
            }
            else
			{
                scriptbloxMenu.Visibility = Visibility.Collapsed;
                Mainsp.Effect = null;
            }
			if (isfirstsbopen)
			{
				var task = client.GetAsync("https://scriptblox.com/api/script/fetch?page=1");
				task.ContinueWith((Task<HttpResponseMessage> httpTask) => {
                    Task<string> task = httpTask.Result.Content.ReadAsStringAsync();
                    Task continuation = task.ContinueWith(t =>
                    {
						string result = t.Result;
						showscriptresults(result);
					});
                });
                isfirstsbopen = false;
			}
        }
		void showscriptresults(string result)
		{
			Dictionary<string, object> data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
			var gresult = (JObject)data["result"];
            Application.Current.Dispatcher.Invoke((Action)delegate {
                scriptsPane.Children.Clear();
                foreach (var script in gresult["scripts"])
				{

                    Border spitemparent = new();
                    spitemparent.BorderThickness = new Thickness(1);
                    spitemparent.Background = backgroundcolor;
					spitemparent.BorderBrush = Brushes.Gray;
					spitemparent.Margin = new Thickness(2);
                    spitemparent.CornerRadius = new CornerRadius(5);
                    StackPanel spitem = new();
                    spitem.Margin = new Thickness(3);
                    spitem.Width = 325;
					Image img = new();
					img.Stretch = Stretch.Fill;
					img.Height = 200;
					img.HorizontalAlignment = HorizontalAlignment.Stretch;
					string urlimg = (string)(script["game"]["imageUrl"]);
					//MessageBox.Show(urlimg);
					if (!urlimg.Contains("://"))
					{
						urlimg = "https://scriptblox.com" + urlimg;
                    }
                    img.Source = new BitmapImage(new Uri(urlimg,UriKind.Absolute));
					spitem.Children.Add(img);
                    Label title = new();
                    title.Padding = new Thickness(0);
                    title.FontWeight = FontWeights.Bold;
					title.Foreground = foregroundcolor;
                    title.Content = script["title"];
                    spitem.Children.Add(title);
                    Label forgame = new();
                    forgame.Padding = new Thickness(0);
					forgame.Foreground = foregroundcolor;
                    forgame.Content = "For " + script["game"]["name"] + " (" + script["game"]["gameId"] + ")";
                    spitem.Children.Add(forgame);
					if (script["owner"] != null)
					{
                        Label author = new();
						author.Foreground = foregroundcolor;
                        author.Padding = new Thickness(0);
                        author.Content = "By " + script["owner"]["username"];
                        spitem.Children.Add(author);
                    }
                    Button selectbtn = new();
					selectbtn.Foreground = foregroundcolor;
                    selectbtn.Background = Brushes.Transparent;
                    selectbtn.BorderBrush = Brushes.Transparent;
                    selectbtn.Padding = new Thickness(0);
                    selectbtn.Content = "Select Script";
                    selectbtn.Click += (s, e) =>
                    {
                        var task = client.GetAsync("https://scriptblox.com/api/script/" + script["_id"]);
                        task.ContinueWith((Task<HttpResponseMessage> httpTask) =>
                        {
                            Task<string> task = httpTask.Result.Content.ReadAsStringAsync();
                            Task continuation = task.ContinueWith(ts =>
                            {
                                string results = ts.Result;
                                Dictionary<string, object> datas = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(results);
                                Application.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    TabItem tab = tabs.SelectedItem as TabItem;
                                    TextBox tb = tab.Content as TextBox;
                                    tb.Text = (string)((datas["script"] as JObject)["script"]);
                                    scriptbloxMenu.Visibility = Visibility.Collapsed;
									Mainsp.Effect = null;
                                });
                            });
                        });
                    };
                    spitem.Children.Add(selectbtn);
                    spitemparent.Child = spitem;
                    scriptsPane.Children.Add(spitemparent);
				}
            });
        }

        private void searchTextbox_KeyUp(object sender, KeyEventArgs e)
        {
			if (e.Key == Key.Enter)
			{
				var url = "https://scriptblox.com/api/script/search?q=" + Uri.EscapeDataString(searchTextbox.Text) + "&max=100&mode=free&page=1";
                if (searchTextbox.Text.Trim() == "")
				{
					url = "https://scriptblox.com/api/script/fetch?page=1";
				}
				var task = client.GetAsync(url);
                task.ContinueWith((Task<HttpResponseMessage> httpTask) => {
                    Task<string> task = httpTask.Result.Content.ReadAsStringAsync();
                    Task continuation = task.ContinueWith(t =>
                    {
                        string result = t.Result;
                        showscriptresults(result);
                    });
                });
            }
        }
    }
}
