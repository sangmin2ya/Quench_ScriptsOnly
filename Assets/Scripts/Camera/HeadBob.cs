using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBob : MonoBehaviour
{
    [SerializeField] private float _frequency = 2.0f;
    [SerializeField] private float _height = 0.1f;

    private CharacterController characterController;
    private PlayerInputs _input;
    private Vector3 defaultPosition;
    private float timer = 0f;

    private void Awake()
    {
        defaultPosition = transform.localPosition;
        characterController = transform.parent.GetComponent<CharacterController>();
        _input = transform.parent.GetComponent<PlayerInputs>();
    }

    void Update()
    {
        if (characterController.velocity.magnitude > 0.1f && characterController.isGrounded)
        {
            timer += Time.deltaTime * _frequency * (_input.sprint ? 1.3f : 1.0f);
            float newYOffset = Mathf.Sin(timer) * _height;

            transform.localPosition = new Vector3(defaultPosition.x, defaultPosition.y + newYOffset, defaultPosition.z);
        }
        else
        {
            timer = 0f;
            transform.localPosition = defaultPosition;
        }
    }
}