using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionScene : MonoBehaviour
{
    [SerializeField] private string _sceneName;
    private SpriteRenderer _spriteRenderer;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // this isnt very safe, make it safer later
        if (collider.TryGetComponent(out PlayerMovementController player))
        {
            SceneManager.LoadScene(_sceneName);
        }
    }
}
