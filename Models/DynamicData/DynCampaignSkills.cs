using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	public partial class GenericUIPanel
	{
		void HandleCampaignSkills()
		{
			//indicates ONE CampaignItem
			sourceUI = ((List<CampaignSkill>)Utils.mainWindow.sourceDynamicUIModel.data)[dContext.arrayIndex];
			translatedUI = ((List<CampaignSkill>)Utils.mainWindow.translatedDynamicUIModel.data)[dContext.arrayIndex];

			//translate side
			translatePanel.Children.Add( UIFactory.TextBlock( (translatedUI as CampaignSkill).id ) );
			translatePanel.Children.Add( UIFactory.TextBox(
				(translatedUI as CampaignSkill).name,
				new()
				{
					name = "name",
					data = translatedUI as CampaignSkill,
				},
				CampaignSkillsLostFocus,
				false
				) );

			//source side
			sourcePanel.Children.Add( UIFactory.TextBlock( (sourceUI as CampaignSkill).id ) );
			sourcePanel.Children.Add( UIFactory.TextBox( (sourceUI as CampaignSkill).name ) );
		}

		void CampaignSkillsLostFocus( object sender, RoutedEventArgs e )
		{
			foreach ( var item in translatePanel.Children )
			{
				var ctx = ((FrameworkElement)item).DataContext;
				if ( ctx is DynamicDataContext )
				{
					var dtx = (DynamicDataContext)ctx;
					if ( dtx.name == "name" )
					{
						var bf = dtx.data as CampaignSkill;
						bf.name = ((TextBox)item).Text;
					}
				}
			}
		}
	}
}
