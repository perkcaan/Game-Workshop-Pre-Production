using UnityEngine;

public class LevelObjectContainer : MonoBehaviour
{
    public GameObject GatesContainer
    {
        get
        {
            Transform gates = transform.Find("Gates");

            if (gates != null) return gates.gameObject;

            GameObject newGates = new GameObject("Gates");
            newGates.transform.SetParent(transform);
            newGates.transform.localPosition = Vector3.zero;

            return newGates;
        }
    }
}
