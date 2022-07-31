using System;
using System.IO;
using System.Timers;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Saga_Translator.Models
{
	public class AppModel : ObservableObject
	{
		private MainWindow mainWindow;
		private Timer infoTimer;
		private string appVersion, formatVersion, infoText, translatedFilePath;
		private bool nothingSelected;
		private SolidColorBrush statusColor;

		public string AppVersion { get => appVersion; set => SetProperty( ref appVersion, value ); }
		public string FormatVersion { get => formatVersion; set => SetProperty( ref formatVersion, value ); }
		public string InfoText { get => infoText; set => SetProperty( ref infoText, value ); }
		/// <summary>
		/// Folder path ONLY, EXCLUDING filename
		/// </summary>
		public string TranslatedFilePath { get => translatedFilePath; set => SetProperty( ref translatedFilePath, value ); }
		public bool NothingSelected { get => nothingSelected; set => SetProperty( ref nothingSelected, value ); }
		public SolidColorBrush StatusColor { get => statusColor; set => SetProperty( ref statusColor, value ); }

		public AppModel( MainWindow main )
		{
			mainWindow = main;
			AppVersion = "1.0";
			FormatVersion = "18";
			InfoText = "";
			NothingSelected = true;
			TranslatedFilePath = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "ImperialCommander" );

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
			SetStatus( "Welcome!" );
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
