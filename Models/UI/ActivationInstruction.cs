using System.Collections.Generic;

namespace Saga_Translator
{
	public class CardInstruction
	{
		public string instName { get; set; }
		public string instID { get; set; }
		public List<InstructionOption> content { get; set; }
	}

	public class InstructionOption
	{
		public List<string> instruction { get; set; }
	}
}