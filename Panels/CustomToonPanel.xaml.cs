using System.Linq;
using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for CustomToonPanel.xaml
	/// </summary>
	public partial class CustomToonPanel : UserControl, ITranslationPanel
	{
		public CustomToon translated { get; set; }
		public CustomToon source { get; set; }

		public CustomToonPanel( CustomToon data )
		{
			InitializeComponent();
			DataContext = this;

			translated = data as CustomToon;
			source = Utils.mainWindow.sourceMission.customCharacters.Where( x => x.customCharacterGUID == data.customCharacterGUID ).First();
		}
	}
}
