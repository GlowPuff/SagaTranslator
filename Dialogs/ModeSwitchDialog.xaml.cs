using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Imperial_Commander_Editor;
using Microsoft.Win32;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for ModeSwitchDialog.xaml
	/// </summary>
	public partial class ModeSwitchDialog : Window, INotifyPropertyChanged
	{
		string _combo1Label, _combo2Label;
		MetaDisplay[] missionExpansions, missionCardTextExpansions, campaignInfoExpansions, deploymentGroups, missionInfoExpansions, tutorialMissions;
		string[] singleShotData;
		int[] missionCounts;
		bool isTutorial;

		public SourceData sourceData;
		public List<string> fileList;
		public string combo1Label { get { return _combo1Label; } set { _combo1Label = value; PC(); } }
		public string combo2Label { get { return _combo2Label; } set { _combo2Label = value; PC(); } }
		public ObservableCollection<MetaDisplay> combo1List, combo2List;
		public MetaDisplay combo1SelectedMeta, combo2SelectedMeta;

		public event PropertyChangedEventHandler? PropertyChanged;
		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		public ModeSwitchDialog()
		{
			InitializeComponent();
			DataContext = this;

			sourceData = new SourceData();

			fileCB.ItemsSource = new List<string>( new string[] {
			"User Interface\t\tui.json",//single shot data
			"Bonus Effects\t\tbonuseffects.json",//single shot data
			"Enemy Instructions\tinstructions.json",//single shot data
			"Events\t\t\tevents.json",//single shot data
			"Campaign Items\t\titems.json",//single shot data
			"Campaign Rewards\trewards.json",//single shot data
			"Campaign Skills\t\tskills.json",//single shot data
			"Campaign Info\t\tBespinInfo.txt, LothalInfo.txt, etc.",//2nd selection needed
			"Deployment Card Text\tallies.json, enemies.json, etc.",//2nd selection needed
			"Mission Card Text\t\tbespin.json, jabba.json, etc.",//2nd selection needed
			"Mission Info / Rules\tcore1info.txt, hoth7rules.txt, etc.",//2nd selection needed
			"Mission\t\t\tVaries by Mission, xxx.json",//2nd selection needed
			"Tutorials\t\t\tTUTORIAL01.json, TUTORIAL02.json, etc.",//2nd selection needed
			} );

			campaignInfoExpansions = new MetaDisplay[]
			{
				new MetaDisplay("Core","CampaignInfo.CoreInfo.txt"),
				new MetaDisplay("Twin Shadows","CampaignInfo.TwinInfo.txt"),
				new MetaDisplay("Return to Hoth","CampaignInfo.HothInfo.txt"),
				new MetaDisplay("The Bespin Gambit","CampaignInfo.BespinInfo.txt"),
				new MetaDisplay("Jabba's Realm","CampaignInfo.JabbaInfo.txt"),
				new MetaDisplay("Heart of the Empire","CampaignInfo.EmpireInfo.txt"),
				new MetaDisplay("Tyrants of Lothal","CampaignInfo.LothalInfo.txt"),
			};

			//mission card text
			missionCardTextExpansions = new MetaDisplay[]
			{
				new MetaDisplay("Core","MissionCardText.core.json"),
				new MetaDisplay("Twin Shadows","MissionCardText.twin.json"),
				new MetaDisplay("Return to Hoth","MissionCardText.hoth.json"),
				new MetaDisplay("The Bespin Gambit","MissionCardText.bespin.json"),
				new MetaDisplay("Jabba's Realm","MissionCardText.jabba.json"),
				new MetaDisplay("Heart of the Empire","MissionCardText.empire.json"),
				new MetaDisplay("Tyrants of Lothal","MissionCardText.lothal.json"),
				new MetaDisplay("Other","MissionCardText.other.json")
			};

			//Mission expansion list, including Custom
			missionExpansions = new MetaDisplay[]
			{
				new MetaDisplay("Core","Core"),
				new MetaDisplay("Twin Shadows","Twin"),
				new MetaDisplay("Return to Hoth","Hoth"),
				new MetaDisplay("The Bespin Gambit","Bespin"),
				new MetaDisplay("Jabba's Realm","Jabba"),
				new MetaDisplay("Heart of the Empire","Empire"),
				new MetaDisplay("Tyrants of Lothal","Lothal"),
				new MetaDisplay("Other","Other"),
				new MetaDisplay("Custom","")//#8
			};

			missionInfoExpansions = new MetaDisplay[]
			{
				new("Core","MissionText.core"),
				new("Twin Shadows","MissionText.twin"),
				new("Return to Hoth","MissionText.hoth"),
				new("The Bespin Gambit","MissionText.bespin"),
				new("Jabba's Realm","MissionText.jabba"),
				new("Heart of the Empire","MissionText.empire"),
				new("Tyrants of Lothal","MissionText.lothal"),
				new("Other","MissionText.other")
			};

			deploymentGroups = new MetaDisplay[]
			{
				new MetaDisplay("Allies","DeploymentGroups.allies.json"),
				new MetaDisplay("Enemies","DeploymentGroups.enemies.json"),
				new MetaDisplay("Heroes","DeploymentGroups.heroes.json"),
				new MetaDisplay("Villains","DeploymentGroups.villains.json"),
			};

			tutorialMissions = new MetaDisplay[]
			{
				new MetaDisplay("TUTORIAL01","https://raw.githubusercontent.com/GlowPuff/ImperialCommander2/main/ImperialCommander2/Assets/Resources/SagaTutorials/En/TUTORIAL01.json"){ missionExpansionFolder="Tutorial"},
				new MetaDisplay("TUTORIAL02","https://raw.githubusercontent.com/GlowPuff/ImperialCommander2/main/ImperialCommander2/Assets/Resources/SagaTutorials/En/TUTORIAL02.json"){ missionExpansionFolder="Tutorial"},
				new MetaDisplay("TUTORIAL03","https://raw.githubusercontent.com/GlowPuff/ImperialCommander2/main/ImperialCommander2/Assets/Resources/SagaTutorials/En/TUTORIAL03.json"){ missionExpansionFolder="Tutorial"}
			};

			singleShotData = new string[]
				{
				"ui.json",
				"bonuseffects.json",
				"instructions.json",
				"events.json",
				"items.json",
				"rewards.json",
				"skills.json"
				};

			missionCounts = new int[] { 32, 6, 16, 6, 16, 16, 6, 40 };
		}

		private void cancelBtn_Click( object sender, RoutedEventArgs e )
		{
			e.Handled = true;
			sourceData = new();
			Close();
		}

		private void supportedBtn_Click( object sender, RoutedEventArgs e )
		{
			var dlg = new SupportedFilesDialog();
			dlg.ShowDialog();
		}

		private void continueBtn_Click( object sender, RoutedEventArgs e )
		{
			Close();
		}

		private void fileCB_SelectionChanged( object sender, System.Windows.Controls.SelectionChangedEventArgs e )
		{
			//reset the UI and metadata
			sourceData = new();
			sourceData.fileMode = (FileMode)fileCB.SelectedIndex;

			isTutorial = false;
			combo1Label = combo2Label = customPathLabel.Text = downloadStatus.Text = "";
			combo1CB.ItemsSource = new List<string>();
			combo2CB.ItemsSource = new List<string>();
			combo1CB.SelectedIndex = -1;
			combo2CB.SelectedIndex = -1;
			combo1Box.Visibility = Visibility.Collapsed;
			combo2Box.Visibility = Visibility.Collapsed;
			customBox.Visibility = Visibility.Collapsed;
			downloadBox.Visibility = Visibility.Collapsed;
			continueBtn.IsEnabled = false;

			//first 7 items are single shot files that don't require a second selection, so allow Continue to open them up immediately
			if ( fileCB.SelectedIndex >= 0 && fileCB.SelectedIndex <= 6 )
			{
				//handle first 7 items, load the built-in JSON data
				sourceData.metaDisplay = new( "", singleShotData[fileCB.SelectedIndex] );
				sourceData.stringifiedJsonData = FileManager.LoadBuiltinJSON( singleShotData[fileCB.SelectedIndex] );
				if ( sourceData.stringifiedJsonData != string.Empty )
					continueBtn.IsEnabled = true;
			}
			//CampaignInfo
			else if ( fileCB.SelectedIndex == 7 )
			{
				combo1Label = "Select a Campaign Expansion:";
				combo1Box.Visibility = Visibility.Visible;
				combo1CB.ItemsSource = campaignInfoExpansions;
				combo1CB.SelectedIndex = -1;
			}
			//DeploymentGroups
			else if ( fileCB.SelectedIndex == 8 )
			{
				combo1Label = "Select a Deployment Group:";
				combo1Box.Visibility = Visibility.Visible;
				combo1CB.ItemsSource = deploymentGroups;
				combo1CB.SelectedIndex = -1;
			}
			//MissionCardText
			else if ( fileCB.SelectedIndex == 9 )
			{
				combo1Label = "Select an Expansion:";
				combo1Box.Visibility = Visibility.Visible;
				combo1CB.ItemsSource = missionCardTextExpansions;
				combo1CB.SelectedIndex = -1;
			}
			//MissionText (mission info)
			else if ( fileCB.SelectedIndex == 10 )
			{
				combo1Label = "Select an Expansion:";
				combo1Box.Visibility = Visibility.Visible;
				combo1CB.ItemsSource = missionInfoExpansions;
				combo1CB.SelectedIndex = -1;
			}
			//Mission
			else if ( fileCB.SelectedIndex == 11 )
			{
				combo1Label = "Select an Expansion:";
				combo1Box.Visibility = Visibility.Visible;
				combo1CB.ItemsSource = missionExpansions;
				combo1CB.SelectedIndex = -1;
			}
			//Tutorials
			else if ( fileCB.SelectedIndex == 12 )
			{
				sourceData.fileMode = FileMode.Mission;
				isTutorial = true;
				combo1Label = "Select a Tutorial:";
				combo1Box.Visibility = Visibility.Visible;
				combo1CB.ItemsSource = tutorialMissions;
				combo1CB.SelectedIndex = -1;
			}
		}

		private void combo1CB_SelectionChanged( object sender, System.Windows.Controls.SelectionChangedEventArgs e )
		{
			if ( combo1CB.SelectedIndex == -1 )
				return;

			combo2Box.Visibility = Visibility.Collapsed;
			customBox.Visibility = Visibility.Collapsed;
			downloadBox.Visibility = Visibility.Collapsed;
			successIcon.Visibility = Visibility.Collapsed;
			spinner.Visibility = Visibility.Collapsed;
			continueBtn.IsEnabled = false;

			sourceData.stringifiedJsonData = string.Empty;

			if ( sourceData.fileMode == FileMode.CampaignInfo
				|| sourceData.fileMode == FileMode.DeploymentGroups
				|| sourceData.fileMode == FileMode.MissionCardText )
			{
				//load the data based on the asset selected
				sourceData.metaDisplay = combo1CB.SelectedItem as MetaDisplay;
				sourceData.stringifiedJsonData = FileManager.LoadBuiltinJSON( sourceData.metaDisplay.assetName );
			}
			else if ( sourceData.fileMode == FileMode.MissionInfo )
			{
				//load the data based on the asset selected
				sourceData.metaDisplay = combo1CB.SelectedItem as MetaDisplay;
				//assetName is the expansion name, ie: "core"
				var files = FileManager.FindAssetsWithName( sourceData.metaDisplay.assetName );
				combo2Label = "Select Info or Rules data to translate:";
				combo2Box.Visibility = Visibility.Visible;
				combo2CB.ItemsSource = files.Select( x =>
				{
					var split = x.Split( '.' ).Reverse().Take( 2 ).Reverse().ToArray();
					return new MetaDisplay( $"{split[0]}.{split[1]}", x );
				} );
			}
			else if ( sourceData.fileMode == FileMode.Mission && !isTutorial )
			{
				//if it's a non-Custom mission...
				if ( combo1CB.SelectedIndex != 8 )
				{
					//load the data based on the asset selected, ie: "Core", "Bespin", etc
					sourceData.metaDisplay = combo1CB.SelectedItem as MetaDisplay;
					//assetName is the expansion name, ie: "core"
					combo2Label = $"Select a '{sourceData.metaDisplay.displayName}' Mission:";
					string[] files = new string[missionCounts[combo1CB.SelectedIndex]];
					combo2Box.Visibility = Visibility.Visible;
					int counter = 0;
					combo2CB.ItemsSource = files.Select( x =>
					{
						//URL FORMAT:
						//https://raw.githubusercontent.com/GlowPuff/ImperialCommander2/main/ImperialCommander2/Assets/SagaMissions/08Other/OTHER33.json
						counter++;
						string baseurl = $"https://raw.githubusercontent.com/GlowPuff/ImperialCommander2/main/ImperialCommander2/Assets/SagaMissions/0{combo1CB.SelectedIndex + 1}{sourceData.metaDisplay.assetName}/{sourceData.metaDisplay.assetName.ToUpper()}{counter}.json";
						//look up the mission name
						string expansion = sourceData.metaDisplay.assetName;
						var data = FileManager.LoadBuiltinJSON( $"MissionCardText.{expansion.ToLower()}.json" );
						int c = counter;
						string e = expansion;
						var mcard = FileManager.LoadJSONFromString<List<MissionCardText>>( data ).Where( x => x.id == $"{expansion}{counter}" ).First();

						return new MetaDisplay( $"{sourceData.metaDisplay.assetName.ToUpper()}{counter}", $"{baseurl}" ) { missionExpansionFolder = $"0{combo1CB.SelectedIndex + 1}{sourceData.metaDisplay.assetName}", comboBoxTitle = $"{sourceData.metaDisplay.assetName.ToUpper()}{counter} - {mcard.name}" };
					} );
				}
				else
				{
					//show the Load Custom mission box
					customPathLabel.Text = "";
					customBox.Visibility = Visibility.Visible;
				}
			}
			else if ( sourceData.fileMode == FileMode.Mission && isTutorial )
			{
				//load the data based on the asset selected
				sourceData.metaDisplay = combo1CB.SelectedItem as MetaDisplay;
				//assetName is the GitHub url
				downloadStatus.Text = "";
				continueBtn.IsEnabled = false;
				successIcon.Visibility = Visibility.Collapsed;
				downloadBox.Visibility = Visibility.Visible;
				downloadBtn.IsEnabled = true;
				//check if there is a cached copy
				if ( CheckCache() )
				{
					downloadStatus.Text = "Or Continue with cached copy.";
					continueBtn.IsEnabled = true;
				}
			}

			if ( sourceData.stringifiedJsonData != string.Empty )
				continueBtn.IsEnabled = true;
		}

		private void combo2CB_SelectionChanged( object sender, System.Windows.Controls.SelectionChangedEventArgs e )
		{
			if ( combo2CB.SelectedIndex == -1 )
				return;

			if ( sourceData.fileMode != FileMode.Mission )
			{
				sourceData.metaDisplay = combo2CB.SelectedItem as MetaDisplay;
				sourceData.stringifiedJsonData = FileManager.LoadBuiltinJSON( sourceData.metaDisplay.assetName );

				if ( sourceData.stringifiedJsonData != string.Empty )
					continueBtn.IsEnabled = true;
			}
			else
			{
				downloadStatus.Text = "";
				continueBtn.IsEnabled = false;
				sourceData.metaDisplay = combo2CB.SelectedItem as MetaDisplay;
				successIcon.Visibility = Visibility.Collapsed;
				downloadBox.Visibility = Visibility.Visible;
				downloadBtn.IsEnabled = true;
				//check if there is a cached copy
				if ( CheckCache() )
				{
					downloadStatus.Text = "Or Continue with cached copy.";
					continueBtn.IsEnabled = true;
				}
				//Utils.Log( $"SELECTED: EXP={sourceData.metaDisplay.missionExpansionFolder}" );
			}
		}

		private void downloadBtn_Click( object sender, RoutedEventArgs e )
		{
			spinner.Visibility = Visibility.Visible;
			successIcon.Visibility = Visibility.Collapsed;
			downloadBtn.IsEnabled = false;
			cancelBtn.IsEnabled = false;
			combo1Box.IsEnabled = combo2Box.IsEnabled = false;
			downloadStatus.Text = "Downloading Mission...";
			sourceData.stringifiedJsonData = string.Empty;

			if ( NetworkInterface.GetIsNetworkAvailable() )
				Task.Run( CheckInternet );
			else
			{
				spinner.Visibility = Visibility.Collapsed;
				downloadBtn.IsEnabled = true;
				cancelBtn.IsEnabled = true;
				combo1Box.IsEnabled = combo2Box.IsEnabled = true;
				downloadStatus.Text = "Error Downloading Mission.";
			}
		}

		private async void CheckInternet()
		{
			//first check if internet is available
			var ping = new Ping();
			var reply = ping.Send( new IPAddress( new byte[] { 8, 8, 8, 8 } ), 5000 );
			if ( reply.Status == IPStatus.Success )
			{
				//internet available, check for latest version
				await DownloadMission();
			}
			else
			{
				spinner.Visibility = Visibility.Collapsed;
				downloadBtn.IsEnabled = true;
				cancelBtn.IsEnabled = true;
				combo1Box.IsEnabled = combo2Box.IsEnabled = true;
				downloadStatus.Text = "Internet Not Available.";
			}
		}

		private async Task DownloadMission()
		{
			var web = new HttpClient();
			web.DefaultRequestHeaders.Add( "User-Agent", "request" );
			var result = await web.GetAsync( sourceData.metaDisplay.assetName );

			if ( !result.IsSuccessStatusCode )//failed
			{
				//Debug.Log( "network error" );
				Dispatcher.Invoke( () =>
				{
					spinner.Visibility = Visibility.Collapsed;
					downloadBtn.IsEnabled = true;
					cancelBtn.IsEnabled = true;
					combo1Box.IsEnabled = combo2Box.IsEnabled = true;
					downloadStatus.Text = "Error Downloading Mission";
				} );
			}
			else//success
			{
				string response = await result.Content.ReadAsStringAsync();
				//response is the stringified JSON
				sourceData.stringifiedJsonData = response;

				Dispatcher.Invoke( () =>
				{
					//save the file to cached mission folder
					SaveMission();
					spinner.Visibility = Visibility.Collapsed;
					successIcon.Visibility = Visibility.Visible;
					cancelBtn.IsEnabled = true;
					continueBtn.IsEnabled = true;
					combo1Box.IsEnabled = combo2Box.IsEnabled = true;
					downloadStatus.Text = "Download Complete";
				} );
			}
		}

		private void SaveMission()
		{
			try
			{
				if ( !Directory.Exists( Path.Combine( MainWindow.appPath, "CachedMissions", $"{sourceData.metaDisplay.missionExpansionFolder}" ) ) )
					Directory.CreateDirectory( Path.Combine( MainWindow.appPath, "CachedMissions", $"{sourceData.metaDisplay.missionExpansionFolder}" ) );
				File.WriteAllText( Path.Combine( MainWindow.appPath, "CachedMissions", $"{sourceData.metaDisplay.missionExpansionFolder}", sourceData.metaDisplay.displayName + ".json" ), sourceData.stringifiedJsonData );
			}
			catch ( Exception e )
			{
				MessageBox.Show( "SaveMission()\nCould not save the cached Mission file." + "\n\n" + e.Message, "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
			}
		}

		/// <summary>
		/// Checks if a mission is cached and loads it into sourceData.stringifiedJsonData
		/// </summary>
		private bool CheckCache()
		{
			bool exists = false;
			try
			{
				//displayname = filename only, add ".json"
				string path = Path.Combine( MainWindow.appPath, "CachedMissions", sourceData.metaDisplay.missionExpansionFolder, sourceData.metaDisplay.displayName + ".json" );
				exists = File.Exists( path );
				//now load it
				if ( exists )
					sourceData.stringifiedJsonData = File.ReadAllText( path );
				else
					return false;
			}
			catch ( Exception e )
			{
				MessageBox.Show( "CheckCache()\nError checking the cached Mission file." + "\n\n" + e.Message, "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
				return false;
			}

			return exists;
		}

		private void customBtn_Click( object sender, RoutedEventArgs e )
		{
			continueBtn.IsEnabled = false;
			customPathLabel.Text = "";

			try
			{
				OpenFileDialog od = new();
				od.InitialDirectory = Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments );
				od.Filter = "Mission File (*.json)|*.json";
				od.Title = "Open SOURCE Custom Mission (English)";
				if ( od.ShowDialog() == true )
				{
					var mission = FileManager.LoadMission( od.FileName );
					if ( mission != null )
					{
						string c = File.ReadAllText( od.FileName );
						//do simple check to see if it's a mission
						if ( c.Contains( "missionGUID" ) )
						{
							var fi = new FileInfo( od.FileName );
							int idx = fi.Name.IndexOf( fi.Extension );
							customPathLabel.Text = $"Loaded Source: {fi.Name}.";
							sourceData.metaDisplay.assetName = "";
							sourceData.metaDisplay.displayName = fi.Name.Substring( 0, idx );
							sourceData.stringifiedJsonData = c;
							continueBtn.IsEnabled = true;
						}
						else
							throw new( "Translator expected a Mission JSON file, but the received file doesn't appear to be one." );
					}
				}
			}
			catch ( Exception ex )
			{
				MessageBox.Show( "Error loading Custom Mission file." + "\n\n" + ex.Message, "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
			}
		}
	}
}
