using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for DoorPanel.xaml
	/// </summary>
	public partial class DoorPanel : UserControl, ITranslationPanel
	{
		public Door translated { get; set; }
		public Door source { get; set; }

		public DoorPanel( IMapEntity me )
		{
			InitializeComponent();
			DataContext = this;

			translated = me as Door;
			source = Utils.mainWindow.sourceMission.GetEntityFromGUID<Door>( me.GUID );
		}
	}
}
