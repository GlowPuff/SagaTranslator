using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for TokenPanel.xaml
	/// </summary>
	public partial class TokenPanel : UserControl, ITranslationPanel
	{
		public Token translated { get; set; }
		public Token source { get; set; }

		public TokenPanel( IMapEntity me )
		{
			InitializeComponent();
			DataContext = this;

			translated = me as Token;
			source = Utils.mainWindow.sourceMission.GetEntityFromGUID<Token>( me.GUID );
		}
	}
}
