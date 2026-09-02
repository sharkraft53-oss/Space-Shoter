using UnityEngine;
namespace SpaceShooter
{
    public class MoveProjectile : MonoBehaviour
    {
        [SerializeField] private float m_Velocity;

        [SerializeField] private float m_Interpolation;

        [SerializeField] private float m_InterpolationAngur;

        [SerializeField] private float m_LifeTime;

        [SerializeField] private int m_Damage;

        [SerializeField] private ImpactEffect m_ImpactEffectPrefab;

        [SerializeField] private GameObject  MoveRocket;

        [SerializeField] private Transform Enemy;

        [SerializeField] private float m_RocketZ;

        [SerializeField] private float m_Forward;
        private float m_Timer;

        private void Update()
        {
            if (MoveRocket = GameObject.FindWithTag("Enemy")) 
           { 
            if (Enemy == null || MoveRocket == null) return;

            Vector2 rocketPos = MoveRocket.transform.position;
            Vector2 targetPos = Enemy.position + Enemy.transform.up * m_Forward;
            Vector2 newrocketPos = Vector2.Lerp(rocketPos, targetPos, m_Interpolation * Time.deltaTime);

            MoveRocket.transform.position = new Vector3(newrocketPos.x, newrocketPos.y, m_RocketZ);

            if (m_InterpolationAngur > 0)
            {
                MoveRocket.transform.rotation = Quaternion.Slerp(MoveRocket.transform.rotation, Enemy.rotation, m_InterpolationAngur * Time.deltaTime);
            }
       }
        }
    }
}