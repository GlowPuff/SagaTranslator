using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Imperial_Commander_Editor;

namespace Saga_Translator
{
	public partial class MainWindow
	{
		public void PopulateMainTree()
		{
			mainTree.Items.Clear();
			TreeViewItem missionProps = new TreeViewItem();
			missionProps.Header = "Mission Properties";
			missionProps.DataContext = translatedMission.missionProperties;
			missionProps.Padding = new Thickness( 3, 3, 3, 3 );
			mainTree.Items.Add( missionProps );

			TreeViewItem missionEvents = new TreeViewItem();
			missionEvents.Header = "Mission Events";
			//EVENTS
			foreach ( var missionEvent in translatedMission.GetAllEvents() )
			{
				if ( missionEvent.GUID != Guid.Empty )
				{
					var tvi = new TreeViewItem();
					tvi.Header = missionEvent.name;
					tvi.DataContext = missionEvent;
					tvi.Padding = new Thickness( 3, 3, 3, 3 );
					//add event actions
					foreach ( var ea in missionEvent.eventActions )
					{
						var tvea = new TreeViewItem();
						tvea.Header = ea.displayName;
						tvea.DataContext = ea;
						tvea.Padding = new Thickness( 3, 3, 3, 3 );
						//filter only translatable event actions
						List<int> filterIn = new( new int[] { 1, 2, 5, 6, 7, 11, 12, 15, 16, 17, 20, 21 } );
						//only add event actions that have translations
						if ( filterIn.Contains( (int)ea.eventActionType ) )
						{
							tvi.Items.Add( tvea );
						}
					}
					missionEvents.Items.Add( tvi );
				}
			}
			mainTree.Items.Add( missionEvents );

			//MAP ENTITIES
			TreeViewItem missionEntities = new TreeViewItem();
			missionEntities.Header = "Map Entities";
			foreach ( var ent in translatedMission.mapEntities )
			{
				if ( ent.entityType == EntityType.Crate
					|| ent.entityType == EntityType.Token
					|| ent.entityType == EntityType.Console
					|| ent.entityType == EntityType.Door
					|| ent.entityType == EntityType.Highlight )
				{
					var tvi = new TreeViewItem();
					tvi.Header = ent.name;
					tvi.DataContext = ent;
					tvi.Padding = new Thickness( 3, 3, 3, 3 );
					missionEntities.Items.Add( tvi );
				}
			}
			mainTree.Items.Add( missionEntities );

			//INITIAL GROUPS
			TreeViewItem initGroups = new TreeViewItem();
			initGroups.Header = "Initial Groups";
			foreach ( var item in translatedMission.initialDeploymentGroups )
			{
				var tvi = new TreeViewItem();
				tvi.Header = item.cardName;
				tvi.DataContext = item;
				tvi.Padding = new Thickness( 3, 3, 3, 3 );
				initGroups.Items.Add( tvi );
			}
			mainTree.Items.Add( initGroups );

			//set first item to mission props
			missionProps.IsSelected = true;

			appModel.NothingSelected = false;
		}

		private void mainTree_SelectedItemChanged( object sender, RoutedPropertyChangedEventArgs<object> e )
		{
			if ( e.NewValue is TreeViewItem )
			{
				if ( ((TreeViewItem)e.NewValue).DataContext is MissionProperties )
				{
					translationObject = new MissionPropsPanel( translatedMission.missionProperties, sourceMission.missionProperties );
				}
				else if ( ((TreeViewItem)e.NewValue).DataContext is MissionEvent )
				{
					translationObject = new EventPanel( ((TreeViewItem)e.NewValue).DataContext as MissionEvent );
				}
				else if ( ((TreeViewItem)e.NewValue).DataContext is IEventAction )
				{
					IEventAction item = (IEventAction)((TreeViewItem)e.NewValue).DataContext;

					if ( item is ChangeMissionInfo )
						translationObject = new ChangeMissionInfoPanel( item );
					else if ( item is ChangeObjective )
						translationObject = new ChangeObjectivePanel( item );
					else if ( item is ShowTextBox )
						translationObject = new ShowTextPanel( item );
					else if ( item is QuestionPrompt )
						translationObject = new PromptPanel( item );
					else if ( item is InputPrompt )
						translationObject = new InputPanel( item );
					else if ( item is EnemyDeployment )
						translationObject = new DeploymentPanel( item );
					else if ( item is CustomEnemyDeployment )
						translationObject = new CustomDeploymentPanel( item );
					else if ( item is ChangeInstructions )
						translationObject = new ChangeInstructionsPanel( item );
					else if ( item is ChangeTarget )
						translationObject = new ChangePriorityPanel( item );
					else if ( item is ChangeReposition )
						translationObject = new ChangeRepositionPanel( item );
					else if ( item is AllyDeployment )
						translationObject = new AllyDeploymentPanel( item );
					else if ( item is ModifyMapEntity )
						translationObject = new ModifyMapEntityPanel( item );
				}
				else if ( ((TreeViewItem)e.NewValue).DataContext is IMapEntity )
				{
					IMapEntity item = (IMapEntity)((TreeViewItem)e.NewValue).DataContext;

					if ( item is Crate )
						translationObject = new CratePanel( item );
					else if ( item is Token )
						translationObject = new TokenPanel( item );
					else if ( item is Imperial_Commander_Editor.Console )
						translationObject = new TerminalPanel( item );
					else if ( item is Door )
						translationObject = new DoorPanel( item );
					else if ( item is SpaceHighlight )
						translationObject = new HighlightPanel( item );
				}
				else if ( ((TreeViewItem)e.NewValue).DataContext is EnemyGroupData )
				{
					EnemyGroupData item = (EnemyGroupData)((TreeViewItem)e.NewValue).DataContext;

					translationObject = new InitGroupPanel( item );
				}
			}
		}
	}
}
