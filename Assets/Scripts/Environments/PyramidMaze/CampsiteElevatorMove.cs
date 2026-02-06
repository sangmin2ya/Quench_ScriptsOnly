using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampsiteElevatorMove : MonoBehaviour
{
    private float DeltaTime = 0f;
    [SerializeField]
    private float speed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            DeltaTime += Time.unscaledDeltaTime;
            if (DeltaTime > 0.7f)
            {
                transform.position += Vector3.up * speed * Time.unscaledDeltaTime;
            }
        }
    }
}
