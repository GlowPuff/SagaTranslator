using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for UIExpansionsPanel.xaml
	/// </summary>
	public partial class UIExpansionsPanel : UserControl, ITranslationPanel
	{
		public UIExpansions sourceUI { get; set; }
		public UIExpansions translatedUI { get; set; }

		public UIExpansionsPanel( UIExpansions ui )
		{
			InitializeComponent();
			DataContext = this;

			translatedUI = ui;
			sourceUI = Utils.mainWindow.sourceUI.uiExpansions;
		}
	}
}
