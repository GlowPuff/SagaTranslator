using System.Text.RegularExpressions;
using Saga_Translator;

namespace Imperial_Commander_Editor
{
	public class ShowTextBox : EventAction, IFindReplace
	{
		string _theText;
		public string theText { get { return _theText; } set { _theText = value; PC(); } }

		public ShowTextBox()
		{

		}

		public ShowTextBox( string dname
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
			return count;
		}
	}
}
