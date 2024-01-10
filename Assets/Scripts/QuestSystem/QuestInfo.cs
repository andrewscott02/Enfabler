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

            UpdateQuestInfo();
        }

        public Quest baseQuest;
        Quest trackingQuest;

        public TextMeshProUGUI title, number, description;

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
                    number.text = sub.currentProgress.ToString();
                    description.text = sub.questDescription;
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