using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

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
    [SerializeField] BodyPart body;
    [SerializeField] BodyPart head;
    [SerializeField] BodyPart hat;
    [SerializeField] BodyPart face;
    [SerializeField] BodyPart legs;
    [SerializeField] BodyPart arms;
    [SerializeField] BodyPart broom;
    [SerializeField] float animationSpeed;
    [SerializeField] float runAnimationSpeedThreshold;
    private PlayerAnimationState playerState;
    private float currentMovementSpeed;
    private int rotation;
    private int mirroredRotation;
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
        switch (playerState)
        {
            case PlayerAnimationState.Idle:
                broom.sr.enabled = false;
                IdleAnimation();
                break;
            case PlayerAnimationState.HoldingSwipe:
                broom.sr.enabled = true;
                HoldingSwipeAnimation();
                break;
            case PlayerAnimationState.Swiping:
                broom.sr.enabled = true;
                SwipeAnimation();
                break;
            case PlayerAnimationState.Sweeping:
                broom.sr.enabled = true;
                SweepAnimation();
                break;
            case PlayerAnimationState.Dash:
                broom.sr.enabled = false;
                DashAnimation();
                break;
            default:
                break;
        }
    }

    void BaseMoveAnimation()
    {
        body.SetSprite("Idle", mirroredRotation);
        head.SetSprite("Idle", mirroredRotation);
        hat.SetSprite("Idle", mirroredRotation);
        face.SetSprite("Idle", mirroredRotation);
        arms.SetSprite("Idle", mirroredRotation);
        legs.SetSprite("Idle", mirroredRotation);

        if (currentMovementSpeed > runAnimationSpeedThreshold && animationStep % 2 == 0)
        {
            arms.SetSpriteOnStep(animationStep / 2, "Run", mirroredRotation);
            legs.SetSpriteOnStep(animationStep / 2, "Run", mirroredRotation);
        }
    }

    void IdleAnimation()
    {
        BaseMoveAnimation();
        if (currentMovementSpeed < runAnimationSpeedThreshold)
        {
            head.OffsetSpriteOnStep(animationStep,
            Vector2.zero, Vector2.down, Vector2.down, Vector2.zero);
            arms.OffsetSpriteOnStep(animationStep,
            Vector2.zero, Vector2.zero, Vector2.down, Vector2.down);
        }
    }

    void HoldingSwipeAnimation()
    {
        BaseMoveAnimation();

        int offsetRotation = mirroredRotation + 1;
        if (mirroredRotation == 4)

        head.SetSprite("Idle", offsetRotation);
        arms.SetSprite("Sweep", mirroredRotation);
        broom.SetSprite("Sweep", mirroredRotation);
    }
    
    void SwipeAnimation()
    {

    }

    void SweepAnimation()
    {
        BaseMoveAnimation();
        arms.SetSprite("Sweep", mirroredRotation);
        broom.SetSprite("Sweep", mirroredRotation);

        int Xoffset = 0;
        int Yoffset = 0;
        if (Math.Abs(mirroredRotation) > 2) Yoffset = 1;
        else if (Math.Abs(mirroredRotation) < 2) Yoffset = -1;
        if (rotation > 4) Xoffset = -1;
        else if (rotation > 0 && rotation < 4) Xoffset = 1;

        arms.OffsetSpriteOnStep(animationStep,
        Vector2.zero, new Vector2(Xoffset, Yoffset), Vector2.zero, new Vector2(Xoffset, Yoffset));
        broom.OffsetSpriteOnStep(animationStep,
        Vector2.zero, new Vector2(Xoffset, Yoffset), Vector2.zero, new Vector2(Xoffset, Yoffset));
    }

    void DashAnimation()
    {

    }

    private void MirroredRotation()
    {
        if (rotation > 4)
        {
            mirroredRotation = 8 - rotation;
            body.sr.flipX = false;
            head.sr.flipX = false;
            legs.sr.flipX = false;
            arms.sr.flipX = false;
            broom.sr.flipX = false;
        }
        else
        {
            body.sr.flipX = true;
            head.sr.flipX = true;
            legs.sr.flipX = true;
            arms.sr.flipX = true;
            broom.sr.flipX = true;
        }
    }

    public void SetSpeed(float speed)
    {
        currentMovementSpeed = speed;
    }

    public void SetRotation(float setRotation)
    {
        rotation = ((int)Math.Round((setRotation + 180) / 45f) + 6) % 8;
        mirroredRotation = rotation;
        if (mirroredRotation > 4) mirroredRotation -= 8;
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
