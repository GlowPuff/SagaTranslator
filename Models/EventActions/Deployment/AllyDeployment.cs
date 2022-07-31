using System;
using System.Text.RegularExpressions;
using Saga_Translator;

namespace Imperial_Commander_Editor
{
	public class AllyDeployment : EventAction, IFindReplace
	{
		string _allyName, _allyID;
		Guid _setTrigger, _specificDeploymentPoint;
		DeploymentSpot _deploymentPoint;
		int _threatCost;
		bool _useThreat, _useGenericMugshot;

		public string allyName { get { return _allyName; } set { _allyName = value; PC(); } }
		public string allyID { get { return _allyID; } set { _allyID = value; PC(); } }
		public Guid setTrigger { get { return _setTrigger; } set { _setTrigger = value; PC(); } }
		public DeploymentSpot deploymentPoint { get { return _deploymentPoint; } set { _deploymentPoint = value; PC(); } }
		public Guid specificDeploymentPoint { get { return _specificDeploymentPoint; } set { _specificDeploymentPoint = value; PC(); } }
		public int threatCost { get { return _threatCost; } set { _threatCost = value; PC(); } }
		public bool useThreat { get { return _useThreat; } set { _useThreat = value; PC(); } }
		public bool useGenericMugshot { get { return _useGenericMugshot; } set { _useGenericMugshot = value; PC(); } }

		public AllyDeployment()
		{

		}

		public AllyDeployment( string dname
			, EventActionType et ) : base( et, dname )
		{
			_setTrigger = Guid.Empty;
			_specificDeploymentPoint = Guid.Empty;
			_allyID = "A001";
			_deploymentPoint = DeploymentSpot.Active;
			_threatCost = 0;
			_useThreat = false;
			_useGenericMugshot = false;
		}

		public int FindReplace( string needle, string replace )
		{
			int count = 0;
			if ( !string.IsNullOrEmpty( allyName ) )
			{
				var regex = new Regex( needle );
				count += regex.Matches( allyName ).Count;
				foreach ( var match in regex.Matches( allyName ) )
				{
					allyName = allyName.Replace( match.ToString(), replace );
				}
			}
			return count;
		}
	}
}
