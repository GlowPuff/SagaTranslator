using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for TerminalPanel.xaml
	/// </summary>
	public partial class TerminalPanel : UserControl, ITranslationPanel
	{
		public Console translated { get; set; }
		public Console source { get; set; }

		public TerminalPanel( IMapEntity me )
		{
			InitializeComponent();
			DataContext = this;

			translated = me as Console;
			source = Utils.mainWindow.sourceMission.GetEntityFromGUID<Console>( me.GUID );
		}
	}
}
