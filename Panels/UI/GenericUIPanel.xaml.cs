using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for GenericUIPanel.xaml
	/// </summary>
	public partial class GenericUIPanel : UserControl, ITranslationPanel
	{
		public dynamic sourceUI { get; set; }
		public dynamic translatedUI { get; set; }

		private DynamicContext dContext;

		public GenericUIPanel( DynamicContext context )
		{
			InitializeComponent();
			DataContext = this;

			dContext = context;
			if ( context.gtype == GenericType.Instructions )
				HandleInstructions();
		}

		void HandleInstructions()
		{
			sourceUI = ((List<CardInstruction>)Utils.mainWindow.sourceDynamicUIModel.data)[dContext.arrayIndex];
			translatedUI = ((List<CardInstruction>)Utils.mainWindow.translatedDynamicUIModel.data)[dContext.arrayIndex];

			//translate side
			for ( int i = 0; i < ((translatedUI as CardInstruction).content).Count; i++ )
			{
				TextBlock header = new();
				header.Text = "Instruction Group";
				header.Margin = new( 0, 10, 0, 5 );
				translatePanel.Children.Add( header );

				foreach ( var inst in ((translatedUI as CardInstruction).content)[i].instruction )
				{
					TextBox tb = new();
					tb.Text = inst;
					tb.BorderThickness = new( 2 );
					tb.Style = FindResource( "multi" ) as Style;
					tb.LostFocus += UILostFocus;
					tb.DataContext = ((translatedUI as CardInstruction).content)[i];
					tb.Tag = i;
					translatePanel.Children.Add( tb );
				}
			}

			//source side
			foreach ( var item in (sourceUI as CardInstruction).content )
			{
				TextBlock header = new();
				header.Text = "Instruction Group";
				header.Margin = new( 0, 10, 0, 5 );
				sourcePanel.Children.Add( header );

				foreach ( var inst in item.instruction )
				{
					TextBox tb = new();
					tb.Text = inst;
					tb.BorderThickness = new( 2 );
					tb.Style = FindResource( "multi" ) as Style;
					sourcePanel.Children.Add( tb );
				}
			}
		}

		private void UILostFocus( object sender, RoutedEventArgs e )
		{
			Utils.Log( "CHANGED" );
			//update translated model
			if ( dContext.gtype == GenericType.Instructions )
			{
				foreach ( var item in translatePanel.Children )
				{
					if ( item is TextBox )
					{
						InstructionOption op = ((FrameworkElement)item).DataContext as InstructionOption;
						int index = (int)((FrameworkElement)item).Tag;
						op.instruction[index] = ((TextBox)item).Text;
					}
				}
			}
		}
	}
}
