using System;
using UnityEngine;


// An abstract class for Trash. All types of Trash can inherit from this.
[RequireComponent(typeof(Rigidbody2D))]
public abstract class Trash : MonoBehaviour, IAbsorbable, IHeatable
{
    [SerializeField] protected GameObject _trashBallPrefab;
    
    [SerializeField] protected float _explosionMultiplier;

    [Header("Trash")]
    public int Size;
    public TrashMaterial trashMaterial;
    //Components
    protected Rigidbody2D _rigidBody;
    // Unity methods
    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
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

        trashBall.AbsorbTrash(this);
        trashBall.GetComponent<Rigidbody2D>().velocity = _rigidBody.velocity;
        gameObject.SetActive(false);
    }

    public virtual void OnAbsorbedByTrashBall(TrashBall trashBall, float absorbingPower, bool forcedAbsorb)
    {
        if (forcedAbsorb || (Size <= trashBall.Size && isActiveAndEnabled))
        {
            trashBall.AbsorbTrash(this);
        }
    }

    public void OnTrashBallExplode(TrashBall trashBall)
    {
        gameObject.SetActive(true);
        transform.position = trashBall.transform.position;
        float explosionForce = (float)(Math.Sqrt(trashBall.Size) * _explosionMultiplier);
        Vector2 randomForce = new Vector2(UnityEngine.Random.Range(-explosionForce, explosionForce), UnityEngine.Random.Range(-explosionForce, explosionForce));
        _rigidBody.velocity = randomForce;
    }

    public void OnIgnite(HeatMechanic heat)
    {
        PlayerState.Instance.trashDeleted.Invoke(Size);
        Destroy(gameObject);
    }

    public void OnTrashBallIgnite()
    {
        PlayerState.Instance.trashDeleted.Invoke(Size);
        Destroy(gameObject);
    }
}
