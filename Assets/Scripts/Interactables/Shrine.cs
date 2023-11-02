using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrine : Interactable, IInteractable
{
    public SliderScript healingSlider;
    Light pointLight;
    public int maxHealing = 30;
    int healingLeft = 0;

    protected override void Start()
    {
        base.Start();
        pointLight = GetComponentInChildren<Light>();
        healingLeft = maxHealing;
        SetSliderValues();
    }

    void SetSliderValues()
    {
        pointLight.intensity = (float)healingLeft / (float)maxHealing;
        healingSlider.ChangeSliderValue(healingLeft, maxHealing);

        if (healingLeft <= 0)
        {
            pointLight.enabled = false;
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (healingLeft <= 0) return;
        base.OnTriggerEnter(other);
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (healingLeft <= 0) return;
        base.OnTriggerExit(other);
    }

    public override void Interacted(BaseCharacterController interactCharacter)
    {
        if (!canBeInteracted) return;
        if (lockedInteraction) return;

        if (!multipleInteractions)
        {
            canBeInteracted = false;
            if (!forceInteraction)
            {
                ShowInteractMessage(false);
                player.EnableInteraction(E_InteractTypes.Null);
            }
        }

        if (healingLeft <= 0) return;

        int neededHealing = interactCharacter.GetHealth().maxHealth - interactCharacter.GetHealth().GetCurrentHealth();

        int healthRestore = Mathf.Clamp(neededHealing, 0, healingLeft);
        healingLeft -= healthRestore;
        interactCharacter.GetHealth().Heal(healthRestore);
        SetSliderValues();

        if (interactFX != null)
            Instantiate(interactFX, interactCharacter.transform);

        if (healingLeft <= 0)
        {
            ShowInteractMessage(false);
            player.EnableInteraction(E_InteractTypes.Null);
        }
    }
}
