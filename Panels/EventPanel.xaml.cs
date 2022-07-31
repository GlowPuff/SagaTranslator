using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for EventPanel.xaml
	/// </summary>
	public partial class EventPanel : UserControl, ITranslationPanel
	{
		public MissionEvent sourceEvent { get; set; }
		public MissionEvent translatedEvent { get; set; }

		/// <summary>
		/// Source event GUID and translated event
		/// </summary>
		public EventPanel( MissionEvent me )
		{
			InitializeComponent();
			DataContext = this;

			translatedEvent = me;
			sourceEvent = Utils.mainWindow.sourceMission.GetEventFromGUID( me.GUID );
		}
	}
}
