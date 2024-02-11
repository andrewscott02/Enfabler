using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Phase : MonoBehaviour
{
    int defaultLayer;
    public int phaseLayer;

    public Transform attach;

    public Object phaseEffect;
    public Object flashEffect;
    ParticleSystem phaseParticle;

    NavMeshObstacle navMeshObstacle;
    NavMeshAgent navMeshAgent;

    // Start is called before the first frame update
    void Start()
    {
        defaultLayer = gameObject.layer;

        CharacterCombat combat = GetComponent<CharacterCombat>();
        combat.phaseDelegate += ActivatePhase;

        navMeshObstacle = GetComponent<NavMeshObstacle>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void ActivatePhase(bool activate)
    {
        Debug.Log("Phasing: " + activate);
        gameObject.layer = activate ? phaseLayer : defaultLayer;

        StartParticles(activate);

        if (navMeshObstacle != null)
            navMeshObstacle.enabled = !activate;

        if (navMeshAgent != null)
            navMeshAgent.enabled = !activate;
    }

    void StartParticles(bool activate)
    {
        Transform flashSpawn = attach == null ? transform : attach;
        Instantiate(flashEffect, flashSpawn.position, flashSpawn.rotation);

        if (!activate)
            return;

        if (phaseParticle == null)
        {
            GameObject go = Instantiate(phaseEffect, attach == null ? transform : attach) as GameObject;
            phaseParticle = go.GetComponent<ParticleSystem>();
            return;
        }

        phaseParticle.gameObject.SetActive(true);
        phaseParticle.Simulate(0, true, true);

        phaseParticle.Play();
    }
}
