using UnityEngine;

namespace SpaceShooter
{

    [RequireComponent(typeof(GenerateEffect))]
    public class EffectSpawner : MonoBehaviour
    {
        private GenerateEffect _generator;

        private void Awake()
        {
            _generator = GetComponent<GenerateEffect>();
        }

        private void OnDisable()
        {
            DestroyEffect.OnEffectActivated -= DestroyEffect_OnEffectActivated;
        }

        private void OnEnable()
        {
            DestroyEffect.OnEffectActivated += DestroyEffect_OnEffectActivated;
        }

        private void DestroyEffect_OnEffectActivated(Transform obj)
        {
            GameObject effect = _generator.GetFreeEffect();
            effect.transform.position = obj.position;
            effect.SetActive(true);
        }


    }
}