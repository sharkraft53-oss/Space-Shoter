using UnityEngine;

namespace SpaceShooter
{
    public class Projectile : Entity
    {
        [SerializeField] private float m_Velocity;

        public float Velocity => m_Velocity;

        [SerializeField] private float m_LifeTime;

        public float Lifetime => m_LifeTime;

        [SerializeField] private int m_Damage;

        public int Damage => m_Damage;

        [SerializeField] private GameObject m_ImpactEffectPrefab;

        public GameObject ImpactEffectPrefab => m_ImpactEffectPrefab;

        protected float m_Timer;

        private void Update()
        {
            float stepLength = Time.deltaTime * m_Velocity;
            Vector2 step = transform.up * stepLength;


            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, stepLength);

            if (hit)
            {
                ScoreHit(hit);
            }

            m_Timer += Time.deltaTime;

            if (m_Timer > m_LifeTime)
                Destroy(gameObject);

            transform.position += new Vector3(step.x, step.y, 0);
        }
        protected void ScoreHit(RaycastHit2D hit)
        {
            Destructible destructible = hit.collider.transform.root.GetComponent<Destructible>();
            if (destructible != null && destructible != m_Parent)
            {
                destructible.ApplyDamage(m_Damage);

                if (destructible.HitPoints <= 0)
                {
                    if (m_Parent == Player.Instance.ActiveShip)
                    {
                        Player.Instance.AddScore(destructible.ScoreValue);

                        if (destructible is SpaceShip)
                        {
                            Player.Instance.AddKill();
                        }
                    }
                }
            }
            OnProjectileLifeEnd(hit.collider, hit.point);
        }
    

        private void OnProjectileLifeEnd(Collider2D col, Vector2 pos)
        {
            Destroy(gameObject);

        }

        protected Destructible m_Parent;
        public void SetPArentShooter(Destructible parent)
        {
            m_Parent = parent;
        }



    }
}
