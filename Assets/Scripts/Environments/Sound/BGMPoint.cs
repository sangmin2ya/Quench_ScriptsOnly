using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPoint : MonoBehaviour
{
    // Save Current BGMPoint for resolve conflicting
    private static GameObject _currentArea;

    private Vector3 _centerPos;
    private float _currentDistance;
    private GameObject _player;

    private AudioClip _windSound;
    private MusicController _audioPlayer;

    [Header("Area BGM")]
    [SerializeField] private AudioClip _bgmClip;

    [Space]

    [Header("Distance")]
    [SerializeField] private float _triggerDistance = 10f;
    [SerializeField] private float _maxVolumeDistance = 2f;

    [Space]

    [Header("BGM Magnitude")]
    [Range(0, 1f)]
    [SerializeField] private float _areaMagnitude = 1f;

    private void Awake()
    {
        _currentArea = null;
        _player = GameObject.FindGameObjectWithTag("Player");
        _windSound = Resources.Load<AudioClip>("Sound/wind");
        _audioPlayer = GameObject.Find("BGMPlayer").GetComponent<MusicController>();
        _currentDistance = 100000f;
    }

    private void FixedUpdate()
    {
        CalculatePlayerDistance();
        ModifyVolume();
    }

    private void CalculatePlayerDistance()
    {
        _currentDistance = Vector3.Distance(
            transform.position, _player.transform.position);
    }

    private void ModifyVolume()
    {
        if (_currentArea != null && _currentArea != gameObject) return;

        if (_currentDistance <= _triggerDistance)
        {
            _currentArea = gameObject;

            _audioPlayer.ChangeMusic(_bgmClip, 0.1f);

            if (_currentDistance <= _maxVolumeDistance)
            {
                _audioPlayer.ChangeVolume(1.0f);
            }
            else
            {
                _audioPlayer.ChangeVolume(
                    Mathf.Clamp01(1 - (_currentDistance - _maxVolumeDistance) / (_triggerDistance - _maxVolumeDistance)));
            }

            _audioPlayer.ChangeMagnitude(_areaMagnitude);
        }
        else
        {
            _currentArea = null;
            _audioPlayer.ChangeMusic(_windSound, 0.7f);
            _audioPlayer.ChangeMagnitude(1f);
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.25f);
        Gizmos.DrawSphere(transform.position, _triggerDistance);

        Gizmos.color = new Color(1, 0, 0, 0.25f);
        Gizmos.DrawSphere(transform.position, _maxVolumeDistance);
    }
}