using System.Text.RegularExpressions;
using Saga_Translator;

namespace Imperial_Commander_Editor
{
	public class ChangeObjective : EventAction, IFindReplace
	{
		string _theText, _longText;
		public string theText { get { return _theText; } set { _theText = value; PC(); } }
		public string longText { get { return _longText; } set { _longText = value; PC(); } }

		public ChangeObjective()
		{

		}
		public ChangeObjective( string dname
			, EventActionType et ) : base( et, dname )
		{

		}

		public int FindReplace( string needle, string replace )
		{
			int count = 0;
			if ( !string.IsNullOrEmpty( theText ) )
			{
				var regex = new Regex( needle );
				count += regex.Matches( theText ).Count;
				foreach ( var match in regex.Matches( theText ) )
				{
					theText = theText.Replace( match.ToString(), replace );
				}
			}

			if ( !string.IsNullOrEmpty( longText ) )
			{
				var regex = new Regex( needle );
				count += regex.Matches( longText ).Count;
				foreach ( var match in regex.Matches( longText ) )
				{
					longText = longText.Replace( match.ToString(), replace );
				}
			}

			return count;
		}
	}
}
