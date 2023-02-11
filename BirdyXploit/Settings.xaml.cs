using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace BirdyXploit
{
	/// <summary>
	/// Interaction logic for Settings.xaml
	/// </summary>
	public partial class Settings : Window
	{
		string userpath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
		public Dictionary<string, object> jsondata = null;
		public Settings(Dictionary<string, object> settings)
		{
			jsondata = settings;
			InitializeComponent();
			Darkmode.IsChecked = (string)jsondata["Theme"] == "Dark";
			Topmosta.IsChecked = (bool)jsondata["TopMost"];
			AutoInject.IsChecked = (bool)jsondata["AutoInject"];
			this.Topmost = (bool)jsondata["TopMost"];
		}

		void saveSettings()
		{
			var settingsstring = Newtonsoft.Json.JsonConvert.SerializeObject(jsondata);
			File.WriteAllText(userpath + "\\BirdyXploit\\Settings.json", settingsstring);
		}

		private void Darkmode_Click(object sender, RoutedEventArgs e)
		{
			if ((bool)Darkmode.IsChecked)
			{
				jsondata["Theme"] = "Dark";
			}
			else
			{
				jsondata["Theme"] = "Light";
			}
			saveSettings();
		}

		private void Topmost_Click(object sender, RoutedEventArgs e)
		{
			jsondata["TopMost"] = Topmosta.IsChecked;
			saveSettings();
		}

		private void RestartButton_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(Application.ResourceAssembly.Location);
			Application.Current.Shutdown();
		}

		private void AutoInject_Click(object sender, RoutedEventArgs e)
		{
			jsondata["AutoInject"] = AutoInject.IsChecked;
			saveSettings();
		}
	}
}
