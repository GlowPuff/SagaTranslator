using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Newtonsoft.Json;
using Saga_Translator;

namespace Imperial_Commander_Editor
{
	public class FileManager
	{
		public FileManager() { }

		/// <summary>
		/// saves a mission to base project folder
		/// </summary>
		public static bool SaveMission( Mission mission, string path, string filename )
		{
			string basePath = path;

			if ( !Directory.Exists( basePath ) )
			{
				var di = Directory.CreateDirectory( basePath );
				if ( di == null )
				{
					MessageBox.Show( "Could not create the folder.\r\nTried to create: " + basePath, "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
					return false;
				}
			}

			string outpath = Path.Combine( basePath, filename );

			//just use the filename, not the whole path
			FileInfo fi = new( outpath );
			mission.fileName = fi.Name;
			mission.saveDate = DateTime.Now.ToString( "M/d/yyyy" );
			mission.timeTicks = DateTime.Now.Ticks;
			mission.fileVersion = Utils.formatVersion;

			string output = JsonConvert.SerializeObject( mission, Formatting.Indented );
			Utils.Log( outpath );
			try
			{
				using ( var stream = File.CreateText( outpath ) )
				{
					stream.Write( output );
				}
			}
			catch ( Exception e )
			{
				MessageBox.Show( "Could not save the Mission file.\r\n\r\nException:\r\n" + e.Message, "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
				return false;
			}
			return true;
		}

		/// <summary>
		/// Loads a mission from its .json.
		/// Supply the FULL PATH with the filename
		/// </summary>
		public static Mission LoadMission( string filename )
		{
			string json = "";

			try
			{
				using ( StreamReader sr = new( filename ) )
				{
					json = sr.ReadToEnd();
				}

				//do a simple string search to make sure it's a Mission
				if ( !json.Contains( "missionGUID" ) )
					throw new( "Loaded file doesn't appear to be a Mission." );

				var m = JsonConvert.DeserializeObject<Mission>( json );
				//overwrite fileName, relativePath and fileVersion properties so they are up-to-date
				FileInfo fi = new FileInfo( filename );
				m.fileName = fi.Name;
				//m.relativePath = "";//Path.GetRelativePath( basePath, new DirectoryInfo( filename ).FullName );
				m.fileVersion = Utils.formatVersion;
				return m;
			}
			catch ( Exception e )
			{
				MessageBox.Show( "Could not load the Mission.\r\n\r\nException:\r\n" + e.Message, "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
				return null;
			}
		}

		public static bool SaveUI( UILanguage ui, string filename, string path )
		{
			string basePath = path;

			if ( !Directory.Exists( basePath ) )
			{
				var di = Directory.CreateDirectory( basePath );
				if ( di == null )
				{
					MessageBox.Show( "Could not create the folder.\r\nTried to create: " + basePath, "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
					return false;
				}
			}

			string outpath = Path.Combine( basePath, filename );

			string output = JsonConvert.SerializeObject( ui, Formatting.Indented );
			Utils.Log( outpath );
			try
			{
				using ( var stream = File.CreateText( outpath ) )
				{
					stream.Write( output );
				}
			}
			catch ( Exception e )
			{
				MessageBox.Show( "Could not save the UI file.\r\n\r\nException:\r\n" + e.Message, "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
				return false;
			}
			return true;
		}

		public static bool SaveOther( GenericUIData data, string filename, string path )
		{
			string basePath = path;

			if ( !Directory.Exists( basePath ) )
			{
				var di = Directory.CreateDirectory( basePath );
				if ( di == null )
				{
					MessageBox.Show( "Could not create the folder.\r\nTried to create: " + basePath, "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
					return false;
				}
			}

			string outpath = Path.Combine( basePath, filename );

			string output = JsonConvert.SerializeObject( data.data, Formatting.Indented );
			Utils.Log( outpath );
			try
			{
				using ( var stream = File.CreateText( outpath ) )
				{
					stream.Write( output );
				}
			}
			catch ( Exception e )
			{
				MessageBox.Show( "Could not save the Data file.\r\n\r\nException:\r\n" + e.Message, "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
				return false;
			}
			return true;
		}

		public static T LoadJSON<T>( string filename )
		{
			try
			{
				string json = "";
				using ( StreamReader sr = new( filename ) )
				{
					json = sr.ReadToEnd();
				}
				return JsonConvert.DeserializeObject<T>( json );
			}
			catch ( JsonReaderException e )
			{
				MessageBox.Show( "Error parsing JSON file.\r\n\r\nException:\r\n" + e.Message, "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
				return default( T );
			}
		}

		/// <summary>
		/// Returns stringified JSON of the specified asset, or empty if error
		/// </summary>
		public static string LoadBuiltinJSON( string assetName )
		{
			try
			{
				string json = "";
				var assembly = Assembly.GetExecutingAssembly();
				var resourceName = assembly.GetManifestResourceNames().Single( str => str.EndsWith( assetName ) );
				using ( Stream stream = assembly.GetManifestResourceStream( resourceName ) )
				using ( StreamReader reader = new StreamReader( stream ) )
				{
					json = reader.ReadToEnd();
				}
				return json;
			}
			catch ( JsonReaderException e )
			{
				MessageBox.Show( "LoadBuiltinJSON()\n\nError loading built-in JSON file.\n\nException:\n" + e.Message, "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
				return string.Empty;
			}
		}

		public static string[] FindAssetsWithName( string name )
		{
			try
			{
				var assembly = Assembly.GetExecutingAssembly();
				var resourceName = assembly.GetManifestResourceNames().Where( str => str.Contains( name ) );
				return resourceName.ToArray();
			}
			catch ( Exception e )
			{
				MessageBox.Show( $"FindAssetsWithName()\n\nError finding asset '{name}'.\n\nException:\n" + e.Message, "App Exception", MessageBoxButton.OK, MessageBoxImage.Error );
				return new string[0];
			}
		}
	}
}
