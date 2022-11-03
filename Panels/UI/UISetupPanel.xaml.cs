using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for UISetupPanel.xaml
	/// </summary>
	public partial class UISetupPanel : UserControl, ITranslationPanel
	{
		public UISetup sourceUI { get; set; }
		public UISetup translatedUI { get; set; }

		public UISetupPanel( UISetup ui )
		{
			InitializeComponent();
			DataContext = this;

			translatedUI = ui;
			sourceUI = Utils.mainWindow.sourceUI.uiSetup;
		}
	}
}
