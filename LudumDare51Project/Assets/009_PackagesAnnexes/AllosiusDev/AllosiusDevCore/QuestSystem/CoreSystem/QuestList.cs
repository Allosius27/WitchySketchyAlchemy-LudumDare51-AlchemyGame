using AllosiusDevUtilities.Audio;
using AllosiusDevUtilities.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AllosiusDevCore.QuestSystem
{
    public class QuestList : MonoBehaviour
    {
        #region Fields

        private List<QuestStatus> statuses = new List<QuestStatus>();

        #endregion

        #region UnityInspector

        [SerializeField] private AudioData sfxAddQuest;
        [SerializeField] private AudioData sfxQuestStepCompleted;
        [SerializeField] private AudioData sfxQuestCompleted;

        #endregion

        #region Events

        public event Action OnUpdate;

        #endregion


        #region Behaviour

        public IEnumerable<QuestStatus> GetStatuses()
        {
            return statuses;
        }

        private QuestStatus GetQuestStatus(QuestData quest, StateQuestWanted state = StateQuestWanted.None)
        {
            foreach (QuestStatus status in statuses)
            {
                if (status.GetQuest() == quest)
                {
                    if (state == StateQuestWanted.None)
                    {
                        return status;
                    }
                    else if (state == StateQuestWanted.Uncompleted)
                    {
                        if (status.GetQuestCompleted() == false)
                        {
                            return status;
                        }
                    }
                    else if (state == StateQuestWanted.Completed)
                    {
                        if (status.GetQuestCompleted() == true)
                        {
                            return status;
                        }
                    }
                }
            }
            return null;
        }

        private QuestStepStatus GetQuestStepStatus(QuestStatus status, QuestStepData objectiveRef, StateQuestWanted state = StateQuestWanted.None)
        {
            if (status != null)
            {
                foreach (QuestStepStatus stepStatus in status.GetQuestStepStatuses())
                {
                    if (stepStatus.GetQuestStep() == objectiveRef && stepStatus.isUnlocked)
                    {
                        if (state == StateQuestWanted.None)
                        {
                            return stepStatus;
                        }
                        else if (state == StateQuestWanted.Uncompleted)
                        {
                            if (stepStatus.GetStepCompleted() == false)
                            {
                                return stepStatus;
                            }
                        }
                        else if (state == StateQuestWanted.Completed)
                        {
                            if (stepStatus.GetStepCompleted() == true)
                            {
                                return stepStatus;
                            }
                        }
                    }
                }

            }

            return null;
        }

        public bool HasQuest(QuestData quest, StateQuestWanted state = StateQuestWanted.None)
        {
            return GetQuestStatus(quest, state) != null;
        }

        public bool HasQuestStep(QuestData quest, QuestStepData objectiveRef, StateQuestWanted state = StateQuestWanted.None)
        {
            QuestStatus status = GetQuestStatus(quest);
            return GetQuestStepStatus(status, objectiveRef, state) != null;
        }

        public void AddQuest(QuestData quest)
        {
            if (HasQuest(quest)) return;
            QuestStatus newStatus = new QuestStatus(quest);
            newStatus.SetQuestStepUnlocked(newStatus.GetQuestStepStatuses()[0], true);
            statuses.Add(newStatus);

            /*if (GameManager.Instance.gameCanvasManager.questTrackingUi.gameObject.activeSelf == false)
            {
                GameManager.Instance.gameCanvasManager.questUi.Redraw();
            }*/

            AudioController.Instance.PlayAudio(sfxAddQuest);

            if (OnUpdate != null)
            {
                OnUpdate();
            }
        }

        public void CompleteQuestStep(QuestData quest, QuestStepData step)
        {
            QuestStatus status = GetQuestStatus(quest);
            if (status != null && status.IsComplete() == false && status.IsQuestStepComplete(step) == false)
            {
                Debug.Log(status);
                status.CompleteQuestStep(step);

                if (OnUpdate != null)
                {
                    OnUpdate();
                }
                else
                {
                    Debug.LogWarning("OnUpdate is null");
                }

                if(status.IsQuestStepComplete(step))
                {
                    AudioController.Instance.PlayAudio(sfxQuestStepCompleted);
                }

                if (status.GetQuestCompleted())
                {
                    AudioController.Instance.PlayAudio(sfxQuestCompleted);
                }
            }

            
        }

        public void CompleteQuestTask(QuestData quest, QuestStepData step, QuestTaskData task)
        {
            QuestStatus status = GetQuestStatus(quest);
            if (status != null)
            {
                Debug.Log(status);
                status.CompleteQuestTask(step, task);

                if (OnUpdate != null)
                {
                    OnUpdate();
                }
                else
                {
                    Debug.LogWarning("OnUpdate is null");
                }
            }
        }


        #endregion
    }
}
