using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompactorObject : MonoBehaviour
{
    [SerializeField] float minTrashBallSize = 1f;    
    [SerializeField] TrashCubeObject trashCubePrefab;
    private List<TrashCubeObject> cubeChildren = new List<TrashCubeObject>();

    [Header("Trash Cube Properties")]
    [SerializeField] int maxTrashCubesAtOnce = 1;
    [SerializeField] float _trashPlatformDuration = 3;
    [SerializeField] int _maxTrashPlatforms = 3;
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out TrashBall trashBall))
        {
            cubeChildren.RemoveAll(item => item == null);
            if (trashBall.Size > minTrashBallSize && cubeChildren.Count < maxTrashCubesAtOnce)
            {
                // Transfer over materials and size to new trash cube
                TrashCubeObject trashCubeInstance = Instantiate(trashCubePrefab);
                trashCubeInstance.absorbedObjects = trashBall.absorbedObjects;
                trashCubeInstance.Size = trashBall.Size;
                trashCubeInstance.transform.position = transform.position;
                trashCubeInstance.trashPlatformDuration = _trashPlatformDuration;
                trashCubeInstance.maxNumberOfTrashPlatforms = _maxTrashPlatforms;
                cubeChildren.Add(trashCubeInstance);
                Destroy(trashBall.gameObject);
            }
        }
    }
}
