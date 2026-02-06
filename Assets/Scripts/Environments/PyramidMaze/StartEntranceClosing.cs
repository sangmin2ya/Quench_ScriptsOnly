using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartEntranceClosing : MonoBehaviour
{
    private Vector3 closingPosition = new Vector3(123, 100, 205);
    [SerializeField]
    private float speed = 1f;
    [SerializeField]
    private GameObject LightEntrance1;
    [SerializeField]
    private GameObject LightEntrance2;
    // Start is called before the first frame update
    void Start()
    {
        // stone moving sound? if necessary
        StartCoroutine("EntranceClosing");
        if (DataManager.Instance.GetStartPoint() == new Vector3(111.3f, 67.33f, 236.64f))
        {
            GameObject.Find("PlayerCameraRoot").GetComponent<CameraShake>().TriggerShake(7f, 0.05f);
            GetComponent<AudioSource>().volume = DataManager.Instance.SoundVolume / 2f;
            GetComponent<AudioSource>().Play();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator EntranceClosing()
    {
        while (transform.position != closingPosition)
        {
            yield return null;
            transform.position = Vector3.MoveTowards(transform.position, closingPosition, speed * Time.deltaTime);
            GetComponent<AudioSource>().volume = DataManager.Instance.SoundVolume / 2f;
        }
        LightEntrance1.SetActive(false);
        LightEntrance2.SetActive(false); // after closing entrance, disable light for optimization
    }
}