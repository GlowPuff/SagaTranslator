using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for ChangeMissionInfoPanel.xaml
	/// </summary>
	public partial class ChangeMissionInfoPanel : UserControl, ITranslationPanel
	{
		public ChangeMissionInfo source { get; set; }
		public ChangeMissionInfo translated { get; set; }

		public ChangeMissionInfoPanel( IEventAction ea )
		{
			InitializeComponent();
			DataContext = this;

			translated = ea as ChangeMissionInfo;
			source = Utils.mainWindow.sourceMission.GetEAByGUID<ChangeMissionInfo>( ea.GUID );
		}
	}
}
