using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class car1move : MonoBehaviour
{
    private float moveSpeed; // 이동 속도
    public Vector3 moveDirection = Vector3.forward; // 움직일 방향 (기본적으로 앞으로)

    void Start()
    {
        // 랜덤한 속도 설정
        moveSpeed = Random.Range(10.0f, 40.0f);
    }

    void Update()
    {
        // 이동 방향과 속도를 곱하여 이동 벡터 계산
        Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;

        // 이동 벡터를 현재 위치에 더해주어 오브젝트 이동
        transform.Translate(movement);
    }
}

