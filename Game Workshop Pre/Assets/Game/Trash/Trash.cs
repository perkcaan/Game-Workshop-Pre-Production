using System;
using System.Collections;
using DG.Tweening;  
using JetBrains.Annotations;
using UnityEngine;


// An abstract class for Trash. All types of Trash can inherit from this.
[RequireComponent(typeof(Rigidbody2D))]
public abstract class Trash : MonoBehaviour, IAbsorbable, IHeatable, ICleanable
{
    [SerializeField] protected GameObject _trashBallPrefab;
    [SerializeField] protected float _explosionMultiplier;

    [Header("Trash")]
    [SerializeField] protected int _size;
    public int Size { get { return _size; } }
    public TrashMaterial trashMaterial;
    public TrashMaterial TrashMat { get { return trashMaterial; } }
    public int trashMaterialWeight = 1;
    public int TrashMatWeight { get { return trashMaterialWeight; } }
    private FMOD.Studio.EventInstance _sweepSoundInstance;

    [SerializeField] protected int _pointValue;
    private bool _pointsConsumed = false;
    protected Room _parentRoom;
    public Rigidbody2D _rigidBody;
    protected SpriteRenderer _spriteRenderer;
    private float soundCooldown = 1f;

    protected bool _isDestroyed = false;

    protected virtual void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (_pointValue <= 0) _pointValue = 1;
    }
    protected void CreateTrashBall()
    {
        if (!_rigidBody.simulated) return;
        GameObject trashBallObject = Instantiate(_trashBallPrefab);
        trashBallObject.transform.position = transform.position;
        if (_parentRoom.ActiveRoomDrawer != null)
        {
            trashBallObject.transform.parent = _parentRoom.ActiveRoomDrawer.transform;
        } else
        {
            Debug.LogWarning("TrashBall attempted to be made in an unactive room (" + _parentRoom.name + "). This shouldn't happen. Make sure Room Drawers are set up properly.");
            trashBallObject.transform.parent = _parentRoom.transform;
        }
        
        TrashBall trashBall = trashBallObject.GetComponent<TrashBall>();

        if (trashBall == null)
        {
            Debug.LogWarning("TrashBall prefab prepared incorrectly");
            Destroy(trashBallObject);
            return;
        }

        GivePoints();
        gameObject.SetActive(false);
        trashBall.AbsorbObject(this);
        trashBall.Rigidbody.velocity = _rigidBody.velocity;
    }

    // IAbsorbable
    public virtual bool OnAbsorbedByTrashBall(TrashBall trashBall, Vector2 ballVelocity, int ballSize, bool forcedAbsorb)
    {
        if (_isDestroyed) return false;
        
        if (forcedAbsorb || (Size <= trashBall.Size && isActiveAndEnabled && _rigidBody.simulated))
        {
            GivePoints();
            _rigidBody.simulated = false;
            if (forcedAbsorb) return true;
            
            Vector2 direction = transform.position - trashBall.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion particleRotation = Quaternion.Euler(0f, 0f, angle-45);    
            if (Size > 4)
            {
                ParticleManager.Instance.Play("TrashAbsorbed", transform.position, particleRotation, null, null, 1.5f);
            } else 
            {
                ParticleManager.Instance.Play("TrashAbsorbed", transform.position, particleRotation, null, null, 1f);
            }
            
            PopupLabel.CreatePlusLabel(transform.position, TrashMat.color, Size);
            
            return true;
        }
        return false;
    }

    public void OnTrashBallRelease(TrashBall trashBall)
    {
        gameObject.SetActive(true);
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutQuad);

        _rigidBody.simulated = true;
        transform.position = trashBall.transform.position;

        float explosionForce = (float)(Math.Sqrt(trashBall.Size) * _explosionMultiplier);
        Vector2 randomForce = new Vector2(UnityEngine.Random.Range(-explosionForce, explosionForce), UnityEngine.Random.Range(-explosionForce, explosionForce));
        _rigidBody.velocity = randomForce;
    }

    public void OnTrashBallDestroy()
    {
        if (_isDestroyed) return;
        _isDestroyed = true;

        if (_parentRoom != null) _parentRoom.ObjectCleaned(this);
        Destroy(gameObject);
    }

    public void PrepareIgnite(HeatMechanic heat)
    {
        if (_isDestroyed) return;
        _isDestroyed = true;

        foreach (Collider2D col in GetComponentsInChildren<Collider2D>()) col.enabled = false;
        _rigidBody.velocity = Vector2.zero;
        _rigidBody.simulated = false;
        if(_parentRoom != null) _parentRoom.ObjectCleaned(this);
    }
    
    public void OnIgnite(HeatMechanic heat)
    {
        Destroy(gameObject);
    }

    public void SetRoom(Room room)
    {
        _parentRoom = room;
    }
    
    private void GivePoints()
    {
        if (!_pointsConsumed)
        {
            ScoreBehavior.SendScore?.Invoke(_pointValue);
            StartCoroutine(Sound());
            _pointsConsumed = true;
        }
    }

    public IEnumerator Sound()
    {
        //AudioManager.Instance.ModifyParameter("Points", "Point", Math.Clamp(_pointValue, 0, 50), "Local");
        //Debug.Log("Played Points Sound: "+_pointValue);
        //AudioManager.Instance.Play("Points", transform.position);
        yield return new WaitForSeconds(soundCooldown);
        soundCooldown = 1f;
    }
}
