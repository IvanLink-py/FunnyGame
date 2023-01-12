using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private float timer;
    [SerializeField] private float delay;
    [SerializeField] private GameObject prefab;

    // Update is called once per frame
    private void FixedUpdate()
    {
        timer -= Time.deltaTime;

        if (!(timer <= 0)) return;
        Instantiate(prefab, transform.position, Quaternion.identity);
        timer = delay;
    }
}