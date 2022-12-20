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

		public void PopulateUIMainTree()
		{
			mainTree.Items.Clear();

			TreeViewItem uiItem = new TreeViewItem();
			uiItem.Header = "Settings";
			uiItem.DataContext = translatedUI.uiSettings;
			uiItem.Padding = new Thickness( 3, 3, 3, 3 );
			mainTree.Items.Add( uiItem );

			uiItem = new TreeViewItem();
			uiItem.Header = "Title Screen";
			uiItem.DataContext = translatedUI.uiTitle;
			uiItem.Padding = new Thickness( 3, 3, 3, 3 );
			mainTree.Items.Add( uiItem );

			uiItem = new TreeViewItem();
			uiItem.Header = "Saga Setup Screen";
			uiItem.DataContext = translatedUI.sagaUISetup;
			uiItem.Padding = new Thickness( 3, 3, 3, 3 );
			mainTree.Items.Add( uiItem );

			uiItem = new TreeViewItem();
			uiItem.Header = "Expansions";
			uiItem.DataContext = translatedUI.uiExpansions;
			uiItem.Padding = new Thickness( 3, 3, 3, 3 );
			mainTree.Items.Add( uiItem );

			uiItem = new TreeViewItem();
			uiItem.Header = "Saga Main App";
			uiItem.DataContext = translatedUI.sagaMainApp;
			uiItem.Padding = new Thickness( 3, 3, 3, 3 );
			mainTree.Items.Add( uiItem );

			uiItem = new TreeViewItem();
			uiItem.Header = "Setup Screen";
			uiItem.DataContext = translatedUI.uiSetup;
			uiItem.Padding = new Thickness( 3, 3, 3, 3 );
			mainTree.Items.Add( uiItem );

			uiItem = new TreeViewItem();
			uiItem.Header = "Campaign Screen";
			uiItem.DataContext = translatedUI.uiCampaign;
			uiItem.Padding = new Thickness( 3, 3, 3, 3 );
			mainTree.Items.Add( uiItem );

			uiItem = new TreeViewItem();
			uiItem.Header = "Classic Main App";
			uiItem.DataContext = translatedUI.uiMainApp;
			uiItem.Padding = new Thickness( 3, 3, 3, 3 );
			mainTree.Items.Add( uiItem );
		}

		public void PopulateDynamicTree( GenericType gtype )
		{
			mainTree.Items.Clear();
			TreeViewItem uiRoot = new TreeViewItem();
			uiRoot.Padding = new Thickness( 3, 3, 3, 3 );
			mainTree.Items.Add( uiRoot );

			if ( gtype == GenericType.Instructions )
			{
				IoCList<CardInstruction>( gtype, ( uiItem, index ) =>
				{
					uiRoot.Header = "Instructions";
					uiItem.Header = (sourceDynamicUIModel.data as List<CardInstruction>)[index].instName;
					uiRoot.Items.Add( uiItem );
				} );
			}
			else if ( gtype == GenericType.BonusEffects )
			{
				IoCList<BonusEffect>( gtype, ( uiItem, index ) =>
				{
					uiRoot.Header = "Bonus Effects";
					uiItem.Header = (sourceDynamicUIModel.data as List<BonusEffect>)[index].bonusID;
					uiRoot.Items.Add( uiItem );
				} );
			}
			else if ( gtype == GenericType.CardLanguage )
			{
				IoCList<CardLanguage>( gtype, ( uiItem, index ) =>
				{
					uiRoot.Header = "Deployment Groups";
					uiItem.Header = (sourceDynamicUIModel.data as List<CardLanguage>)[index].id;
					uiRoot.Items.Add( uiItem );
				} );
			}
			else if ( gtype == GenericType.MissionCardText )
			{
				IoCList<MissionCardText>( gtype, ( uiItem, index ) =>
				{
					uiRoot.Header = "Mission Card Text";
					uiItem.Header = (sourceDynamicUIModel.data as List<MissionCardText>)[index].id;
					uiRoot.Items.Add( uiItem );
				} );
			}
			else if ( gtype == GenericType.MissionRulesInfo )
			{
				IoC<string>( gtype, ( uiItem ) =>
				{
					uiRoot.Header = "Mission Rules/Info Text";
					uiItem.Header = "Text";
					uiRoot.Items.Add( uiItem );
				} );
			}
			else if ( gtype == GenericType.CampaignItems )
			{
				IoCList<CampaignItem>( gtype, ( uiItem, index ) =>
				{
					uiRoot.Header = "Campaign Items";
					uiItem.Header = (sourceDynamicUIModel.data as List<CampaignItem>)[index].id;
					uiRoot.Items.Add( uiItem );
				} );
			}
			else if ( gtype == GenericType.CampaignRewards )
			{
				IoCList<CampaignReward>( gtype, ( uiItem, index ) =>
				{
					uiRoot.Header = "Campaign Rewards";
					uiItem.Header = (sourceDynamicUIModel.data as List<CampaignReward>)[index].id;
					uiRoot.Items.Add( uiItem );
				} );
			}
			else if ( gtype == GenericType.CampaignSkills )
			{
				IoCList<CampaignSkill>( gtype, ( uiItem, index ) =>
				{
					uiRoot.Header = "Campaign Skills";
					uiItem.Header = (sourceDynamicUIModel.data as List<CampaignSkill>)[index].id;
					uiRoot.Items.Add( uiItem );
				} );
			}
			else if ( gtype == GenericType.CampaignInfo )
			{
				IoC<CampaignSkill>( gtype, ( uiItem ) =>
				{
					uiRoot.Header = "Campaign Info";
					uiItem.Header = "Text";
					uiRoot.Items.Add( uiItem );
				} );
			}
		}

		private void IoC<T>( GenericType gtype, Action<TreeViewItem> ioc )
		{
			TreeViewItem uiItem = new TreeViewItem();
			uiItem.DataContext = new DynamicContext()
			{
				gtype = gtype
			};
			uiItem.Padding = new Thickness( 3, 3, 3, 3 );
			ioc( uiItem );
		}

		private void IoCList<T>( GenericType gtype, Action<TreeViewItem, int> ioc )
		{
			for ( int i = 0; i < (sourceDynamicUIModel.data as List<T>).Count; i++ )
			{
				TreeViewItem uiItem = new TreeViewItem();
				uiItem.DataContext = new DynamicContext()
				{
					arrayIndex = i,
					gtype = gtype
				};
				uiItem.Padding = new Thickness( 3, 3, 3, 3 );
				ioc( uiItem, i );
			}
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

				//UI
				else if ( ((TreeViewItem)e.NewValue).DataContext is UITitle )
				{
					translationObject = new UITitlePanel( ((TreeViewItem)e.NewValue).DataContext as UITitle );
				}
				else if ( ((TreeViewItem)e.NewValue).DataContext is UISettings )
				{
					translationObject = new UISettingsPanel( ((TreeViewItem)e.NewValue).DataContext as UISettings );
				}
				else if ( ((TreeViewItem)e.NewValue).DataContext is SagaUISetup )
				{
					translationObject = new UISagaSetupPanel( ((TreeViewItem)e.NewValue).DataContext as SagaUISetup );
				}
				else if ( ((TreeViewItem)e.NewValue).DataContext is UIExpansions )
				{
					translationObject = new UIExpansionsPanel( ((TreeViewItem)e.NewValue).DataContext as UIExpansions );
				}
				else if ( ((TreeViewItem)e.NewValue).DataContext is SagaMainApp )
				{
					translationObject = new UISagaMainAppPanel( ((TreeViewItem)e.NewValue).DataContext as SagaMainApp );
				}
				else if ( ((TreeViewItem)e.NewValue).DataContext is UISetup )
				{
					translationObject = new UISetupPanel( ((TreeViewItem)e.NewValue).DataContext as UISetup );
				}
				else if ( ((TreeViewItem)e.NewValue).DataContext is UICampaign )
				{
					translationObject = new UICampaignPanel( ((TreeViewItem)e.NewValue).DataContext as UICampaign );
				}
				else if ( ((TreeViewItem)e.NewValue).DataContext is UIMainApp )
				{
					translationObject = new UIMainAppPanel( ((TreeViewItem)e.NewValue).DataContext as UIMainApp );
				}

				//DYNAMIC UI DATA
				else if ( ((TreeViewItem)e.NewValue).DataContext is DynamicContext )
				{
					translationObject = new GenericUIPanel( ((TreeViewItem)e.NewValue).DataContext as DynamicContext );
				}
			}
		}
	}
}
