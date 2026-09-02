using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    public class ExplosiveProjectile : Projectile
    {
        [Header("Explosion Setting")]
        [SerializeField] private float m_ExplosionRadius;
        [SerializeField] private bool m_UseDamageFalloff = true;
        [SerializeField] private LayerMask m_ExplosionLayerMask = Physics2D.AllLayers;
        [SerializeField] private GameObject m_ExplosionEffectPrefab;

        private bool m_HasExploded = false;

        private void Update()
        {
            if (m_HasExploded) return;

            float stepLenth = Time.deltaTime * Velocity;
            Vector2 step = transform.up * stepLenth;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, stepLenth);

            if (hit)
            {
                Explode(hit.point);
                ScoreHit(hit);
                return;
            }

            m_Timer += Time.deltaTime;

            if (m_Timer > Lifetime)
            {
                Explode(transform.position);
                return;
            }

            transform.position += new Vector3(step.x, step.y, 0);
        }

        private void Explode(Vector2 explosionPoint)
        {
            if (m_HasExploded) return;
            m_HasExploded = true;


            GameObject effectPrefab = m_ExplosionEffectPrefab != null ? m_ExplosionEffectPrefab : ImpactEffectPrefab;

            if (effectPrefab != null)
            {
                GameObject impactEffect = Instantiate(effectPrefab, explosionPoint, Quaternion.identity);
                impactEffect.transform.localScale = Vector3.one * m_ExplosionRadius * 0.5f;
            }

            ApplyAreaDamage(explosionPoint);

            Destroy(gameObject);
        }

        private void ApplyAreaDamage(Vector2 explosionPoint)
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(explosionPoint, m_ExplosionRadius, m_ExplosionLayerMask);

            foreach (Collider2D hitCollider in hitColliders)
            {
                Destructible destructible = hitCollider.transform.root.GetComponent<Destructible>();

                if (destructible != null && destructible != m_Parent)
                {
                    int finalDamage = CalculateDamage(explosionPoint, hitCollider.transform.position);
                    destructible.ApplyDamage(finalDamage);
                }
            }
        }

        private int CalculateDamage(Vector2 explosionCenter, Vector2 targetPosition)
        {
            if (!m_UseDamageFalloff) return Damage;

            float distance = Vector2.Distance(explosionCenter, targetPosition);
            float damageMultiplier = 1f - Mathf.Clamp01(distance / m_ExplosionRadius);
            int finalDamage = Mathf.RoundToInt(Damage * damageMultiplier);

            return Mathf.Max(finalDamage, Damage / 10);
        }

        private void OnDrawGizmosSelected()
        {
            if (m_ExplosionRadius > 0)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, m_ExplosionRadius);
            }
        }
    }
}