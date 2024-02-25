using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoldUI : MonoBehaviour
{
    public TextMeshProUGUI goldText;

    public void Setup()
    {
        TreasureManager.instance.D_GiveGold += AdjustGoldUI;
        AdjustGoldUI(0);
    }

    public void RemoveDelegate()
    {
        TreasureManager.instance.D_GiveGold -= AdjustGoldUI;
    }

    public void AdjustGoldUI(int amount)
    {
        goldText.text = TreasureManager.goldCount.ToString();
    }
}
