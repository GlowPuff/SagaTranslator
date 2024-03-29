﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Imperial_Commander_Editor
{
	public class Mission : INotifyPropertyChanged
	{
		string _languageID;

		public string languageID { get { return _languageID; } set { _languageID = value; PC(); } }
		public MissionProperties missionProperties { get; set; }
		public Guid missionGUID;
		/// <summary>
		/// JUST the filename+extension
		/// </summary>
		public string fileName { get; set; }
		/// <summary>
		/// folder path+filename RELATIVE to SpecialFolder.MyDocuments
		/// </summary>
		//public string relativePath;

		/// <summary>
		///	increment this each time file format gets updated
		/// </summary>
		public string fileVersion = "1";

		public string saveDate;
		/// <summary>
		/// File save time so recent list can sort by recent first
		/// </summary>
		public long timeTicks;

		public event PropertyChangedEventHandler? PropertyChanged;

		public ObservableCollection<MapSection> mapSections { get; set; }
		public ObservableCollection<Trigger> globalTriggers { get; set; }
		public ObservableCollection<MissionEvent> globalEvents { get; set; }

		[JsonConverter( typeof( MapEntityConverter ) )]
		public ObservableCollection<IMapEntity> mapEntities { get; set; }
		public ObservableCollection<EnemyGroupData> initialDeploymentGroups { get; set; }
		public ObservableCollection<EnemyGroupData> reservedDeploymentGroups { get; set; }
		public ObservableCollection<EventGroup> eventGroups { get; set; }
		public ObservableCollection<EntityGroup> entityGroups { get; set; }
		public ObservableCollection<CustomToon> customCharacters { get; set; }

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		public Mission()
		{
			//defaults
			missionGUID = Guid.NewGuid();
			fileName = "";
			//relativePath = "";
			saveDate = DateTime.Now.ToString( "M/d/yyyy" );
			timeTicks = DateTime.Now.Ticks;
			missionProperties = new MissionProperties();
			languageID = "Select Target Language";

			mapSections = new();
			globalTriggers = new();
			globalEvents = new();
			mapEntities = new();
			initialDeploymentGroups = new();
			reservedDeploymentGroups = new();
			eventGroups = new();
			entityGroups = new();
			customCharacters = new();
		}

		public void InitDefaultData()
		{
			MapSection mapSection = new()
			{
				name = "Start Section",
				GUID = Guid.Parse( "11111111-1111-1111-1111-111111111111" ),//Guid.Empty,
				canRemove = false,
				isActive = true
			};
			mapSection.Init();

			mapSections.Add( mapSection );

			//default global NONE trigger
			globalTriggers.Add( new()
			{
				name = "None (Global)",
				isGlobal = true,
				GUID = Guid.Empty
			} );
			//default global NONE event
			globalEvents.Add( new() { name = "None (Global)", GUID = Guid.Empty } );

			//debug
			//mapSections.Add( new() { name = "blah" } );
		}

		public bool TriggerExists( Trigger t )
		{
			bool g = globalTriggers.Any( x => x.GUID == t.GUID );
			bool m = mapSections.Any( x => x.triggers.Any( xt => xt.GUID == t.GUID ) );
			return g || m;
		}

		public bool TriggerExists( Guid guid )
		{
			bool g = globalTriggers.Any( x => x.GUID == guid );
			bool m = mapSections.Any( x => x.triggers.Any( xt => xt.GUID == guid ) );
			return g || m;
		}

		public bool EventExists( Guid guid )
		{
			bool g = globalEvents.Any( x => x.GUID == guid );
			bool m = mapSections.Any( x => x.missionEvents.Any( xt => xt.GUID == guid ) );
			return g || m;
		}

		public Trigger GetTriggerFromGUID( Guid guid )
		{
			if ( globalTriggers.Any( x => x.GUID == guid ) )
				return globalTriggers.First( x => x.GUID == guid );
			else if ( mapSections.Any( x => x.triggers.Any( xt => xt.GUID == guid ) ) )
				return mapSections.First( x => x.triggers.Any( xt => xt.GUID == guid ) ).triggers.First( x => x.GUID == guid );
			else
				return null;
		}

		public MissionEvent GetEventFromGUID( Guid guid )
		{
			if ( globalEvents.Any( x => x.GUID == guid ) )
				return globalEvents.First( x => x.GUID == guid );
			else if ( mapSections.Any( x => x.missionEvents.Any( xt => xt.GUID == guid ) ) )
				return mapSections.First( x => x.missionEvents.Any( xt => xt.GUID == guid ) ).missionEvents.First( x => x.GUID == guid );
			else
				return null;
		}

		public bool EntityExists( Guid guid )
		{
			if ( guid == Guid.Empty )
				return true;
			return mapEntities.Any( x => x.GUID == guid );
		}

		public T GetEntityFromGUID<T>( Guid guid )
		{
			return (T)mapEntities.Where( x => x.GUID == guid ).FirstOr( null );
		}

		public T GetEAByGUID<T>( Guid eaGUID )
		{
			//var ev = GetAllEvents();
			//return (T)ev.Select( x => x.eventActions.First( ea => ea.GUID == eaGUID ) );
			//return (T)ev.First( x => x.eventActions.Any( ea => ea.GUID == eaGUID ) ).First();
			var x = from ev in GetAllEvents() from ea in ev.eventActions where ea.GUID == eaGUID select ea;
			return (T)x.First();
		}

		public List<MissionEvent> GetAllEvents()
		{
			var events = new List<MissionEvent>();
			events = events.Concat( globalEvents ).ToList();
			foreach ( var item in mapSections )
			{
				events.AddRange( item.missionEvents );
			}
			return events;
		}
	}
}
