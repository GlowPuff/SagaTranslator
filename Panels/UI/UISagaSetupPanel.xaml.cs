using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for UISetupPanel.xaml
	/// </summary>
	public partial class UISagaSetupPanel : UserControl, ITranslationPanel
	{
		public SagaUISetup sourceUI { get; set; }
		public SagaUISetup translatedUI { get; set; }

		public UISagaSetupPanel( SagaUISetup ui )
		{
			InitializeComponent();
			DataContext = this;

			translatedUI = ui;
			sourceUI = Utils.mainWindow.sourceUI.sagaUISetup;
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
