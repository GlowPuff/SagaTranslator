using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for ChangeInstructionsPanel.xaml
	/// </summary>
	public partial class ChangeInstructionsPanel : UserControl, ITranslationPanel
	{
		public ChangeInstructions translated { get; set; }
		public ChangeInstructions source { get; set; }

		public ChangeInstructionsPanel( IEventAction ea )
		{
			InitializeComponent();
			DataContext = this;

			translated = ea as ChangeInstructions;
			source = Utils.mainWindow.sourceMission.GetEAByGUID<ChangeInstructions>( ea.GUID );
		}
	}
}
