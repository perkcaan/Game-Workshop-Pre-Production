using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashPlatform : StandingGround
{
    public float floatingDuration = 1f;
    [SerializeField] SpriteRenderer spriteRenderer;
    void Update()
    {
        floatingDuration -= Time.deltaTime;
        if (floatingDuration <= 0f)
        {
            foreach (GroundedMechanic gm in groundedObjects)
            {
                gm.IsGrounded--;
            }
            groundedObjects.Clear();
            Destroy(gameObject);
        }
    }
}
