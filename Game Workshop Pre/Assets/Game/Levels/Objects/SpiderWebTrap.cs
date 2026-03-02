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

    // Web Size
    [SerializeField] private float _webScale = 1f;

    void Awake()
    {
        gameObject.transform.localScale = new UnityEngine.Vector3(_webScale, _webScale, 1f);
        if (gameObject.TryGetComponent(out Collider2D _collider))
        {
            _collider.transform.localScale = new UnityEngine.Vector3(_webScale, _webScale, 1f);
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
            if (TrashBallHasPaper(tb))
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Trash Ball
        if (collision.TryGetComponent(out TrashBall tb))
        {
            // Check for paper every frame while inside
            if (TrashBallHasPaper(tb))
            {
                Destroy(gameObject); // If paper is found, destroy web
                return;
            }

            // Damepning
            float slowPercentPerSecond = _trashBallSpeedReduction;
            float dampenFactor = Mathf.Clamp01(1f - slowPercentPerSecond * Time.fixedDeltaTime);

            tb.Rigidbody.velocity *= dampenFactor;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Player
        if (collision.TryGetComponent(out PlayerMovementController pmc))
        {
            pmc.RemoveWebSlow(_playerSpeedReduction);
        }
    }

    
    private bool TrashBallHasPaper(TrashBall tb)
    {
        foreach (IAbsorbable absorbable in tb.AbsorbedObjects)
        {
            if (absorbable.TrashMat != null && absorbable.TrashMat.name == "Paper")
            {
                return true;
            }
        }

        return false;
    }

}
