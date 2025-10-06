using System.Collections;
using UnityEngine;

public class CollectableTrash : Trash, IAbsorbable
{
    [SerializeField] GameObject _trashBallPrefab;
    [SerializeField] float _velocityToTurnIntoTrash;
    [SerializeField] float _explosionMultiplier;
    private bool mergable = true;
    void Update()
    {
        if (_rigidBody.velocity.magnitude > _velocityToTurnIntoTrash * Size && mergable)
        {
            CreateTrashBall();
        }
    }

    protected void CreateTrashBall()
    {
        GameObject trashBallObject = Instantiate(_trashBallPrefab);
        trashBallObject.transform.position = transform.position;
        TrashBall trashBall = trashBallObject.GetComponent<TrashBall>();

        if (trashBall == null)
        {
            Debug.LogWarning("TrashBall prefab prepared incorrectly");
            Destroy(trashBallObject);
            return;
        }

        trashBall.absorbedObjects.Add(this);
        trashBall.Size = Size;
        gameObject.SetActive(false);
    }

    public void OnAbsorbedByTrashBall(TrashBall trashBall, float absorbingPower, bool forcedAbsorb)
    {
        if (forcedAbsorb || (Size <= trashBall.Size && isActiveAndEnabled))
        {
            trashBall.absorbedObjects.Add(this);
            trashBall.Size += Size;
            gameObject.SetActive(false);
        }

    }

    public void OnTrashBallExplode(TrashBall trashBall)
    {
        StartCoroutine(MergeDelay());
        transform.position = trashBall.transform.position;
        float explosionForce = (1 - trashBall.Size) * _explosionMultiplier;
        Vector2 randomForce = new Vector2(Random.Range(-explosionForce, explosionForce), Random.Range(-explosionForce, explosionForce));
        _rigidBody.velocity = randomForce;
    }

    public IEnumerator MergeDelay()
    {
        mergable = false;
        yield return new WaitForSeconds(1.5f);
        mergable = true;
    }
}
