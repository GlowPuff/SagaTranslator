using System;
using System.Windows;
using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	public partial class GenericUIPanel
	{
		void HandlCampaignInfo()
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

		void CampaignInfoLostFocus( object sender, EventArgs e )
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
