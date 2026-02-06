using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    private AudioClip _currentClip;
    private AudioClip _tempClip;
    private AudioSource _audioSource;
    [SerializeField]private GameObject _tornado;

    private float _magnitude;

    private bool _isTransition;

    private float _targetVolume;
    private float _currentVolume;
    private float _volume;

    private void Awake()
    {
        _tornado = GameObject.Find("TornadoEfc");
        _currentClip = Resources.Load<AudioClip>("Sound/wind");
        _audioSource = GetComponent<AudioSource>();
        _isTransition = false;
        _volume = 1f;
        _audioSource.volume = DataManager.Instance.SoundVolume;
    }

    public void ChangeMusic(AudioClip clip, float startVolume)
    {
        if (_tempClip == clip) return;


        _tempClip = clip;

        StopAllCoroutines();
        StartCoroutine(BGM_Transition(clip, startVolume));
    }

    public void ChangeVolume(float value)
    {
        if (_isTransition) return;

        _targetVolume = value;
    }

    public void ChangeMagnitude(float value)
    {
        _magnitude = value;
    }

    private void Update()
    {
        if (!_isTransition && _volume != _targetVolume)
        {
            _currentVolume = Mathf.MoveTowards(_currentVolume, _targetVolume, Time.deltaTime);
            _volume = _currentVolume * _magnitude;
        }
        _audioSource.volume = _volume * DataManager.Instance.SoundVolume;
        if(_tornado != null)
            _tornado.GetComponent<AudioSource>().volume = DataManager.Instance.SoundVolume;
    }

    private IEnumerator BGM_Transition(AudioClip clip, float startVolume)
    {
        _isTransition = true;

        while (true)
        {
            if (_volume <= 0) break;

            _volume = Mathf.MoveTowards(_volume, 0, Time.deltaTime);

            yield return null;
        }

        _currentClip = clip;
        _audioSource.clip = _currentClip;
        _audioSource.Play();
        _targetVolume = startVolume;


        while (_volume < _targetVolume)
        {
            _volume = Mathf.MoveTowards(_volume, _targetVolume, Time.deltaTime);
            yield return null;
        }


        _isTransition = false;
    }
}
