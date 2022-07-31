using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	/// <summary>
	/// Interaction logic for InitGroupPanel.xaml
	/// </summary>
	public partial class InitGroupPanel : UserControl, ITranslationPanel
	{
		public EnemyGroupData translated { get; set; }
		public EnemyGroupData source { get; set; }

		public InitGroupPanel( EnemyGroupData data )
		{
			InitializeComponent();
			DataContext = this;

			translated = data as EnemyGroupData;
			source = Utils.mainWindow.sourceMission.initialDeploymentGroups.Where( x => x.GUID == data.GUID ).First();
		}

		public int FindReplace( string needle, string replace )
		{
			int count = 0;
			if ( !string.IsNullOrEmpty( translated.customText ) )
			{
				var regex = new Regex( needle );
				count += regex.Matches( translated.customText ).Count;
				foreach ( var match in regex.Matches( translated.customText ) )
				{
					translated.customText = translated.customText.Replace( match.ToString(), replace );
				}
			}
			return count;
		}
	}
}
