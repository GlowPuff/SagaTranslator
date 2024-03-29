﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace Imperial_Commander_Editor
{
	public class ParsedObject
	{
		public bool isSuccess;
		public GenericUIData data;
		public GenericUIData dataCopy;
		public GenericType gType;
		public Type dataType;
		public string errorMsg;
	}

	public class GenericUIData
	{
		public dynamic data;
	}

	public class DynamicContext
	{
		public int arrayIndex;
		public GenericType gtype;
	}

	public class ProjectItem : IComparable<ProjectItem>
	{
		public string Title { get; set; }
		public string Date { get; set; }
		public string Description { get; set; }
		public string fileName { get; set; }
		//public string relativePath { get; set; }
		public string fileVersion { get; set; }
		public long timeTicks { get; set; }

		public int CompareTo( ProjectItem other ) => timeTicks > other.timeTicks ? -1 : timeTicks < other.timeTicks ? 1 : 0;
	}

	public class DeploymentColor
	{
		public string name { get; set; }
		public Color color { get; set; }

		public DeploymentColor( string n, Color c )
		{
			name = n;
			color = c;
		}
	}

	public class EntityModifier
	{
		public Guid sourceGUID { get; set; }
		public bool hasColor { get; set; }
		public bool hasProperties { get; set; }
		public EntityProperties entityProperties { get; set; }

	}

	public class ButtonAction
	{
		public string buttonText { get; set; }
		public Guid triggerGUID { get; set; }
		public Guid eventGUID { get; set; }
		//public string triggerName { get; set; }
	}

	public class DPData
	{
		public Guid GUID { get; set; }
	}

	public class GitHubResponse
	{
		public string tag_name;
		public string body;
	}

	public class EnemyGroupData : INotifyPropertyChanged
	{
		CustomInstructionType _customInstructionType;
		string _customText, _cardName, _cardID;
		Guid _defeatedTrigger, _defeatedEvent;
		bool _useGenericMugshot, _useInitialGroupCustomName;

		public Guid GUID { get; set; }
		public string cardName { get { return _cardName; } set { _cardName = value; PC(); } }
		public string cardID { get { return _cardID; } set { _cardID = value; PC(); } }
		public CustomInstructionType customInstructionType { get { return _customInstructionType; } set { _customInstructionType = value; PC(); } }
		public string customText { get { return _customText; } set { _customText = value; PC(); } }
		public ObservableCollection<DPData> pointList { get; set; } = new();
		public GroupPriorityTraits groupPriorityTraits { get; set; }
		public Guid defeatedTrigger { get { return _defeatedTrigger; } set { _defeatedTrigger = value; PC(); } }
		public Guid defeatedEvent { get { return _defeatedEvent; } set { _defeatedEvent = value; PC(); } }
		public bool useGenericMugshot { get { return _useGenericMugshot; } set { _useGenericMugshot = value; PC(); } }
		public bool useInitialGroupCustomName { get { return _useInitialGroupCustomName; } set { _useInitialGroupCustomName = value; PC(); } }

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}
		public event PropertyChangedEventHandler PropertyChanged;

		public EnemyGroupData()
		{

		}

		public EnemyGroupData( DeploymentCard dc, DeploymentPoint dp )
		{
			GUID = Guid.NewGuid();
			cardName = dc.name;
			cardID = dc.id;
			customText = "";
			customInstructionType = CustomInstructionType.Replace;
			groupPriorityTraits = new();
			defeatedTrigger = Guid.Empty;
			defeatedEvent = Guid.Empty;
			useGenericMugshot = false;
			useInitialGroupCustomName = false;
			for ( int i = 0; i < dc.size; i++ )
			{
				pointList.Add( new() { GUID = dp.GUID } );
			}
		}

		public void SetDP( Guid guid )
		{
			int c = pointList.Count;
			pointList.Clear();
			for ( int i = 0; i < c; i++ )
			{
				pointList.Add( new() { GUID = guid } );
			}
		}

		public void UpdateCard( DeploymentCard newcard )
		{
			cardName = newcard.name;
			cardID = newcard.id;

			var oldPoints = pointList.ToArray();
			pointList.Clear();
			for ( int i = 0; i < newcard.size; i++ )
			{
				if ( i < oldPoints.Length )
					pointList.Add( oldPoints[i] );
				else
					pointList.Add( new() { GUID = Guid.Empty } );
			}
		}
	}

	public class InputRange : INotifyPropertyChanged
	{
		string _theText;
		int _fromValue, _toValue;
		Guid _triggerGUID, _eventGUID;

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}
		public event PropertyChangedEventHandler PropertyChanged;

		public string theText { get { return _theText; } set { _theText = value; PC(); } }
		public int fromValue { get { return _fromValue; } set { _fromValue = value; PC(); } }
		public int toValue { get { return _toValue; } set { _toValue = value; PC(); } }
		public Guid triggerGUID { get { return _triggerGUID; } set { _triggerGUID = value; PC(); } }
		public Guid eventGUID { get { return _eventGUID; } set { _eventGUID = value; PC(); } }

		public InputRange()
		{

		}
	}

	public class SourceData
	{
		public FileMode fileMode;
		public string stringifiedJsonData;
		public MetaDisplay metaDisplay;
		public string fileName
		{
			get
			{
				var split = metaDisplay.assetName.Split( '.' ).Reverse().Take( 2 ).Reverse().ToArray();
				return $"{split[0]}.{split[1]}";
			}
		}

		public SourceData()
		{
			fileMode = FileMode.Cancel;
			stringifiedJsonData = string.Empty;
			metaDisplay = new();
		}
	}

	public class MetaDisplay
	{
		public string displayName { get; set; }
		public string comboBoxTitle { get; set; }
		public string assetName { get; set; }
		public string missionExpansionFolder;//for saving cached file

		public MetaDisplay( string dname = "", string aname = "" )
		{
			displayName = dname;
			assetName = aname;
			comboBoxTitle = displayName;
		}
	}

	public class CampaignSkill
	{
		public string owner;
		public string id;
		public string name;
		public int cost;
	}

	public class Thumbnail
	{
		public string Name { get; set; }//full name of icon's character
		public string ID { get; set; }//basically the filename
	}

	public class BonusEffect
	{
		public string bonusID;
		public List<string> effects;
	}

	public class CardInstruction
	{
		public string instName, instID;
		public List<InstructionOption> content;
	}

	public class InstructionOption
	{
		public List<string> instruction;
	}
}
