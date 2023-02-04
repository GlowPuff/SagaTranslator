using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	public partial class GenericUIPanel
	{
		void HandleInstructions()
		{
			//indicates ONE CardInstruction
			sourceUI = ((List<CardInstruction>)Utils.mainWindow.sourceDynamicUIModel.data)[dContext.arrayIndex];
			translatedUI = ((List<CardInstruction>)Utils.mainWindow.translatedDynamicUIModel.data)[dContext.arrayIndex];

			//instName
			translatePanel.Children.Add( UIFactory.TextBlock( "Name" ) );
			translatePanel.Children.Add( UIFactory.TextBox(
					(translatedUI as CardInstruction).instName,
					new() { name = "name" },
					InstructionsLostFocus
				) );

			//translate side
			for ( int i = 0; i < ((translatedUI as CardInstruction).content).Count; i++ )
			{
				translatePanel.Children.Add( UIFactory.TextBlock( "Instruction Group" ) );

				//content
				int c = 0;
				foreach ( var inst in (translatedUI as CardInstruction).content[i].instruction )
				{
					translatePanel.Children.Add( UIFactory.TextBox(
						inst,
						new()
						{
							name = "content",
							data = (translatedUI as CardInstruction).content[i],
							index = c++
						},
						InstructionsLostFocus
						) );
				}
			}

			//source side
			//instName
			sourcePanel.Children.Add( UIFactory.TextBlock( "Name" ) );
			TextBox tbns = new();
			tbns.Text = (sourceUI as CardInstruction).instName;
			tbns.BorderThickness = new( 2 );
			sourcePanel.Children.Add( tbns );

			foreach ( var item in (sourceUI as CardInstruction).content )
			{
				sourcePanel.Children.Add( UIFactory.TextBlock( "Instruction Group" ) );

				foreach ( var inst in item.instruction )
				{
					sourcePanel.Children.Add( UIFactory.TextBox( inst ) );
				}
			}
		}

		public void InstructionsLostFocus( object sender, RoutedEventArgs e )
		{
			//update translated model
			foreach ( var item in translatePanel.Children )
			{
				var ctx = ((FrameworkElement)item).DataContext;
				if ( ctx is DynamicDataContext )
				{
					var dtx = (DynamicDataContext)ctx;
					if ( dtx.name == "name" )
					{
						(translatedUI as CardInstruction).instName = ((TextBox)item).Text;
					}
					else if ( dtx.name == "content" )
					{
						InstructionOption op = dtx.data as InstructionOption;
						op.instruction[dtx.index] = ((TextBox)item).Text;
					}
				}
			}
		}
	}
}
