namespace Saga_Translator
{
	using System.Windows;
	using System.Windows.Controls;
	using Imperial_Commander_Editor;

	public partial class GenericUIPanel
	{
		void HandleInfoRUles()
		{
			//string
			sourceUI = (string)Utils.mainWindow.sourceDynamicUIModel.data;
			translatedUI = (string)Utils.mainWindow.translatedDynamicUIModel.data;

			//translate side
			translatePanel.Children.Add( UIFactory.TextBlock( "Text" ) );
			translatePanel.Children.Add( UIFactory.TextBox(
				(translatedUI as string),
				new()
				{
					name = "text",
					data = translatedUI,
				},
				InfoRUlesLostFocus
				) );

			//source side
			sourcePanel.Children.Add( UIFactory.TextBlock( "Text" ) );
			sourcePanel.Children.Add( UIFactory.TextBox( sourceUI as string ) );
		}

		void InfoRUlesLostFocus( object sender, RoutedEventArgs e )
		{
			foreach ( var item in translatePanel.Children )
			{
				var ctx = ((FrameworkElement)item).DataContext;
				if ( ctx is DynamicDataContext )
				{
					var dtx = (DynamicDataContext)ctx;
					if ( dtx.name == "text" )
					{
						Utils.mainWindow.translatedDynamicUIModel.data = ((TextBox)item).Text;
					}
				}
			}
		}
	}
}
