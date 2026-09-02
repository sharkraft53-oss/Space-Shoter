using UnityEngine;

namespace SpaceShooter
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class RoccketMove : Projectile
    {
        public Transform target;

        public float speed = 5f;

        public float rotateSpeed = 200f;

        public GameObject explosionEffect;

        private Rigidbody2D rb;
        void Start()
        {
           
            rb = GetComponent<Rigidbody2D>();
            target = GameObject.FindGameObjectWithTag("Enemy").transform;

        }


        void FixedUpdate()
        {
            Vector2 direction = (Vector2)target.position - rb.position;

            direction.Normalize();

            float rotateAmount = Vector3.Cross(direction, transform.up).z;

            rb.angularVelocity = -rotateAmount * rotateSpeed;

            rb.linearVelocity = transform.up * speed;
            
        }

     void OnTriggerEnter2D()
        {
           
            Instantiate(explosionEffect, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}