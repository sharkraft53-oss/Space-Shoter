using UnityEngine;
namespace SpaceShooter
{
    public class StarsSpeed : MonoBehaviour
    {
        [SerializeField] private float SpeedStars;
        void Start()
        {

        }


        void Update()
        {
            transform.Translate(Vector2.down * SpeedStars * Time.deltaTime);
        }
    }
}
