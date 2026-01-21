using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompactorObject : MonoBehaviour
{

    [SerializeField] private float minSize;

    [SerializeField] private TrashCubeObject trashCubePrefab;


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out TrashBall trashBall))
        {
            if (trashBall.Size > minSize)
            {

                // Transfer over materials and size to new trash cube
                TrashCubeObject trashCubeInstance = Instantiate(trashCubePrefab);
                trashCubeInstance.absorbedObjects = trashBall.absorbedObjects;
                trashCubeInstance.cubeSize = trashBall.Size;

                trashCubeInstance.transform.position = transform.position;

                // Delete trash ball (*needs new method)
                Destroy(trashBall.gameObject);

            }

        }
    }
}
