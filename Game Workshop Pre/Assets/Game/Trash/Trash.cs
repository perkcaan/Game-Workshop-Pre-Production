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
    public int trashMaterialWeight = 1;
    private FMOD.Studio.EventInstance _sweepSoundInstance;

    [SerializeField] protected int _pointValue;
    private bool _pointsConsumed = false;
    public static Action<int> SendScore;

    protected Room _parentRoom;
    public Rigidbody2D _rigidBody;
    protected SpriteRenderer _spriteRenderer;
    private float soundCooldown = 1f;

    protected bool _isDestroyed = false;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_pointValue <= 0) _pointValue = 1;
    }
    protected void CreateTrashBall()
    {
        if (!_rigidBody.simulated) return;
        GameObject trashBallObject = Instantiate(_trashBallPrefab);
        trashBallObject.transform.position = transform.position;
        TrashBall trashBall = trashBallObject.GetComponent<TrashBall>();

        if (trashBall == null)
        {
            Debug.LogWarning("TrashBall prefab prepared incorrectly");
            Destroy(trashBallObject);
            return;
        }

        GivePoints();
        _rigidBody.simulated = false;
        gameObject.SetActive(false);
        trashBall.AbsorbTrash(this);
        trashBall.GetComponent<Rigidbody2D>().velocity = _rigidBody.velocity;
    }

    public virtual void OnAbsorbedByTrashBall(TrashBall trashBall, Vector2 ballVelocity, int ballSize, bool forcedAbsorb)
    {   
        if (forcedAbsorb || (Size <= trashBall.Size && isActiveAndEnabled && _rigidBody.simulated))
        {
            GivePoints();
            _rigidBody.simulated = false;
            trashBall.AbsorbTrash(this);
        }
    }

    public void OnTrashBallExplode(TrashBall trashBall)
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

    public void PrepareIgnite(HeatMechanic heat)
    {
        foreach (Collider2D col in GetComponentsInChildren<Collider2D>()) col.enabled = false;
        _rigidBody.velocity = Vector2.zero;
        _rigidBody.simulated = false;
    }
    
    public void OnIgnite(HeatMechanic heat)
    {
        if (_isDestroyed) return;
        _isDestroyed = true;

        if(_parentRoom != null) _parentRoom.ObjectCleaned(this);
        
        Destroy(gameObject);
    }

    public void OnTrashBallIgnite()
    {;
        if (_isDestroyed) return;
        _isDestroyed = true;

        if (_parentRoom != null) _parentRoom.ObjectCleaned(this);
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
            SendScore?.Invoke(_pointValue);
            

            StartCoroutine(Sound());



            _pointsConsumed = true;
        }

        
    }

    public IEnumerator Sound()
    {
        bool playing = false;

        if (!playing) 
        { 
            AudioManager.Instance.Play("Points", transform.position);
            yield return new WaitForSeconds(soundCooldown);


        }
            

        switch (_pointValue)
        {
            case int n when (n <= 10):
                AudioManager.Instance.ModifyParameter("Points", "Point", 10, "Local");
                Debug.Log("Played Points Sound: 10");
                playing = false;
                break;
            case int n when (n <= 20):
                AudioManager.Instance.ModifyParameter("Points", "Point", 20, "Local");
                Debug.Log("Played Points Sound: 20");
                break;

            case int n when (n <= 30):
                AudioManager.Instance.ModifyParameter("Points", "Point", 30, "Local");
                Debug.Log("Played Points Sound: 30");
                break;

            case int n when (n <= 40):
                AudioManager.Instance.ModifyParameter("Points", "Point", 40, "Local");
                Debug.Log("Played Points Sound: 40");
                break;

            case int n when (n <= 50):
                AudioManager.Instance.ModifyParameter("Points", "Point", 30, "Local");
                Debug.Log("Played Points Sound: 40");
                break;

            default:
                if (_pointValue > 50)
                {
                    AudioManager.Instance.ModifyParameter("Points", "Point", 50, "Local");
                    Debug.Log("Played Points Sound: 50");
                }
                break;
        }
        soundCooldown = 1f;
    }
}
