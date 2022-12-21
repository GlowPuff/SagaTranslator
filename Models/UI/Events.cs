using System.Collections.Generic;

namespace Saga_Translator
{
	public class EventList
	{
		public List<CardEvent> events { get; set; }
	}

	public class CardEvent
	{
		public string eventName { get; set; }
		public string eventID { get; set; }
		public string eventFlavor { get; set; }
		public string eventRule { get; set; }
		public List<string> content { get; set; }
	}
}
