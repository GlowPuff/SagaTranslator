using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	public partial class GenericUIPanel
	{
		void HandleCardLanguage()
		{
			//indicates ONE CardLanguage
			sourceUI = ((List<CardLanguage>)Utils.mainWindow.sourceDynamicUIModel.data)[dContext.arrayIndex];
			translatedUI = ((List<CardLanguage>)Utils.mainWindow.translatedDynamicUIModel.data)[dContext.arrayIndex];

			//translate side
			translatePanel.Children.Add( UIFactory.TextBlock( "Name" ) );
			translatePanel.Children.Add( UIFactory.TextBox(
				(translatedUI as CardLanguage).name,
				new()
				{
					name = "name",
					data = (translatedUI as CardLanguage),
				},
				CardLanguageLostFocus,
				false
				) );

			translatePanel.Children.Add( UIFactory.TextBlock( "Subname" ) );
			translatePanel.Children.Add( UIFactory.TextBox(
				(translatedUI as CardLanguage).subname,
				new()
				{
					name = "subname",
					data = (translatedUI as CardLanguage),
				},
				CardLanguageLostFocus,
				false
				) );

			translatePanel.Children.Add( UIFactory.TextBlock( "Traits" ) );
			for ( int i = 0; i < (translatedUI as CardLanguage).traits.Length; i++ )
			{
				translatePanel.Children.Add( UIFactory.TextBox(
					(translatedUI as CardLanguage).traits[i],
					new()
					{
						name = "trait",
						data = (translatedUI as CardLanguage),
						index = i
					},
					CardLanguageLostFocus
					) );
			}

			translatePanel.Children.Add( UIFactory.TextBlock( "Keywords" ) );
			for ( int i = 0; i < (translatedUI as CardLanguage).keywords.Length; i++ )
			{
				translatePanel.Children.Add( UIFactory.TextBox(
					(translatedUI as CardLanguage).keywords[i],
					new()
					{
						name = "keyword",
						data = (translatedUI as CardLanguage),
						index = i
					},
					CardLanguageLostFocus
					) );
			}

			translatePanel.Children.Add( UIFactory.TextBlock( "Surges" ) );
			for ( int i = 0; i < (translatedUI as CardLanguage).surges.Length; i++ )
			{
				translatePanel.Children.Add( UIFactory.TextBox(
					(translatedUI as CardLanguage).surges[i],
					new()
					{
						name = "surge",
						data = (translatedUI as CardLanguage),
						index = i
					},
					CardLanguageLostFocus
					) );
			}

			for ( int i = 0; i < (translatedUI as CardLanguage).abilities.Length; i++ )
			{
				translatePanel.Children.Add( UIFactory.TextBlock( $"Ability {i + 1}" ) );

				translatePanel.Children.Add( UIFactory.TextBox(
					(translatedUI as CardLanguage).abilities[i].name,
					new()
					{
						name = "abilityname",
						data = (translatedUI as CardLanguage).abilities[i],
						index = i
					},
					CardLanguageLostFocus
					) );
				translatePanel.Children.Add( UIFactory.TextBox(
					(translatedUI as CardLanguage).abilities[i].text,
					new()
					{
						name = "abilitytext",
						data = (translatedUI as CardLanguage).abilities[i],
						index = i
					},
					CardLanguageLostFocus
					) );
			}

			//source side
			sourcePanel.Children.Add( UIFactory.TextBlock( "Name" ) );
			sourcePanel.Children.Add( UIFactory.TextBox( (sourceUI as CardLanguage).name ) );

			sourcePanel.Children.Add( UIFactory.TextBlock( "Subname" ) );
			sourcePanel.Children.Add( UIFactory.TextBox( (sourceUI as CardLanguage).subname ) );

			sourcePanel.Children.Add( UIFactory.TextBlock( "Traits" ) );
			foreach ( var item in (sourceUI as CardLanguage).traits )
				sourcePanel.Children.Add( UIFactory.TextBox( item ) );

			sourcePanel.Children.Add( UIFactory.TextBlock( "Keywords" ) );
			foreach ( var item in (sourceUI as CardLanguage).keywords )
				sourcePanel.Children.Add( UIFactory.TextBox( item ) );

			sourcePanel.Children.Add( UIFactory.TextBlock( "Surges" ) );
			foreach ( var item in (sourceUI as CardLanguage).surges )
				sourcePanel.Children.Add( UIFactory.TextBox( item ) );

			foreach ( var item in (sourceUI as CardLanguage).abilities )
			{
				sourcePanel.Children.Add( UIFactory.TextBlock( "Ability" ) );
				sourcePanel.Children.Add( UIFactory.TextBox( item.name ) );
				sourcePanel.Children.Add( UIFactory.TextBox( item.text ) );
			}
		}

		void CardLanguageLostFocus( object sender, RoutedEventArgs e )
		{
			foreach ( var item in translatePanel.Children )
			{
				var ctx = ((FrameworkElement)item).DataContext;
				if ( ctx is DynamicDataContext )
				{
					var dtx = (DynamicDataContext)ctx;
					if ( dtx.name == "name" )
					{
						var bf = dtx.data as CardLanguage;
						bf.name = ((TextBox)item).Text;
					}
					else if ( dtx.name == "subname" )
					{
						var bf = dtx.data as CardLanguage;
						bf.subname = ((TextBox)item).Text;
					}
					else if ( dtx.name == "trait" )
					{
						var bf = dtx.data as CardLanguage;
						bf.traits[dtx.index] = ((TextBox)item).Text;
					}
					else if ( dtx.name == "keyword" )
					{
						var bf = dtx.data as CardLanguage;
						bf.keywords[dtx.index] = ((TextBox)item).Text;
					}
					else if ( dtx.name == "surge" )
					{
						var bf = dtx.data as CardLanguage;
						bf.surges[dtx.index] = ((TextBox)item).Text;
					}
					else if ( dtx.name == "abilityname" )
					{
						var bf = dtx.data as GroupAbility;
						bf.name = ((TextBox)item).Text;
					}
					else if ( dtx.name == "abilitytext" )
					{
						var bf = dtx.data as GroupAbility;
						bf.text = ((TextBox)item).Text;
					}
				}
			}
		}
	}
}
