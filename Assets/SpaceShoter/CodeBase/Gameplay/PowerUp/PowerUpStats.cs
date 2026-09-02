using UnityEngine;

namespace SpaceShooter
{
    public class PowerUpStats : PowerUp
    {

        public enum EffectType
        {
            AddAmmo,
            AddEnergy,
            immortality,
            SuperSpeed
        }


        [SerializeField] private EffectType m_EffectType;

        [SerializeField] private float m_Value;
        protected override void OnPickedUp(SpaceShip ship)
        {
            if (m_EffectType == EffectType.AddEnergy)
                ship.AddEnergy((int) m_Value);

            if (m_EffectType == EffectType.AddAmmo)
                ship.AddAmmo((int) m_Value);

            if (m_EffectType == EffectType.immortality)
            {
                ship.BecomeImmortal(m_Value);
            }

            if (m_EffectType == EffectType.SuperSpeed)
            {
                ship.BeconeSuperSpeed(m_Value, 10.0f);
            }
        }
    }
}
