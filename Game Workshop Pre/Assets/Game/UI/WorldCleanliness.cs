using UnityEngine;
using UnityEngine.UI;
public class WorldCleanliness : MonoBehaviour
{
    public static WorldCleanliness Instance;
    private Image cleanlinessMeter;
    public float startingWorldTrash;
    public float currentWorldTrash;
    void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
        cleanlinessMeter = GetComponent<Image>();
    }

    public void RemoveTrash()
    {
        currentWorldTrash--;
        cleanlinessMeter.fillAmount = currentWorldTrash / startingWorldTrash;
    }
}
