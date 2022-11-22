using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructPlayerModel : MonoBehaviour
{
    #region Setup

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

    #region Player Actions

    public void PlayerAttack(bool hit)
    {
        if (CheckCounter())
        {
            descriptorValues[Descriptor.Counter] += 0.5f;
        }
        else
        {
            if (hit)
            {
                descriptorValues[Descriptor.Aggressive] += 0.3f;
            }
            else
            {
                descriptorValues[Descriptor.Panic] += 0.5f;
            }
        }
        AdjustDisplay();
    }

    public void PlayerParry(bool beingAttacked)
    {
        if (beingAttacked)
        {
            descriptorValues[Descriptor.Defensive] += 0.5f;
            SetupCounter();
        }
        else
        {
            descriptorValues[Descriptor.Panic] += 0.5f;
        }
        AdjustDisplay();
    }

    public void PlayerDodge(bool away, bool beingAttacked)
    {
        if (beingAttacked)
        {
            if (away)
            {
                descriptorValues[Descriptor.Cautious] += 0.5f;
            }
            else
            {
                descriptorValues[Descriptor.Defensive] += 0.5f;
            }
            SetupCounter();
        }
        else
        {
            descriptorValues[Descriptor.Panic] += 0.5f;
        }
        AdjustDisplay();
    }

    public void PlayerHit(bool attacking, bool blocking, bool dodging)
    {
        descriptorValues[Descriptor.Aggressive] += 0.5f;
        AdjustDisplay();
    }

    #endregion

    #region Counter Attack Considerations

    public bool counterAvailable;
    public float counterWindow = 3f;

    bool CheckCounter()
    {
        return counterAvailable;
    }

    void SetupCounter()
    {
        counterAvailable = true;

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