using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BoatRideScript : MonoBehaviour
{
    [Header("Boat Ride Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rockAmount = 0.5f;
    [SerializeField] private float rockSpeed = 1.5f;
    [SerializeField] private float rideDuration = 5f;

    private PlayerMovementController _playerController;
    private bool _isBoatRiding = false;
    private Vector3 _boatStartPosition;
    private Vector3 _playerStartPosition;

    void Start()
    {
        _boatStartPosition = transform.position;
    }

    void Update()
    {
        if (_isBoatRiding)
        {
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
            _playerController.transform.position = transform.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerMovementController player) && !_isBoatRiding)
        {
            _playerController = player;
            GrabPlayer();
            StartBoatRide(player);
        }
    }

    private void StartBoatRide(PlayerMovementController player)
    {
        _isBoatRiding = true;

        RockBoat();
        Invoke(nameof(EndBoatRide), rideDuration);
    }

    private void GrabPlayer()
    {
        _playerController.enabled = false;
        _playerController.transform.SetParent(transform);
    }

    private void RockBoat()
    {
        Sequence rockSequence = DOTween.Sequence();
        
        rockSequence.Append(transform.DOLocalMoveY(_boatStartPosition.y + rockAmount, rockSpeed / 2)
            .SetEase(Ease.InOutSine))
            .Append(transform.DOLocalMoveY(_boatStartPosition.y, rockSpeed / 2)
            .SetEase(Ease.InOutSine));

        rockSequence.SetLoops(-1);
    }

    private void EndBoatRide()
    {
        _isBoatRiding = false;

        if (_playerController != null)
        {
            _playerController.canSweep = true;
            _playerController.canSwipe = true;
            _playerController.canDash = true;
        }

        transform.DOKill();
    }
}
