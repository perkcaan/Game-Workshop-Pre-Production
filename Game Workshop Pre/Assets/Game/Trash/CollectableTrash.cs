using System.Collections;
using UnityEngine;

public class CollectableTrash : Trash, IAbsorbable
{
    [SerializeField] private GameObject _trashBallPrefab;

    protected override void OnTrashMerge(Trash otherTrash)
    {
        base.OnTrashMerge(otherTrash);
        GameObject trashBallObject = Instantiate(_trashBallPrefab);
        trashBallObject.transform.position = transform.position;
        TrashBall trashBall = trashBallObject.GetComponent<TrashBall>();
        if (trashBall == null)
        {
            Debug.LogWarning("TrashBall prefab prepared incorrectly");
            Destroy(trashBallObject);
            return;
        }
        trashBall.Initialize(Size, _rigidBody.velocity);
        Destroy(gameObject);
    }

    public void OnAbsorbedByTrashBall(TrashBall trashBall)
    {
        trashBall.Size += Size;
        Destroy(gameObject);
    }

}
