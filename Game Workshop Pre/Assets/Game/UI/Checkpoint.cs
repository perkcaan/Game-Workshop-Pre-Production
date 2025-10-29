using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Room assignedRoom;

    private CheckpointManager Checkpoint_Manager;

    private DistrictManager District_Manager;

    private bool checkGained = false;

    public bool starterPoint;

    private void Start()
    {
        Checkpoint_Manager = FindObjectOfType<CheckpointManager>();
        District_Manager = FindObjectOfType<DistrictManager>();

        if (Checkpoint_Manager == null)
        {
            Debug.LogWarning("No CheckpointManager found");
        }

        if (starterPoint && !checkGained)
        {
          
            Checkpoint_Manager.checkpoints.Add(this);
            checkGained = true;

        }

        

    }

    private void Update()
    {

        if (!starterPoint)
        {


            if (District_Manager.FocusedRoom != null && (District_Manager.FocusedRoom == assignedRoom))
            {
                
                if (!checkGained)
                {
                    Checkpoint_Manager.checkpoints.Add (this);
                    checkGained = true;
                }

                Checkpoint_Manager.SetActiveCheckpoint(this);

            }

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && Checkpoint_Manager.respawnChoice && Checkpoint_Manager.activeCheckpoint != this)
        {
            Checkpoint_Manager.SetActiveCheckpoint(this);
        }
    }

}