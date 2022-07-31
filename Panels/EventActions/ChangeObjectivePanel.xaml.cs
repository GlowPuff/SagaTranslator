using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for ChangeObjectivePanel.xaml
	/// </summary>
	public partial class ChangeObjectivePanel : UserControl, ITranslationPanel
	{
		public ChangeObjective translated { get; set; }
		public ChangeObjective source { get; set; }

		public ChangeObjectivePanel( IEventAction ea )
		{
			InitializeComponent();
			DataContext = this;

			translated = ea as ChangeObjective;
			source = Utils.mainWindow.sourceMission.GetEAByGUID<ChangeObjective>( ea.GUID );
		}
	}
}
