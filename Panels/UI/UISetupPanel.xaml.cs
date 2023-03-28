using System.Windows;
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
