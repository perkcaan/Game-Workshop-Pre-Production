using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FishLaunch : MonoBehaviour
{ 
    [SerializeField] GameObject _fishPrefab;
    [SerializeField] GameObject _landingZone;
    [SerializeField] Vector2 _landingPoint;
    [SerializeField] Lava _lavaSpot;
    Rigidbody2D _fishRb;
    float distance;

    // Start is called before the first frame update
    void Start()
    {
         _fishRb = _fishPrefab.GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector2.Distance(_fishRb.position, _landingZone.transform.position);

        if (_landingZone.activeSelf)
        {
            _landingZone.transform.localScale = new Vector3(distance/4, distance/4, 1);
            if (distance < 1f)
            {
                _fishPrefab.GetComponent<Collider2D>().enabled = true;
                if (distance < .2f)
                {

                    _landingZone.SetActive(false);
                    _fishPrefab.SetActive(false);
                    StartCoroutine(SpawnLava());


                }
            }
            else
            {
                _fishPrefab.GetComponent<Collider2D>().enabled = false;
            }
        }

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        GrimeReaper grimeReaper = collision.GetComponent<GrimeReaper>();
        if (grimeReaper != null)
        {
            _landingPoint = new Vector2(Random.Range(-3f, 9f), Random.Range(-10f, 1f));
            _landingZone.SetActive(true);
            _landingZone.transform.position = _landingPoint;
            _fishPrefab.SetActive(true);
            _fishPrefab.transform.position = new Vector2(_landingPoint.x, _landingPoint.y + 10);
            
            
        }
    }

    public IEnumerator SpawnLava()
    {
        _lavaSpot.gameObject.SetActive(true);
        _lavaSpot.transform.position = _landingPoint;
        yield return new WaitForSeconds(2f);
        _lavaSpot.gameObject.SetActive(false);
    }
}

