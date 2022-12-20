using System.Windows;
using System.Windows.Controls;

namespace Saga_Translator
{
	public static class UIFactory
	{
		public static TextBlock TextBlock( string title )
		{
			return new()
			{
				Text = title,
				Margin = new( 0, 10, 0, 5 )
			};
		}

		public static TextBox TextBox( string text, bool isMulti = true )
		{
			return TextBox( text, null, null, isMulti );
		}

		public static TextBox TextBox( string text, DynamicDataContext dtx, RoutedEventHandler evHandler, bool isMulti = true )
		{
			var tb = new TextBox()
			{
				Text = text,
				BorderThickness = new( 2 ),
				DataContext = dtx,
			};
			if ( evHandler != null )
				tb.LostFocus += evHandler;
			if ( isMulti )
				tb.Style = Application.Current.FindResource( "multi" ) as Style;

			return tb;
		}
	}
}
