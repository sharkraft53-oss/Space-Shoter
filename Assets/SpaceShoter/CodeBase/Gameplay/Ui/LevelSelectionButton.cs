using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
namespace SpaceShooter
{
    public class LevelSelectionButton : MonoBehaviour
    {
        [SerializeField] private LevelPropeties m_LevelProperties;
        [SerializeField] private Text m_LevelTitle;
        [SerializeField] private Image m_PreviewImage;

        private void Start()
        {
            m_PreviewImage.sprite = m_LevelProperties.PreviewImage;
            m_LevelTitle.text = m_LevelProperties.Title;
        }

        public void LoadLevel()
        {
            SceneManager.LoadScene(m_LevelProperties.SceneName);
        }
    }
}

