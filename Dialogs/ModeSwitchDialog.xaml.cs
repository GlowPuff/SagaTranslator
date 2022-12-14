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
			appMode = TranslateMode.Other;
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
	}
}
