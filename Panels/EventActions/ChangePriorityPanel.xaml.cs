using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for ChangePriorityPanel.xaml
	/// </summary>
	public partial class ChangePriorityPanel : UserControl, ITranslationPanel
	{
		public ChangeTarget translated { get; set; }
		public ChangeTarget source { get; set; }

		public ChangePriorityPanel( IEventAction ea )
		{
			InitializeComponent();
			DataContext = this;

			translated = ea as ChangeTarget;
			source = Utils.mainWindow.sourceMission.GetEAByGUID<ChangeTarget>( ea.GUID );
		}
	}
}
