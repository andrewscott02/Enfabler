using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructPlayerModel : MonoBehaviour
{
    public Descriptor playerState;

    public Dictionary<Descriptor, float> descriptorValues = new Dictionary<Descriptor, float>();
    public List<DescriptorValue> showValues = new List<DescriptorValue>();

    private void Start()
    {
        descriptorValues.Add(Descriptor.Aggressive, 0);
        descriptorValues.Add(Descriptor.Cautious, 0);
        descriptorValues.Add(Descriptor.Panic, 0);
    }

    private void AdjustDisplay()
    {
        showValues.Clear();

        Descriptor highestState = Descriptor.Null;
        float highestValue = 0;

        foreach(var item in descriptorValues)
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

    public void PlayerAttack(bool hit)
    {
        descriptorValues[Descriptor.Aggressive] += 0.5f;
        AdjustDisplay();
    }

    public void PlayerBlock(bool successful)
    {
        descriptorValues[Descriptor.Defensive] += 0.5f;
        AdjustDisplay();
    }

    public void PlayerDodge(bool away, bool successful, bool beingAttacked)
    {
        descriptorValues[Descriptor.Cautious] += 0.5f;
        AdjustDisplay();
    }

    public void PlayerHit(bool attacking, bool blocking, bool dodging)
    {
        descriptorValues[Descriptor.Aggressive] += 0.5f;
        AdjustDisplay();
    }
}

[System.Serializable]
public struct DescriptorValue
{
    public Descriptor descriptor;
    public float value;
}

public enum Descriptor
{
    Null, Aggressive, Defensive, Cautious, Panic
}