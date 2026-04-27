using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class SpiderWebTrap : MonoBehaviour
{

    // Speed reduction for player
    [SerializeField] private int _playerSpeedReduction;

    // Drag increase for trash ball
    [SerializeField] private float _trashBallSpeedReduction = 5f;

    [SerializeField] private float _timeUntilDecay = 10f;
    private float _decayTimer = 0;
    // Web Size
    [SerializeField] private float _webScale = 1f;

    void Awake()
    {
        gameObject.transform.localScale = new UnityEngine.Vector3(_webScale, _webScale, 1f);
        if (gameObject.TryGetComponent(out Collider2D _collider))
        {
            _collider.transform.localScale = new UnityEngine.Vector3(_webScale, _webScale, 1f);
        }
        _decayTimer = _timeUntilDecay;
    }

    private void Update()
    {
        _decayTimer -= Time.deltaTime;
        if (_decayTimer <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Player
        if (collision.TryGetComponent(out PlayerMovementController pmc))
        {
            pmc.ApplyWebSlow(_playerSpeedReduction);
        }

        // Trash Ball
        if (collision.TryGetComponent(out TrashBall tb))
        {
            if (tb.isBurning)
            {
                Destroy(gameObject);
            }
        }
    }

    // private void OnTriggerStay2D(Collider2D collision)
    // {
    //     // Trash Ball
    //     if (collision.TryGetComponent(out TrashBall tb))
    //     {
    //         // Check for paper every frame while inside


    //         // Damepning
    //         float slowPercentPerSecond = _trashBallSpeedReduction;
    //         float dampenFactor = Mathf.Clamp01(1f - slowPercentPerSecond * Time.fixedDeltaTime);

    //         tb.Rigidbody.linearVelocity *= dampenFactor;
    //     }
    // }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Player
        if (collision.TryGetComponent(out PlayerMovementController pmc))
        {
            pmc.RemoveWebSlow(_playerSpeedReduction);
        }
    }

}