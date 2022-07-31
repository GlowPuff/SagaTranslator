namespace Saga_Translator
{
	public interface ITranslationPanel { }

	public interface IFindReplace
	{
		int FindReplace( string needle, string replace );
	}
}
