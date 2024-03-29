﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Saga_Translator;

namespace Imperial_Commander_Editor
{
	public class MissionProperties : INotifyPropertyChanged, IFindReplace
	{
		public event PropertyChangedEventHandler PropertyChanged;

		string _missionName, _missionID, _fixedAlly, _bannedAlly, _missionInfo, _specificAlly, _specificHero, _priorityOther, _missionDescription, _campaignName, _startingObjective, _additionalMissionInfo;
		bool _optionalDeployment, _factionImperial, _factionMercenary;
		YesNoAll _useFixedAlly, _useBannedAlly, _banAllAllies;
		CustomInstructionType _customInstructionType;
		//ThreatModifierType _initialThreatType;
		PriorityTargetType _priorityTargetType;
		Guid _startingEvent;
		//int _initialThreatModifier, _initialThreatMultiplier;
		MissionType _missionType;
		ChangeReposition _changeRepositionOverride;
		ObservableCollection<MissionSubType> _missionSubType;
		ObservableCollection<string> _multipleBannedAllies;

		public string missionName
		{
			get { return _missionName; }
			set { _missionName = value; PC(); }
		}
		public string campaignName
		{
			get { return _campaignName; }
			set { _campaignName = value; PC(); }
		}
		public string missionDescription
		{
			get { return _missionDescription; }
			set { _missionDescription = value; PC(); }
		}
		public string fixedAlly
		{
			get { return _fixedAlly; }
			set { _fixedAlly = value; PC(); }
		}
		public string bannedAlly
		{
			get { return _bannedAlly; }
			set { _bannedAlly = value; PC(); }
		}
		public string startingObjective
		{
			get { return _startingObjective; }
			set { _startingObjective = value; PC(); }
		}
		public string additionalMissionInfo
		{
			get { return _additionalMissionInfo; }
			set { _additionalMissionInfo = value; PC(); }
		}

		//set to EXPANSIONXX where XX is the mission number and EXPANSION is the expansion name (ie: CORE, EMPIRE)
		public string missionID
		{
			get { return _missionID; }
			set { _missionID = value; PC(); }
		}
		public string missionInfo
		{
			get { return _missionInfo; }
			set { _missionInfo = value; PC(); }
		}

		public bool optionalDeployment
		{
			get { return _optionalDeployment; }
			set { _optionalDeployment = value; PC(); }
		}

		public bool factionImperial
		{
			get { return _factionImperial; }
			set { _factionImperial = value; PC(); }
		}

		public bool factionMercenary
		{
			get { return _factionMercenary; }
			set { _factionMercenary = value; PC(); }
		}
		public YesNoAll banAllAllies
		{
			get { return _banAllAllies; }
			set { _banAllAllies = value; PC(); }
		}

		public YesNoAll useFixedAlly
		{
			get { return _useFixedAlly; }
			set { _useFixedAlly = value; PC(); }
		}
		public YesNoAll useBannedAlly
		{
			get { return _useBannedAlly; }
			set { _useBannedAlly = value; PC(); }
		}

		public CustomInstructionType customInstructionType
		{
			get { return _customInstructionType; }
			set { _customInstructionType = value; PC(); }
		}

		//public ThreatModifierType initialThreatType
		//{
		//	get { return _initialThreatType; }
		//	set { _initialThreatType = value; PC(); }
		//}

		public PriorityTargetType priorityTargetType
		{
			get { return _priorityTargetType; }
			set { _priorityTargetType = value; PC(); }
		}

		public string specificAlly
		{
			get { return _specificAlly; }
			set { _specificAlly = value; PC(); }
		}

		public string specificHero
		{
			get { return _specificHero; }
			set { _specificHero = value; PC(); }
		}

		public string priorityOther
		{
			get { return _priorityOther; }
			set { _priorityOther = value; PC(); }
		}

		public Guid startingEvent
		{
			get { return _startingEvent; }
			set { _startingEvent = value; PC(); }
		}

		public MissionType missionType { get { return _missionType; } set { _missionType = value; PC(); } }
		public ChangeReposition changeRepositionOverride { get { return _changeRepositionOverride; } set { _changeRepositionOverride = value; PC(); } }
		public ObservableCollection<MissionSubType> missionSubType { get { return _missionSubType; } set { _missionSubType = value; PC(); } }

		public ObservableCollection<string> bannedGroups { get; set; } = new();
		public ObservableCollection<string> multipleBannedAllies
		{
			get
			{
				//added for Mission Format v.20
				if ( _multipleBannedAllies == null )
					_multipleBannedAllies = new();
				return _multipleBannedAllies;
			}
			set
			{
				_multipleBannedAllies = value;
				PC();
			}
		}

		public void PC( [CallerMemberName] string n = "" )
		{
			if ( !string.IsNullOrEmpty( n ) )
				PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
		}

		public MissionProperties()
		{
			missionID = "Custom";//"Example: CORE01";//"Unique Mission ID";
			missionName = "Mission Name";
			missionDescription = "";
			missionInfo = "";
			startingObjective = "";
			fixedAlly = "A001";
			bannedAlly = "A001";
			priorityOther = "";
			specificAlly = "A001";
			specificHero = "H1";
			optionalDeployment = false;
			factionImperial = true;
			factionMercenary = true;
			customInstructionType = CustomInstructionType.Replace;
			useFixedAlly = YesNoAll.No;
			useBannedAlly = YesNoAll.No;
			banAllAllies = YesNoAll.No;
			priorityTargetType = PriorityTargetType.Rebel;
			startingEvent = Guid.Empty;
			missionType = MissionType.Story;
			changeRepositionOverride = null;
			missionSubType = new();
		}

		public int FindReplace( string needle, string replace )
		{
			if ( string.IsNullOrEmpty( needle ) || string.IsNullOrEmpty( replace ) )
				return 0;

			int count = 0;

			if ( !string.IsNullOrEmpty( missionName ) )
			{
				var regex = new Regex( needle );
				count += regex.Matches( missionName ).Count;
				foreach ( var match in regex.Matches( missionName ) )
				{
					missionName = missionName.Replace( match.ToString(), replace );
				}
			}

			if ( !string.IsNullOrEmpty( missionDescription ) )
			{
				var regex = new Regex( needle );
				count += regex.Matches( missionDescription ).Count;
				foreach ( var match in regex.Matches( missionDescription ) )
				{
					missionDescription = missionDescription.Replace( match.ToString(), replace );
				}
			}

			if ( !string.IsNullOrEmpty( missionInfo ) )
			{
				var regex = new Regex( needle );
				count += regex.Matches( missionInfo ).Count;
				foreach ( var match in regex.Matches( missionInfo ) )
				{
					missionInfo = missionInfo.Replace( match.ToString(), replace );
				}
			}

			if ( !string.IsNullOrEmpty( startingObjective ) )
			{
				var regex = new Regex( needle );
				count += regex.Matches( startingObjective ).Count;
				foreach ( var match in regex.Matches( startingObjective ) )
				{
					startingObjective = startingObjective.Replace( match.ToString(), replace );
				}
			}

			return count;
		}
	}
}
