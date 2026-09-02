using UnityEngine;
namespace SpaceShooter
{
    public class PowerUpWeapon : PowerUp
    {
        [SerializeField] private TurretProperties m_ProperTies;
        protected override void OnPickedUp(SpaceShip ship)
        {
            ship.AssignWeapon(m_ProperTies);
        }
    }
}
