using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class BodyPart : MonoBehaviour
{
    [SerializeField] SpriteLibraryAsset spriteAnimation;
    public SpriteRenderer sr;
    private Vector3 basePosition;
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        basePosition = transform.localPosition;
    }

    public void Reset()
    {
        transform.localPosition = basePosition;
    }

    public void OffsetSpriteOnStep(int animationStep, Vector2 step1, Vector2 step2, Vector2 step3, Vector2 step4)
    {
        transform.localPosition = basePosition;
        switch (animationStep)
        {
            case 0:
                transform.localPosition += (Vector3)step1 * 0.0625f;
                break;
            case 1:
                transform.localPosition += (Vector3)step2 * 0.0625f;
                break;
            case 2:
                transform.localPosition += (Vector3)step3 * 0.0625f;
                break;
            case 3:
                transform.localPosition += (Vector3)step4 * 0.0625f;
                break;
        }
    }

    public void SetSprite(string id, int rotation)
    {
        Reset();
        sr.flipX = true;
        if (rotation < 1) sr.flipX = false;
        sr.sprite = spriteAnimation.GetSprite(id, "" + Math.Abs(rotation));
    }
    
    public void SetSpriteOnStep(int animationStep, string id, int rotation)
    {
        sr.flipX = true;
        if (rotation < 0) sr.flipX = false;
        string spriteID = Math.Abs(rotation) + "" + animationStep;
        sr.sprite = spriteAnimation.GetSprite(id, "" + spriteID);
    }
}
