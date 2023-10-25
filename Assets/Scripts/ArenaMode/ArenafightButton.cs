using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ArenafightButton : MonoBehaviour
{
    public ArenaFight fight;

    TextMeshProUGUI buttonText;

    private void Start()
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = fight.fightName;
    }

    public void StartFight()
    {
        ArenaManager.instance.SelectFight(fight);
    }
}
