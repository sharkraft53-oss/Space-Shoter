using UnityEngine;

namespace SpaceShooter
{
    public class LevelBoundaryLimiter : MonoBehaviour
    {
        private void Update()
        {
            if (LevelBoundary.Instance == null) return;

            var Ib = LevelBoundary.Instance;
            var r = Ib.Radius;

            if (transform.position.magnitude > r)
            {
                if (Ib.LimitMode == LevelBoundary.Mode.Limit)
                {
                    transform.position = transform.position.normalized * r;
                }

                if (Ib.LimitMode == LevelBoundary.Mode.Teleport)
                {
                    transform.position = -transform.position.normalized * r;
                }
            }
        }
    }
}
