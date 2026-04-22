using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class TrashEaterPlant : MonoBehaviour
{

    private TrashBall _heldTrashBall;

    [SerializeField] private int _timeToShoot = 10;
    [SerializeField] private int _timeOfRest = 10;
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private float _findTargetRadius = 20f;

    [SerializeField] private float _fireForce = 25f;

    private GameObject _player;
    private bool _resting;
    private bool _isAttacking = false;

    void FixedUpdate()
    {
        FindAndAttackPlayer(); // Attack player when they are found
        FreezePosition(); // Stop held trash ball from moving
    }

    private void GrabTrashBall(TrashBall tb)
    {
        _heldTrashBall = tb;

        _heldTrashBall.SetDecayPause(true); // prevent decay

        // Freeze Trash Ball in Plant Center
        _heldTrashBall.transform.SetParent(transform);
        _heldTrashBall.transform.localPosition = Vector3.zero;

    }

    // Fires Trash Ball
    IEnumerator FireTrashBall()
    {
        
        yield return new WaitForSeconds(_timeToShoot);

        if (_heldTrashBall == null)
        {

            _isAttacking = false;
            yield break;
        }

        if (_player == null)
        {

            _isAttacking = false;
            yield break;
        }

        Rigidbody2D rb = _heldTrashBall.Rigidbody;

        if (rb == null)
        {

            _isAttacking = false;
            yield break;
        }

        Vector2 direction = (_player.transform.position - transform.position).normalized;
        rb.linearVelocity = direction * _fireForce;

        _heldTrashBall.SetDecayPause(false);
        _heldTrashBall.transform.SetParent(null);

        _heldTrashBall = null;
        _player = null;

        StartCoroutine(RestingTime());
        _isAttacking = false;
    }

    IEnumerator RestingTime()
    {

        _resting = true;
        yield return new WaitForSeconds(_timeOfRest);
        _resting = false;
    }
    

    void OnTriggerEnter2D(Collider2D collision)
    {
        
        // If a Trashball collides and not resting, call GrabTrashBall on it
        TrashBall tb = collision.GetComponent<TrashBall>();
        if (tb != null && !_resting && _heldTrashBall == null)
        {
            GrabTrashBall(tb);
        }

    }


    // Search for the PLayer and Attack When Valid
    private void FindAndAttackPlayer()
    {

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _findTargetRadius, _targetLayer);
        foreach (var hit in hits)
        {
            if (hit.GetComponent<PlayerMovementController>())
            {
                _player = hit.gameObject;
            }
        }

        if (_player != null && !_resting && !_isAttacking && _heldTrashBall != null)
        {
            
            _isAttacking = true;
            StartCoroutine(FireTrashBall());
        }

    } 

    // Freeze trash ball position
    private void FreezePosition()
    {
       
        if (_heldTrashBall != null)
        {
            _heldTrashBall.Rigidbody.linearVelocity = new Vector2(0f, 0f);
            _heldTrashBall.transform.position = transform.position;
            


        }

    }

}
