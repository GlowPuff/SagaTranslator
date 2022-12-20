using System.Windows;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for SupportedFilesDialog.xaml
	/// </summary>
	public partial class SupportedFilesDialog : Window
	{
		public SupportedFilesDialog()
		{
			InitializeComponent();
		}

		private void okBtn_Click( object sender, RoutedEventArgs e )
		{
			Close();
		}
	}
}
