using System.Collections;
using UnityEngine;

namespace SpaceShooter
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class SpaceShip : Destructible
    {
        [SerializeField] private Sprite m_PreviewImage;
        /// <summary>
        ///  Масса для автоматической установки у ригида.
        /// </summary>
        [Header("Space ship")]
        [SerializeField] private float m_Mass;

        /// <summary>
        /// Толкающая вперёд сила.
        /// </summary>
        [SerializeField] private float m_Thrust;

        /// <summary>
        /// Вращающая сила.
        /// </summary>
        [SerializeField] private float m_Mobility;

        /// <summary>
        /// Максимальная линейная скорость.
        /// </summary>
        [SerializeField] private float m_MaxLinearVelocity;


        /// <summary>
        /// Максимальная вращательная скорость. В градусах/сек
        /// </summary>
        [SerializeField] private float m_MaxAngularVelocity;


        /// <summary>
        /// Сохраненная ссылка на ригид.
        /// </summary>
        private Rigidbody2D m_Rigid;

        public float MaxLinearVelocity => m_MaxLinearVelocity;
        public float MaxAngularVelocity => m_MaxAngularVelocity;
        public Sprite PreviewImage => m_PreviewImage;

        #region Public API
        /// <summary>
        /// Управление линейной тягой. -1.0 до +1.0
        /// </summary>
        public float ThrustControl { get; set; }

        /// <summary>
        ///  Управление вращательной тягой. -1.0 до +1.0
        /// </summary>
        public float TorqueControl { get; set; }

        private bool isImmortal = false;
        #endregion

        

        public bool IsImmortal
        {
            get { return isImmortal; }
            set { isImmortal = value; }
        }

        #region Unity Event
        protected override void Start()
        {
            base.Start();

            m_Rigid = GetComponent<Rigidbody2D>();
            m_Rigid.mass = m_Mass;

            m_Rigid.inertia = 1;

            InOffensive();
        }

       

        private void FixedUpdate()
        {
            UpdateRigidBody();

            UpdateEnergyRegen();
        }
        #endregion
        /// <summary>
        /// Метод добавления сил кораблю для движения.
        /// </summary>
        private void UpdateRigidBody()
        {
            m_Rigid.AddForce(ThrustControl * m_Thrust  * transform.up * Time.fixedDeltaTime, ForceMode2D.Force);

            m_Rigid.AddForce(-m_Rigid.linearVelocity * (m_Thrust / m_MaxLinearVelocity) * Time.fixedDeltaTime, ForceMode2D.Force);

            m_Rigid.AddTorque(TorqueControl * m_Mobility * Time.fixedDeltaTime, ForceMode2D.Force);

            m_Rigid.AddTorque(-m_Rigid.angularVelocity * (m_Mobility / m_MaxAngularVelocity) * Time.fixedDeltaTime, ForceMode2D.Force);
        }

        [SerializeField] private Turret[] m_Turrets;
        
        public void Fire(TurrentMode mode)
        {
            for (int i = 0; i < m_Turrets.Length; i++)
            {
               if (m_Turrets[i].Mode == mode)
               {
                    m_Turrets[i].Fire();
               }
            }
        }

        [SerializeField] private int m_MaxEnergy;
        [SerializeField] private int m_MaxAmmo;
        [SerializeField] private int m_EnergyRegenPerSecond;

        private float m_PrimaryEnergy;
        private int m_SecondaryAmmo;


        public void AddEnergy(int e)
        {
            m_PrimaryEnergy = Mathf.Clamp(m_PrimaryEnergy + e, 0, m_MaxEnergy);
        }

        public void AddAmmo(int ammo)
        {
            m_SecondaryAmmo = Mathf.Clamp(m_SecondaryAmmo + ammo, 0, m_MaxAmmo);
        }

        public void BecomeImmortal (float duration)
        {
            StartCoroutine(ApplyImmortalityEffect(duration));
        }

        public void BeconeSuperSpeed(float duration, float factor)
        {
            StartCoroutine(ApplySuperSpeedEffect(duration, factor));
        }

        private IEnumerator ApplySuperSpeedEffect (float duration, float factor)
        {
            m_Mobility *= factor;
            m_Thrust *= factor;
            m_MaxLinearVelocity *= 1.5f;
            yield return new WaitForSeconds(duration);
            m_MaxLinearVelocity /= 1.5f;
            m_Mobility /= 1.5f;
            m_Thrust /= factor; 

        }

        private IEnumerator ApplyImmortalityEffect (float duration)
        {
            SpriteRenderer[] spaceShipRenderers = GetComponentsInChildren<SpriteRenderer>();

            Color[] originalColor = new Color[spaceShipRenderers.Length];

            for (int i = 0; i < spaceShipRenderers.Length; i++)
            {
                originalColor[i] = spaceShipRenderers[i].color;
            }

            IsImmortal = true;

            foreach (SpriteRenderer renderer in spaceShipRenderers)
            {
                renderer.color = new Color(1f, 1f, 1f, 0.5f);
            }

            yield return new WaitForSeconds(duration);

            IsImmortal = false;

            for (int i = 0; i < spaceShipRenderers.Length; i ++)
            {
                if (spaceShipRenderers[i] != null)
                {
                    spaceShipRenderers[i].color = originalColor[i];
                }
            }
        }

        private void InOffensive()
        {
            m_PrimaryEnergy = m_MaxEnergy;
            m_SecondaryAmmo = m_MaxAmmo;
        }

        private void UpdateEnergyRegen()
        {
            m_PrimaryEnergy += (float)m_EnergyRegenPerSecond * Time.fixedDeltaTime;
            m_PrimaryEnergy = Mathf.Clamp(m_PrimaryEnergy, 0, m_MaxEnergy);
        }

        public bool DrawEnergy(int count)
        {
            if (count == 0)
                return true;

            if (m_PrimaryEnergy >= count)
            {
                m_PrimaryEnergy -= count;
                return true;
            }

            return false;
        }
        public bool DrawAmmo(int count)
        {
            if (count == 0)
                return true;

            if(m_SecondaryAmmo >= count)
            {
                m_SecondaryAmmo -= count;
                return true;
            }

            return false;
        }

        public void AssignWeapon(TurretProperties props)
        {
            for (int i = 0; i < m_Turrets.Length; i++)
            {
                m_Turrets[i].AssingLoadout(props);
            }
        }

    }
}
