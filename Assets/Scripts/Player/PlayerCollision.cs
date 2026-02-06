using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private bool _isGround;
    private bool _isHardGround;
    
    public bool IsGround => _isGround || _isHardGround;

    public bool IsHardGround => _isHardGround;

    [Header("Collider Offset")]
    [SerializeField] private Vector3 _offset = Vector3.zero;

    [Space]
    [Header("Collider Radius")]
    [SerializeField] private float _radius = 1f;

    [Space]
    [Header("Collider Distance")]
    [SerializeField] private float _distance = 1f;

    [Space]
    [Header("Collider LayerMask")]
    [SerializeField] private LayerMask _layerMask;
    [Header("HardGround LayerMask")]
    [SerializeField] private LayerMask _hardGroundMask;

    private void FixedUpdate()
    {
        _isGround = Physics.OverlapSphere(transform.position + _offset, _radius, _layerMask).Length > 0;
        _isHardGround = Physics.OverlapSphere(transform.position + _offset, _radius, _hardGroundMask).Length > 0;
    }

    void OnDrawGizmos()
    {
        Vector3 playerPosition = transform.position;
        Vector3 startPosition = playerPosition + _offset;
        Vector3 direction = Vector3.down;

        Gizmos.color = _isGround ? Color.red : Color.green;

        Gizmos.DrawWireSphere(startPosition, _radius);
        Gizmos.DrawWireSphere(startPosition + direction * _distance, _radius);

        Gizmos.DrawLine(startPosition, startPosition + direction * _distance);
    }
}
