using UnityEngine;
using UnityEngine.UI;

namespace SpaceShooter
{

    public class LivesIndicator : MonoBehaviour
    {
        [SerializeField] private Text m_Text;
        [SerializeField] private Image m_Icon;

        private int lassLives;

        private void Start()
        {
            m_Icon.sprite = Player.Instance.ActiveShip.PreviewImage;
        }

        void Update()
        {
            int lives = Player.Instance.NumLives;

            if(lassLives != lives)
            {
                m_Text.text =  lives.ToString();
                lassLives = lives;

            }
        }
    }
}