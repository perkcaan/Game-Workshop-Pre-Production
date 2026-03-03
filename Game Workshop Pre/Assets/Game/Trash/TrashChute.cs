using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashChute : MonoBehaviour
{
    [SerializeField] List<Trash> _possibleTrash;
    [SerializeField] float _dropInterval = 5f;
    [SerializeField] private float _maxRandomTimeOffset = 1f;
    [SerializeField] private bool _canDropAutomatically = true;

    private float _currentTime = 0f;
    private Room _parentRoom;
    private Collider2D _dropArea;
    private List<Trash> _trashInDropArea = new List<Trash>();

    private void Awake()
    {
        _parentRoom = GetComponentInParent<Room>();
        if (_parentRoom == null) Debug.Log("Trash chute needs to be under a parent Room");
        _dropArea = GetComponent<Collider2D>();
        _currentTime += Random.Range(0,_maxRandomTimeOffset);
    }

    private void Update()
    {
        if (!_canDropAutomatically) return;
        if (_possibleTrash == null || _possibleTrash.Count <= 0) return;

        _currentTime += Time.deltaTime;
        if (_currentTime >= _dropInterval)
        {
            if (TryDropSomeTrash()) _currentTime = 0f;
        }

    }

    public bool TryDropSomeTrash()
    {
        int chosenIndex = Random.Range(0, _possibleTrash.Count);
        Trash chosenTrash = _possibleTrash[chosenIndex];
        if (chosenTrash == null) return false;

        if (isActiveAndEnabled && chosenTrash.Size <= _parentRoom.FreeTrashAmount && _trashInDropArea.Count <= 0) {
            DropTrash(chosenTrash);
            return true;
        }
        return false;
    }

    private void DropTrash(Trash chosenTrash)
    {
        Trash newTrash = Instantiate(chosenTrash, _dropArea.bounds.center, Quaternion.identity);
        _parentRoom.AddCleanableToRoom(newTrash);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent(out Trash trash))
        {
            if (!_trashInDropArea.Contains(trash)) _trashInDropArea.Add(trash);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.TryGetComponent(out Trash trash))
        {
            if (_trashInDropArea.Contains(trash)) _trashInDropArea.Remove(trash);
        }
    }


}