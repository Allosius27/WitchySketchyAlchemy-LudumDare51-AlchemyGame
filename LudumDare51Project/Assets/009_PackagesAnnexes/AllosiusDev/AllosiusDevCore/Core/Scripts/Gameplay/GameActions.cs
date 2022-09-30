using AllosiusDevCore.DialogSystem;
using AllosiusDevCore.QuestSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace AllosiusDevCore
{
    [System.Serializable]
    public class GameActions
    {
        #region UnityInspector

        [SerializeField] public List<GameAction> actionsList = new List<GameAction>();

        #endregion

        #region Behaviour

        public void ExecuteGameActions()
        {
            if (actionsList.Count > 0)
            {
                /*if(GameManager.Instance.PlayerConversant.currentNode != null)
                    GameManager.Instance.PlayerConversant.currentNode.timerBeforeNextNode = 0.0f;*/
                
                DialogueTextNode newNode = null;

                foreach (var item in actionsList)
                {
                    if (item.CheckCondition())
                    {
                        if (item.actionType == ActionType.ReturnMainNodeDialogue)
                        {
                            //newNode = (DialogueTextNode)item.ExecuteReturnMainNodeDialogueAction(GameManager.Instance.PlayerConversant.CurrentDialog);
                        }
                        else if (item.actionType == ActionType.AddQuest)
                        {
                            item.ExecuteAddQuest(GameManager.Instance.QuestManager);
                        }
                        else if (item.actionType == ActionType.CompleteQuestStep)
                        {
                            item.ExecuteCompleteQuestStep(GameManager.Instance.QuestManager);
                        }
                        else if (item.actionType == ActionType.LaunchDialogue)
                        {
                            //item.ExecuteLaunchDialogue();
                        }
                        else if (item.actionType == ActionType.CreateBoxMessage)
                        {
                            //item.ExecuteCreateBoxMessage();
                        }
                    }
                }

                if (newNode != null)
                {
                    //GameManager.Instance.PlayerConversant.currentNode = newNode;
                }
            }
        }

        #endregion
    }
}

namespace AllosiusDevCore
{
    [System.Serializable]
    public class GameAction
    {
        #region UnityInspector

        [Header("Generals Properties")]
        [SerializeField] public ActionType actionType;

        [SerializeField] public bool hasCondition;
        [SerializeField] public List<DialogueTextNode> nodesRequiredToRead = new List<DialogueTextNode>();

        [Space]
        [Header("Add Quest Properties")]
        [SerializeField] public QuestData questToAdd;

        [Space]
        [Header("Complete Quest Step Properties")]
        [SerializeField] public QuestData questAssociated;
        [SerializeField] public QuestStepData questStepToComplete;

        [Space]
        [Header("Launch Dialogue Properties")]
        //[SerializeField] public NpcConversant newConversantToCall;
        [SerializeField] public DialogueGraph dialogueToLaunch;
        [SerializeField] public bool launchDialogueToMainNode;

        [Space]
        [Header("Create Box Message Properties")]
        [SerializeField] public string boxMessageTextToDisplay;
        [SerializeField] public float boxMessageSize;

        #endregion

        #region Behaviour

        public bool CheckCondition()
        {
            if (hasCondition)
            {
                if (nodesRequiredToRead.Count > 0)
                {
                    for (int i = 0; i < nodesRequiredToRead.Count; i++)
                    {
                        if (nodesRequiredToRead[i].GetAlreadyRead() == false)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /*public Node ExecuteReturnMainNodeDialogueAction(DialogueGraph dialogueGraph)
        {
            GameManager.Instance.PlayerConversant.currentNode.SetAlreadyReadValue(true);
            return dialogueGraph.mainNodeParent;
        }*/

        public void ExecuteAddQuest(QuestList questList)
        {
            questList.AddQuest(questToAdd);
        }

        public void ExecuteCompleteQuestStep(QuestList questList)
        {
            questList.CompleteQuestStep(questAssociated, questStepToComplete);
        }

        /*public void ExecuteLaunchDialogue()
        {
            NpcConversant conversant = null;
            if (newConversantToCall != null)
            {
                conversant = newConversantToCall;
            }
            if (conversant == null)
            {
                conversant = GameManager.Instance.PlayerConversant.CurrentConversant;
            }

            if (GameManager.Instance.PlayerConversant.CurrentDialog != null)
            {
                GameManager.Instance.PlayerConversant.Quit();
            }
            GameManager.Instance.PlayerConversant.StartDialog(conversant, dialogueToLaunch);

            if(launchDialogueToMainNode)
            {
                DialogueTextNode newNode = (DialogueTextNode)ExecuteReturnMainNodeDialogueAction(GameManager.Instance.PlayerConversant.CurrentDialog);

                if (newNode != null)
                {
                    GameManager.Instance.PlayerConversant.currentNode = newNode;
                    GameManager.Instance.PlayerConversant.Next();
                }
            }
        }*/

        /*public void ExecuteCreateBoxMessage()
        {
            GameManager.Instance.gameCanvasManager.CreateMessageBox(boxMessageTextToDisplay, boxMessageSize);
        }*/

        #endregion
    }
}

namespace AllosiusDevCore
{
    public enum ActionType
    {
        ReturnMainNodeDialogue,
        AddQuest,
        CompleteQuestStep,
        LaunchDialogue,
        CreateBoxMessage,
    }
}
