using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horsespawn : MonoBehaviour
{
    public GameObject[] prefabs;
    private BoxCollider area;
    public int minCount = 1; // 최소 생성 갯수
    public int maxCount = 3; // 최대 생성 갯수

    private List<GameObject> spawnedObjects = new List<GameObject>();

    void Start()
    {
        area = GetComponent<BoxCollider>();

        // 처음에 10초 후에 첫번째 호출을 하고, 그 이후로는 10초 주기로 반복 호출
        InvokeRepeating("SpawnHorses", 0f, 3f);

        area.enabled = false;
    }

    void SpawnHorses()
    {
        int randomCount = Random.Range(minCount, maxCount + 1); // 최소에서 최대까지의 랜덤한 갯수

        for (int i = 0; i < randomCount; ++i)
        {
            Spawn();
        }
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 basePosition = transform.position;
        Vector3 size = area.size;

        float posX = basePosition.x + Random.Range(-size.x / 2f, size.x / 2f);
        float posY = basePosition.y + Random.Range(-size.y / 2f, size.y / 2f);
        float posZ = basePosition.z + Random.Range(-size.z / 2f, size.z / 2f);

        Vector3 spawnPos = new Vector3(posX, posY, posZ);

        return spawnPos;
    }

    private void Spawn()
    {
        int selection = Random.Range(0, prefabs.Length);

        GameObject selectedPrefab = prefabs[selection];

        Vector3 spawnPos = GetRandomPosition();

        // 수정된 부분: 부모의 회전값을 가져와서 자식의 회전값에 적용
        Quaternion spawnRotation = Quaternion.Euler(new Vector3(0, Random.Range(-60, 60), 0));
        spawnRotation *= transform.rotation;

        GameObject instance = Instantiate(selectedPrefab, spawnPos, spawnRotation);
        spawnedObjects.Add(instance);
    }
}