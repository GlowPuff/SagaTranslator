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

		public GenericUIData sourceDynamicUIModel { get { return _sourceModel; } set { _sourceModel = value; PC(); } }
		public GenericUIData translatedDynamicUIModel { get { return _translatedModel; } set { _translatedModel = value; PC(); } }
		public ITranslationPanel translationObject { get { return _translationObject; } set { _translationObject = value; PC(); } }
		public AppModel appModel { get; set; }
		public Mission sourceMission { get => _sourceMission; set { _sourceMission = value; PC(); } }
		public Mission? translatedMission { get => _translatedMission; set { _translatedMission = value; PC(); } }
		public UILanguage sourceUI { get => _sourceUI; set { _sourceUI = value; PC(); } }
		public UILanguage? translatedUI { get => _translatedUI; set { _translatedUI = value; PC(); } }

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		public MainWindow() : this( TranslateMode.Mission ) { }

		public MainWindow( TranslateMode tmode )
		{
			InitializeComponent();
			DataContext = this;

			appModel = new AppModel( this, tmode );

			translationObject = new WelcomePanel();
			appModel.NothingSelected = false;

			if ( NetworkInterface.GetIsNetworkAvailable() )
				Task.Run( StartVersionCheck );
			else
				updateCheck.Text = "Error Checking For Update";

			Utils.Init( this );
			Title = $"Saga Translator [{tmode}]";
			explorerTitle.Text = $"{tmode} Explorer";
		}

		public MainWindow( string filePath )
		{
			InitializeComponent();
			DataContext = this;
			Utils.Init( this );

			appModel = new AppModel( this, TranslateMode.Other );
			appModel.TranslatedFilePath = Path.GetDirectoryName( filePath );

			appModel.NothingSelected = false;

			if ( NetworkInterface.GetIsNetworkAvailable() )
				Task.Run( StartVersionCheck );
			else
				updateCheck.Text = "Error Checking For Update";

			ParsedObject data = DynamicParser.Parse( filePath );
			//set the SOURCE data
			sourceDynamicUIModel = data.data;
			//set the TRANSLATED MISSION
			translatedDynamicUIModel = data.dataCopy;
			//PopulateDynamicTree( data.gType );
			//appModel.SetStatus( $"Loaded '{new FileInfo( filePath ).Name}'" );
			//Title = $"Saga Translator [{data.gType}] - FILE NOT SAVED";
			//saveMissionButton.IsEnabled = true;
			//openMissionTranslatedButton.IsEnabled = true;
			//sourceUIText.Text = new FileInfo( filePath ).Name;

			sourceText.Visibility = Visibility.Collapsed;
			sourceUIText.Visibility = Visibility.Visible;
			sourceUIText.Text = new FileInfo( filePath ).Name;
			//languageCB.IsEnabled = true;
			saveMissionButton.IsEnabled = true;
			openMissionTranslatedButton.IsEnabled = true;
			//findReplace.IsEnabled = true;
			canSave = true;

			PopulateDynamicTree( data.gType );
			translationObject = new WelcomePanel();
			((WelcomePanel)translationObject).EnableTranslationDrop();

			appModel.SetStatus( $"{data.gType} Loaded" );

			Title = "Saga Translator [Other] - FILE NOT SAVED";
			explorerTitle.Text = $"{data.gType} Explorer";
		}

		public MainWindow( Mission s, string filePath )
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
		}

		public MainWindow( UILanguage ui, string filePath )
		{
			InitializeComponent();
			DataContext = this;
			Utils.Init( this );

			appModel = new AppModel( this, TranslateMode.UI );
			appModel.TranslatedFilePath = Path.GetDirectoryName( filePath );

			translationObject = new WelcomePanel();
			((WelcomePanel)translationObject).EnableTranslationDrop();
			appModel.NothingSelected = false;

			if ( NetworkInterface.GetIsNetworkAvailable() )
				Task.Run( StartVersionCheck );
			else
				updateCheck.Text = "Error Checking For Update";

			//set the ENGLISH SOURCE
			sourceUI = ui;
			//make a copy of the ui
			string json = JsonConvert.SerializeObject( sourceUI );
			//set the TRANSLATED UI
			translatedUI = JsonConvert.DeserializeObject<UILanguage>( json );
			if ( string.IsNullOrEmpty( translatedUI.languageID ) )
				translatedUI.languageID = "Select Target Language";

			sourceText.Visibility = Visibility.Collapsed;
			sourceUIText.Visibility = Visibility.Visible;
			languageCB.IsEnabled = true;
			saveMissionButton.IsEnabled = true;
			openMissionTranslatedButton.IsEnabled = true;
			findReplace.IsEnabled = false;
			canSave = true;

			PopulateUIMainTree();

			appModel.SetStatus( "UI Loaded" );

			Title = "Saga Translator [UI] - FILE NOT SAVED";
			explorerTitle.Text = "UI Explorer";
			Binding binding = new( "languageID" )
			{
				Source = translatedUI
			};
			languageCB.SetBinding( ComboBox.SelectedValueProperty, binding );
		}

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

		private void openMissionButton_Click( object sender, RoutedEventArgs e )
		{
			if ( appModel.TranslateMode == TranslateMode.Mission )
			{
				OpenFileDialog od = new();
				od.InitialDirectory = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "ImperialCommander" );
				od.Filter = "Mission File (*.json)|*.json";
				od.Title = "Open SOURCE Mission (English)";
				if ( od.ShowDialog() == true )
				{
					OpenSourceFile( od.FileName );
				}
			}
			else
			{
				OpenFileDialog od = new();
				od.InitialDirectory = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "ImperialCommander" );
				od.Filter = "UI File (*.json)|*.json";
				od.Title = "Open SOURCE UI (English)";
				if ( od.ShowDialog() == true )
				{
					OpenSourceFile( od.FileName );
				}
			}
		}

		private void saveMissionButton_Click( object sender, RoutedEventArgs e )
		{
			if ( appModel.TranslateMode == TranslateMode.Mission )
				HandleSaveMission();
			else
				HandleSaveUI();
		}

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
					appModel.UiFileName = od.SafeFileName;
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

				if ( FileManager.SaveUI( translatedUI, appModel.UiFileName, appModel.TranslatedFilePath ) )
				{
					hasSaved = true;
					Title = "Saga Translator [UI] - " + Path.Combine( appModel.TranslatedFilePath, appModel.UiFileName );
					appModel.SetStatus( "Translated UI Saved" );
				}
				else
				{
					hasSaved = false;
					appModel.SetStatus( "Error Saving Translated UI" );
				}
			}
		}

		private void HandleSaveMission()
		{
			if ( !hasSaved )
			{
				SaveFileDialog od = new SaveFileDialog();
				od.AddExtension = true;
				od.InitialDirectory = appModel.TranslatedFilePath;
				od.Filter = "Mission File (*.json)|*.json";
				od.Title = "Save Translated Mission";
				od.FileName = Path.Combine( appModel.TranslatedFilePath, translatedMission.fileName );
				if ( od.ShowDialog() == true )
				{
					appModel.TranslatedFilePath = Path.GetDirectoryName( od.FileName );
					translatedMission.fileName = od.SafeFileName;
					if ( FileManager.SaveMission( translatedMission, appModel.TranslatedFilePath ) )
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

				if ( FileManager.SaveMission( translatedMission, appModel.TranslatedFilePath ) )
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
				od.InitialDirectory = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "ImperialCommander" );
			else
				od.InitialDirectory = appModel.TranslatedFilePath;

			if ( appModel.TranslateMode == TranslateMode.Mission )
			{
				od.Filter = "Mission File (*.json)|*.json";
				od.Title = "Open Translated Mission";
				if ( od.ShowDialog() == true )
				{
					OpenTranslatedFile( od.FileName );
				}
			}
			else
			{
				od.Filter = "UI File (*.json)|*.json";
				od.Title = "Open Translated UI";
				if ( od.ShowDialog() == true )
				{
					OpenTranslatedFile( od.FileName );
				}
			}
		}

		private void findReplace_Click( object sender, RoutedEventArgs e )
		{
			List<IFindReplace> panels = new();
			foreach ( var item in mainTree.Items )
			{
				GetPanels( (TreeViewItem)item, panels );
			}
			var dlg = new FindReplace( panels );
			dlg.ShowDialog();
		}

		void GetPanels( TreeViewItem item, List<IFindReplace> panels )
		{
			//if ( item.DataContext is MissionEvent
			//	|| item.DataContext is IEventAction
			//	|| item.DataContext is IMapEntity
			//	|| item.DataContext is MissionProperties )
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
			if ( dlg.appMode != TranslateMode.Cancel )
			{
				mainTree.Items.Clear();
				appModel.TranslateMode = dlg.appMode;
				MainWindow mainWindow = new( appModel.TranslateMode );
				mainWindow.Show();
				Close();
			}
			//var ret = MessageBox.Show( "SWITCHING MODES WILL CLEAR ALL DATA.\n\nAre you sure you want to switch modes?", "Switch Modes?", MessageBoxButton.YesNo, MessageBoxImage.Question );

			//if ( ret == MessageBoxResult.Yes )
			//{
			//	mainTree.Items.Clear();
			//	appModel.TranslateMode = appModel.TranslateMode == TranslateMode.Mission ? TranslateMode.UI : TranslateMode.Mission;
			//	MainWindow mainWindow = new( appModel.TranslateMode );
			//	mainWindow.Show();
			//	Close();
			//}
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

		public bool OpenSourceFile( string filename )
		{
			if ( appModel.TranslateMode == TranslateMode.Mission )
			{
				var mission = FileManager.LoadMission( filename );
				if ( mission != null && mission.fileName != "ui.json" )
				{
					MainWindow mainWindow = new( mission, filename );
					mainWindow.Show();
					Close();
				}
				else
				{
					MessageBox.Show( "Loaded object was null or incorrect JSON.", "Error Loading Source Mission" );
					appModel.SetStatus( "Error Loading Mission" );
					return false;
				}
			}
			else if ( appModel.TranslateMode == TranslateMode.Other )
			{
				ParsedObject data = DynamicParser.Parse( filename );
				if ( data.isSuccess )
				{
					MainWindow mainWindow = new( filename );
					mainWindow.Show();
					Close();
				}
				else
				{
					appModel.SetStatus( "Error Loading Source Data" );
					MessageBox.Show( data.errorMsg, "Error Loading Source Data" );
				}

				return false;
			}
			else if ( appModel.TranslateMode == TranslateMode.UI )
			{
				var ui = FileManager.LoadUI<UILanguage>( filename );
				if ( ui != null && ui.sagaUISetup != null )
				{
					MainWindow mainWindow = new( ui, filename );
					mainWindow.Show();
					Close();
				}
				else
				{
					MessageBox.Show( "Loaded object was null or incorrect JSON.", "Error Loading Source UI" );
					appModel.SetStatus( "Error Loading UI" );
					return false;
				}
			}
			return true;
		}

		public bool OpenTranslatedFile( string filename )
		{
			if ( appModel.TranslateMode == TranslateMode.Mission )
			{
				translatedMission = FileManager.LoadMission( filename );
				//make sure a mission was loaded, and not anything else by checking one of the loaded models
				if ( translatedMission != null && translatedMission.fileName != "ui.json" )
				{
					hasSaved = true;
					appModel.TranslatedFilePath = Path.GetDirectoryName( filename );
					translatedMission.fileName = Path.GetFileName( filename );
					Title = "Saga Translator [Mission] - " + Path.Combine( appModel.TranslatedFilePath, translatedMission.fileName );
					appModel.SetStatus( "Loaded Translated Mission" );
					PopulateMainTree();
					Binding binding = new( "languageID" )
					{
						Source = translatedMission
					};
					languageCB.SetBinding( ComboBox.SelectedValueProperty, binding );
					translationObject = new WelcomePanel();
					((WelcomePanel)translationObject).EnableTranslationDrop();
					return true;
				}
				else
				{
					appModel.SetStatus( "Error Loading Translated Mission" );
					MessageBox.Show( "Loaded object was null or incorrect JSON.", "Error Loading Translated Mission" );
					return false;
				}
			}
			else if ( appModel.TranslateMode == TranslateMode.UI )
			{
				translatedUI = FileManager.LoadUI<UILanguage>( filename );
				//make sure a UI was loaded, and not anything else by checking one of the loaded models
				if ( translatedUI != null && translatedUI.sagaUISetup != null )
				{
					hasSaved = true;
					appModel.TranslatedFilePath = Path.GetDirectoryName( filename );
					appModel.UiFileName = Path.GetFileName( filename );
					Title = "Saga Translator [UI] - " + Path.Combine( appModel.TranslatedFilePath, Path.GetFileName( filename ) );
					appModel.SetStatus( "Loaded Translated UI" );
					PopulateUIMainTree();
					Binding binding = new( "languageID" )
					{
						Source = translatedUI
					};
					languageCB.SetBinding( ComboBox.SelectedValueProperty, binding );
					translationObject = new WelcomePanel();
					((WelcomePanel)translationObject).EnableTranslationDrop();
					return true;
				}
				else
				{
					appModel.SetStatus( "Error Loading Translated UI" );
					MessageBox.Show( "Loaded object was null or incorrect JSON.", "Error Loading Translated UI" );
					return false;
				}
			}
			else if ( appModel.TranslateMode == TranslateMode.Other )
			{
				var data = DynamicParser.Parse( filename );
				if ( data.isSuccess )
				{
					translatedDynamicUIModel = data.data;
					hasSaved = true;
					appModel.TranslatedFilePath = Path.GetDirectoryName( filename );
					appModel.OtherFileName = Path.GetFileName( filename );
					Title = "Saga Translator [Other] - " + Path.Combine( appModel.TranslatedFilePath, Path.GetFileName( filename ) );
					appModel.SetStatus( "Loaded Translated File" );
					PopulateDynamicTree( data.gType );
					translationObject = new WelcomePanel();
					((WelcomePanel)translationObject).EnableTranslationDrop();
					return true;
				}
				return false;
			}

			return false;
		}

		//private void ToolBar_Loaded( object sender, RoutedEventArgs e )
		//{
		//	ToolBar toolBar = sender as ToolBar;
		//	var overflowGrid = toolBar.Template.FindName( "OverflowGrid", toolBar ) as FrameworkElement;
		//	if ( overflowGrid != null )
		//	{
		//		overflowGrid.Visibility = Visibility.Collapsed;
		//	}
		//	var mainPanelBorder = toolBar.Template.FindName( "MainPanelBorder", toolBar ) as FrameworkElement;
		//	if ( mainPanelBorder != null )
		//	{
		//		mainPanelBorder.Margin = new Thickness();
		//	}
		//}
	}
}
