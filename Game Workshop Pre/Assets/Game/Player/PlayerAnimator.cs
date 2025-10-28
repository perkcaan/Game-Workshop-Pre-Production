using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.U2D.Animation;


public enum PlayerAnimationState
{
    Idle,
    Sweeping,
    HoldingSwipe,
    Swiping,
    Dash,
    Tumble,
    Absorbed
}
public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] SpriteLibraryAsset bodyAnimation;
    [SerializeField] SpriteLibraryAsset headAnimation;
    [SerializeField] SpriteLibraryAsset legsAnimation;
    [SerializeField] SpriteLibraryAsset armsAnimation;
    [SerializeField] SpriteRenderer body;
    [SerializeField] SpriteRenderer head;
    [SerializeField] SpriteRenderer legs;
    [SerializeField] SpriteRenderer arms;
    [SerializeField] float animationSpeed;
    private PlayerAnimationState playerState;

    private float currentMovementSpeed;
    private int rotation = 0;

    private float animationTimer;
    private int animationStep;

    void Start()
    {

    }

    void Update()
    {
        animationTimer += Time.deltaTime;
        if (animationTimer > animationSpeed)
        {
            animationTimer = 0;
            animationStep++;
            if (animationStep > 3) animationStep = 0;
        }
        PlayAnimation();
    }

    void PlayAnimation()
    {
        body.transform.localPosition = new Vector3(0, 0, body.transform.localPosition.z);
        head.transform.localPosition = new Vector3(0, 0.125f, head.transform.localPosition.z);
        legs.transform.localPosition = new Vector3(0, -0.125f, legs.transform.localPosition.z);
        arms.transform.localPosition = new Vector3(0, 0, arms.transform.localPosition.z);
        
        switch (playerState)
        {
            case PlayerAnimationState.Idle:
                IdleAnimation();
                break;
            case PlayerAnimationState.HoldingSwipe:
                HoldingSwipeAnimation();
                break;
            case PlayerAnimationState.Swiping:
                SwipeAnimation();
                break;
            case PlayerAnimationState.Sweeping:
                SweepAnimation();
                break;
            case PlayerAnimationState.Dash:
                DashAnimation();
                break;
            default:
                break;
        }
    }

    void IdleAnimation()
    {
        string mirroredRotation = MirroredRotation().ToString();
        body.sprite = bodyAnimation.GetSprite("Idle", mirroredRotation);
        head.sprite = headAnimation.GetSprite("Idle", mirroredRotation);

        if (currentMovementSpeed < 0.5f)
        {
            legs.sprite = legsAnimation.GetSprite("Idle", mirroredRotation);
            arms.sprite = armsAnimation.GetSprite("Idle", mirroredRotation);
            IdleHeadBob();
        }
        else
        {
            if ((animationStep + 1) % 2 == 0)
            {
                legs.sprite = legsAnimation.GetSprite("Idle", mirroredRotation);
                arms.sprite = armsAnimation.GetSprite("Idle", mirroredRotation);
            }
            else
            {
                mirroredRotation += (animationStep / 2).ToString();
                legs.sprite = legsAnimation.GetSprite("Run", mirroredRotation);
                arms.sprite = armsAnimation.GetSprite("Run", mirroredRotation);
            }
        }
    }
    
    void IdleHeadBob()
    {
        switch (animationStep)
        {
            case 0:
                head.transform.localPosition += new Vector3(0, 0);
                arms.transform.localPosition += new Vector3(0, 0);
                break;
            case 1:
                head.transform.localPosition += new Vector3(0, -0.0625f);
                arms.transform.localPosition += new Vector3(0, 0);
                break;
            case 2:
                head.transform.localPosition += new Vector3(0, -0.0625f);
                arms.transform.localPosition += new Vector3(0, -0.0625f);
                break;
            case 3:
                head.transform.localPosition += new Vector3(0, 0);
                arms.transform.localPosition += new Vector3(0, -0.0625f);
                break;
        }
    }
    void HoldingSwipeAnimation()
    {
        
    }
    void SwipeAnimation()
    {

    }
    void SweepAnimation()
    {

    }
    void DashAnimation()
    {

    }

    private int MirroredRotation()
    {
        int idleRotation = rotation;
        if (rotation > 4)
        {
            idleRotation = 8 - rotation;
            body.flipX = false;
            head.flipX = false;
            legs.flipX = false;
            arms.flipX = false;
        }
        else
        {
            body.flipX = true;
            head.flipX = true;
            legs.flipX = true;
            arms.flipX = true;
        }
        return idleRotation;
    }

    public void SetSpeed(float speed)
    {
        currentMovementSpeed = speed;
    }

    public void SetRotation(float setRotation)
    {
        rotation = ((int)Math.Round((setRotation + 180) / 45f) + 6) % 8;
    }

    public void ChangeState(PlayerAnimationState newState, bool isTrue)
    {
        if (isTrue)
        {
            playerState = newState;
        }
        else if (playerState == newState)
        {
            playerState = PlayerAnimationState.Idle;
        }
    }
}
