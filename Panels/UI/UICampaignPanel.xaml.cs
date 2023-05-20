using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for UICampaignPanel.xaml
	/// </summary>
	public partial class UICampaignPanel : UserControl, ITranslationPanel
	{
		public UICampaign sourceUI { get; set; }
		public UICampaign translatedUI { get; set; }

		public UICampaignPanel( UICampaign ui )
		{
			InitializeComponent();
			DataContext = this;

			translatedUI = ui;
			sourceUI = Utils.mainWindow.sourceUI.uiCampaign;
		}
	}
}
