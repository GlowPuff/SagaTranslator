using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Imperial_Commander_Editor;
using Microsoft.Win32;
using Newtonsoft.Json;
using Saga_Translator.Models;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		//private AppModel _appModel;
		private ITranslationPanel _translationObject;
		private bool hasSaved = false, canSave = false;
		private Mission _translatedMission, _sourceMission;

		public ITranslationPanel translationObject { get { return _translationObject; } set { _translationObject = value; PC(); } }
		public AppModel appModel { get; set; }
		public Mission sourceMission { get => _sourceMission; set { _sourceMission = value; PC(); } }
		public Mission? translatedMission { get => _translatedMission; set { _translatedMission = value; PC(); } }

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		public MainWindow()
		{
			InitializeComponent();
			DataContext = this;

			appModel = new AppModel( this );

			translationObject = new WelcomePanel();
			appModel.NothingSelected = false;

			Utils.Init( this );
		}

		public MainWindow( Mission s, string filePath )
		{
			InitializeComponent();
			DataContext = this;
			Utils.Init( this );

			appModel = new AppModel( this );
			appModel.TranslatedFilePath = Path.GetDirectoryName( filePath );

			translationObject = new WelcomePanel();
			appModel.NothingSelected = false;
			//set the ENGLISH SOURCE
			sourceMission = s;
			//make a copy of the mission
			string json = JsonConvert.SerializeObject( sourceMission );
			//set the TRANSLATED MISSION
			translatedMission = JsonConvert.DeserializeObject<Mission>( json );
			if ( string.IsNullOrEmpty( translatedMission.languageID ) )
				translatedMission.languageID = "Select Target Language";

			sourceText.Visibility = Visibility.Visible;
			languageCB.IsEnabled = true;
			saveMissionButton.IsEnabled = true;
			findReplace.IsEnabled = true;
			openMissionTranslatedButton.IsEnabled = true;
			canSave = true;

			PopulateMainTree();

			appModel.SetStatus( "Mission Loaded" );

			Title = "Saga Translator - FILE NOT SAVED";
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
			OpenFileDialog od = new();
			od.InitialDirectory = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "ImperialCommander" );
			od.Filter = "Mission File (*.json)|*.json";
			od.Title = "Open SOURCE Mission (English)";
			if ( od.ShowDialog() == true )
			{
				var filePath = od.FileName;
				var project = FileManager.LoadMission( filePath );
				if ( project != null )
				{
					MainWindow mainWindow = new( project, filePath );
					mainWindow.Show();
					Close();
				}
			}
		}

		private void saveMissionButton_Click( object sender, RoutedEventArgs e )
		{
			if ( !hasSaved )
			{
				SaveFileDialog od = new SaveFileDialog();
				od.InitialDirectory = appModel.TranslatedFilePath;
				od.Filter = "Mission File (*.json)|*.json";
				od.Title = "Save Translated Mission";
				od.FileName = Path.Combine( appModel.TranslatedFilePath, translatedMission.fileName );
				if ( od.ShowDialog() == true )
				{
					appModel.TranslatedFilePath = Path.GetDirectoryName( od.FileName );
					if ( FileManager.Save( translatedMission, appModel.TranslatedFilePath ) )
					{
						hasSaved = true;
						Title = "Saga Translator - " + Path.Combine( appModel.TranslatedFilePath, translatedMission.fileName );
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
						MessageBox.Show( "Could not create the Mission project folder.\r\nTried to create: " + appModel.TranslatedFilePath, "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
						hasSaved = false;
						appModel.SetStatus( "Error Creating Directory" );
						return;
					}
				}

				if ( FileManager.Save( translatedMission, appModel.TranslatedFilePath ) )
				{
					hasSaved = true;
					Title = "Saga Translator - " + Path.Combine( appModel.TranslatedFilePath, translatedMission.fileName );
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
			od.Filter = "Mission File (*.json)|*.json";
			od.Title = "Open Translated Mission";
			if ( od.ShowDialog() == true )
			{
				var filePath = od.FileName;
				translatedMission = FileManager.LoadMission( filePath );
				if ( translatedMission != null )
				{
					hasSaved = true;
					appModel.TranslatedFilePath = Path.GetDirectoryName( filePath );
					Title = "Saga Translator - " + Path.Combine( appModel.TranslatedFilePath, translatedMission.fileName );
					appModel.SetStatus( "Loaded Translated Mission" );
					PopulateMainTree();
				}
				else
					appModel.SetStatus( "Error Loading Translated Mission" );
			}
		}

		private void helpButton_Click( object sender, RoutedEventArgs e )
		{
			translationObject = new WelcomePanel();
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
