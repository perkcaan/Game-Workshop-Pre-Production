using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Heatable : MonoBehaviour
{

    //Current Heat Level; Serialized for testing purposes
    [Header("Objects Current Heat Level")]
    [SerializeField] public int heatLevel;

    //Rate at which heat level falls for actor
    [Header("Rate at which heat falls per second")]
    [SerializeField] int heatEntropy;

    //Max heat for actor before adverse affects
    [Header("Max Allowed Heat")]
    [SerializeField] public int heatThreshold;

    //Level of heat emitted by object
    [Header("Heat Emmission")]
    [SerializeField] int heatEmmission;

    private readonly int PLAYER_BASE_HEAT_FLOOR = 0;
    private readonly float UPDATE_HEAT_TICK_RATE = 0.5f;
    private readonly float ABSORPTION_TICK_RATE = 0.15f;
    private readonly float ENTROPY_TICK_RATE = 1f;
    private string NAME;


    public UnityEvent onRoomEnter;

    
    private List<GameObject> heatableList;
    private CircleCollider2D heatDetectionRadius;
    private Coroutine entropyCoroutine;
    private Coroutine absorptionCoroutine;




    void Start()
    {
        heatableList = new List<GameObject>();
        heatDetectionRadius = GetComponent<CircleCollider2D>();
        NAME = transform.parent.name;
        

        StartCoroutine(UpdateHeat());

    }


    public void RaiseHeat(int heatValue)
    {
        this.heatLevel = Mathf.Clamp(this.heatLevel + heatValue, 0, heatThreshold);
    }

    public void LowerHeat(int heatValue)
    {
        this.heatLevel = Mathf.Clamp(this.heatLevel - heatValue, 0, heatThreshold);
    }

    private IEnumerator HeatEntropy()
    {

        while (heatLevel > 0)
        {
            yield return new WaitForSeconds(ENTROPY_TICK_RATE);
            LowerHeat(heatEntropy);
        }
        heatLevel = 0;
    }

    private IEnumerator AbsorbHeat(int heatValue)
    {
        while (this.heatLevel < heatValue)
        {
            float incrementAmount = heatValue * ABSORPTION_TICK_RATE;
            yield return new WaitForSeconds(ABSORPTION_TICK_RATE);
            if (incrementAmount < 1)
            {
                heatLevel += 1;
            }
            else
                heatLevel += (int)(heatValue * ABSORPTION_TICK_RATE);
        }
        StopCoroutine(absorptionCoroutine);
        absorptionCoroutine = null;
    }

    private IEnumerator UpdateHeat()
    {
        while (true)
        {
            yield return new WaitForSeconds(UPDATE_HEAT_TICK_RATE);
            GetAreaHeat();
        }
    }

    private void GetAreaHeat()
    {

        if (heatableList.Count < 1)
        {
            if (entropyCoroutine == null)
            {
                entropyCoroutine = StartCoroutine(HeatEntropy());
            }
            return;
        }

        if (entropyCoroutine != null)
        {
            StopCoroutine(entropyCoroutine);
            entropyCoroutine = null;
        }

        int total = 0;

        foreach (GameObject other in heatableList)
        {
            float distanceBetween = Vector2.Distance(this.transform.position, other.transform.position);
            Debug.Log($"Loop for: {other.transform.name} for {NAME}");

            //Check to make sure center of enemy is within the affecting proximity
            if (distanceBetween > heatDetectionRadius.radius)
            {
                continue;
            }

            //Offset to get max heat effects on player when enemy is standing right next to player
            float selfOffset = transform.parent.GetComponent<CircleCollider2D>().radius;
            float otherOffset = other.GetComponent<CircleCollider2D>().radius;
            float miniumDistanceBound = selfOffset + otherOffset;
            float heatProximityRatio = (heatDetectionRadius.radius - distanceBetween) / (heatDetectionRadius.radius - miniumDistanceBound);
            float otherHeatEmmissionLevel = other.transform.Find("HeatRadius").GetComponent<Heatable>().heatEmmission;


            total += (int)Mathf.Clamp((otherHeatEmmissionLevel * heatProximityRatio), 0f, otherHeatEmmissionLevel);
        }

        if (total > this.heatLevel && absorptionCoroutine == null)
        {
            absorptionCoroutine = StartCoroutine(AbsorbHeat(total));
        }
        else if (total < this.heatLevel && entropyCoroutine == null && absorptionCoroutine != null) 
        {
            StopCoroutine(absorptionCoroutine);
            entropyCoroutine = StartCoroutine(HeatEntropy());
            absorptionCoroutine = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        heatableList.Add(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        heatableList.Remove(collision.gameObject);
    }


}
