using System.Windows;
using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	public partial class GenericUIPanel
	{
		void HandleCardEvents()
		{
			//indicates ONE CardEvent
			sourceUI = ((EventList)Utils.mainWindow.sourceDynamicUIModel.data).events[dContext.arrayIndex];
			translatedUI = ((EventList)Utils.mainWindow.translatedDynamicUIModel.data).events[dContext.arrayIndex];

			//translate side
			translatePanel.Children.Add( UIFactory.TextBlock( "Event Name" ) );
			translatePanel.Children.Add( UIFactory.TextBox(
				(translatedUI as CardEvent).eventName,
				new()
				{
					name = "name",
					data = translatedUI as CardEvent,
				},
				CardEventsLostFocus,
				false
				) );

			translatePanel.Children.Add( UIFactory.TextBlock( "Event Flavor" ) );
			translatePanel.Children.Add( UIFactory.TextBox(
				(translatedUI as CardEvent).eventFlavor,
				new()
				{
					name = "flavor",
					data = translatedUI as CardEvent,
				},
				CardEventsLostFocus
				) );

			translatePanel.Children.Add( UIFactory.TextBlock( "Event Content" ) );
			for ( int i = 0; i < (translatedUI as CardEvent).content.Count; i++ )
			{
				translatePanel.Children.Add( UIFactory.TextBox(
					(translatedUI as CardEvent).content[i],
					new()
					{
						name = "content",
						data = translatedUI as CardEvent,
						index = i,
					},
					CardEventsLostFocus
					) );
			}

			//source side
			sourcePanel.Children.Add( UIFactory.TextBlock( "Event Name" ) );
			sourcePanel.Children.Add( UIFactory.TextBox( (sourceUI as CardEvent).eventName ) );

			sourcePanel.Children.Add( UIFactory.TextBlock( "Event Flavor" ) );
			sourcePanel.Children.Add( UIFactory.TextBox( (sourceUI as CardEvent).eventFlavor ) );

			sourcePanel.Children.Add( UIFactory.TextBlock( "Event Content" ) );
			for ( int i = 0; i < (sourceUI as CardEvent).content.Count; i++ )
			{
				sourcePanel.Children.Add( UIFactory.TextBox( (sourceUI as CardEvent).content[i] ) );
			}
		}

		void CardEventsLostFocus( object sender, RoutedEventArgs e )
		{
			foreach ( var item in translatePanel.Children )
			{
				var ctx = ((FrameworkElement)item).DataContext;
				if ( ctx is DynamicDataContext )
				{
					var dtx = (DynamicDataContext)ctx;
					if ( dtx.name == "name" )
					{
						var bf = dtx.data as CardEvent;
						bf.eventName = ((TextBox)item).Text;
					}
					else if ( dtx.name == "flavor" )
					{
						var bf = dtx.data as CardEvent;
						bf.eventFlavor = ((TextBox)item).Text;
					}
					else if ( dtx.name == "content" )
					{
						var bf = dtx.data as CardEvent;
						bf.content[dtx.index] = ((TextBox)item).Text;
					}
				}
			}
		}
	}
}
