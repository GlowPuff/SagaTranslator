using Imperial_Commander_Editor;

namespace Saga_Translator
{
	public class CampaignReward
	{
		public string id { get; set; }
		public string name { get; set; }
		public RewardType type { get; set; }
		public string extra { get; set; }

		public CampaignReward()
		{
			extra = "";
		}
	}
}
