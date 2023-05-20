using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json;
using Saga_Translator;

namespace Imperial_Commander_Editor
{
	public static class Utils
	{
		public const string formatVersion = "21";
		public const string appVersion = "2.7";

		public static List<DeploymentCard> allyData;
		public static List<DeploymentCard> enemyData;
		public static List<DeploymentCard> villainData;
		public static List<DeploymentCard> heroData;
		public static List<TileDescriptor> tileData;

		public static ObservableCollection<DeploymentColor> deploymentColors;

		public static MainWindow mainWindow;

		public static Guid GUIDOne { get { return Guid.Parse( "11111111-1111-1111-1111-111111111111" ); } }

		public static void InitColors()
		{
			deploymentColors = new ObservableCollection<DeploymentColor>
			{
				new( "Gray", ColorFromFloats( .3301887f, .3301887f, .3301887f ) ),
				new( "Purple", ColorFromFloats( .6784314f, 0f, 1f ) ),
				new( "Black", ColorFromFloats( 0, 0, 0 ) ),
				new( "Blue", ColorFromFloats( 0, 0.3294118f, 1 ) ),
				new( "Green", ColorFromFloats( 0, 0.735849f, 0.1056484f ) ),
				new( "Red", ColorFromFloats( 1, 0, 0 ) ),
				new( "Yellow", ColorFromFloats( 1, 202f / 255f, 40f / 255f ) )
			};
		}

		public static void Init( MainWindow mw )
		{
			mainWindow = mw;
			InitColors();
		}

		public static void LoadAllCardData()
		{
			LoadCardData();
			tileData = TileDescriptor.LoadData();
		}

		public static DeploymentColor ColorFromName( string n )
		{
			if ( string.IsNullOrEmpty( n ) )
				return deploymentColors[0];
			return deploymentColors.Where( x => x.name == n ).First();
		}

		public static Color ColorFromFloats( float r, float g, float b )
		{
			return Color.FromRgb(
				(byte)(r * 255f),
				(byte)(g * 255f),
				(byte)(b * 255f) );
		}

		public static void LoseFocus( Control element )
		{
			FrameworkElement parent = (FrameworkElement)element.Parent;
			while ( parent != null && parent is IInputElement && !((IInputElement)parent).Focusable )
			{
				parent = (FrameworkElement)parent.Parent;
			}

			DependencyObject scope = FocusManager.GetFocusScope( element );
			FocusManager.SetFocusedElement( scope, parent );
		}

		public static void Log( string s )
		{
			Debug.WriteLine( s );
		}

		public static void LoadCardData()
		{
			allyData = DeploymentCard.LoadData( "allies.json" );
			enemyData = DeploymentCard.LoadData( "enemies.json" );
			villainData = DeploymentCard.LoadData( "villains.json" );
			heroData = DeploymentCard.LoadData( "heroes.json" );

			enemyData = enemyData.Concat( villainData ).ToList();
		}

		public static bool ValidateTrigger( Guid guid )
		{
			return true;
			//return mainWindow.mission.TriggerExists( guid );
		}

		public static bool ValidateEvent( Guid guid )
		{
			return true;
			//return mainWindow.mission.EventExists( guid );
		}

		public static bool ValidateMapEntity( Guid guid )
		{
			return true;
			//return mainWindow.mission.EntityExists( guid ) || guid == Utils.GUIDOne;
		}

		public static T UniqueCopy<T>( T copyFrom ) where T : class
		{
			string copy = JsonConvert.SerializeObject( copyFrom );
			return JsonConvert.DeserializeObject<T>( copy );
		}

		/// <summary>
		/// If a property value string is null/empty, set it to the English value
		/// </summary>
		public static void ValidateProperties<T>( T obj, string propName )
		{
			var props = typeof( T ).GetProperties();

			foreach ( var prop in props )
			{
				if ( string.IsNullOrEmpty( (string)prop.GetValue( obj ) ) )
				{
					var sourceObject = typeof( UILanguage ).GetProperty( propName ).GetValue( Utils.mainWindow.sourceUI );
					var sourceValue = props.Where( x => x.Name == prop.Name ).FirstOrDefault().GetValue( sourceObject );

					typeof( T ).GetProperty( prop.Name ).SetValue( obj, sourceValue );
				}
			}
		}

		///EXTENSIONS
		public static ObservableCollection<IMapEntity> Sort<T>( this ObservableCollection<IMapEntity> collection )
		{
			ObservableCollection<IMapEntity> temp;
			temp = new ObservableCollection<IMapEntity>( collection.OrderBy( p => p.name ) );
			collection.Clear();
			foreach ( IMapEntity j in temp )
				collection.Add( j );
			return collection;
		}
	}
}
