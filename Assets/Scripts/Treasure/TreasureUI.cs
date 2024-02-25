using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureUI : MonoBehaviour
{
    public TextPopup goldText, goldValue;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("Setup", 0.1f);
    }

    void Setup()
    {
        TreasureManager.instance.D_GiveGold += AdjustGoldUI;
        AdjustGoldUI(0);
    }

    void AdjustGoldUI(int amount)
    {

        goldText.ShowText("Gold");
        goldValue.ShowText(TreasureManager.goldCount.ToString());
    }
}
