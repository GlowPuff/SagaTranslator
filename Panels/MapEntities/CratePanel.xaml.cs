using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for CratePanel.xaml
	/// </summary>
	public partial class CratePanel : UserControl, ITranslationPanel
	{
		public Crate translated { get; set; }
		public Crate source { get; set; }

		public CratePanel( IMapEntity me )
		{
			InitializeComponent();
			DataContext = this;

			translated = me as Crate;
			source = Utils.mainWindow.sourceMission.GetEntityFromGUID<Crate>( me.GUID );
		}
	}
}
