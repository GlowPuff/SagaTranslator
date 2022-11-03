using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for UITitle.xaml
	/// </summary>
	public partial class UITitlePanel : UserControl, ITranslationPanel
	{
		public UITitle sourceUI { get; set; }
		public UITitle translatedUI { get; set; }

		public UITitlePanel( UITitle ui )
		{
			InitializeComponent();
			DataContext = this;

			translatedUI = ui;
			sourceUI = Utils.mainWindow.sourceUI.uiTitle;
		}
	}
}
