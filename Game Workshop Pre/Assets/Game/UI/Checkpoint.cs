using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Room assignedRoom;

    private CheckpointManager _checkpointManager;

    private DistrictManager _districtManager;

    private bool checkGained = false;

    public bool starterPoint;

    private void Start()
    {
        _checkpointManager = CheckpointManager.Instance;
        _districtManager = DistrictManager.Instance;

        if (_checkpointManager == null)
        {
            Debug.LogWarning("No CheckpointManager found");
        }

        if (starterPoint && !checkGained)
        {
          
            _checkpointManager.checkpoints.Add(this);
            checkGained = true;

        }

        

    }

    private void Update()
    {

        if (!starterPoint)
        {


            if (_districtManager.FocusedRoom != null && (_districtManager.FocusedRoom == assignedRoom))
            {
                
                if (!checkGained)
                {
                    _checkpointManager.checkpoints.Add (this);
                    checkGained = true;
                }

                _checkpointManager.SetActiveCheckpoint(this);

            }

        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent(out PlayerMovementController player) && _checkpointManager.respawnChoice && _checkpointManager.activeCheckpoint != this)
        {
            _checkpointManager.SetActiveCheckpoint(this);
        }
    }

    // called when checkpoint is gone to
    public void OnGoTo()
    {
        assignedRoom.TriggerRoomClose();
    }

}