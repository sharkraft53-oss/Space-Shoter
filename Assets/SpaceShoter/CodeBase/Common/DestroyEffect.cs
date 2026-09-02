using UnityEngine;
using System;

namespace SpaceShooter
{
    public class DestroyEffect : MonoBehaviour
    {
        public static event Action<Transform> OnEffectActivated;

        public void Activate()
        {
            OnEffectActivated?.Invoke(transform);
        }
    }
}