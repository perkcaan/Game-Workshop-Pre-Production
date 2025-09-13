using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrashBall : PushableObject
{
    [SerializeField] float scaleMultiplier;
    [SerializeField] float grabbingRequirement;
    [SerializeField] float decayMultiplier;
    public List<CollectableTrash> consumedTrash = new List<CollectableTrash>();
    public PlayerState grabbedPlayer;
    private float ballGrabbingPower;
    private float ballDecay;
    public void Update()
    {
        if (grabbedPlayer != null)
        {
            GrabbingPlayer();
        }
    }

    private void GrabbingPlayer()
    {
        ballGrabbingPower = rb.velocity.magnitude * weight;
        grabbedPlayer.transform.position = transform.position;
        grabbedPlayer.hitbox.enabled = false;
        ballDecay += Time.deltaTime * decayMultiplier;

        if (ballGrabbingPower - ballDecay < 0)
        {
            ballDecay = 0;
            grabbedPlayer.hitbox.enabled = true;
            grabbedPlayer = null;
            ExplodeBall();
        }
    }

    private void ExplodeBall()
    {
        foreach (CollectableTrash trash in consumedTrash)
        {
            trash.gameObject.SetActive(true);
            trash.StartCoroutine("MergeDelay");
            trash.transform.position = transform.position;

            float explosionForce = 5 + (3 * consumedTrash.Count);
            Vector2 randomForce = new Vector2(Random.Range(-explosionForce, explosionForce), Random.Range(-explosionForce, explosionForce));
            trash.GetComponent<Rigidbody2D>().velocity = randomForce;
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out CollectableTrash trash))
        {
            consumedTrash.Add(trash);
            weight += trash.weight;
            SetSize();
            trash.gameObject.SetActive(false);
            return;
        }

        if (other.gameObject.TryGetComponent(out TrashBall trashBall))
        {
            if (rb.velocity.magnitude > trashBall.rb.velocity.magnitude)
            {
                consumedTrash.AddRange(trashBall.consumedTrash);
                weight += trashBall.weight;
                SetSize();
                Destroy(trashBall.gameObject);
            }
            return;
        }

        if (grabbedPlayer == null && other.gameObject.TryGetComponent(out PlayerState player))
        {
            ballGrabbingPower = rb.velocity.magnitude * weight;
            if (ballGrabbingPower > grabbingRequirement)
            {
                ballDecay = 0;
                grabbedPlayer = player;
            }
        }
    }

    public void SetSize()
    {
        float newSize = scaleMultiplier * Mathf.Sqrt(weight);
        transform.localScale = Vector3.one * newSize;
    }
}
