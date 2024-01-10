using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enfabler.Quests
{
    [CreateAssetMenu(fileName = "NewQuest", menuName = "Quests/Create Quest", order = 0)]
    public class Quest : ScriptableObject
    {
        #region Setup

        #region Variables

        [Header("Basic Info")]
        public string questName;
        public bool mainQuest;
        public int questNumber;
        [TextArea(3, 10)]
        public string questDescription;
        string questGiver = "";

        [Header("Advanced Info")]
        public E_QuestStates state;
        public int currentProgress = -1;
        public int maxProgress = 1;

        public Quest parentQuest;
        public Quest[] subQuests;
        public bool linear = true;

        //public LootPool rewards;
        //public bool overrideParentRewards = false;

        #endregion

        [ContextMenu("Force Restart Quest")]
        public void ForceRestartQuest()
        {
            ForceResetQuest();
            StartQuest("Mama R", null);
            UpdateQuestInfo(true);
        }

        [ContextMenu("Force Start Quest")]
        public void ForceStartQuest()
        {
            StartQuest("Mama R", null);
            UpdateQuestInfo(true);
        }

        [ContextMenu("Force Reset Quest")]
        public void ForceResetQuest()
        {
            state = E_QuestStates.NotStarted;
            currentProgress = -1;

            foreach (Quest quest in subQuests)
            {
                quest.ForceResetQuest();
            }

            UpdateQuestInfo(true);
        }

        public Quest GetParent()
        {
            if (parentQuest == null)
                return this;

            return parentQuest.GetParent();
        }

        #endregion

        #region Quest Progress

        public void StartQuest(string questGiver, Quest parent)
        {
            //Debug.Log("Start quest " + questName);

            if (parent != null)
                parentQuest = parent;

            this.questGiver = questGiver;
            state = E_QuestStates.InProgress;

            currentProgress = 0;

            EnableNextObjective();
            EnableAllObjectives();

            if (QuestInfo.instance != null)
                QuestInfo.instance.SetTrackingQuest(this.GetParent());
            UpdateQuestInfo(true);
        }

        [ContextMenu("Quest Progress")]
        public void QuestProgressContext()
        {
            QuestProgress(true);
        }

        public void QuestProgress(bool updateMarkers)
        {
            if (state != E_QuestStates.InProgress)
                return;

            currentProgress++;

            if (currentProgress >= maxProgress)
            {
                if (parentQuest != null)
                {
                    parentQuest.QuestProgress(updateMarkers);
                }

                state = E_QuestStates.Completed;
                GiveRewards();
            }
            else
            {
                EnableNextObjective();
            }

            UpdateQuestInfo(updateMarkers);
        }

        void EnableNextObjective()
        {
            if (subQuests.Length > 0)
            {
                if (linear)
                {
                    subQuests[currentProgress].StartQuest(questGiver, this);
                }
            }

            UpdateQuestInfo(true);
        }

        void EnableAllObjectives()
        {
            if (!linear)
            {
                for (int i = 0; i < subQuests.Length; i++)
                {
                    subQuests[i].StartQuest(questGiver, this);
                }
            }

            UpdateQuestInfo(true);
        }

        void GiveRewards()
        {
            UpdateQuestInfo(true);
            /*
            if (overrideParentRewards)
            {
                OverrideRewards(rewards);
                return;
            }
            else
                rewards.RewardItems();
            */
        }

        public void OverrideRewards(/*LootPool newRewards*/)
        {
            /*
            if (parentQuest == null)
            {
                rewards = newRewards;
            }
            else
            {
                parentQuest.OverrideRewards(newRewards);
            }
            */
        }

        void UpdateQuestInfo(bool updateMarkers)
        {
            if (QuestInfo.instance != null)
            {
                QuestInfo.instance.UpdateQuestInfo();
            }

            if (Application.isPlaying && updateMarkers)
            {
                //Debug.Log("updating quest markers");
                CheckQuestMarkers();
            }
        }

        void CheckQuestMarkers()
        {
            /*
            if (Compass.instance != null)
                Compass.instance.CheckQuestMarkers();
            */
        }

        public Quest GetCurrentQuestProgress()
        {
            //currently this only allows quests with a linear progression, so no choices yet
            if (state != E_QuestStates.InProgress)
                return null;

            Quest quest = this;

            if (subQuests.Length > 0)
                quest = subQuests[currentProgress].GetCurrentQuestProgress();

            return quest;
        }

        #endregion

        #region Saving and Loading

        public void LoadQuestData(int progress)
        {
            currentProgress = progress;
            CheckProgress();
        }

        void CheckProgress()
        {
            if (currentProgress == -1)
                state = E_QuestStates.NotStarted;
            else if (currentProgress == maxProgress)
                state = E_QuestStates.Completed;
            else
                state = E_QuestStates.InProgress;
        }

        #endregion

        #region Dev Command

        [ContextMenu("Force Quest - Cave Start")]
        public void TestQuestCommand13()
        {
            DevForceSetQuestProgress(13);
        }

        public void DevForceSetQuestProgress(int progress)
        {
            if (parentQuest != null)
            {
                parentQuest.DevForceSetQuestProgress(progress);
                return;
            }

            Debug.Log(questName + " is the parent for quest resetting");
            ForceRestartQuest();

            RForceSetQuestProgress(progress, 0);

            if (Application.isPlaying)
                UpdateQuestInfo(true);
        }

        public int RForceSetQuestProgress(int progress, int count)
        {
            if (subQuests.Length == 0)
            {
                while (count < progress && currentProgress < maxProgress)
                {
                    Debug.Log(questName + " is progressing " + count);
                    QuestProgress(false);
                    count++;
                }
                return count;
            }

            foreach (var item in subQuests)
            {
                if (count < progress)
                {
                    Debug.Log(questName + " is progressing children (recursive) " + count);
                    count = item.RForceSetQuestProgress(progress, count);
                }
            }

            return count;
        }

        #endregion
    }

    public enum E_QuestStates
    {
        NotStarted, InProgress, Failed, Completed
    }
}