using UnityEngine;
using System.Collections.Generic;

namespace SpaceShooter
{
    public class GenerateEffect : MonoBehaviour
    {
        private const int DefaultCount = 3;

        [SerializeField] private ParticleSystem _prefab;

        private List<GameObject> _effect = new List<GameObject>();

        private void Awake()
        {
            for (int i = 0; i < DefaultCount; i++)
            {
                Create();
            }
        }

        private GameObject Create()
        {
            GameObject effect = Instantiate(_prefab.gameObject, transform);
            _effect.Add(effect);

            return effect;
        }

        public GameObject GetFreeEffect()
        {
            foreach (var item in _effect)
            {
                if (item.activeInHierarchy == false)
                    return item;
            }

            return Create();
        }
    }
}