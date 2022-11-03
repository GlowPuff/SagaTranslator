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
			Title = tmode == TranslateMode.Mission ? "Saga Translator [Mission]" : "Saga Translator [UI]";
			explorerTitle.Text = tmode == TranslateMode.Mission ? "Mission Explorer" : "UI Explorer";
		}

		public MainWindow( Mission s, string filePath )
		{
			InitializeComponent();
			DataContext = this;
			Utils.Init( this );

			appModel = new AppModel( this, TranslateMode.Mission );
			appModel.TranslatedFilePath = Path.GetDirectoryName( filePath );

			translationObject = new WelcomePanel();
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
			findReplace.IsEnabled = true;
			openMissionTranslatedButton.IsEnabled = true;
			canSave = true;

			PopulateMainTree();

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
			findReplace.IsEnabled = false;
			openMissionTranslatedButton.IsEnabled = true;
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

		private void Window_PreviewKeyDown( object sender, System.Windows.Input.KeyEventArgs e )
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
					var filePath = od.FileName;
					var project = FileManager.LoadMission( filePath );
					if ( project != null && project.missionProperties != null )
					{
						MainWindow mainWindow = new( project, filePath );
						mainWindow.Show();
						Close();
					}
					else
					{
						MessageBox.Show( "Loaded object was null or incorrect JSON.", "Error Loading Source Mission" );
						appModel.SetStatus( "Error Loading Mission" );
					}
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
					var filePath = od.FileName;
					var ui = FileManager.LoadUI( filePath );
					if ( ui != null && ui.sagaUISetup != null )
					{
						MainWindow mainWindow = new( ui, filePath );
						mainWindow.Show();
						Close();
					}
					else
					{
						MessageBox.Show( "Loaded object was null or incorrect JSON.", "Error Loading Source UI" );
						appModel.SetStatus( "Error Loading UI" );
					}
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
					var filePath = od.FileName;
					translatedMission = FileManager.LoadMission( filePath );
					//make sure a mission was loaded, and not anything else by checking one of the loaded models
					if ( translatedMission != null && translatedMission.missionProperties != null )
					{
						hasSaved = true;
						appModel.TranslatedFilePath = Path.GetDirectoryName( filePath );
						//translatedMission.fileName = od.SafeFileName;
						Title = "Saga Translator [Mission] - " + Path.Combine( appModel.TranslatedFilePath, translatedMission.fileName );
						appModel.SetStatus( "Loaded Translated Mission" );
						PopulateMainTree();
						Binding binding = new( "languageID" )
						{
							Source = translatedMission
						};
						languageCB.SetBinding( ComboBox.SelectedValueProperty, binding );
						translationObject = new WelcomePanel();
					}
					else
					{
						appModel.SetStatus( "Error Loading Translated Mission" );
						MessageBox.Show( "Loaded object was null or incorrect JSON.", "Error Loading Translated Mission" );
					}
				}
			}
			else
			{
				od.Filter = "UI File (*.json)|*.json";
				od.Title = "Open Translated UI";
				if ( od.ShowDialog() == true )
				{
					var filePath = od.FileName;
					translatedUI = FileManager.LoadUI( filePath );
					//make sure a UI was loaded, and not anything else by checking one of the loaded models
					if ( translatedUI != null && translatedUI.sagaUISetup != null )
					{
						hasSaved = true;
						appModel.TranslatedFilePath = Path.GetDirectoryName( filePath );
						appModel.UiFileName = od.SafeFileName;
						Title = "Saga Translator [UI] - " + Path.Combine( appModel.TranslatedFilePath, od.SafeFileName );
						appModel.SetStatus( "Loaded Translated UI" );
						PopulateUIMainTree();
						Binding binding = new( "languageID" )
						{
							Source = translatedUI
						};
						languageCB.SetBinding( ComboBox.SelectedValueProperty, binding );
						translationObject = new WelcomePanel();
					}
					else
					{
						appModel.SetStatus( "Error Loading Translated UI" );
						MessageBox.Show( "Loaded object was null or incorrect JSON.", "Error Loading Translated UI" );
					}
				}
			}
		}

		private void helpButton_Click( object sender, RoutedEventArgs e )
		{
			translationObject = new WelcomePanel();
		}

		private void modeToggle_Click( object sender, RoutedEventArgs e )
		{
			var ret = MessageBox.Show( "SWITCHING MODES WILL CLEAR ALL DATA.\n\nAre you sure you want to switch modes?", "Switch Modes?", MessageBoxButton.YesNo, MessageBoxImage.Question );

			if ( ret == MessageBoxResult.Yes )
			{
				appModel.TranslateMode = appModel.TranslateMode == TranslateMode.Mission ? TranslateMode.UI : TranslateMode.Mission;
				MainWindow mainWindow = new( appModel.TranslateMode );
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
