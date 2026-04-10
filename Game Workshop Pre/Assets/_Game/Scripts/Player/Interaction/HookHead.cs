using UnityEngine;

public class HookHead : MonoBehaviour
{
    private HookHandler _handler;

    public void Initialize(HookHandler handler)
    {
        _handler = handler;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _handler.OnHookHit(other);
    }
}