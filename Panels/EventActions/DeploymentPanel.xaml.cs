using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for DeploymentPanel.xaml
	/// </summary>
	public partial class DeploymentPanel : UserControl, ITranslationPanel
	{
		public EnemyDeployment translated { get; set; }
		public EnemyDeployment source { get; set; }

		public DeploymentPanel( IEventAction ea )
		{
			InitializeComponent();
			DataContext = this;

			translated = ea as EnemyDeployment;
			source = Utils.mainWindow.sourceMission.GetEAByGUID<EnemyDeployment>( ea.GUID );
		}
	}
}
