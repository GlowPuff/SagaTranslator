﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;

namespace Imperial_Commander_Editor
{
	public class DeploymentPoint : INotifyPropertyChanged, IMapEntity
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
				_name = string.IsNullOrEmpty( value ) ? "New Deployment Point" : value;
				PC();
			}
		}
		public EntityType entityType { get; set; }
		public Vector entityPosition { get; set; }
		public double entityRotation { get; set; }
		[JsonIgnore]
		public EntityRenderer mapRenderer { get; set; }
		public EntityProperties entityProperties { get; set; }
		public bool hasProperties { get { return false; } }
		public bool hasColor { get { return true; } }

		//dp props
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
		public Guid mapSectionOwner { get { return _mapSectionOwner; } set { _mapSectionOwner = value; PC(); } }
		public DeploymentPointProps deploymentPointProps { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;
		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		public DeploymentPoint()
		{
		}

		public DeploymentPoint( Guid ownerGUID )
		{
			GUID = Guid.NewGuid();
			name = "New Deployment Point";
			entityType = EntityType.DeploymentPoint;
			entityProperties = new() { name = name };
			mapSectionOwner = ownerGUID;
			deploymentPointProps = new();

			entityProperties.isActive = false;

			deploymentColor = "Gray";
		}

		public IMapEntity Duplicate()
		{
			var dupe = new DeploymentPoint();
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
			dupe.deploymentPointProps = new();
			dupe.deploymentPointProps.CopyFrom( this.deploymentPointProps );

			return dupe;
		}

		public void BuildRenderer( Canvas canvas, Vector where, double scale )
		{
			Color c = Utils.ColorFromName( entityProperties.entityColor ).color;

			mapRenderer = new( this, where, scale, new( 1, 1 ) )
			{
				selectedZ = 300,
				selectedImageZ = 305,
				selectedBGColor = new( c ),
				unselectedBGColor = new( c ),
				unselectedStrokeColor = new( Colors.Red )
			};
			mapRenderer.BuildShape( TokenShape.Circle );
			mapRenderer.BuildImage( "pack://application:,,,/Imperial Commander Editor;component/Assets/Tiles/dp.png" );
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
	}
}
