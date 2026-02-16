using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : MonoBehaviour,ISwipeable
    
{
    [Tooltip("The ball size necessary to break this wall")]
    [SerializeField] int breakSize;
    [Tooltip("The ball speed necessary to break this wall")]
    [SerializeField] float breakSpeed;

    [SerializeField] float _health;
    [SerializeField] float _onDamagedShakeForce;
    private float _shakeSpeed = 0.110f;
    private SpriteRenderer _sprite;
    // Start is called before the first frame update
    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_health <= 0) 
        {
            Destroy(this.gameObject);
        }

    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        TrashBall trashBall = collision.gameObject.GetComponent<TrashBall>();

        //if (trashBall != null && (trashBall.Size > breakSize) && ((trashBall.RigidBody.velocity.magnitude * 10) > breakSpeed))
        //{
        //    Destroy(this.gameObject);
        //}
    }

    public void OnSwipe(Vector2 direction, float force)
    {
        TakeDamage(3, direction, force);
        Debug.Log(_health);
    }

    public void TakeDamage(int damage, Vector2 direction, float force)
    {
        _health -= damage;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(_sprite.transform.DOLocalMove(direction.normalized * _onDamagedShakeForce * damage, _shakeSpeed));
        //sequence.Append(_sprite.transform.DOLocalMove(-direction.normalized * _onDamagedShakeForce * damage / 4, _shakeSpeed));
        sequence.Append(_sprite.transform.DOLocalMove(Vector3.zero, _shakeSpeed));

        
        
    }

    public void OnSwipe(Vector2 direction, float force, Collider2D collision)
    {
        throw new System.NotImplementedException();
    }
}
