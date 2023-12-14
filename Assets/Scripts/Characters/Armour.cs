using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armour : MonoBehaviour
{
    public SliderScript armourSlider;
    public int maxArmour = 5;
    public int currentArmour { get; private set; }
    public float armourCooldown = 6f;
    public float armourRegen = 3f;

    public delegate void BlockDelegate(bool blocking);

    Coroutine armourRegenCoroutine;

    private void Start()
    {
        Health healthScript = GetComponent<Health>();

        healthScript.killDelegate += Kill;

        CharacterCombat combat = GetComponent<CharacterCombat>();

        combat.canBlockDelegate += CanBlock;

        combat.blockingDelegate += Block;
        combat.blockedDelegate += ConsumeArmour;

        combat.parriedDelegate += ParrySuccess;
        combat.onAttackHit += Hit;

        currentArmour = maxArmour;
        armourRegenCoroutine = StartCoroutine(IResetArmour(armourCooldown));
    }

    #region Blocking

    public virtual void Block(bool blocking)
    {
        //Empty for now
    }

    public void ConsumeArmour()
    {
        if (armourRegenCoroutine != null)
            StopCoroutine(armourRegenCoroutine);

        currentArmour--;
        armourRegenCoroutine = StartCoroutine(IResetArmour(armourCooldown));
        ChangeArmourUI();
    }

    public void GainArmour(int armour)
    {
        currentArmour = Mathf.Clamp(currentArmour + armour, 0, maxArmour);
        ChangeArmourUI();
    }

    public bool CanBlock()
    {
        return currentArmour > 0;
    }

    IEnumerator IResetArmour(float delay)
    {
        yield return new WaitForSeconds(delay);
        GainArmour(1);
        armourRegenCoroutine = StartCoroutine(IResetArmour(armourRegen));
    }

    void ChangeArmourUI()
    {
        if (armourSlider != null)
            armourSlider.ChangeSliderValue(currentArmour, maxArmour);
    }

    #endregion

    public int armourOnParry = 1, armourOnHit = 1;

    public void ParrySuccess()
    {
        GainArmour(armourOnParry);
    }

    public void Hit(bool hit)
    {
        if (hit)
            GainArmour(armourOnHit);
    }

    public void Kill(Vector3 attacker, int damage)
    {
        if (armourSlider != null)
            armourSlider.gameObject.SetActive(false);
    }
}
