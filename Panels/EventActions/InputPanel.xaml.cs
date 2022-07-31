using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for InputPanel.xaml
	/// </summary>
	public partial class InputPanel : UserControl, ITranslationPanel
	{
		public InputPrompt translated { get; set; }
		public InputPrompt source { get; set; }

		public InputPanel( IEventAction ea )
		{
			InitializeComponent();
			DataContext = this;

			translated = ea as InputPrompt;
			source = Utils.mainWindow.sourceMission.GetEAByGUID<InputPrompt>( ea.GUID );
		}
	}
}
