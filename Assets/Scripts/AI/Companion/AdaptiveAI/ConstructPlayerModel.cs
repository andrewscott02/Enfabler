using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructPlayerModel : MonoBehaviour
{
    #region Setup

    public CharacterCombat characterCombat;

    public Descriptor playerState;

    public Dictionary<Descriptor, float> descriptorValues = new Dictionary<Descriptor, float>();
    public List<DescriptorValue> showValues = new List<DescriptorValue>();

    private void Start()
    {
        descriptorValues.Add(Descriptor.Aggressive, 0);
        descriptorValues.Add(Descriptor.Counter, 0);
        descriptorValues.Add(Descriptor.Defensive, 0);
        descriptorValues.Add(Descriptor.Cautious, 0);
        descriptorValues.Add(Descriptor.Panic, 0);

        if (characterCombat != null)
        {
            characterCombat.modelConstructor = this;
        }
    }

    private void AdjustDisplay()
    {
        showValues.Clear();

        Descriptor highestState = Descriptor.Null;
        float highestValue = 0;

        DescriptorValue baseValue = new DescriptorValue();
        baseValue.descriptor = Descriptor.Null;
        baseValue.value = 0;

        showValues.Add(baseValue);

        foreach (var item in descriptorValues)
        {
            DescriptorValue newValue = new DescriptorValue();
            newValue.descriptor = item.Key;
            newValue.value = item.Value;

            showValues.Add(newValue);

            if (newValue.value > highestValue)
            {
                highestValue = newValue.value;
                highestState = newValue.descriptor;
            }
        }

        if (highestState != Descriptor.Null)
        {
            playerState = highestState;
        }
    }

    #endregion

    #region Decay Models

    public float aggressiveDecay = 0.3f;
    public float counterDecay = 0.5f;
    public float defensiveDecay = 0.5f;
    public float cautiousDecay = 0.5f;
    public float panicDecay = 0.5f;

    private void Update()
    {
        descriptorValues[Descriptor.Aggressive] = Mathf.Clamp(descriptorValues[Descriptor.Aggressive] - (aggressiveDecay * Time.deltaTime), 0, Mathf.Infinity);
        descriptorValues[Descriptor.Counter] = Mathf.Clamp(descriptorValues[Descriptor.Counter] - (counterDecay * Time.deltaTime), 0, Mathf.Infinity);
        descriptorValues[Descriptor.Defensive] = Mathf.Clamp(descriptorValues[Descriptor.Defensive] - (defensiveDecay * Time.deltaTime), 0, Mathf.Infinity);
        descriptorValues[Descriptor.Cautious] = Mathf.Clamp(descriptorValues[Descriptor.Cautious] - (cautiousDecay * Time.deltaTime), 0, Mathf.Infinity);
        descriptorValues[Descriptor.Panic] = Mathf.Clamp(descriptorValues[Descriptor.Panic] - (panicDecay * Time.deltaTime), 0, Mathf.Infinity);

        AdjustDisplay();
    }

    #endregion

    #region Player Actions

    public void PlayerAttack(bool hit)
    {
        descriptorValues[Descriptor.Aggressive] += 3f;

        if (CheckCounter())
        {
            descriptorValues[Descriptor.Counter] += 5f;
        }

        if (!hit)
        {
            descriptorValues[Descriptor.Panic] += 5f;
        }

        AdjustDisplay();
    }

    public void PlayerParry(bool beingAttacked)
    {
        if (beingAttacked)
        {
            descriptorValues[Descriptor.Defensive] += 5f;
            SetupCounter(counterWindowParry);
        }
        else
        {
            descriptorValues[Descriptor.Panic] += 5f;
        }
        AdjustDisplay();
    }

    public void PlayerDodge(bool away, bool beingAttacked)
    {
        if (beingAttacked)
        {
            if (away)
            {
                descriptorValues[Descriptor.Cautious] += 5f;
            }
            else
            {
                descriptorValues[Descriptor.Defensive] += 5f;
            }
            SetupCounter(counterWindowDodge);
        }
        else
        {
            descriptorValues[Descriptor.Panic] += 5f;
        }
        AdjustDisplay();
    }

    public void PlayerHit(bool attacking, bool blocking, bool dodging)
    {
        descriptorValues[Descriptor.Aggressive] += 5f;
        AdjustDisplay();
    }

    #endregion

    #region Counter Attack Considerations

    public bool counterAvailable;
    public float counterWindowParry = 1.5f;
    public float counterWindowDodge = 3f;

    bool CheckCounter()
    {
        return counterAvailable;
    }

    void SetupCounter(float counterWindow)
    {
        counterAvailable = true;
        CancelInvoke();
        Invoke("EndCounter", counterWindow);
    }

    void EndCounter()
    {
        counterAvailable = false;
    }

    #endregion
}

[System.Serializable]
public struct DescriptorValue
{
    public Descriptor descriptor;
    public float value;
}

public enum Descriptor
{
    Null, Aggressive, Counter, Defensive, Cautious, Panic
}