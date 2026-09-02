using UnityEngine;
using System.Collections;

namespace SpaceShooter
{
    public class SpawnerStars : MonoBehaviour
    {
        public GameObject Stars;
        [SerializeField] private float StartSpawn;
        [SerializeField] private float FinishSpawn;
        [SerializeField] private float UpSpawn;
        [SerializeField] private float TimeSpawn;
        [SerializeField] private float TimeSpawnDestroy;
        void Start()
        {
            StartCoroutine(Spawner());
        }

        IEnumerator Spawner()
        {
            while (true)
            {
                yield return new WaitForSeconds(TimeSpawn);
                float rand = Random.Range(StartSpawn, FinishSpawn);
                GameObject newStars = Instantiate(Stars, new Vector2(rand, UpSpawn), Quaternion.identity);
                Destroy(newStars, TimeSpawnDestroy);
            }
        }
        
    }
}
