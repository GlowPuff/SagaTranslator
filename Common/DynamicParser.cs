using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Imperial_Commander_Editor;
using Newtonsoft.Json;

namespace Saga_Translator
{
	public static class DynamicParser
	{
		public static ParsedObject Parse( string filenamePath )
		{
			Regex regex = new Regex( @"(bespin|core|empire|hoth|jabba|lothal|other|twin)\d{1,2}(info|rules).txt", RegexOptions.IgnoreCase );
			Regex regex2 = new( @"(bespin|core|empire|hoth|jabba|lothal|other|twin)(info).txt" );

			//GenericUIData source, sourceCopy;
			string fname = new FileInfo( filenamePath ).Name.ToLower();
			var m = regex.Matches( fname );

			if ( fname == "instructions.json" )
				return Parse<List<CardInstruction>>( filenamePath, GenericType.Instructions );
			else if ( fname == "bonuseffects.json" )
				return Parse<List<BonusEffect>>( filenamePath, GenericType.BonusEffects );
			else if ( fname == "allies.json" || fname == "enemies.json" || fname == "heroes.json" || fname == "villains.json" )
				return Parse<List<CardLanguage>>( filenamePath, GenericType.CardLanguage );
			else if ( fname == "bespin.json" || fname == "core.json" || fname == "empire.json" || fname == "hoth.json" || fname == "jabba.json" || fname == "lothal.json" || fname == "other.json" || fname == "twin.json" )
				return Parse<List<MissionCardText>>( filenamePath, GenericType.MissionCardText );
			else if ( regex.Match( fname ).Success )
				return LoadText( filenamePath, GenericType.MissionRulesInfo );
			else if ( fname == "items.json" )
				return Parse<List<CampaignItem>>( filenamePath, GenericType.CampaignItems );
			else if ( fname == "rewards.json" )
				return Parse<List<CampaignReward>>( filenamePath, GenericType.CampaignRewards );
			else if ( fname == "skills.json" )
				return Parse<List<CampaignSkill>>( filenamePath, GenericType.CampaignSkills );
			else if ( regex2.Match( fname ).Success )
				return LoadText( filenamePath, GenericType.CampaignInfo );
			else if ( fname == "events.json" )
				return Parse<EventList>( filenamePath, GenericType.CardEvent );

			return new() { isSuccess = false, errorMsg = "Unrecognized data file type. You may be trying to open the file in the wrong translation Mode. Switch the Mode via the icon at the top left of the toolbar, then try again." };
		}

		private static ParsedObject LoadText( string filenamePath, GenericType gType )
		{
			GenericUIData source, sourceCopy;

			string txt = File.ReadAllText( filenamePath );
			source = new() { data = txt };
			sourceCopy = new() { data = txt };

			return new()
			{
				isSuccess = true,
				data = source,
				dataCopy = sourceCopy,
				gType = gType
			};
		}

		private static ParsedObject Parse<T>( string filenamePath, GenericType gType )
		{
			GenericUIData source, sourceCopy;

			var ui = FileManager.LoadJSON<T>( filenamePath );
			if ( ui != null )
			{
				source = new();
				source.data = ui;
				//make a copy of the data
				string json = JsonConvert.SerializeObject( ui );
				//set the TRANSLATED data
				sourceCopy = new();
				sourceCopy.data = JsonConvert.DeserializeObject<T>( json );

				return new()
				{
					isSuccess = true,
					data = source,
					dataCopy = sourceCopy,
					gType = gType
				};
			}
			else
			{
				return new() { isSuccess = false, errorMsg = "Loaded object was null." };
			}
		}
	}
}
