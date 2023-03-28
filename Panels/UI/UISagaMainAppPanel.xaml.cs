using System.Windows;
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
		}

		private void UserControl_Loaded( object sender, System.Windows.RoutedEventArgs e )
		{
			foreach ( var item in dataPanel.Children )
			{
				if ( item is TextBox && ((TextBox)item).CharacterCasing == CharacterCasing.Upper )
				{
					Clipboard.SetData( DataFormats.Text, ((TextBox)item).Text );
					((TextBox)item).Text = "";
					((TextBox)item).Paste();
				}
			}
		}
	}
}
