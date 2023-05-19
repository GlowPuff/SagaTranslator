using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for UILoggerPanel.xaml
	/// </summary>
	public partial class UILoggerPanel : UserControl, ITranslationPanel
	{
		public UILogger sourceUI { get; set; }
		public UILogger translatedUI { get; set; }

		public UILoggerPanel( UILogger ui )
		{
			InitializeComponent();
			DataContext = this;

			translatedUI = ui;
			sourceUI = Utils.mainWindow.sourceUI.uiLogger;
		}
	}
}
