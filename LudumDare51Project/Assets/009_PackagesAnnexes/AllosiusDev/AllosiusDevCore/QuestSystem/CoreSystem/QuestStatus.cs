using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AllosiusDevCore.QuestSystem
{
    [Serializable]
    public class QuestStatus
    {
        #region Fields

        private QuestData quest;

        private bool questCompleted;

        private List<QuestStepStatus> questStepStatuses = new List<QuestStepStatus>();

        #endregion

        #region Properties

        public QuestStatus(QuestData quest)
        {
            this.quest = quest;

            for (int i = 0; i < quest.questSteps.Count; i++)
            {
                questStepStatuses.Add(new QuestStepStatus(quest.questSteps[i]));
            }
        }

        #endregion

        #region UnityInspector

        public List<QuestStepData> completedSteps = new List<QuestStepData>();

        #endregion

        #region Functions

        public QuestData GetQuest()
        {
            return quest;
        }

        public List<QuestStepStatus> GetQuestStepStatuses()
        {
            return questStepStatuses;
        }
        public int GetCompletedCount()
        {
            return completedSteps.Count;
        }

        public bool GetQuestCompleted()
        {
            return questCompleted;
        }

        public bool IsComplete()
        {
            foreach (var item in quest.questSteps)
            {
                if (!completedSteps.Contains(item))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsQuestStepComplete(QuestStepData objective)
        {
            return completedSteps.Contains(objective);
        }


        public void UnlockNextQuestStep(QuestStepStatus currentStep)
        {
            for (int i = 0; i < questStepStatuses.Count; i++)
            {
                if (questStepStatuses[i] == currentStep && i < questStepStatuses.Count - 1)
                {
                    SetQuestStepUnlocked(questStepStatuses[i + 1], true);
                    return;
                }
            }
        }

        public void SetQuestStepUnlocked(QuestStepStatus step, bool value)
        {
            step.SetStepUnlocked(value);
        }


        public void CompleteQuest()
        {
            if (IsComplete())
            {
                SetQuestCompleted(true);
            }
        }

        public void SetQuestCompleted(bool value)
        {
            questCompleted = value;
        }


        public void CompleteQuestStep(QuestStepData objective)
        {
            foreach (QuestStepStatus step in questStepStatuses)
            {
                if (step.GetQuestStep() == objective)
                {
                    if (step.GetQuestStep().objectiveType == QuestStepData.QuestObjectiveType.WithTasks)
                    {
                        if (quest.HasQuestStep(objective) && step.IsComplete() && completedSteps.Contains(objective) == false)
                        {
                            CompleteQuestStepAction(objective, step);
                        }
                    }
                    else if (step.GetQuestStep().objectiveType == QuestStepData.QuestObjectiveType.Default)
                    {
                        if (quest.HasQuestStep(objective) && completedSteps.Contains(objective) == false)
                        {
                            CompleteQuestStepAction(objective, step);
                        }
                    }
                }
            }

        }

        private void CompleteQuestStepAction(QuestStepData objective, QuestStepStatus step)
        {
            step.SetStepCompleted(true);
            completedSteps.Add(objective);
            UnlockNextQuestStep(step);
            CompleteQuest();
        }

        public void CompleteQuestTask(QuestStepData stepData, QuestTaskData taskData)
        {
            foreach (QuestStepStatus step in questStepStatuses)
            {
                if (step.GetQuestStep() == stepData)
                {
                    if (step.GetQuestStep().HasQuestTask(taskData) && step.completedTasks.Contains(taskData) == false)
                    {
                        step.completedTasks.Add(taskData);
                        CompleteQuestStep(step.GetQuestStep());
                    }
                }
            }

        }

        #endregion
    }

    [Serializable]
    public class QuestStepStatus
    {
        #region Fields

        private QuestStepData questStep;

        private bool stepCompleted;

        #endregion

        #region Properties

        public QuestStepStatus(QuestStepData questStep)
        {
            this.questStep = questStep;
        }

        public bool isUnlocked { get; protected set; }

        #endregion

        #region UnityInspector

        public List<QuestTaskData> completedTasks = new List<QuestTaskData>();

        #endregion

        #region Functions

        public QuestStepData GetQuestStep()
        {
            return questStep;
        }

        public int GetCompletedCount()
        {
            return completedTasks.Count;
        }

        public bool GetStepCompleted()
        {
            return stepCompleted;
        }
        public bool IsComplete()
        {
            if (questStep.objectiveType == QuestStepData.QuestObjectiveType.Default)
            {
                return stepCompleted;
            }
            else if (questStep.objectiveType == QuestStepData.QuestObjectiveType.WithTasks)
            {
                foreach (var item in questStep.tasksObjectives)
                {
                    if (!completedTasks.Contains(item.taskREF))
                    {
                        return false;
                    }
                }
            }

            return true;

        }

        public bool IsQuestTaskComplete(QuestTaskData objective)
        {
            return completedTasks.Contains(objective);
        }
        public void SetStepUnlocked(bool value)
        {
            isUnlocked = value;
        }

        public void SetStepCompleted(bool value)
        {
            stepCompleted = value;
        }



        /*public void CompleteQuestTask(QuestTaskData objective)
        {
            if (questStep.HasQuestTask(objective) && completedTasks.Contains(objective) == false)
            {
                completedTasks.Add(objective);
            }
        }*/

        #endregion
    }
}

