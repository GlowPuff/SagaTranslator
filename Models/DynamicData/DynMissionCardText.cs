using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	public partial class GenericUIPanel
	{
		void HandleMissionCardText()
		{
			//indicates ONE MissionCardText
			sourceUI = ((List<MissionCardText>)Utils.mainWindow.sourceDynamicUIModel.data)[dContext.arrayIndex];
			translatedUI = ((List<MissionCardText>)Utils.mainWindow.translatedDynamicUIModel.data)[dContext.arrayIndex];

			//translate side
			translatePanel.Children.Add( UIFactory.TextBlock( "Name" ) );
			translatePanel.Children.Add( UIFactory.TextBox(
				(translatedUI as MissionCardText).name,
				new()
				{
					name = "name",
					data = (translatedUI as MissionCardText),
				},
				MissionCardTextLostFocus,
				false
				) );

			translatePanel.Children.Add( UIFactory.TextBlock( "Description" ) );
			translatePanel.Children.Add( UIFactory.TextBox(
				(translatedUI as MissionCardText).descriptionText,
				new()
				{
					name = "descriptionText",
					data = (translatedUI as MissionCardText),
				},
				MissionCardTextLostFocus,
				true
				) );

			translatePanel.Children.Add( UIFactory.TextBlock( "Bonus Text" ) );
			translatePanel.Children.Add( UIFactory.TextBox(
				(translatedUI as MissionCardText).bonusText,
				new()
				{
					name = "bonusText",
					data = (translatedUI as MissionCardText),
				},
				MissionCardTextLostFocus,
				false
				) );

			translatePanel.Children.Add( UIFactory.TextBlock( "Hero Text" ) );
			translatePanel.Children.Add( UIFactory.TextBox(
				(translatedUI as MissionCardText).heroText,
				new()
				{
					name = "heroText",
					data = (translatedUI as MissionCardText),
				},
				MissionCardTextLostFocus,
				false
				) );

			translatePanel.Children.Add( UIFactory.TextBlock( "Ally Text" ) );
			translatePanel.Children.Add( UIFactory.TextBox(
				(translatedUI as MissionCardText).allyText,
				new()
				{
					name = "allyText",
					data = (translatedUI as MissionCardText),
				},
				MissionCardTextLostFocus,
				false
				) );

			translatePanel.Children.Add( UIFactory.TextBlock( "Villain Text" ) );
			translatePanel.Children.Add( UIFactory.TextBox(
				(translatedUI as MissionCardText).villainText,
				new()
				{
					name = "villainText",
					data = (translatedUI as MissionCardText),
				},
				MissionCardTextLostFocus,
				false
				) );

			translatePanel.Children.Add( UIFactory.TextBlock( "Tags" ) );
			for ( int i = 0; i < (translatedUI as MissionCardText).tagsText.Length; i++ )
			{
				translatePanel.Children.Add( UIFactory.TextBox(
					(translatedUI as MissionCardText).tagsText[i],
					new()
					{
						name = "tagsText",
						data = (translatedUI as MissionCardText),
						index = i
					},
					MissionCardTextLostFocus
					) );
			}

			translatePanel.Children.Add( UIFactory.TextBlock( "Expansion Text" ) );
			translatePanel.Children.Add( UIFactory.TextBox(
				(translatedUI as MissionCardText).expansionText,
				new()
				{
					name = "expansionText",
					data = (translatedUI as MissionCardText),
				},
				MissionCardTextLostFocus,
				false
				) );

			translatePanel.Children.Add( UIFactory.TextBlock( "Rebel Reward Text" ) );
			translatePanel.Children.Add( UIFactory.TextBox(
				(translatedUI as MissionCardText).rebelRewardText,
				new()
				{
					name = "rebelRewardText",
					data = (translatedUI as MissionCardText),
				},
				MissionCardTextLostFocus,
				false
				) );

			translatePanel.Children.Add( UIFactory.TextBlock( "Imperial Reward Text" ) );
			translatePanel.Children.Add( UIFactory.TextBox(
				(translatedUI as MissionCardText).imperialRewardText,
				new()
				{
					name = "imperialRewardText",
					data = (translatedUI as MissionCardText),
				},
				MissionCardTextLostFocus,
				false
				) );

			//source side
			sourcePanel.Children.Add( UIFactory.TextBlock( "Name" ) );
			sourcePanel.Children.Add( UIFactory.TextBox( (translatedUI as MissionCardText).name ) );

			sourcePanel.Children.Add( UIFactory.TextBlock( "Description" ) );
			sourcePanel.Children.Add( UIFactory.TextBox( (translatedUI as MissionCardText).descriptionText ) );

			sourcePanel.Children.Add( UIFactory.TextBlock( "Bonus Text" ) );
			sourcePanel.Children.Add( UIFactory.TextBox( (translatedUI as MissionCardText).bonusText ) );

			sourcePanel.Children.Add( UIFactory.TextBlock( "Hero Text" ) );
			sourcePanel.Children.Add( UIFactory.TextBox( (translatedUI as MissionCardText).heroText ) );

			sourcePanel.Children.Add( UIFactory.TextBlock( "Ally Text" ) );
			sourcePanel.Children.Add( UIFactory.TextBox( (translatedUI as MissionCardText).allyText ) );

			sourcePanel.Children.Add( UIFactory.TextBlock( "Villain Text" ) );
			sourcePanel.Children.Add( UIFactory.TextBox( (translatedUI as MissionCardText).villainText ) );

			sourcePanel.Children.Add( UIFactory.TextBlock( "Tags" ) );
			foreach ( var item in (sourceUI as MissionCardText).tagsText )
				sourcePanel.Children.Add( UIFactory.TextBox( item ) );

			sourcePanel.Children.Add( UIFactory.TextBlock( "Expansion Text" ) );
			sourcePanel.Children.Add( UIFactory.TextBox( (translatedUI as MissionCardText).expansionText ) );

			sourcePanel.Children.Add( UIFactory.TextBlock( "Rebel Reward Text" ) );
			sourcePanel.Children.Add( UIFactory.TextBox( (translatedUI as MissionCardText).rebelRewardText ) );

			sourcePanel.Children.Add( UIFactory.TextBlock( "Imperial Reward Text" ) );
			sourcePanel.Children.Add( UIFactory.TextBox( (translatedUI as MissionCardText).imperialRewardText ) );
		}

		void MissionCardTextLostFocus( object sender, RoutedEventArgs e )
		{
			foreach ( var item in translatePanel.Children )
			{
				var ctx = ((FrameworkElement)item).DataContext;
				if ( ctx is DynamicDataContext )
				{
					var dtx = (DynamicDataContext)ctx;
					if ( dtx.name == "name" )
					{
						var bf = dtx.data as MissionCardText;
						bf.name = ((TextBox)item).Text;
					}
					else if ( dtx.name == "descriptionText" )
					{
						var bf = dtx.data as MissionCardText;
						bf.descriptionText = ((TextBox)item).Text;
					}
					else if ( dtx.name == "bonusText" )
					{
						var bf = dtx.data as MissionCardText;
						bf.bonusText = ((TextBox)item).Text;
					}
					else if ( dtx.name == "heroText" )
					{
						var bf = dtx.data as MissionCardText;
						bf.heroText = ((TextBox)item).Text;
					}
					else if ( dtx.name == "allyText" )
					{
						var bf = dtx.data as MissionCardText;
						bf.allyText = ((TextBox)item).Text;
					}
					else if ( dtx.name == "villainText" )
					{
						var bf = dtx.data as MissionCardText;
						bf.villainText = ((TextBox)item).Text;
					}
					else if ( dtx.name == "tagsText" )
					{
						var bf = dtx.data as MissionCardText;
						bf.tagsText[dtx.index] = ((TextBox)item).Text;
					}
					else if ( dtx.name == "expansionText" )
					{
						var bf = dtx.data as MissionCardText;
						bf.expansionText = ((TextBox)item).Text;
					}
					else if ( dtx.name == "rebelRewardText" )
					{
						var bf = dtx.data as MissionCardText;
						bf.rebelRewardText = ((TextBox)item).Text;
					}
					else if ( dtx.name == "imperialRewardText" )
					{
						var bf = dtx.data as MissionCardText;
						bf.imperialRewardText = ((TextBox)item).Text;
					}
				}
			}
		}
	}
}
