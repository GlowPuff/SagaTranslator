using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for HighlightPanel.xaml
	/// </summary>
	public partial class HighlightPanel : UserControl, ITranslationPanel
	{
		public SpaceHighlight translated { get; set; }
		public SpaceHighlight source { get; set; }

		public HighlightPanel( IMapEntity me )
		{
			InitializeComponent();
			DataContext = this;

			translated = me as SpaceHighlight;
			source = Utils.mainWindow.sourceMission.GetEntityFromGUID<SpaceHighlight>( me.GUID );
		}
	}
}
