using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BoatRideScript : MonoBehaviour
{
    [Header("Boat Ride Settings")]
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float rockAmount = 0.5f;
    [SerializeField] float rockSpeed = 1.5f;
    [SerializeField] Transform endPoint;
    [SerializeField] float _playerDepositY;
    [SerializeField] float _grabSpeed;
    [SerializeField] float playerJumpHeight;

    private PlayerMovementController _playerController;
    private bool _isBoatRiding = false;
    private Vector3 _boatStartPosition;

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
            _playerController.enabled = false;
            GrabPlayer(player);
        }
    }

    private void StartBoatRide(PlayerMovementController player)
    {
        _isBoatRiding = true;
        _playerController.GetComponentInChildren<Animator>().SetInteger("OnBoat", 2);
        RockBoat();
        float rideDuration = Vector3.Distance(transform.position, endPoint.position) / moveSpeed;
        Invoke(nameof(EndBoatRide), rideDuration);
    }

    void GrabPlayer(PlayerMovementController player)
    {
        _playerController.GetComponentInChildren<Animator>().SetInteger("OnBoat", 1);
        _playerController.enabled = false;
    
        Sequence grabSequence = DOTween.Sequence();
        grabSequence.Append(_playerController.transform.DOLocalMove(transform.position, _grabSpeed));
        grabSequence.OnComplete(() => {StartBoatRide(player);});
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
        _playerController.GetComponentInChildren<Animator>().SetInteger("OnBoat", 0);
        _isBoatRiding = false;
        this.enabled = false;
        GetComponent<Collider2D>().enabled = false;
        if (_playerController != null)
        {
            _playerController.enabled = true;
            _playerController.transform.DOLocalMoveY(transform.position.y + _playerDepositY, _grabSpeed);
        }
        transform.DOKill();
    }
}
