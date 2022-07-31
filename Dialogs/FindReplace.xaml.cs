using System.Collections.Generic;
using System.Windows;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for FindReplace.xaml
	/// </summary>
	public partial class FindReplace : Window
	{
		List<IFindReplace> eventList = new();

		public FindReplace( List<IFindReplace> evList )
		{
			InitializeComponent();

			eventList = evList;
			findText.Focus();
		}

		private void doItBtn_Click( object sender, RoutedEventArgs e )
		{
			foundText.Text = "0";
			int count = 0;

			foreach ( var item in eventList )
			{
				if ( !string.IsNullOrEmpty( findText.Text.Trim() ) && !string.IsNullOrEmpty( replaceText.Text.Trim() ) )
				{
					count += item.FindReplace( $"({findText.Text.Trim()})", replaceText.Text.Trim() );
				}
			}
			foundText.Text = count.ToString();


			//if ( useRegEx.IsChecked == false )
			//{
			//	foreach ( var item in eventList )
			//	{
			//		if ( !string.IsNullOrEmpty( findText.Text.Trim() ) && !string.IsNullOrEmpty( replaceText.Text.Trim() ) )
			//		{
			//			count += item.FindReplace( $"({findText.Text.Trim()})", replaceText.Text.Trim() );
			//		}
			//	}
			//	foundText.Text = count.ToString();
			//}
			//else
			//{
			//	foreach ( var item in eventList )
			//	{
			//		if ( !string.IsNullOrEmpty( findText.Text.Trim() ) && !string.IsNullOrEmpty( replaceText.Text.Trim() ) )
			//		{
			//			count += item.FindReplace( findText.Text.Trim(), replaceText.Text.Trim() );
			//		}
			//	}
			//	foundText.Text = count.ToString();
			//}
		}

		private void closeBtn_Click( object sender, RoutedEventArgs e )
		{
			Close();
		}
	}
}
