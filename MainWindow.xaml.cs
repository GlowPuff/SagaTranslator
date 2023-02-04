using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Imperial_Commander_Editor;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		private GitHubResponse gitHubResponse = null;
		private ITranslationPanel _translationObject;
		private bool hasSaved = false, canSave = false;
		private Mission _translatedMission, _sourceMission;
		private UILanguage _sourceUI, _translatedUI;
		private GenericUIData _sourceModel, _translatedModel;
		private string _sourceMissionFilename;

		public GenericUIData sourceDynamicUIModel { get { return _sourceModel; } set { _sourceModel = value; PC(); } }
		public GenericUIData translatedDynamicUIModel { get { return _translatedModel; } set { _translatedModel = value; PC(); } }
		public ITranslationPanel translationObject { get { return _translationObject; } set { _translationObject = value; PC(); } }
		public AppModel appModel { get; set; }
		public Mission sourceMission { get => _sourceMission; set { _sourceMission = value; PC(); } }
		public Mission? translatedMission { get => _translatedMission; set { _translatedMission = value; PC(); } }
		public UILanguage sourceUI { get => _sourceUI; set { _sourceUI = value; PC(); } }
		public UILanguage? translatedUI { get => _translatedUI; set { _translatedUI = value; PC(); } }
		public string sourceMissionFilename { get => _sourceMissionFilename; set { _sourceMissionFilename = value; PC(); } }
		public static string appPath { get { return AppDomain.CurrentDomain.BaseDirectory; } }

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		/// <summary>
		/// ctor for starting app the first time in the session
		/// </summary>
		public MainWindow()
		{
			InitializeComponent();
			DataContext = this;

			appModel = new( this, TranslateMode.Cancel, FileMode.Cancel );
			translationObject = new WelcomePanel();

			if ( NetworkInterface.GetIsNetworkAvailable() )
				Task.Run( StartVersionCheck );
			else
				updateCheck.Text = "Error Checking For Update";

			Utils.Init( this );
			Title = "Saga Translator [Choose a Translation Source]";

			//create default folder for cached Missions
			if ( !Directory.Exists( Path.Combine( appPath, "CachedMissions" ) ) )
				Directory.CreateDirectory( Path.Combine( appPath, "CachedMissions" ) );
			if ( !Directory.Exists( Path.Combine( appPath, "SavedTranslations" ) ) )
				Directory.CreateDirectory( Path.Combine( appPath, "SavedTranslations" ) );
		}

		/// <summary>
		/// ctor after selecting data to translate
		/// </summary>
		public MainWindow( SourceData sourceData, TranslateMode tmode )
		{
			InitializeComponent();
			DataContext = this;

			Utils.Init( this );
			appModel = new AppModel( this, tmode, sourceData.fileMode );
			if ( tmode == TranslateMode.Mission )
			{
				sourceMissionFilename = sourceData.metaDisplay.displayName + ".json";
				appModel.TranslatedFileName = sourceMissionFilename;
			}
			else
			{
				appModel.TranslatedFileName = sourceData.fileName;
			}
			//default save base path is the app location
			appModel.TranslatedFilePath = Path.Combine( appPath, "SavedTranslations" );
			appModel.NothingSelected = false;

			translationObject = new WelcomePanel();
			((WelcomePanel)translationObject).EnableTranslationDrop();

			if ( NetworkInterface.GetIsNetworkAvailable() )
				Task.Run( StartVersionCheck );
			else
				updateCheck.Text = "Error Checking For Update";

			if ( tmode == TranslateMode.Supplemental )
				SetupSupplemental( sourceData );
			else if ( tmode == TranslateMode.UI )
				SetupUI( sourceData );
			else if ( tmode == TranslateMode.Mission )
				SetupMission( sourceData );

			Title = $"Saga Translator [{sourceData.fileMode}] - FILE NOT SAVED";

			saveMissionButton.IsEnabled = true;
			openMissionTranslatedButton.IsEnabled = true;
			canSave = true;
		}

		/// <summary>
		/// Setup app for non-Mission and non-UI data
		/// </summary>
		private void SetupSupplemental( SourceData sourceData )
		{
			ParsedObject data = DynamicParser.Parse( sourceData );
			//set the SOURCE data
			sourceDynamicUIModel = data.data;
			//set the TRANSLATED MISSION
			translatedDynamicUIModel = data.dataCopy;

			sourceText.Visibility = Visibility.Collapsed;
			sourceUIText.Visibility = Visibility.Visible;
			sourceUIText.Text = sourceData.fileName;

			PopulateDynamicTree( data.gType );

			languageCB.Visibility = Visibility.Collapsed;
		}

		/// <summary>
		/// Setup app for Mission data
		/// </summary>
		private void SetupMission( SourceData sourceData )
		{
			//set the ENGLISH SOURCE
			sourceMission = JsonConvert.DeserializeObject<Mission>( sourceData.stringifiedJsonData );
			//set the TRANSLATED MISSION
			translatedMission = JsonConvert.DeserializeObject<Mission>( sourceData.stringifiedJsonData );
			if ( string.IsNullOrEmpty( translatedMission.languageID ) )
				translatedMission.languageID = "Select Target Language";

			sourceText.Visibility = Visibility.Visible;
			sourceUIText.Visibility = Visibility.Collapsed;
			languageCB.IsEnabled = true;

			PopulateMainTree();

			Binding binding = new( "languageID" )
			{
				Source = translatedMission
			};
			languageCB.SetBinding( ComboBox.SelectedValueProperty, binding );
		}

		/// <summary>
		/// Setup app for UI data
		/// </summary>
		private void SetupUI( SourceData translationMeta )
		{
			//set the ENGLISH SOURCE
			sourceUI = JsonConvert.DeserializeObject<UILanguage>( translationMeta.stringifiedJsonData );
			//set the TRANSLATED UI
			translatedUI = JsonConvert.DeserializeObject<UILanguage>( translationMeta.stringifiedJsonData );
			if ( string.IsNullOrEmpty( translatedUI.languageID ) )
				translatedUI.languageID = "Select Target Language";

			sourceText.Visibility = Visibility.Collapsed;
			sourceUIText.Visibility = Visibility.Visible;
			languageCB.IsEnabled = true;

			PopulateUIMainTree();

			Binding binding = new( "languageID" )
			{
				Source = translatedUI
			};
			languageCB.SetBinding( ComboBox.SelectedValueProperty, binding );
		}

		/// <summary>
		/// ctor for Mission mode
		/// </summary>
		/*public MainWindow( Mission s, string filePath )
		{
			InitializeComponent();
			DataContext = this;
			Utils.Init( this );

			appModel = new AppModel( this, TranslateMode.Mission );
			appModel.TranslatedFilePath = Path.GetDirectoryName( filePath );

			appModel.NothingSelected = false;

			if ( NetworkInterface.GetIsNetworkAvailable() )
				Task.Run( StartVersionCheck );
			else
				updateCheck.Text = "Error Checking For Update";

			//set the ENGLISH SOURCE
			sourceMission = s;
			//make a copy of the mission
			string json = JsonConvert.SerializeObject( sourceMission );
			//set the TRANSLATED MISSION
			translatedMission = JsonConvert.DeserializeObject<Mission>( json );
			if ( string.IsNullOrEmpty( translatedMission.languageID ) )
				translatedMission.languageID = "Select Target Language";

			sourceText.Visibility = Visibility.Visible;
			sourceUIText.Visibility = Visibility.Collapsed;
			languageCB.IsEnabled = true;
			saveMissionButton.IsEnabled = true;
			openMissionTranslatedButton.IsEnabled = true;
			findReplace.IsEnabled = true;
			canSave = true;

			PopulateMainTree();
			translationObject = new WelcomePanel();
			((WelcomePanel)translationObject).EnableTranslationDrop();

			appModel.SetStatus( "Mission Loaded" );

			Title = "Saga Translator [Mission] - FILE NOT SAVED";
			explorerTitle.Text = "Mission Explorer";
			Binding binding = new( "languageID" )
			{
				Source = translatedMission
			};
			languageCB.SetBinding( ComboBox.SelectedValueProperty, binding );
		}*/

		public event PropertyChangedEventHandler? PropertyChanged;

		private void Window_PreviewKeyDown( object sender, KeyEventArgs e )
		{
			if ( canSave && (e.Key == Key.S && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) )
			{
				saveMissionButton_Click( null, null );
			}
			//if ( (e.Key == Key.L && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) )
			//{
			//	openMissionButton_Click( null, null );
			//}
		}

		/// <summary>
		/// DEPRECATED
		/// </summary>
		//private void openSourceButton_Click( object sender, RoutedEventArgs e )
		//{
		//	//OpenFileDialog od = new();
		//	//od.InitialDirectory = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "ImperialCommander" );

		//	//if ( appModel.TranslateMode == TranslateMode.Mission )
		//	//{
		//	//	od.Filter = "Mission File (*.json)|*.json";
		//	//	od.Title = "Open SOURCE Mission (English)";
		//	//}
		//	//else if ( appModel.TranslateMode == TranslateMode.UI )
		//	//{
		//	//	od.Filter = "UI File (*.json)|*.json";
		//	//	od.Title = "Open SOURCE UI (English)";
		//	//}
		//	//else if ( appModel.TranslateMode == TranslateMode.Supplemental )
		//	//{
		//	//	od.Filter = "JSON/TXT File (*.json, *.txt)|*.json;*.txt";
		//	//	od.Title = "Open SOURCE DATA (English)";
		//	//}

		//	//if ( od.ShowDialog() == true )
		//	//	OpenSourceFile( od.FileName );
		//}

		private void saveMissionButton_Click( object sender, RoutedEventArgs e )
		{
			if ( appModel.TranslateMode == TranslateMode.Mission )
				HandleSaveMission();
			else if ( appModel.TranslateMode == TranslateMode.UI )
				HandleSaveUI();
			else if ( appModel.TranslateMode == TranslateMode.Supplemental )
				HandleSaveOther();
		}

		/// <summary>
		/// Save anything other than Mission and UI
		/// </summary>
		private void HandleSaveOther()
		{
			if ( !hasSaved )
			{
				SaveFileDialog od = new SaveFileDialog();
				od.AddExtension = true;
				od.InitialDirectory = appModel.TranslatedFilePath;
				od.Filter = $"{appModel.TranslateMode} File (*.json, *.txt)|*.json;*.txt";
				od.Title = $"Save Translated {appModel.TranslateMode}";
				od.FileName = Path.Combine( appModel.TranslatedFilePath, appModel.TranslatedFileName );
				if ( od.ShowDialog() == true )
				{
					appModel.TranslatedFilePath = Path.GetDirectoryName( od.FileName );
					appModel.TranslatedFileName = od.SafeFileName;
					if ( FileManager.SaveOther( translatedDynamicUIModel, od.SafeFileName, appModel.TranslatedFilePath ) )
					{
						hasSaved = true;
						Title = $"Saga Translator [{appModel.FileMode}] - " + Path.Combine( appModel.TranslatedFilePath, od.SafeFileName );
						appModel.SetStatus( "Translation Saved" );
					}
					else
					{
						appModel.SetStatus( "Error Saving Translation" );
					}
				}
			}
			else
			{
				if ( !Directory.Exists( appModel.TranslatedFilePath ) )
				{
					var di = Directory.CreateDirectory( appModel.TranslatedFilePath );
					if ( di == null )
					{
						MessageBox.Show( "Could not create the folder.\r\nTried to create: " + appModel.TranslatedFilePath, "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
						hasSaved = false;
						appModel.SetStatus( "Error Creating Directory" );
						return;
					}
				}

				if ( FileManager.SaveOther( translatedDynamicUIModel, appModel.TranslatedFileName, appModel.TranslatedFilePath ) )
				{
					hasSaved = true;
					Title = $"Saga Translator [{appModel.FileMode}] - " + Path.Combine( appModel.TranslatedFilePath, appModel.TranslatedFileName );
					appModel.SetStatus( "Translation Saved" );
				}
				else
				{
					hasSaved = false;
					appModel.SetStatus( "Error Saving Translation" );
				}
			}
		}

		/// <summary>
		/// Save UI (ui.json)
		/// </summary>
		private void HandleSaveUI()
		{
			if ( !hasSaved )
			{
				SaveFileDialog od = new SaveFileDialog();
				od.AddExtension = true;
				od.InitialDirectory = appModel.TranslatedFilePath;
				od.Filter = "UI File (*.json)|*.json";
				od.Title = "Save Translated UI";
				od.FileName = Path.Combine( appModel.TranslatedFilePath, "ui.json" );
				if ( od.ShowDialog() == true )
				{
					appModel.TranslatedFilePath = Path.GetDirectoryName( od.FileName );
					appModel.TranslatedFileName = od.SafeFileName;
					if ( FileManager.SaveUI( translatedUI, od.SafeFileName, appModel.TranslatedFilePath ) )
					{
						hasSaved = true;
						Title = "Saga Translator [UI] - " + Path.Combine( appModel.TranslatedFilePath, od.SafeFileName );
						appModel.SetStatus( "Translated UI Saved" );
					}
					else
					{
						appModel.SetStatus( "Error Saving Translated UI" );
					}
				}
			}
			else
			{
				if ( !Directory.Exists( appModel.TranslatedFilePath ) )
				{
					var di = Directory.CreateDirectory( appModel.TranslatedFilePath );
					if ( di == null )
					{
						MessageBox.Show( "Could not create the folder.\r\nTried to create: " + appModel.TranslatedFilePath, "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
						hasSaved = false;
						appModel.SetStatus( "Error Creating Directory" );
						return;
					}
				}

				if ( FileManager.SaveUI( translatedUI, appModel.TranslatedFileName, appModel.TranslatedFilePath ) )
				{
					hasSaved = true;
					Title = "Saga Translator [UI] - " + Path.Combine( appModel.TranslatedFilePath, appModel.TranslatedFileName );
					appModel.SetStatus( "Translated UI Saved" );
				}
				else
				{
					hasSaved = false;
					appModel.SetStatus( "Error Saving Translated UI" );
				}
			}
		}

		/// <summary>
		/// Save Mission
		/// </summary>
		private void HandleSaveMission()
		{
			if ( !hasSaved )
			{
				SaveFileDialog od = new SaveFileDialog();
				od.AddExtension = true;
				od.InitialDirectory = appModel.TranslatedFilePath;
				od.Filter = "Mission File (*.json)|*.json";
				od.Title = "Save Translated Mission";
				od.FileName = Path.Combine( appModel.TranslatedFilePath, appModel.TranslatedFileName );
				if ( od.ShowDialog() == true )
				{
					appModel.TranslatedFilePath = Path.GetDirectoryName( od.FileName );
					appModel.TranslatedFileName = Path.GetFileName( od.FileName );
					translatedMission.fileName = od.SafeFileName;
					if ( FileManager.SaveMission( translatedMission, appModel.TranslatedFilePath, appModel.TranslatedFileName ) )
					{
						hasSaved = true;
						Title = "Saga Translator [Mission] - " + Path.Combine( appModel.TranslatedFilePath, translatedMission.fileName );
						appModel.SetStatus( "Translated Mission Saved" );
					}
					else
					{
						appModel.SetStatus( "Error Saving Translated Mission" );
					}
				}
			}
			else
			{
				if ( !Directory.Exists( appModel.TranslatedFilePath ) )
				{
					var di = Directory.CreateDirectory( appModel.TranslatedFilePath );
					if ( di == null )
					{
						MessageBox.Show( "Could not create the folder.\r\nTried to create: " + appModel.TranslatedFilePath, "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
						hasSaved = false;
						appModel.SetStatus( "Error Creating Directory" );
						return;
					}
				}

				if ( FileManager.SaveMission( translatedMission, appModel.TranslatedFilePath, appModel.TranslatedFileName ) )
				{
					hasSaved = true;
					Title = "Saga Translator [Mission] - " + Path.Combine( appModel.TranslatedFilePath, translatedMission.fileName );
					appModel.SetStatus( "Translated Mission Saved" );
				}
				else
				{
					hasSaved = false;
					appModel.SetStatus( "Error Saving Translated Mission" );
				}
			}
		}

		private void openMissionTranslatedButton_Click( object sender, RoutedEventArgs e )
		{
			OpenFileDialog od = new();
			if ( !hasSaved )
				od.InitialDirectory = Path.Combine( appPath, "SavedTranslations" );
			else
				od.InitialDirectory = appModel.TranslatedFilePath;

			if ( appModel.TranslateMode == TranslateMode.Mission )
			{
				od.Filter = "Mission File (*.json)|*.json";
				od.Title = "Open Translated Mission";
				if ( od.ShowDialog() == true )
					OpenTranslatedFile( od.FileName );
			}
			else if ( appModel.TranslateMode == TranslateMode.UI )
			{
				od.Filter = "UI File (*.json)|*.json";
				od.Title = "Open Translated UI";
				if ( od.ShowDialog() == true )
					OpenTranslatedFile( od.FileName );
			}
			else if ( appModel.TranslateMode == TranslateMode.Supplemental )
			{
				od.Filter = $"{appModel.TranslateMode} File (*.json, *.txt)|*.json;*.txt";
				od.Title = "Open Translated UI";
				if ( od.ShowDialog() == true )
					OpenTranslatedFile( od.FileName );
			}
		}

		void GetPanels( TreeViewItem item, List<IFindReplace> panels )
		{
			if ( item.DataContext is IFindReplace )
			{
				panels.Add( (IFindReplace)item.DataContext );
			}

			foreach ( TreeViewItem child in item.Items )
			{
				GetPanels( child, panels );
			}
		}

		private void helpButton_Click( object sender, RoutedEventArgs e )
		{
			translationObject = new WelcomePanel();
		}

		private void modeToggle_Click( object sender, RoutedEventArgs e )
		{
			var dlg = new ModeSwitchDialog();
			dlg.ShowDialog();
			if ( dlg.sourceData.fileMode != FileMode.Cancel )
			{
				mainTree.Items.Clear();
				//appModel.TranslateMode = dlg.appMode;
				if ( dlg.sourceData.fileMode == FileMode.Mission )
					appModel.TranslateMode = TranslateMode.Mission;
				else if ( dlg.sourceData.fileMode == FileMode.UI )
					appModel.TranslateMode = TranslateMode.UI;
				else
					appModel.TranslateMode = TranslateMode.Supplemental;
				MainWindow mainWindow = new( dlg.sourceData, appModel.TranslateMode );
				mainWindow.Show();
				Close();
			}
		}

		private async void StartVersionCheck()
		{
			//first check if internet is available
			var ping = new Ping();
			var reply = ping.Send( new IPAddress( new byte[] { 8, 8, 8, 8 } ), 5000 );
			if ( reply.Status == IPStatus.Success )
			{
				//internet available, check for latest version
				await CheckVersion();
			}
			else
			{
				gitHubResponse = null;
			}
		}

		private async Task CheckVersion()
		{
			// /repos/{owner}/{repo}/releases
			var web = new HttpClient();
			web.DefaultRequestHeaders.Add( "User-Agent", "request" );
			var result = await web.GetAsync( "https://api.github.com/repos/GlowPuff/SagaTranslator/releases/latest" );

			if ( !result.IsSuccessStatusCode )
			{
				//Debug.Log( "network error" );
				gitHubResponse = null;
			}
			else
			{
				var response = await result.Content.ReadAsStringAsync();
				//parse JSON response
				gitHubResponse = JsonConvert.DeserializeObject<GitHubResponse>( response );

				if ( gitHubResponse.tag_name.Substring( 2 ) == appModel.AppVersion )//remove beginning "v."
				{
					Dispatcher.Invoke( () => updateCheck.Text = "Latest Version" );
				}
				else
				{
					Dispatcher.Invoke( () => updateCheck.Text = "Update Available: " + gitHubResponse.tag_name );
				}
			}
		}

		/// <summary>
		/// DEPRECATED
		/// </summary>
		//public bool OpenSourceFile( string filename )
		//{
		//	if ( appModel.TranslateMode == TranslateMode.Mission )
		//	{
		//		Regex regex = new Regex( @"(bespin|core|empire|hoth|jabba|lothal|other|twin)\d{1,2}(info|rules).txt", RegexOptions.IgnoreCase );
		//		Regex regex2 = new( @"(bespin|core|empire|hoth|jabba|lothal|other|twin)(info).txt" );
		//		List<string> taboo = new( new string[] { "ui.json", "instructions.json", "bonuseffects.json", "allies.json", "enemies.json", "heroes.json", "villains.json", "bespin.json", "core.json", "empire.json", "hoth.json", "jabba.json", "lothal.json", "other.json", "twin.json", "items.json", "rewards.json", "skills.json", "events.json" } );

		//		string fname = new FileInfo( filename ).Name.ToLower();
		//		var mission = FileManager.LoadMission( filename );
		//		if ( mission != null && !taboo.Contains( fname ) && !regex.IsMatch( fname ) && !regex2.IsMatch( fname ) )
		//		{
		//			MainWindow mainWindow = new( mission, filename );
		//			mainWindow.Show();
		//			Close();
		//		}
		//		else
		//		{
		//			MessageBox.Show( "Loaded object was null or incorrect JSON.  You may be trying to open the file in the wrong translation Mode. Switch the Mode via the icon at the top left of the toolbar, then try again.", "Error Loading Source Mission" );
		//			appModel.SetStatus( "Error Loading Mission" );
		//			return false;
		//		}
		//	}
		//	else if ( appModel.TranslateMode == TranslateMode.Supplemental )
		//	{
		//		ParsedObject data = DynamicParser.Parse( filename );
		//		if ( data.isSuccess )
		//		{
		//			MainWindow mainWindow = new( filename );
		//			mainWindow.Show();
		//			Close();
		//		}
		//		else
		//		{
		//			appModel.SetStatus( "Error Loading Source Data" );
		//			MessageBox.Show( data.errorMsg, "Error Loading Source Data" );
		//		}

		//		return false;
		//	}
		//	else if ( appModel.TranslateMode == TranslateMode.UI )
		//	{
		//		var ui = FileManager.LoadJSON<UILanguage>( filename );
		//		if ( ui != null && ui.sagaUISetup != null )
		//		{
		//			MainWindow mainWindow = new( ui, filename );
		//			mainWindow.Show();
		//			Close();
		//		}
		//		else
		//		{
		//			MessageBox.Show( "Loaded object was null or incorrect JSON.  You may be trying to open the file in the wrong translation Mode. Switch the Mode via the icon at the top left of the toolbar, then try again.", "Error Loading Source UI" );
		//			appModel.SetStatus( "Error Loading UI" );
		//			return false;
		//		}
		//	}
		//	return true;
		//}

		public bool OpenTranslatedFile( string filename )
		{
			//The SOURCE file is always loaded before opening an existing translation is even allowed

			string fname = new FileInfo( filename ).Name.ToLower();
			bool success = false;

			if ( appModel.TranslateMode == TranslateMode.Mission )
			{
				var loadedMission = FileManager.LoadMission( filename );
				//make sure a mission was loaded, and not anything else by checking one of the loaded models
				if ( loadedMission != null )
				{
					success = true;
					translatedMission = loadedMission;
					translatedMission.fileName = Path.GetFileName( filename );
					PopulateMainTree();
					Binding binding = new( "languageID" ) { Source = translatedMission };
					languageCB.SetBinding( ComboBox.SelectedValueProperty, binding );
				}
			}
			else if ( appModel.TranslateMode == TranslateMode.UI )
			{
				//make sure the opened filename matches the expected filename
				if ( fname != appModel.TranslatedFileName.ToLower() )
				{
					MessageBox.Show( $"Translator expected a file with filename '{appModel.TranslatedFileName}' but received a file with filename '{fname}'.", "Error Loading Translation" );
					return false;
				}

				translatedUI = FileManager.LoadJSON<UILanguage>( filename );
				//make sure a UI was loaded, and not anything else by checking one of the loaded models
				if ( translatedUI != null && translatedUI.sagaUISetup != null )
				{
					success = true;
					PopulateUIMainTree();
					Binding binding = new( "languageID" ) { Source = translatedUI };
					languageCB.SetBinding( ComboBox.SelectedValueProperty, binding );
				}
			}
			else if ( appModel.TranslateMode == TranslateMode.Supplemental )
			{
				//make sure the opened filename matches the expected filename
				if ( fname != appModel.TranslatedFileName.ToLower() )
				{
					MessageBox.Show( $"Translator expected a file with filename '{appModel.TranslatedFileName}' but received a file with filename '{fname}'.", "Error Loading Translation" );
					return false;
				}

				var data = DynamicParser.Parse( filename );
				if ( data.isSuccess )
				{
					success = true;
					translatedDynamicUIModel = data.data;
					PopulateDynamicTree( data.gType );
				}
			}

			if ( success )
			{
				appModel.TranslatedFilePath = Path.GetDirectoryName( filename );
				appModel.TranslatedFileName = Path.GetFileName( filename );
				appModel.SetStatus( "Loaded Translated Data" );
				hasSaved = true;
				translationObject = new WelcomePanel();
				((WelcomePanel)translationObject).EnableTranslationDrop();
				Title = $"Saga Translator [{appModel.TranslateMode}] - " + Path.Combine( appModel.TranslatedFilePath, appModel.TranslatedFileName );

				return true;
			}
			else
			{
				appModel.SetStatus( "Error Loading Translated Data" );
				MessageBox.Show( "Loaded object was null or incorrect JSON.  You may be trying to open the file in the wrong translation Source Mode. Switch the Source Mode via the icon at the top left of the toolbar, then try again.", "Error Loading Translated Data" );
				return false;
			}
		}
	}
}
