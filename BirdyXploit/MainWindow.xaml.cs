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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BirdyXploit
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		DispatcherTimer timer = new();
		DispatcherTimer animation = new();
		string userpath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
		Dictionary<string, object> settings;
		public MainWindow()
		{
			InitializeComponent();
			timer.Interval = TimeSpan.FromSeconds(1);
			timer.Start();
			timer.Tick += ticked;
			animation.Interval = TimeSpan.FromMilliseconds(5);
			animation.Start();
			animation.Tick += animationtick;
			if (!Directory.Exists(userpath + "\\BirdyXploit"))
			{
				Directory.CreateDirectory(userpath + "\\BirdyXploit");
				Directory.CreateDirectory(userpath + "\\BirdyXploit\\Scripts");
			}
			if (!File.Exists(userpath + "\\BirdyXploit\\Settings.json")) {
				File.WriteAllText(userpath + "\\BirdyXploit\\Settings.json", "{}");
			}
			settings = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string,object>>(File.ReadAllText(userpath + "\\BirdyXploit\\Settings.json"));
			if (!settings.ContainsKey("Theme"))
			{
				settings["Theme"] = "Light";
			}
			if (!settings.ContainsKey("TopMost"))
			{
				settings["TopMost"] = true;
			}
			if (!settings.ContainsKey("AutoInject"))
			{
				settings["AutoInject"] = false;
			}
		}

		void animationtick(object sender, EventArgs e) {
			rotationspin.Angle += 10;
		}

		void ticked(object sender, EventArgs e)
		{
			timer.Stop();
			animation.Stop();
			Main window = new(settings);
			this.Close();
			window.Show();
		}
	}
}
