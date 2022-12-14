using System.Collections.Generic;
using System.IO;
using Imperial_Commander_Editor;
using Newtonsoft.Json;

namespace Saga_Translator
{
	public static class DynamicParser
	{
		public static ParsedObject Parse( string filenamePath )
		{
			GenericUIData source, sourceCopy;
			string fname = new FileInfo( filenamePath ).Name;

			if ( fname == "instructions.json" )
			{
				var ui = FileManager.LoadUI<List<CardInstruction>>( filenamePath );
				if ( ui != null )
				{
					source = new();
					source.data = ui;
					//make a copy of the mission
					string json = JsonConvert.SerializeObject( ui );
					//set the TRANSLATED MISSION
					sourceCopy = new();
					sourceCopy.data = JsonConvert.DeserializeObject<List<CardInstruction>>( json );

					return new()
					{
						isSuccess = true,
						data = source,
						dataCopy = sourceCopy,
						gType = GenericType.Instructions
					};
				}
				else
				{
					return new() { isSuccess = false, errorMsg = "Loaded object was null." };
				}
			}

			return new() { isSuccess = false, errorMsg = "Unrecognized data filename." };
		}
	}
}
