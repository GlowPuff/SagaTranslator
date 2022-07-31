using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for ShowTextPanel.xaml
	/// </summary>
	public partial class ShowTextPanel : UserControl, ITranslationPanel
	{
		public ShowTextBox translated { get; set; }
		public ShowTextBox source { get; set; }

		public ShowTextPanel( IEventAction ea )
		{
			InitializeComponent();
			DataContext = this;

			translated = ea as ShowTextBox;
			source = Utils.mainWindow.sourceMission.GetEAByGUID<ShowTextBox>( ea.GUID );
		}
	}
}
