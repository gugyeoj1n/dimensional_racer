using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject[] prefabs;
    private BoxCollider area;
    public int minCount = 1; // 최소 생성 갯수
    public int maxCount = 3; // 최대 생성 갯수

    private List<GameObject> spawnedObjects = new List<GameObject>();

    void Start()
    {
        area = GetComponent<BoxCollider>();

        int randomCount = Random.Range(minCount, maxCount + 1); // 최소에서 최대까지의 랜덤한 갯수

        for (int i = 0; i < randomCount; ++i)
        {
            Spawn();
        }

        area.enabled = false;
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

        // 수정된 부분: x축 방향을 -90으로 설정
        Quaternion spawnRotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));

        GameObject instance = Instantiate(selectedPrefab, spawnPos, spawnRotation);
        spawnedObjects.Add(instance);
    }
}
