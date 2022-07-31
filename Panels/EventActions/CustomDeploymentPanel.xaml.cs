using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for CustomDeploymentPanel.xaml
	/// </summary>
	public partial class CustomDeploymentPanel : UserControl, ITranslationPanel
	{
		public CustomEnemyDeployment translated { get; set; }
		public CustomEnemyDeployment source { get; set; }

		public CustomDeploymentPanel( IEventAction ea )
		{
			InitializeComponent();
			DataContext = this;

			translated = ea as CustomEnemyDeployment;
			source = Utils.mainWindow.sourceMission.GetEAByGUID<CustomEnemyDeployment>( ea.GUID );
		}
	}
}
