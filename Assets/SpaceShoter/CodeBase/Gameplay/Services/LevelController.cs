using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace SpaceShooter
{
    public class LevelController : SingletonBase<LevelController>

    {
        private const string MainMenuScene = "main_menu";

        public event UnityAction LevelPassed;
        public event UnityAction LevelLost;

        [SerializeField] private LevelPropeties m_LevelPropeties;
        [SerializeField] private LevelCondition[] m_Conditions;

        private bool m_IsLevelCompleted;
        private float m_LevelTime;

        public bool HasNextLevel => m_LevelPropeties.NextLevel != null;
        public float LevelTime => m_LevelTime;

        private void Start()
        {
            Time.timeScale = 1;
            m_LevelTime = 0;
        }

        private void Update()
        {
            if(m_IsLevelCompleted == false)
            {
                m_LevelTime += Time.deltaTime;
                CheckLevelConditions();
            }
                   

            if(Player.Instance != null && Player.Instance.NumLives == 0)
            {
                Lose();
            }
        }

        private void CheckLevelConditions()
        {
           

            int numCompleted = 0;

            for (int i = 0; i < m_Conditions.Length; i++)
            {
                if (m_Conditions[i].IsCompleted == true)
                {
                    numCompleted++;
                }
            }

            if (numCompleted == m_Conditions.Length)
            {
                m_IsLevelCompleted = true;

                Pass();
            }
        }

        private void Lose()
        {
            LevelLost?.Invoke();
            Time.timeScale = 0;
        }

        private void Pass()
        {
            LevelPassed?.Invoke();
            Time.timeScale = 0;
        }

        public void LoadNextLevel()
        {
            if (HasNextLevel == true)
                SceneManager.LoadScene(m_LevelPropeties.NextLevel.SceneName);

            else
                SceneManager.LoadScene(MainMenuScene);
        }

        public void RestartLevel()
        {
            SceneManager.LoadScene(m_LevelPropeties.SceneName);
        }
    }
}
