﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;
using Saga_Translator;

namespace Imperial_Commander_Editor
{
	public class Console : INotifyPropertyChanged, IMapEntity, IFindReplace
	{
		string _name;
		Guid _mapSectionOwner;

		//common props
		public Guid GUID { get; set; }
		public string name
		{
			get { return _name; }
			set
			{
				_name = string.IsNullOrEmpty( value ) ? "New Console" : value;
				PC();
			}
		}
		public EntityType entityType { get; set; }
		public Vector entityPosition { get; set; }
		public double entityRotation { get; set; }
		[JsonIgnore]
		public EntityRenderer mapRenderer { get; set; }
		public EntityProperties entityProperties { get; set; }
		public Guid mapSectionOwner { get { return _mapSectionOwner; } set { _mapSectionOwner = value; PC(); } }
		public bool hasProperties { get { return true; } }
		public bool hasColor { get { return true; } }

		//console props
		public string deploymentColor
		{
			get { return entityProperties.entityColor; }
			set
			{
				entityProperties.entityColor = value;
				PC();
				if ( mapRenderer != null )
				{
					Color c = Utils.ColorFromName( entityProperties.entityColor ).color;
					mapRenderer.entityShape.Fill = new SolidColorBrush( c );
					mapRenderer.unselectedBGColor = new SolidColorBrush( c );
					mapRenderer.selectedBGColor = new SolidColorBrush( c );
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		public Console()
		{
		}

		public Console( Guid ownderGUID )
		{
			GUID = Guid.NewGuid();
			name = "New Terminal";
			entityType = EntityType.Console;
			entityProperties = new() { name = name };
			mapSectionOwner = ownderGUID;

			deploymentColor = "Gray";
		}

		public IMapEntity Duplicate()
		{
			var dupe = new Console();
			dupe.GUID = Guid.NewGuid();
			dupe.name = name + " (Duplicate)";
			dupe.entityType = entityType;
			dupe.entityProperties = new();
			dupe.entityProperties.CopyFrom( this );
			dupe.entityProperties.name = dupe.name;
			dupe.entityPosition = entityPosition;
			dupe.entityRotation = entityRotation;
			dupe.mapSectionOwner = mapSectionOwner;
			dupe.deploymentColor = deploymentColor;
			return dupe;
		}

		public void BuildRenderer( Canvas canvas, Vector where, double scale )
		{
			Color c = Utils.ColorFromName( entityProperties.entityColor ).color;

			mapRenderer = new( this, where, scale, new( 1, 1 ) )
			{
				selectedImageZ = 305,
				selectedZ = 300,
				unselectedBGColor = new( c ),
				selectedBGColor = new( c )
			};
			mapRenderer.BuildShape( TokenShape.Square );
			mapRenderer.BuildImage( "pack://application:,,,/Imperial Commander Editor;component/Assets/Tiles/console.png" );
			canvas.Children.Add( mapRenderer.entityImage );
			canvas.Children.Add( mapRenderer.entityShape );
		}

		public bool Validate()
		{
			//if ( !Utils.mainWindow.mission.EntityExists( GUID ) )
			//{
			//	if ( GUID != Guid.Empty )
			//		name += " (NO LONGER VALID)";
			//	GUID = Guid.Empty;
			//	return false;
			//}
			return true;
		}

		public void Dim( Guid guid )
		{
			mapRenderer.Dim( mapSectionOwner != guid );
		}

		public int FindReplace( string needle, string replace )
		{
			int count = 0;
			if ( !string.IsNullOrEmpty( entityProperties.theText ) )
			{
				var regex = new Regex( needle );
				count += regex.Matches( entityProperties.theText ).Count;
				foreach ( var match in regex.Matches( entityProperties.theText ) )
				{
					entityProperties.theText = entityProperties.theText.Replace( match.ToString(), replace );
				}
			}
			return count;
		}
	}
}
