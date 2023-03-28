using System.Windows;
using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for UISettingsPanel.xaml
	/// </summary>
	public partial class UISettingsPanel : UserControl, ITranslationPanel
	{
		public UISettings sourceUI { get; set; }
		public UISettings translatedUI { get; set; }

		public UISettingsPanel( UISettings ui )
		{
			InitializeComponent();
			DataContext = this;

			translatedUI = ui;
			sourceUI = Utils.mainWindow.sourceUI.uiSettings;
		}

		private void UserControl_Loaded( object sender, RoutedEventArgs e )
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
