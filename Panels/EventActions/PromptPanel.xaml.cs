using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for PromptPanel.xaml
	/// </summary>
	public partial class PromptPanel : UserControl, ITranslationPanel
	{
		public QuestionPrompt translated { get; set; }
		public QuestionPrompt source { get; set; }

		public PromptPanel( IEventAction ea )
		{
			InitializeComponent();
			DataContext = this;

			translated = ea as QuestionPrompt;
			source = Utils.mainWindow.sourceMission.GetEAByGUID<QuestionPrompt>( ea.GUID );
		}

		private void TextBox_KeyDown( object sender, System.Windows.Input.KeyEventArgs e )
		{
			if ( e.Key == System.Windows.Input.Key.Enter )
				Utils.LoseFocus( sender as Control );
		}
	}
}
