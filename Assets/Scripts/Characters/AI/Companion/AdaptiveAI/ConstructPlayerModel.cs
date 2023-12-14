using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConstructPlayerModel : MonoBehaviour
{
    #region Setup

    [Header("Agent Model Info")]
    public GameObject modelCharacter;
    public TextMeshProUGUI stateText;

    Descriptor trueState;
    public Descriptor playerState;

    public Dictionary<Descriptor, float> descriptorValues = new Dictionary<Descriptor, float>();
    public List<DescriptorValue> showValues = new List<DescriptorValue>();

    public bool test = false;

    private void Start()
    {
        //Setup the descriptors with values of 0
        descriptorValues.Add(Descriptor.Aggressive, 0);
        descriptorValues.Add(Descriptor.Counter, 0);
        descriptorValues.Add(Descriptor.Defensive, 0);
        descriptorValues.Add(Descriptor.Cautious, 0);
        descriptorValues.Add(Descriptor.Panic, 0);

        if (test) { return; }

        if (modelCharacter != null)
        {
            modelCharacter.GetComponent<Health>().modelConstructor = this;
        }

        InvokeRepeating("CurrentTarget", 0, currentTargetCastInterval);
    }

    private void Update()
    {
        DecayModels();
        UpdateModel();

        //Values for choosing next state if in current state for too long, this is intended to add variety
        explore += Time.deltaTime;
        currentSwitchCooldown += Time.deltaTime;
    }

    /// <summary>
    /// Updates the player model with the descriptor of the highest value and updates the UI
    /// </summary>
    private void UpdateModel()
    {
        showValues.Clear();

        //Sets up values for the highest state, overridden in loop
        Descriptor highestState = Descriptor.Null;
        float highestValue = 0;

        //Empty item in case of null values
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

            //If the descriptor being checked is higher than the current highest descriptor, set the current highest descriptor to this one
            if (newValue.value > highestValue)
            {
                highestValue = newValue.value;
                highestState = newValue.descriptor;
            }
        }

        if (highestState != Descriptor.Null)
        {
            //Changes to one of the next highest states if the player has been stuck in the same state for too long
            playerState = GetExploreState(highestState);
            trueState = highestState;
        }

        if (test) { return; }

        //Updates the UI with the player state if it is active
        if (stateText != null)
        {
            stateText.text = "PS: " + playerState.ToString() + "TS: " + trueState.ToString();
        }
    }

    float explore = 0;
    public float switchCooldown = 5;
    float currentSwitchCooldown = 0;

    /// <summary>
    /// Changes to one of the next highest states if the player has been stuck in the same state for too long
    /// </summary>
    /// <param name="newState">The true state the player is in</param>
    /// <returns>The state to switch to, either the true state or one of the next highest states</returns>
    private Descriptor GetExploreState(Descriptor newState)
    {
        //If the new state is different from the last state, then it has not been in the state too long, return the new state
        if (trueState != newState)
        {
            explore = 0;
            return newState;
        }

        //If the cooldown for switching state in this way has not elapsed, return the existing player state
        if (currentSwitchCooldown < switchCooldown)
        {
            return playerState;
        }

        List<Descriptor> possibleStates = new List<Descriptor>();

        float highestValue = descriptorValues[newState];

        //Makes it so that it will be less likely to explore states if the player is struggling
        if (newState == Descriptor.Panic)
            highestValue *= 1.5f;

        //Loops through each state, checking if the distance between that and the new state is larger than the explore value, if so it adds it to the list of explore states
        foreach (var item in descriptorValues)
        {
            if (item.Key != newState)
            {
                float difference = highestValue - item.Value;

                if (difference < explore)
                {
                    possibleStates.Add(item.Key);
                }
            }
        }

        //If there is at least 1 state that it can explore to, randomly choose between those states
        if (possibleStates.Count > 0)
        {
            Descriptor exploreState = possibleStates[Random.Range(0, possibleStates.Count)];
            Debug.Log("Switched from " + trueState.ToString() + " to " + exploreState.ToString());
            currentSwitchCooldown = 0;

            return exploreState;
        }

        return newState;
    }

    #endregion

    #region Decay Models

    [Header("Decay Values")]
    public float aggressiveDecay = 0.3f;
    public float counterDecay = 0.5f;
    public float defensiveDecay = 0.5f;
    public float cautiousDecay = 0.5f;
    public float panicDecay = 0.5f;

    /// <summary>
    /// Decays all descriptors by their decay values
    /// </summary>
    private void DecayModels()
    {
        descriptorValues[Descriptor.Aggressive] = Mathf.Clamp(descriptorValues[Descriptor.Aggressive] - (aggressiveDecay * Time.deltaTime), 0, Mathf.Infinity);
        descriptorValues[Descriptor.Counter] = Mathf.Clamp(descriptorValues[Descriptor.Counter] - (counterDecay * Time.deltaTime), 0, Mathf.Infinity);
        descriptorValues[Descriptor.Defensive] = Mathf.Clamp(descriptorValues[Descriptor.Defensive] - (defensiveDecay * Time.deltaTime), 0, Mathf.Infinity);
        descriptorValues[Descriptor.Cautious] = Mathf.Clamp(descriptorValues[Descriptor.Cautious] - (cautiousDecay * Time.deltaTime), 0, Mathf.Infinity);
        descriptorValues[Descriptor.Panic] = Mathf.Clamp(descriptorValues[Descriptor.Panic] - (panicDecay * Time.deltaTime), 0, Mathf.Infinity);
    }

    #endregion

    #region Player Actions

    [Header("Player Targeting")]
    public List<BaseCharacterController> currentTargets;
    public float currentTargetCastInterval = 0.6f;

    /// <summary>
    /// Gets the enemies the player is currently targetting
    /// </summary>
    void CurrentTarget()
    {
        if (modelCharacter != null)
            currentTargets = modelCharacter.GetComponent<CharacterCombat>().currentTargets;
        else
            currentTargets.Clear();
    }

    [Header("Attack Model Additions")]
    public float attackHitAdd = 1.5f;
    public float attackCounterAdd = 7.5f;
    public float attackMissAdd = 3f;

    /// <summary>
    /// When the player attacks, add to the corresponding descriptors
    /// </summary>
    /// <param name="hit">Whether the player successfully hit their target</param>
    public void PlayerAttack(bool hit)
    {
        descriptorValues[Descriptor.Aggressive] += attackHitAdd; //Add to the aggressive descriptor

        if (CheckCounter()) { descriptorValues[Descriptor.Counter] += attackCounterAdd; } //If the attack was made after a recent counter, add to the counter descriptor

        if (!hit) { descriptorValues[Descriptor.Panic] += attackMissAdd; } //If the attack missed, add to the panic descriptor

        // ^ Still adds to the aggressive and counter descriptors on miss because we still want to know that the player is aggressive even if they are panicking

        UpdateModel();
    }

    [Header("Parry Model Additions")]
    public float parrySuccessAdd = 5f;
    public float parryFailAdd = 7f;

    /// <summary>
    /// When the player parries, add to the corresponding descriptors
    /// </summary>
    /// <param name="hit">Whether the player is being targetted by an enemy</param>
    public void PlayerParry(bool beingAttacked)
    {
        descriptorValues[Descriptor.Defensive] += parrySuccessAdd; //Add to the defensive descriptor

        if (beingAttacked) { SetupCounter(counterWindowParry); } //If the player was being attacked, set up a counter window
        else { descriptorValues[Descriptor.Panic] += parryFailAdd; } //If the parry was made when no enemies were targetting the player, add to the panic descriptor

        // ^ Still adds to the defensive descriptor on miss because we still want to know that the player is defensive even if they are panicking
        //However, the counter window should not be opened if the player is not being attacked

        UpdateModel();
    }

    [Header("Dodge Model Additions")]

    public float dodgeSuccessCAdd = 3.5f;
    public float dodgeFailCAdd = 4f;
    public float dodgeSuccessDAdd = 3f;
    public float dodgeFailPAdd = 5f;

    /// <summary>
    /// When the player dodges, add to the corresponding descriptors
    /// </summary>
    /// <param name="hit">Whether the player is being targetted by an enemy</param>
    public void PlayerDodge(bool beingAttacked)
    {
        //More complexity here because we want to add differing values to the descriptors on success/fail
        if (beingAttacked)
        {
            //Add to cautious and defensive descriptors if being attacked
            descriptorValues[Descriptor.Cautious] += dodgeSuccessCAdd;
            descriptorValues[Descriptor.Defensive] += dodgeSuccessDAdd;
            SetupCounter(counterWindowDodge); //If the player was being attacked, set up a counter window

            //Want to add to the defensive descriptor here because dodging is still a defensive action
        }
        else 
        {
            //Add to cautious and panic descriptors if not being attacked
            descriptorValues[Descriptor.Cautious] += dodgeFailCAdd;
            descriptorValues[Descriptor.Panic] += dodgeFailPAdd;

            //We still want to add to a cautious descriptor because some players might want to be mobile even if they
            //are not being attacked since dodging can be faster than running if they need a quick burst of speed
        }

        UpdateModel();
    }

    [Header("Hit Model Additions")]
    public float hitPanicAdd = 3f;

    /// <summary>
    /// When the player is hit, add to the panic descriptor
    /// </summary>
    public void PlayerHit()
    {
        descriptorValues[Descriptor.Panic] += hitPanicAdd;
        UpdateModel();
    }

    #endregion

    #region Counter Attack Considerations

    [Header("Counter Info")]
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