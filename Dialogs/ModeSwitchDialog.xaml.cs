using System.Windows;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for ModeSwitchDialog.xaml
	/// </summary>
	public partial class ModeSwitchDialog : Window
	{
		public TranslateMode appMode;

		public ModeSwitchDialog()
		{
			InitializeComponent();
		}

		private void cancelBtn_Click( object sender, RoutedEventArgs e )
		{
			appMode = TranslateMode.Cancel;
			Close();
		}

		private void otherBtn_Click( object sender, RoutedEventArgs e )
		{
			appMode = TranslateMode.Supplemental;
			Close();
		}

		private void uiBtn_Click( object sender, RoutedEventArgs e )
		{
			appMode = TranslateMode.UI;
			Close();
		}

		private void missionBtn_Click( object sender, RoutedEventArgs e )
		{
			appMode = TranslateMode.Mission;
			Close();
		}

		private void supportedBtn_Click( object sender, RoutedEventArgs e )
		{
			var dlg = new SupportedFilesDialog();
			dlg.ShowDialog();
		}
	}
}
