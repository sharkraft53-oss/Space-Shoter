using UnityEngine;

namespace SpaceShooter
{
    public abstract class LevelCondition : MonoBehaviour
    {
        public virtual bool IsCompleted { get; }
       
    }
}