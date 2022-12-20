using Imperial_Commander_Editor;

namespace Saga_Translator
{
	public class MissionCardText
	{
		public string name { get; set; }
		public string id { get; set; }
		public string hero { get; set; }
		public string descriptionText { get; set; }
		public string bonusText { get; set; }
		public string heroText { get; set; }
		public string allyText { get; set; }
		public string villainText { get; set; }
		public string expansionText { get; set; }
		public string rebelRewardText { get; set; }
		public string imperialRewardText { get; set; }
		public MissionType[] missionType { get; set; }
		public string[] ally { get; set; }
		public string[] villain { get; set; }
		public string[] tags { get; set; }
		public string[] tagsText { get; set; }
		public int page { get; set; }
		public int influenceCost { get; set; }
		public int[] timePeriod { get; set; }
		public Expansion expansion { get; set; }
	}
}
