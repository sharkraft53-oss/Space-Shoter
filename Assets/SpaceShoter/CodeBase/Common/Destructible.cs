using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SpaceShooter
{
    /// <summary>
    /// Уничтожаемый обьект на сцене. То что может иметь хитпоинты.
    /// </summary>
    public class Destructible : Entity
    {
        #region Properties

        /// <summary>
        /// Обьект игнорирует поврждения.
        /// </summary>
        [SerializeField] private bool m_Indestrictible;
        public bool IsIndestrictible => m_Indestrictible;

        /// <summary>
        /// Стартовое количество хитпоинтов.
        /// </summary>
        [SerializeField] private int m_HitPoints;
        public int MaxHitPoints => m_HitPoints;

        /// <summary>
        /// Текущие хитпоинты.
        /// </summary>
        private int m_CurrentHitPoints;
        public int HitPoints => m_CurrentHitPoints;

        [Header("Effect")]
        [SerializeField] private ParticleSystem m_DeathExplosion;

        #endregion

        #region Unity Events

        protected virtual void Awake()
        {
            FindDeathExplosion();
        }

        protected virtual void Start()
        {
            m_CurrentHitPoints = m_HitPoints;

            transform.SetParent(null);
        }

        #endregion

        #region Public API

        /// <summary>
        /// Применение урона к обьекту.
        /// </summary>
        /// param name="damage"> Количество урона.</param>
        public void ApplyDamage(int damage)
        {
            // Проверка на бессмертие
            if (this is SpaceShip ship && ship.IsImmortal)
                return;

            if (m_Indestrictible) return;

            m_CurrentHitPoints -= damage;
            if (m_CurrentHitPoints <= 0)
                OnDeath();

        }
        #endregion





        public void FindDeathExplosion()
        {
            if (m_DeathExplosion == null)
            {
                m_DeathExplosion = GetComponentInChildren<ParticleSystem>(true);
            }
        }

        /// <summary>
        /// Переопределяемое событие уничтожения обьекта, кигда хитпоинты достигают и ниже нуля.
        /// </summary>
        protected virtual void OnDeath()
        {
            if (m_DeathExplosion == null)
            {
                FindDeathExplosion();
            }

            if (m_DeathExplosion != null)
            {
                ParticleSystem explosionInstance = Instantiate(m_DeathExplosion, transform.position, transform.rotation);
                explosionInstance.transform.parent = null;
                explosionInstance.Play();


                Destroy(explosionInstance.gameObject,
                        explosionInstance.main.duration +
                        explosionInstance.main.startLifetime.constantMax);
            }

            m_EventOnDeath?.Invoke();
            Destroy(gameObject);
        }

        private static HashSet<Destructible> m_AllDestructibles;

        public static IReadOnlyCollection<Destructible> AllDestructibles => m_AllDestructibles;

        protected virtual void OnEnable()
        {
            if (m_AllDestructibles == null)
                m_AllDestructibles = new HashSet<Destructible>();

            m_AllDestructibles.Add(this);
        }

        protected virtual void OnDestroy()
        {
            m_AllDestructibles.Remove(this);
        }

        public const int TeamIdNeural = 0;

        [SerializeField] private int m_TeamId;
        public int TeamId => m_TeamId;

        [SerializeField] private UnityEvent m_EventOnDeath;
        public UnityEvent EventOnDeath => m_EventOnDeath;

        [SerializeField] private int m_ScoreValue;
        public int ScoreValue => m_ScoreValue;

        

    }
}