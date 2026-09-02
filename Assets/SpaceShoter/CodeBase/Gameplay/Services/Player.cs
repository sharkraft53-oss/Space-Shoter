using System.Collections;
using UnityEngine;

namespace SpaceShooter
{
    public class Player : SingletonBase<Player>
    {
        public static SpaceShip SelectedShip;
        [SerializeField] private int m_NumLives;
         private SpaceShip m_Ship;
        [SerializeField] private SpaceShip m_PlayerShipPrefab;
        public SpaceShip ActiveShip => m_Ship;

        private FollowCamera m_FollowCamera;
        private ShipInputController m_ShipInputController;
        private Transform m_SpawnPoint;

        public FollowCamera FollowCamera => m_FollowCamera;

        public void Construct(FollowCamera followCamera, ShipInputController shipInputController, Transform spawnPoint)
        {
            m_FollowCamera = followCamera;
            m_ShipInputController = shipInputController;
            m_SpawnPoint = spawnPoint;
        }

        [SerializeField] private int m_Respawndelay;
        private int m_Score;
        private int m_NumKills;

        public int Score => m_Score;
        public int NumKills => m_NumKills;
        public int NumLives => m_NumLives;

        public SpaceShip ShipPrefabs
        {
            get
            {
                if (SelectedShip == null)
                {
                    return m_PlayerShipPrefab;

                }
                else
                {
                    return SelectedShip;
                }
            }
        }

        private void Start()
        {
            Respawn();

            m_Ship.EventOnDeath.AddListener(OnShopDeath);
        }

        private void OnShopDeath()
        {
            
            m_NumLives--;

            if (m_NumLives > 0)
            {
                StartCoroutine(RespawnWithDelay());
               
            }
        }

        private IEnumerator RespawnWithDelay()
        {
            yield return new WaitForSeconds(m_Respawndelay);
            Respawn();
            m_Ship.EventOnDeath.AddListener(OnShopDeath);
        }

        private void Respawn()
        {
           var newPlayerShip = Instantiate(ShipPrefabs, m_SpawnPoint.position, m_SpawnPoint.rotation);

           m_Ship = newPlayerShip.GetComponent<SpaceShip>();

           
            
            m_FollowCamera.SetTarget(m_Ship.transform);
            m_ShipInputController.SetTargetShip(m_Ship);
        }

        public void AddKill ()
        {
            m_NumKills += 1;
        }

        public void AddScore(int num)
        {
            m_Score += num;
        }
    }
}
