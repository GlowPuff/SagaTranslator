using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Timers;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Imperial_Commander_Editor;
using Newtonsoft.Json;

namespace Saga_Translator
{
	public class AppModel : ObservableObject
	{
		private MainWindow mainWindow;
		private Timer infoTimer;
		private string appVersion, formatVersion, infoText, translatedFilePath, translatedFileName;
		private bool nothingSelected;
		private SolidColorBrush statusColor;
		private TranslateMode translateMode;
		private FileMode fileMode;

		public string AppVersion { get => appVersion; set => SetProperty( ref appVersion, value ); }
		public string FormatVersion { get => formatVersion; set => SetProperty( ref formatVersion, value ); }
		public string InfoText { get => infoText; set => SetProperty( ref infoText, value ); }
		/// <summary>
		/// Folder path ONLY, EXCLUDING filename
		/// </summary>
		public string TranslatedFilePath { get => translatedFilePath; set => SetProperty( ref translatedFilePath, value ); }
		public string TranslatedFileName { get => translatedFileName; set => SetProperty( ref translatedFileName, value ); }
		public bool NothingSelected { get => nothingSelected; set => SetProperty( ref nothingSelected, value ); }
		public SolidColorBrush StatusColor { get => statusColor; set => SetProperty( ref statusColor, value ); }
		public TranslateMode TranslateMode { get => translateMode; set => SetProperty( ref translateMode, value ); }
		public FileMode FileMode { get => fileMode; set => SetProperty( ref fileMode, value ); }
		public static List<DeploymentCard> enemiesList = new();
		public static List<DeploymentCard> villainsList = new();

		public AppModel( MainWindow main, TranslateMode tmode, FileMode fmode )
		{
			mainWindow = main;
			AppVersion = Utils.appVersion;
			FormatVersion = Utils.formatVersion;
			TranslateMode = tmode;
			FileMode = fmode;
			InfoText = "";
			NothingSelected = true;
			TranslatedFilePath = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "SavedTranslations" );

			infoTimer = new Timer( 3000 );
			infoTimer.AutoReset = false;
			infoTimer.Elapsed += ( s, e ) =>
			{
				mainWindow.Dispatcher.Invoke( () =>
				{
					InfoText = "";
					StatusColor = new SolidColorBrush( Color.FromRgb( 69, 69, 69 ) );
				} );
			};

			var assembly = Assembly.GetExecutingAssembly();
			var enemyRsc = "Saga_Translator.Assets.enemies.json";
			var villainRsc = "Saga_Translator.Assets.villains.json";

			using ( Stream stream = assembly.GetManifestResourceStream( enemyRsc ) )
			using ( StreamReader reader = new StreamReader( stream ) )
			{
				string json = reader.ReadToEnd();
				enemiesList = JsonConvert.DeserializeObject<List<DeploymentCard>>( json );
			}
			using ( Stream stream = assembly.GetManifestResourceStream( villainRsc ) )
			using ( StreamReader reader = new StreamReader( stream ) )
			{
				string json = reader.ReadToEnd();
				villainsList = JsonConvert.DeserializeObject<List<DeploymentCard>>( json );
			}

			SetStatus( $"Mode: {tmode}" );
		}

		public void SetStatus( string s )
		{
			infoTimer.Stop();
			InfoText = s;
			StatusColor = new SolidColorBrush( Color.FromRgb( 39, 39, 39 ) );
			infoTimer.Start();
		}
	}
}
