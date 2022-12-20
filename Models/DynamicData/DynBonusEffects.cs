using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	public partial class GenericUIPanel
	{
		void HandleBonusEffects()
		{
			//indicates ONE BonusEffect
			sourceUI = ((List<BonusEffect>)Utils.mainWindow.sourceDynamicUIModel.data)[dContext.arrayIndex];
			translatedUI = ((List<BonusEffect>)Utils.mainWindow.translatedDynamicUIModel.data)[dContext.arrayIndex];

			//translate side
			for ( int i = 0; i < (translatedUI as BonusEffect).effects.Count; i++ )
			{
				translatePanel.Children.Add( UIFactory.TextBox(
					(translatedUI as BonusEffect).effects[i],
					new()
					{
						name = "effect",
						data = (translatedUI as BonusEffect).effects,
						index = i
					},
					BonusEffectsLostFocus
					) );
			}

			//source side
			foreach ( var item in (sourceUI as BonusEffect).effects )
			{
				sourcePanel.Children.Add( UIFactory.TextBox( item ) );
			}
		}

		void BonusEffectsLostFocus( object sender, RoutedEventArgs e )
		{
			foreach ( var item in translatePanel.Children )
			{
				var ctx = ((FrameworkElement)item).DataContext;
				if ( ctx is DynamicDataContext )
				{
					var dtx = (DynamicDataContext)ctx;
					if ( dtx.name == "effect" )
					{
						var bf = dtx.data as List<string>;
						bf[dtx.index] = ((TextBox)item).Text;
					}
				}
			}
		}
	}
}
