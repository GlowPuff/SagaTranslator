using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	public partial class GenericUIPanel
	{
		void HandleCampaignRewards()
		{
			//indicates ONE CampaignReward
			sourceUI = ((List<CampaignReward>)Utils.mainWindow.sourceDynamicUIModel.data)[dContext.arrayIndex];
			translatedUI = ((List<CampaignReward>)Utils.mainWindow.translatedDynamicUIModel.data)[dContext.arrayIndex];

			//translate side
			translatePanel.Children.Add( UIFactory.TextBlock( (translatedUI as CampaignReward).id ) );
			translatePanel.Children.Add( UIFactory.TextBox(
				(translatedUI as CampaignReward).name,
				new()
				{
					name = "name",
					data = translatedUI as CampaignReward,
				},
				CampaignRewardsLostFocus,
				false
				) );

			//source side
			sourcePanel.Children.Add( UIFactory.TextBlock( (sourceUI as CampaignReward).id ) );
			sourcePanel.Children.Add( UIFactory.TextBox( (sourceUI as CampaignReward).name ) );
		}

		void CampaignRewardsLostFocus( object sender, RoutedEventArgs e )
		{
			foreach ( var item in translatePanel.Children )
			{
				var ctx = ((FrameworkElement)item).DataContext;
				if ( ctx is DynamicDataContext )
				{
					var dtx = (DynamicDataContext)ctx;
					if ( dtx.name == "name" )
					{
						var bf = dtx.data as CampaignReward;
						bf.name = ((TextBox)item).Text;
					}
				}
			}
		}
	}
}
