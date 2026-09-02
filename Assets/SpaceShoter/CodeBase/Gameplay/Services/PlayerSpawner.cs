using UnityEngine;

namespace SpaceShooter
{
    public class PlayerSpawner : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private FollowCamera m_FollowCameraPrefab;
        [SerializeField] private Player m_PlayerPrefab;
        [SerializeField] private ShipInputController m_ShipInputControllerPrefab;
        [SerializeField] private VirtualGamePad m_VirtualGamePadPrefab;

        [SerializeField] private Transform m_SpawmPoint;
        public Player Spawn()
        {
            FollowCamera folowCamera = Instantiate(m_FollowCameraPrefab);
            VirtualGamePad virtualGamePad = Instantiate(m_VirtualGamePadPrefab);

            ShipInputController shipInputController = Instantiate(m_ShipInputControllerPrefab);
            shipInputController.Construct(virtualGamePad);

            Player player = Instantiate(m_PlayerPrefab);
            player.Construct(folowCamera, shipInputController, m_SpawmPoint);

            return player;
        }
    }
}
