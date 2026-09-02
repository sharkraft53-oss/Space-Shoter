using UnityEngine;

namespace SpaceShooter
{
    [CreateAssetMenu]
    public class LevelPropeties : ScriptableObject
    {
        [SerializeField] private string m_Title;
        [SerializeField] private string m_SceneName;
        [SerializeField] private Sprite m_PreviewImage;
        [SerializeField] private LevelPropeties m_NextLevel;

        public string Title => m_Title;
        public string SceneName => m_SceneName;
        public Sprite PreviewImage => m_PreviewImage;
        public LevelPropeties NextLevel =>  m_NextLevel;


    }
}
