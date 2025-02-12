using UnityEngine;

namespace TopDown.Movement
{
    public class HitboxRotation : MonoBehaviour
    {
        public float moveSpeed = 3f;

        public Rigidbody2D rb;
        public Camera cam;

        Vector2 movement;
        Vector2 mousePos;

        //Update
        void Update()
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            mousePos = cam.ScreenToWorldPoint(Input.mousePosition);


        }

        void FixedUpdate()
        {

            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

            Vector2 lookDir = mousePos - rb.position;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;

            rb.rotation = angle;


        }


    }


}