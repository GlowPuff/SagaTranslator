using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for ChangeRepositionPanel.xaml
	/// </summary>
	public partial class ChangeRepositionPanel : UserControl, ITranslationPanel
	{
		public ChangeReposition translated { get; set; }
		public ChangeReposition source { get; set; }

		public ChangeRepositionPanel( IEventAction ea )
		{
			InitializeComponent();
			DataContext = this;

			translated = ea as ChangeReposition;
			source = Utils.mainWindow.sourceMission.GetEAByGUID<ChangeReposition>( ea.GUID );
		}
	}
}
