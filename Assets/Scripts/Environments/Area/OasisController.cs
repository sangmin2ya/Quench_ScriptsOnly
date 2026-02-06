using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OasisController : MonoBehaviour
{
    private Transform _player;
    private float _minY = -8.5f;
    private float _maxY = -0.2f;
    private float _maxDistance = 100f;
    private float _minDistance = 10f;
    private float distance;

    private void Start()
    {
        _player = GameObject.FindWithTag("Player").transform;

    }
    private void Update()
    {
        Vector3 playerPositionXZ = new Vector3(_player.position.x, 0, _player.position.z);
        Vector3 objectPositionXZ = new Vector3(transform.position.x + 75, 0, transform.position.z + 70);
        distance = Vector3.Distance(playerPositionXZ, objectPositionXZ);
        float yValue;
        if (distance > _maxDistance)
        {
            // 거리가 startDistance 이상이면 y 값을 -44로 고정
            yValue = _maxY;
        }
        else if (distance <= _minDistance)
        {
            // 거리가 endDistance 이하이면 y 값을 -60으로 고정
            yValue = _minY;
        }
        else
        {
            // 거리 범위 내에서의 비율 계산 (Lerp를 위한 t 값)
            float _t = Mathf.InverseLerp(_minDistance, _maxDistance, distance);
            // y 값 계산
            yValue = Mathf.Lerp(_minY, _maxY, _t);
        }
        Vector3 newPosition = transform.localPosition;
        newPosition.y = yValue;
        transform.localPosition = newPosition;
    }
}
