using System.Collections.Generic;
using UnityEngine;

public class PopupLabelPooler : Singleton<PopupLabelPooler>
{
    [SerializeField] private GameObject _popupLabelPrefab; //this probably shouldnt be here to be honest... move to pooler?
    public GameObject PopupLabelPrefab
    {
        get { return _popupLabelPrefab; }
    }
    [SerializeField] private int _maxPopupPoolSize = 8;
    
    private Queue<PopupLabel> _popupLabelPool = new Queue<PopupLabel>();

    public PopupLabel GetLabel()
    {
        PopupLabel label;
        if (_popupLabelPool.Count > 0)
        {
            label = _popupLabelPool.Dequeue();
            label.gameObject.SetActive(true);
        } else
        {
            GameObject prefab = PopupLabelPrefab;
            GameObject labelObject = Instantiate(prefab, transform);
            label = labelObject.GetComponent<PopupLabel>();   
        }

        return label;
    }

    public void ReturnLabel(PopupLabel label)
    {
        if (_popupLabelPool.Count > _maxPopupPoolSize)
        {
            Destroy(label.gameObject);
        } else
        {
            label.gameObject.SetActive(false);
            _popupLabelPool.Enqueue(label);
        }

    }
}
