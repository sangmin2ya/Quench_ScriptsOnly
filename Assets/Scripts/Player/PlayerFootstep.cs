using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerFootstep : MonoBehaviour
{
    [SerializeField] private AudioClip[] _footstepSounds;
    [SerializeField] private AudioClip[] _footstepSounds_hard;
    private AudioSource _audioSource;
    [SerializeField] private float footstepInterval = 0.3f;

    private CharacterController _characterController;
    private PlayerCollision _playerCollision;
    private PlayerInputs _input;
    private bool _isWalking = false;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _playerCollision = GetComponent<PlayerCollision>();
        _audioSource = GetComponent<AudioSource>();
        _input = GetComponent<PlayerInputs>();
        StartCoroutine("PlayFootsteps");
    }

    void FixedUpdate()
    {
        if (_playerCollision.IsGround && _characterController.velocity.magnitude > 0.1f)
        {
            _isWalking = true;
        }
        else
        {
            _isWalking = false;
        }
    }

    IEnumerator PlayFootsteps()
    {
        while (true)
        {
            if (_isWalking)
            {
                PlayRandomFootstep();
            }
            yield return new WaitForSeconds(footstepInterval * (_input.sprint ? 1f / 1.3f : 1.0f));
        }
    }
    public void StopFootsteps()
    {
        StopCoroutine("PlayFootsteps");
    }
    void PlayRandomFootstep()
    {
        int maxSize = _playerCollision.IsHardGround ? 9 : 20;
        int randomIndex = Random.Range(0, maxSize);

        AudioClip randomFootstep =
            _playerCollision.IsHardGround ? _footstepSounds_hard[randomIndex] : _footstepSounds[randomIndex];
        _audioSource.volume = DataManager.Instance.SoundVolume / 3f;
        _audioSource.PlayOneShot(randomFootstep);
    }
}
