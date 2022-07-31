using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for MissionPropsPanel.xaml
	/// </summary>
	public partial class MissionPropsPanel : UserControl, ITranslationPanel
	{
		public MissionProperties sourceProps { get; set; }
		public MissionProperties translatedProps { get; set; }

		public MissionPropsPanel( MissionProperties props, MissionProperties sProps )
		{
			InitializeComponent();
			DataContext = this;

			translatedProps = props;
			sourceProps = sProps;
		}

		private void TextBox_KeyDown( object sender, System.Windows.Input.KeyEventArgs e )
		{
			if ( e.Key == System.Windows.Input.Key.Enter )
				Utils.LoseFocus( sender as Control );
		}
	}
}
