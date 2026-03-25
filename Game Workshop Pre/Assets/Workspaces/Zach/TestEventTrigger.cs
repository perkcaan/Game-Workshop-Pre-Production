using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEventTrigger : MonoBehaviour, ISwipeable
{
    public GameObject HitParent { get { return gameObject; } }
    [SerializeField] private EventTrigger _events;
    
    public void OnSwipe(Vector2 direction, float force, Collider2D collision, ref float knockbackMultiplier)
    {
        //Triggers all actions on the event with what was serialized
        _events.Trigger();

        //Triggers action at index 0 with what was serialized
        _events.Trigger(0);

        //Triggers action at index 0 with THESE values.
        _events.Trigger(0, 1, Color.cyan);
        
    }
}
