namespace Imperial_Commander_Editor
{
	///enums <summary>
	public enum GenericType { Instructions, BonusEffects, CardLanguage, MissionCardText, MissionRulesInfo, CampaignItems, CampaignRewards, CampaignSkills, CampaignInfo, CardEvent }
	public enum CustomInstructionType { Top, Bottom, Replace }
	public enum ThreatModifierType { None, Fixed, Multiple }
	public enum YesNoAll { Yes, No, All, Multi }
	public enum PriorityTargetType { Rebel, Hero, Ally, Other, Trait }
	public enum Expansion { Core, Twin, Hoth, Bespin, Jabba, Empire, Lothal }
	public enum EntityType { Tile, Console, Crate, DeploymentPoint, Token, Highlight, Door }
	public enum TokenShape { Circle, Square, Rectangle }
	public enum EventActionType { G1, G2, G3, G4, G5, G6, D1, D2, D3, D4, D5, GM1, GM2, GM3, M1, M2, G7, GM4, GM5, G8, G9, D6, GM6 }
	public enum ThreatAction { Add, Remove }
	public enum DeploymentSpot { Active, Specific }
	public enum GroupType { All, Specific }
	public enum MarkerType { Neutral, Rebel, Imperial }
	public enum MissionType { Story, Side, Forced }
	public enum MissionSubType { Agenda, Threat, Other, Finale, General, Personal, Villain, Ally }
	public enum DiceColor { White, Black, Yellow, Red, Green, Blue, Grey }
	public enum AttackType { Ranged, Melee, None }
	public enum FigureSize { Small1x1, Medium1x2, Large2x2, Huge2x3 }
	public enum GroupTraits { Trooper, Leader, HeavyWeapon, Guardian, Brawler, Droid, Vehicle, Hunter, Creature, Smuggler, Spy, ForceUser, Wookiee, Hero }
	public enum RewardType { Campaign, General, HeroNumber, Personal }
}

public enum TranslateMode { Mission, UI, Supplemental, Cancel }