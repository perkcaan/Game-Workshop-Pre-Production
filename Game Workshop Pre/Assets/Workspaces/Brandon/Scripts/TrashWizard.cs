using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashWizard : EnemyBase
{

    [Tooltip("Toggle to eneable behavior demonstration for testing purposes")]
    [SerializeField] bool demonstration;

    [Tooltip("How often the wizard teleports")]
    [SerializeField] float teleportDelay = 5f;

    [Tooltip("The Minions that the Wizard will summon during the fight")]
    [SerializeField] GameObject[] minionRoster;

    [SerializeField] GameObject[] teleSpots;

    [SerializeField] GameObject normalAttack;
    [SerializeField] GameObject bigAttack;


    int spotsAvailable;

    // Start is called before the first frame update
    protected  override void OnStart()
    {
        _blackboard.Set<float>("delay", teleportDelay);
        _blackboard.Set<GameObject[]>("minions", minionRoster);
        _blackboard.Set<GameObject>("normalAttack", normalAttack);
        _blackboard.Set<GameObject>("bigAttack", bigAttack);

        if (teleSpots.Length > 0) 
             spotsAvailable = teleSpots.Length - 1;
        else
             spotsAvailable = 0;
    }

    // Update is called once per frame
    protected  override void OnUpdate()
    {
        
    }
    
    public void TeleportAction()
    {
        int selectedSpot = UnityEngine.Random.Range(0, spotsAvailable);
        this.transform.position = teleSpots[selectedSpot].transform.position;
        StartCoroutine(TeleportDelay(teleportDelay));
    }

    private IEnumerator TeleportDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _blackboard.Set<bool>("isInAction", false);
    }




}
