using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for WelcomePanel.xaml
	/// </summary>
	public partial class WelcomePanel : UserControl, ITranslationPanel
	{
		public WelcomePanel()
		{
			InitializeComponent();

			MainWindow window = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
			if ( window != null )
			{
				if ( window.mainTree.Items.Count > 0 )
					EnableTranslationDrop();
			}
		}

		private void dragSourceBox_DragEnter( object sender, DragEventArgs e )
		{
			e.Effects = DragDropEffects.None;

			if ( e.Data.GetDataPresent( DataFormats.FileDrop ) )
			{
				string[] filename = e.Data.GetData( DataFormats.FileDrop ) as string[];
				if ( Path.GetExtension( filename[0] ) == ".json" )
					e.Effects = DragDropEffects.All;
			}
		}

		private void dragSourceBox_Drop( object sender, DragEventArgs e )
		{
			if ( e.Data.GetDataPresent( DataFormats.FileDrop ) )
			{
				//grab the filename
				string[] filename = e.Data.GetData( DataFormats.FileDrop ) as string[];
				if ( filename.Length == 1 )
				{
					MainWindow window = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
					if ( window != null && Path.GetExtension( filename[0] ) == ".json" )
					{
						if ( window.OpenSourceFile( filename[0] ) )
						{
							dragTranslatedBox.AllowDrop = true;
							dragTranslatedBox.Opacity = 1;
						}
					}
				}
			}
		}

		private void dragTranslatedBox_DragEnter( object sender, DragEventArgs e )
		{
			e.Effects = DragDropEffects.None;

			if ( e.Data.GetDataPresent( DataFormats.FileDrop ) )
			{
				string[] filename = e.Data.GetData( DataFormats.FileDrop ) as string[];
				if ( Path.GetExtension( filename[0] ) == ".json" )
					e.Effects = DragDropEffects.All;
			}
		}

		private void dragTranslatedBox_Drop( object sender, DragEventArgs e )
		{
			if ( e.Data.GetDataPresent( DataFormats.FileDrop ) )
			{
				//grab the filename
				string[] filename = e.Data.GetData( DataFormats.FileDrop ) as string[];
				if ( filename.Length == 1 )
				{
					MainWindow window = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
					if ( window != null && Path.GetExtension( filename[0] ) == ".json" )
					{
						window.OpenTranslatedFile( filename[0] );
					}
				}
			}
		}

		public void EnableTranslationDrop()
		{
			dragTranslatedBox.AllowDrop = true;
			dragTranslatedBox.Opacity = 1;
		}
	}
}
