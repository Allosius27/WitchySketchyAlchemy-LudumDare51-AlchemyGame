#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using XNode;
using UnityEditor.SceneManagement;
using AllosiusDevCore.QuestSystem;
using AllosiusDevCore.TranslationSystem;

namespace AllosiusDevCore.DialogSystem
{
    [CustomEditor(typeof(DialogueTextNode))]
    public class DialogueTextNodeEditor : Editor
    {
        DialogueTextNode dialogNode;



        void OnEnable()
        {
            dialogNode = (DialogueTextNode)target;
        }
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            //dialogNode.showGeneralProperties = EditorGUILayout.Toggle("Show General Properties", dialogNode.showGeneralProperties);

            dialogNode.graph = (NodeGraph)EditorGUILayout.ObjectField("Graph", dialogNode.graph, typeof(NodeGraph), true);
            dialogNode.position = EditorGUILayout.Vector2Field("Position", dialogNode.position);

            EditorGUILayout.Separator();

            dialogNode.identityType = (DialogueTextNode.IdentityType)EditorGUILayout.EnumPopup("Identity Type", dialogNode.identityType);

            dialogNode.texteType = (TexteType)EditorGUILayout.EnumPopup("Text Type", dialogNode.texteType);

            dialogNode.keyText = EditorGUILayout.TextField("Key Text", dialogNode.keyText);

            //dialogNode.messagePreview = EditorGUILayout.TextField("Message Preview", dialogNode.messagePreview);

            string messagePreview = LocalisationManager.GetLocalisedValue(dialogNode.keyText, TypeDictionary.Dialogues, false);
            dialogNode.messagePreview = EditorGUILayout.TextField("Message Preview", messagePreview);

            dialogNode.singleRead = EditorGUILayout.Toggle("Single Read", dialogNode.singleRead);


            dialogNode.hasEnterNodeActions = EditorGUILayout.Toggle("Has Enter Node Actions", dialogNode.hasEnterNodeActions);

            EditorGUILayout.Separator();

            dialogNode.showEnterNodeActions = EditorGUILayout.Toggle("Show Enter Node Actions", dialogNode.showEnterNodeActions);

            if(dialogNode.showEnterNodeActions)
            {
                ShowActions(dialogNode.enterNodeActions.actionsList);
            }

            EditorGUILayout.Separator();

            dialogNode.hasGameActions = EditorGUILayout.Toggle("Has Game Actions", dialogNode.hasGameActions);

            EditorGUILayout.Separator();

            dialogNode.showGameActions = EditorGUILayout.Toggle("Show Game Actions", dialogNode.showGameActions);

            if (dialogNode.showGameActions)
            {
                ShowActions(dialogNode.gameActions.actionsList);
            }

            EditorGUILayout.Separator();

            dialogNode.hasRequirements = EditorGUILayout.Toggle("Has Requirements", dialogNode.hasRequirements);

            EditorGUILayout.Separator();

            dialogNode.showGameRequirements = EditorGUILayout.Toggle("Show Game Requirements", dialogNode.showGameRequirements);

            if (dialogNode.showGameRequirements)
            {
                List<Requirement> gameRequirements = dialogNode.gameRequirements.requirementsList;
                int size = Mathf.Max(0, EditorGUILayout.IntField("Game Requirements Size :", gameRequirements.Count));

                EditorGUILayout.Separator();

                while (size > gameRequirements.Count)
                {
                    gameRequirements.Add(new Requirement());
                }

                while (size < gameRequirements.Count)
                {
                    gameRequirements.RemoveAt(gameRequirements.Count - 1);
                }

                EditorGUILayout.Separator();

                if (GUILayout.Button("Add New Requirement"))
                {
                    gameRequirements.Add(new Requirement());
                }

                EditorGUILayout.Separator();
                EditorGUILayout.Separator();

                int requirementsCount = 0;
                foreach (Requirement requirement in gameRequirements)
                {
                    requirement.requirementType = (RequirementType)EditorGUILayout.EnumPopup("Requirement Type " + requirementsCount, requirement.requirementType);
                    switch (requirement.requirementType)
                    {
                        case RequirementType.HasQuest:
                            {
                                requirement.questToCheck = (QuestData)EditorGUILayout.ObjectField("Quest To Check", requirement.questToCheck, typeof(QuestData), true);
                                requirement.questCheckedValueWanted = EditorGUILayout.Toggle("Quest Checked Value Wanted", requirement.questCheckedValueWanted);
                                requirement.questToCheckState = (StateQuestWanted)EditorGUILayout.EnumPopup("Quest To Check State", requirement.questToCheckState);
                                break;
                            }
                        case RequirementType.QuestState:
                            {
                                requirement.questAssociatedToCheck = (QuestData)EditorGUILayout.ObjectField("Quest Associated To Check", requirement.questAssociatedToCheck, typeof(QuestData), true);
                                requirement.questStepToCheck = (QuestStepData)EditorGUILayout.ObjectField("Quest Step To Check", requirement.questStepToCheck, typeof(QuestStepData), true);
                                requirement.questStepCheckedValueWanted = EditorGUILayout.Toggle("Quest Step Checked Value Wanted", requirement.questStepCheckedValueWanted);
                                requirement.questStepToCheckState = (StateQuestWanted)EditorGUILayout.EnumPopup("Quest Step To Check State", requirement.questStepToCheckState);
                                break;
                            }

                    }

                    EditorGUILayout.Separator();
                    EditorGUILayout.Separator();

                    if (GUILayout.Button("Remove This Index (" + requirementsCount.ToString() + ")"))
                    {
                        gameRequirements.RemoveAt(requirementsCount);
                        break;
                    }

                    requirementsCount++;

                    EditorGUILayout.Separator();
                    EditorGUILayout.Separator();
                    EditorGUILayout.Separator();
                    EditorGUILayout.Separator();
                    EditorGUILayout.Separator();
                }
            }

            base.serializedObject.ApplyModifiedProperties();

            EditorUtility.SetDirty(dialogNode);

        }

        private void ShowActions(List<GameAction> actions)
        {
            List<GameAction> gameActions = actions;
            int gameActionsSize = Mathf.Max(0, EditorGUILayout.IntField("Game Actions Size :", gameActions.Count));


            EditorGUILayout.Separator();

            while (gameActionsSize > gameActions.Count)
            {
                gameActions.Add(new GameAction());
            }

            while (gameActionsSize < gameActions.Count)
            {
                gameActions.RemoveAt(gameActions.Count - 1);
            }

            EditorGUILayout.Separator();

            if (GUILayout.Button("Add New Game Action"))
            {
                gameActions.Add(new GameAction());
            }

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            int actionsCount = 0;
            foreach (GameAction action in gameActions)
            {
                action.actionType = (ActionType)EditorGUILayout.EnumPopup("Action Type " + actionsCount, action.actionType);

                action.hasCondition = EditorGUILayout.Toggle("Has Condition", action.hasCondition);

                if (action.hasCondition)
                {
                    List<DialogueTextNode> nodesRequired = action.nodesRequiredToRead;
                    int nodesRequiredToReadSize = Mathf.Max(0, EditorGUILayout.IntField("Nodes Required To Read Size :", nodesRequired.Count));

                    EditorGUILayout.Separator();

                    while (nodesRequiredToReadSize > nodesRequired.Count)
                    {
                        nodesRequired.Add(null);
                    }

                    while (nodesRequiredToReadSize < nodesRequired.Count)
                    {
                        nodesRequired.RemoveAt(nodesRequired.Count - 1);
                    }

                    if (GUILayout.Button("Add New Index", GUILayout.MaxWidth(130), GUILayout.MaxHeight(20)))
                    {
                        nodesRequired.Add(null);
                    }

                    for (int i = 0; i < nodesRequired.Count; i++)
                    {
                        nodesRequired[i] = (DialogueTextNode)EditorGUILayout.ObjectField("Dialogue Text Node " + i, nodesRequired[i], typeof(DialogueTextNode), true);

                        if (GUILayout.Button("Remove  (" + i.ToString() + ")", GUILayout.MaxWidth(100), GUILayout.MaxHeight(15)))
                        {
                            nodesRequired.RemoveAt(i);
                            break;
                        }
                    }

                    EditorGUILayout.Separator();
                }

                switch (action.actionType)
                {
                    case ActionType.ReturnMainNodeDialogue:
                        {
                            break;
                        }
                    case ActionType.AddQuest:
                        {
                            action.questToAdd = (QuestData)EditorGUILayout.ObjectField("Quest To Add", action.questToAdd, typeof(QuestData), true);
                            break;
                        }
                    case ActionType.CompleteQuestStep:
                        {
                            action.questAssociated = (QuestData)EditorGUILayout.ObjectField("Quest Associated", action.questAssociated, typeof(QuestData), true);
                            action.questStepToComplete = (QuestStepData)EditorGUILayout.ObjectField("Quest Step To Complete", action.questStepToComplete, typeof(QuestStepData), true);
                            break;
                        }
                    case ActionType.LaunchDialogue:
                        {
                            action.dialogueToLaunch = (DialogueGraph)EditorGUILayout.ObjectField("Dialogue To Launch", action.dialogueToLaunch, typeof(DialogueGraph), true);
                            action.launchDialogueToMainNode = EditorGUILayout.Toggle("Launch Dialogue To Main Node", action.launchDialogueToMainNode);
                            break;
                        }
                    case ActionType.CreateBoxMessage:
                        {
                            action.boxMessageTextToDisplay = EditorGUILayout.TextField("Box Message Text To Display", action.boxMessageTextToDisplay);
                            action.boxMessageSize = EditorGUILayout.FloatField("Box Message Size", action.boxMessageSize);
                            break;
                        }

                }

                EditorGUILayout.Separator();
                EditorGUILayout.Separator();

                if (GUILayout.Button("Remove This Index (" + actionsCount.ToString() + ")"))
                {
                    gameActions.RemoveAt(actionsCount);
                    break;
                }

                actionsCount++;

                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
            }
        }
    }
}
#endif