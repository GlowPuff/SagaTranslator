using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for UIMainAppPanel.xaml
	/// </summary>
	public partial class UIMainAppPanel : UserControl, ITranslationPanel
	{
		public UIMainApp sourceUI { get; set; }
		public UIMainApp translatedUI { get; set; }

		public UIMainAppPanel( UIMainApp ui )
		{
			InitializeComponent();
			DataContext = this;

			translatedUI = ui;
			sourceUI = Utils.mainWindow.sourceUI.uiMainApp;
		}

		private void UserControl_Loaded( object sender, System.Windows.RoutedEventArgs e )
		{
			//foreach ( var item in dataPanel.Children )
			//{
			//	if ( item is TextBox && ((TextBox)item).CharacterCasing == CharacterCasing.Upper )
			//	{
			//		Clipboard.SetData( DataFormats.Text, ((TextBox)item).Text );
			//		((TextBox)item).Text = "";
			//		((TextBox)item).Paste();
			//	}
			//}
		}
	}
}
