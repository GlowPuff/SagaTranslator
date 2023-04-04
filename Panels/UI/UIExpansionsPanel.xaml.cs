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
