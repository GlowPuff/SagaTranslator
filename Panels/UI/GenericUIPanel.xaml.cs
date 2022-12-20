using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for GenericUIPanel.xaml
	/// Specific data type (XXX) logic handled in Models/DynamicData/DynXXX.cs
	/// </summary>
	public partial class GenericUIPanel : UserControl, ITranslationPanel
	{
		public dynamic sourceUI { get; set; }
		public dynamic translatedUI { get; set; }

		private DynamicContext dContext;

		public GenericUIPanel( DynamicContext context )
		{
			InitializeComponent();
			DataContext = this;

			dContext = context;
			if ( context.gtype == GenericType.Instructions )
				HandleInstructions();
			else if ( context.gtype == GenericType.BonusEffects )
				HandleBonusEffects();
			else if ( context.gtype == GenericType.CardLanguage )
				HandleCardLanguage();
			else if ( context.gtype == GenericType.MissionCardText )
				HandleMissionCardText();
			else if ( context.gtype == GenericType.MissionRulesInfo )
				HandleInfoRUles();
			else if ( context.gtype == GenericType.CampaignItems )
				HandleCampaignItems();
			else if ( context.gtype == GenericType.CampaignRewards )
				HandleCampaignRewards();
			else if ( context.gtype == GenericType.CampaignSkills )
				HandleCampaignSkills();
			else if ( context.gtype == GenericType.CampaignInfo )
				HandlCampaignInfo();
		}
	}
}
