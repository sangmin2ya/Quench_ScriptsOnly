using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.2f;

    private Vector3 initialPosition;
    private float currentShakeDuration;

    void Start()
    {
        initialPosition = transform.localPosition;
    }

    public void TriggerShake(float duration, float magnitude)
    {
        if (gameObject.GetComponent<HeadBob>() != null)
            gameObject.GetComponent<HeadBob>().enabled = false;
        shakeDuration = duration;
        shakeMagnitude = magnitude;
        currentShakeDuration = shakeDuration;
    }

    void Update()
    {
        if (currentShakeDuration > 0)
        {

            transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;

            currentShakeDuration -= Time.deltaTime;
        }
        else
        {
            if (gameObject.GetComponent<HeadBob>().enabled == false)
                transform.localPosition = initialPosition;
            if (gameObject.GetComponent<HeadBob>() != null)
                gameObject.GetComponent<HeadBob>().enabled = true;
            currentShakeDuration = 0f;

        }
    }
}
