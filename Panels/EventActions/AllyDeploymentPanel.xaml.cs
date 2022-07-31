using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for AllyDeploymentPanel.xaml
	/// </summary>
	public partial class AllyDeploymentPanel : UserControl, ITranslationPanel
	{
		public AllyDeployment translated { get; set; }
		public AllyDeployment source { get; set; }

		public AllyDeploymentPanel( IEventAction ea )
		{
			InitializeComponent();
			DataContext = this;

			translated = ea as AllyDeployment;
			source = Utils.mainWindow.sourceMission.GetEAByGUID<AllyDeployment>( ea.GUID );
		}
	}
}
