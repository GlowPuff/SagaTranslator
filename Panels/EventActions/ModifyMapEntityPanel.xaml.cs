using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for ModifyMapEntityPanel.xaml
	/// </summary>
	public partial class ModifyMapEntityPanel : UserControl, ITranslationPanel
	{
		public ModifyMapEntity translated { get; set; }
		public ModifyMapEntity source { get; set; }

		public ModifyMapEntityPanel( IEventAction ea )
		{
			InitializeComponent();
			DataContext = this;

			translated = ea as ModifyMapEntity;
			source = Utils.mainWindow.sourceMission.GetEAByGUID<ModifyMapEntity>( ea.GUID );
		}
	}
}
