using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for UISagaMainAppPanel.xaml
	/// </summary>
	public partial class UISagaMainAppPanel : UserControl, ITranslationPanel
	{
		public SagaMainApp sourceUI { get; set; }
		public SagaMainApp translatedUI { get; set; }

		public UISagaMainAppPanel( SagaMainApp ui )
		{
			InitializeComponent();
			DataContext = this;

			translatedUI = ui;
			sourceUI = Utils.mainWindow.sourceUI.sagaMainApp;

			Utils.ValidateProperties( translatedUI, "sagaMainApp" );
		}
	}
}
