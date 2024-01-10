using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Enfabler.Quests
{
    public class QuestInfo : MonoBehaviour
    {
        public static QuestInfo instance;

        // Start is called before the first frame update
        void Start()
        {
            /*
            if (instance != null)
            {
                Destroy(this.gameObject);
            }
            */
            instance = this;
            //DontDestroyOnLoad(this.gameObject);

            if (trackingQuest == null)
                SetTrackingQuest(baseQuest);

            baseQuest.ForceRestartQuest();

            UpdateQuestInfo();
        }

        public Quest baseQuest;
        Quest trackingQuest;

        public TextMeshProUGUI title, number, description, progressCount;

        public void SetTrackingQuest(Quest quest)
        {
            trackingQuest = quest.GetParent();
            UpdateQuestInfo();
        }

        public void UpdateQuestInfo()
        {
            if (trackingQuest != null)
            {
                Quest sub = trackingQuest.GetCurrentQuestProgress();
                if (sub != null)
                {
                    title.text = sub.questName;
                    number.text = sub.questNumber.ToString();
                    description.text = sub.questDescription;
                    progressCount.text = sub.currentProgress.ToString() + "/" + sub.maxProgress.ToString();
                }
                else
                {
                    title.text = "No Quest";
                    number.text = "0";
                    description.text = "You are not tracking a quest, open the inventory with i and then open the journal to see your quests";
                }
            }
        }
    }
}