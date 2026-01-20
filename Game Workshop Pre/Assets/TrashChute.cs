using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashChute : MonoBehaviour
{
    [SerializeField] List<Trash> _possibleTrash;
    [SerializeField] float _dropCooldown;
    [SerializeField] float _dropAreaMinX;
    [SerializeField] float _dropAreaMaxX;
    [SerializeField] float _dropAreaMinY;
    [SerializeField] float _dropAreaMaxY;



    private Vector2 _landingPoint;
    private GameObject _spawnedTrash;
    private Rigidbody2D _trashRb;
    private float _nextDropTime = 0f;
    private Room _parentRoom;
    private bool _canDrop;
    private Collider2D _feeler;
    private GameObject trash;


    private void Awake()
    {
        _parentRoom = GetComponentInParent<Room>();
        _feeler = GetComponent<Collider2D>();
        _feeler.enabled = true;
        _canDrop = true;
    }

    void Update()
    {
        
        if (Time.time >= _nextDropTime && _spawnedTrash == null)
        {
            int index = Random.Range(0, _possibleTrash.Count);
            trash = _possibleTrash[index].gameObject;
            
            _landingPoint = new Vector2(Random.Range(_dropAreaMinX, _dropAreaMaxX), Random.Range(_dropAreaMinY, _dropAreaMaxY));
            _feeler.enabled = true;
            _feeler.transform.position = _landingPoint;

            if (_parentRoom._currentTrashSizeAmount - trash.GetComponent<ICleanable>().Size <= 0)
            {
               StopTrash();
            }

            if (trash != null && trash.GetComponent<ICleanable>().Size + _parentRoom._currentTrashSizeAmount < _parentRoom._roomTrashSizeAmount && _canDrop)
            {

                _spawnedTrash = Instantiate(trash, new Vector2(_landingPoint.x, _landingPoint.y + 20), Quaternion.identity);
                _parentRoom.AddCleanableToRoom(_spawnedTrash.GetComponent<ICleanable>());
                _trashRb = _spawnedTrash.GetComponent<Rigidbody2D>();
                _spawnedTrash.GetComponent<Collider2D>().enabled = false;
                _trashRb.gravityScale = 4;
                _nextDropTime = Time.time + _dropCooldown;
                
            }
            else
            {
                trash = null;
                index = Random.Range(0, _possibleTrash.Count);
                Debug.Log("Trash limit reached in room, skipping drop");
                return;

            }

            

        }
        


        if (_spawnedTrash != null)
        {
            float distance = Vector2.Distance(_trashRb.position, _landingPoint);
            

            
                
            if (distance < 0.2f)
            {
                _spawnedTrash.GetComponent<Collider2D>().enabled = true;
                _trashRb.gravityScale = 0;
                _trashRb.velocity = Vector2.zero;
                _trashRb.angularVelocity = 0f;
                _spawnedTrash = null; 
            }
            else
            {
                    _spawnedTrash.GetComponent<Collider2D>().enabled = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject _landingTile = collision.gameObject;

        if (_landingTile.GetComponent<Lava>() == null && _landingTile.tag != "Wall")
        {
            _canDrop = true;
            Debug.Log(collision.gameObject.name + " landed, can drop trash here");
            Debug.Log(_landingTile.tag);
        }
        else
        {
            _canDrop = false;
            Debug.Log(collision.gameObject.name);
            Debug.Log("Cannot drop trash here, area blocked");
        }
    }

    void StartTrash() 
    {
        _canDrop = true;
        this.enabled = true;
        _feeler.enabled = true;
    }
    void StopTrash()
    {
        _canDrop = false;
        this.enabled = false;
        _feeler.enabled = false;
    }

}



