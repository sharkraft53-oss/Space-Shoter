using UnityEngine;

namespace SpaceShooter
{
    public class CollisionDamageApplicator : MonoBehaviour
    {

        public static string IgnoreTag = "WorldBoundary";

        [SerializeField] private int m_VelocityDamageModifier;
        [SerializeField] private int m_DamageConstant;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.transform.tag == IgnoreTag)
                return;

            var destructable = transform.root.GetComponent<Destructible>();

            if (destructable != null)
            {
                destructable.ApplyDamage((int)m_DamageConstant + (int) (m_VelocityDamageModifier * collision.relativeVelocity.magnitude));
            }
        }

    }
}