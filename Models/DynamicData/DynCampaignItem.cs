using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	public partial class GenericUIPanel
	{
		void HandleCampaignItems()
		{
			//indicates ONE BonusEffect
			sourceUI = ((List<CampaignItem>)Utils.mainWindow.sourceDynamicUIModel.data)[dContext.arrayIndex];
			translatedUI = ((List<CampaignItem>)Utils.mainWindow.translatedDynamicUIModel.data)[dContext.arrayIndex];

			//translate side
			translatePanel.Children.Add( UIFactory.TextBlock( (translatedUI as CampaignItem).id ) );
			translatePanel.Children.Add( UIFactory.TextBox(
				(translatedUI as CampaignItem).name,
				new()
				{
					name = "name",
					data = translatedUI as CampaignItem,
				},
				CampaignItemsLostFocus,
				false
				) );

			//source side
			sourcePanel.Children.Add( UIFactory.TextBlock( (sourceUI as CampaignItem).id ) );
			sourcePanel.Children.Add( UIFactory.TextBox( (sourceUI as CampaignItem).name ) );
		}

		void CampaignItemsLostFocus( object sender, RoutedEventArgs e )
		{
			foreach ( var item in translatePanel.Children )
			{
				var ctx = ((FrameworkElement)item).DataContext;
				if ( ctx is DynamicDataContext )
				{
					var dtx = (DynamicDataContext)ctx;
					if ( dtx.name == "name" )
					{
						var bf = dtx.data as CampaignItem;
						bf.name = ((TextBox)item).Text;
					}
				}
			}
		}
	}
}
