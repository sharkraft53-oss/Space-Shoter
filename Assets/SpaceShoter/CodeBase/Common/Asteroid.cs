using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    public class Asteroid : Destructible
    {
        public enum Size
        {
            Small,
            Normal
        }

        [SerializeField] private Size size;
        [SerializeField] private float horizontalSpeed;
        [SerializeField] private Asteroid prefabAsteroid;

        private Vector3 velocity;
        private Rigidbody2D rb;
        private PolygonCollider2D polygonCollider;

        private new void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            polygonCollider = GetComponent<PolygonCollider2D>();

            base.Awake();

            EventOnDeath.AddListener(OnAsteroidDestroyed);

            SetSize(size);

            InitializeVelocity();

            if (rb != null)
            {
                rb.simulated = true;
                rb.isKinematic = false;
            }

            if (polygonCollider != null)
            {
                polygonCollider.enabled = true;
                polygonCollider.isTrigger = false;
            }
        }

        protected override void OnDestroy()
        {
            EventOnDeath.RemoveListener(OnAsteroidDestroyed);
            base.OnDestroy();
        }

        private void OnAsteroidDestroyed()
        {
            if (size != Size.Small)
            {
                SpawnAsteroid();
            }
        }

        private void InitializeVelocity()
        {
            if (rb != null)
            {
                rb.linearVelocity = velocity;
            }
        }

        public void SetHorizontalDirection(float direction)
        {
            velocity.x = Mathf.Sign(direction) * horizontalSpeed;
            if (rb != null)
            {
                rb.linearVelocity = velocity;
            }
        }

        private void SpawnAsteroid()
        {
            for (int i = 0; i < 2; i++)
            {
                Vector3 spawnOffset = new Vector3((i - 0.5f) * 1.5f, 0, 0);
                Asteroid newAsteroid = Instantiate(prefabAsteroid, transform.position + spawnOffset, Quaternion.identity);

                newAsteroid.SetSize(size - 1);
                newAsteroid.SetHorizontalDirection((i % 2 * 2) - 1);

                newAsteroid.gameObject.SetActive(true);
                newAsteroid.ForceEnableComponents();
                newAsteroid.FindDeathExplosion();
            }
        }

        public void ForceEnableComponents()
        {
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            if (polygonCollider == null) polygonCollider = GetComponent<PolygonCollider2D>();

            if (rb != null)
            {
                rb.simulated = true;
                rb.isKinematic = false;
                rb.linearVelocity = velocity;
            }

            if (polygonCollider != null)
            {
                polygonCollider.enabled = true;
                polygonCollider.isTrigger = false;
            }
        }

        public void SetSize(Size newSize)
        {
            if ((int)newSize < (int)Size.Small) return;

            size = newSize;
            transform.localScale = GetVectorFromSize(newSize);
        }

        private Vector3 GetVectorFromSize(Size size)
        {
            if (size == Size.Normal) return new Vector3(1, 1, 1);
            if (size == Size.Small) return new Vector3(0.50f, 0.50f, 0.50f);

            return Vector3.one;
        }
    }
} 