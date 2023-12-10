using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carmove : MonoBehaviour
{
    public float destructionDelay = 30.0f;
    private float moveSpeed; // �̵� �ӵ�
    public Vector3 moveDirection = Vector3.forward; // ������ ���� (�⺻������ ������)

    void Start()
    {
        // ������ �ӵ� ����
        moveSpeed = Random.Range(40.0f, 120.0f);

        Destroy(gameObject, destructionDelay);
    }

    void Update()
    {
        // �̵� ����� �ӵ��� ���Ͽ� �̵� ���� ���
        Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;

        // �̵� ���͸� ���� ��ġ�� �����־� ������Ʈ �̵�
        transform.Translate(movement);
    }
}

