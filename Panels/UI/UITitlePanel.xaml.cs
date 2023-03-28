using System.Windows;
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
