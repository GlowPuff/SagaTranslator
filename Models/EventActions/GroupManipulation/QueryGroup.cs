using System;

namespace Imperial_Commander_Editor
{
	public class QueryGroup : EventAction
	{
		public DCPointer groupEnemyToQuery { get; set; } = null;
		public DCPointer groupRebelToQuery { get; set; } = null;
		public Guid foundTrigger;
		public Guid foundEvent;

		public QueryGroup()
		{

		}
	}
}
