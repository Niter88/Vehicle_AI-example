using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerAI : MonoBehaviour
{
    [SerializeField]
    GameObject prefab;

    [SerializeField]
    int targetPopulation = 10;

    [SerializeField]
    Vector2 maxArea;

    [SerializeField]
    float heightSpawn = 0f;

    [SerializeField]
    float skyHeight = 50f;

    [SerializeField]
    Vector3 cachedV3 = Vector3.zero;
    GameObject cachedGameObject;

    RaycastHit hit;

    [SerializeField]
    int instancedEnt = 0;

    float haltTimestamp = 0f;

    WaitForSeconds waitShort = new WaitForSeconds(0.1f);

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnInicialEntities());
        //SpawnOneEntity();
    }

    private void Update()
    {
        Debug.DrawRay(cachedV3, transform.TransformDirection(Vector3.down) * 500f, Color.red);
    }

    private IEnumerator SpawnInicialEntities()
    {
        yield return new WaitForEndOfFrame();
        do
        {
            SpawnOneEntity();
            yield return waitShort;
        } while (instancedEnt < targetPopulation);

        StaticEvents.SpawnNewEntity += SpawnOneEntity;
    }

    public void SpawnOneEntity()
    {
        haltTimestamp = Time.realtimeSinceStartup;

        bool ableToSpawn = false;
        
        do
        {
            //raycast from sky
            cachedV3 = new Vector3(Random.Range(-maxArea.x, maxArea.x), skyHeight, Random.Range(-maxArea.y, maxArea.y));

            if (Physics.Raycast(cachedV3, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
            {
                if (hit.collider.transform.CompareTag("Ground"))
                {
                    //been able to hit the ground, can spawn a new entity
                    cachedV3.y = heightSpawn;

                    cachedGameObject = Instantiate(prefab, cachedV3, Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up));
                    instancedEnt++;
                    cachedGameObject.transform.name = "Vehicle " + instancedEnt;
                    ableToSpawn = true;
                }
            }
        } while (!ableToSpawn && Time.realtimeSinceStartup - haltTimestamp < 2f);
    }
}
