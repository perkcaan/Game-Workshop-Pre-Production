using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Heatable : MonoBehaviour
{

    //Current Heat Level
    [Header("Objects Current Heat Level")]
    [SerializeField] public int heatLevel;

    //Rate at which heat level falls for actor
    [Header("Rate at which heat falls per second")]
    [SerializeField] int heatEntropy;

    //Max heat for actor before adverse affects
    [Header("Max Allowed Heat")]
    [SerializeField] public int heatThreshold;

    private readonly int PLAYER_BASE_HEAT_FLOOR = 0;
    private readonly float FLOOR_UPDATE_RATE = 0.5f;


    public UnityEvent onRoomEnter;

    private int heatFloor;
    private List<GameObject> heatableList;
    private CircleCollider2D heatLevelRadius;




    void Start()
    {
        heatableList = new List<GameObject>();
        heatLevelRadius = GetComponent<CircleCollider2D>();
        heatFloor = 0;
        StartCoroutine(HeatEntropy());
        UpdateHeatFloor();

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
        while (true)
        {
            yield return new WaitForSeconds(1f);
            LowerHeat(heatEntropy);
        }
        
    }

    private IEnumerator UpdateHeatFloor()
    {
        while (true)
        {
            yield return new WaitForSeconds(FLOOR_UPDATE_RATE);
            heatFloor = (int)Mathf.Clamp(heatFloor, PLAYER_BASE_HEAT_FLOOR, GetAreaHeat());
        }
    }

    private float GetAreaHeat()
    {
        return 0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
            
    }


}
