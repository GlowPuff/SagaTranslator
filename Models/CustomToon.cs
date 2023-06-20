using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Imperial_Commander_Editor
{
	public class CustomToon : ObservableObject
	{
		public Guid customCharacterGUID;

		//public string cardName;
		public string cardID;

		public string groupAttack;
		public string groupDefense;
		public GroupPriorityTraits groupPriorityTraits;

		public DeploymentCard deploymentCard;

		public Thumbnail thumbnail;
		public BonusEffect bonusEffect;
		public CardInstruction cardInstruction;

		public bool canRedeploy;
		public bool canReinforce;
		public bool canBeDefeated;
		public bool useThreatMultiplier;
		public Factions faction;
		public List<CampaignSkill> heroSkills;

		//convert values into strings
		string _cardName, _cardSubName;

		public string cardName
		{
			get => _cardName;
			set
			{
				SetProperty( ref _cardName, value );
				deploymentCard.name = value;
				cardInstruction.instName = value;
			}
		}

		public string keywords
		{
			get
			{
				string s = "";
				if ( deploymentCard.keywords.Length > 0 )
					s = string.Join( "\n", deploymentCard.keywords );
				return s;
			}
			set
			{
				if ( string.IsNullOrEmpty( value.Trim() ) )
					deploymentCard.keywords = new string[0];
				else
					deploymentCard.keywords = value.Split( "\n" ).Select( x => x.Trim() ).ToArray();
			}
		}

		public string surges
		{
			get
			{
				string s = "";
				if ( deploymentCard.surges.Length > 0 )
					s = string.Join( "\n", deploymentCard.surges );
				return s;
			}
			set
			{
				if ( string.IsNullOrEmpty( value.Trim() ) )
					deploymentCard.surges = new string[0];
				else
					deploymentCard.surges = value.Split( "\n" ).Select( x => x.Trim() ).ToArray();
			}
		}

		public string abilities
		{
			get
			{
				string s = "";
				if ( deploymentCard.abilities.Length > 0 )
				{
					s = deploymentCard.abilities.Select( x => $"{x.name}:{x.text}" ).Aggregate( ( acc, cur ) => acc + "\n" + cur );
				}
				return s;
			}
			set
			{
				if ( string.IsNullOrEmpty( value.Trim() ) )
					deploymentCard.abilities = new GroupAbility[0];
				else
				{
					var array = value.Trim().Split( "\n" );//get each line of text
					var list = new List<GroupAbility>();
					foreach ( var item in array )
					{
						var a = item.Trim().Split( ":" );//split into name and text
						list.Add( new() { name = a[0].Trim(), text = a[1].Trim() } );
					}
					deploymentCard.abilities = list.ToArray();
				}
			}
		}

		public string bonuses
		{
			get
			{
				string s = "";
				if ( bonusEffect != null )
					s = string.Join( "\n", bonusEffect.effects );
				return s;
			}
			set
			{
				if ( string.IsNullOrEmpty( value.Trim() ) )
				{
					bonusEffect = new BonusEffect()
					{
						bonusID = cardID,
						effects = new()
					};
				}
				else
				{
					bonusEffect = new BonusEffect()
					{
						bonusID = cardID,
						effects = value.Trim().Split( "\n" ).Select( x => x.Trim() ).ToList()
					};
				}
			}
		}

		public string cardSubName { get => _cardSubName; set { SetProperty( ref _cardSubName, value ); deploymentCard.subname = value; } }

		public string instructions
		{
			get
			{
				string s = "";
				if ( cardInstruction.content.Count > 0 )
				{
					foreach ( var item in cardInstruction.content )
					{
						s += item.instruction.Aggregate( ( acc, cur ) => acc + "\n" + cur );
						s += "\n===\n";
					}
					s = s.Substring( 0, s.LastIndexOf( "===" ) ).Trim();
				}
				return s;
			}
			set
			{
				cardInstruction.content = new();
				value = value.Trim().Replace( "\r", "" );
				var groups = value.Trim().Split( "\n===\n" );
				foreach ( var item in groups )
				{
					cardInstruction.content.Add( new() { instruction = item.Trim().Split( "\n" ).ToList() } );
				}
			}
		}

		public CustomToon()
		{
			deploymentCard = new();
			heroSkills = new();
			cardInstruction = new();
		}
	}
}
