using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchInteract : MonoBehaviour
{
    [SerializeField]
    private GameObject ShortCutRock1;
    [SerializeField]
    private GameObject ShortCutRock2;

    private bool switchEnable = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenShortCut()
    {
        //if (switchEnable)
        //    return; // if switch is interacted in this scene, return immediately

        if (SceneManager.GetActiveScene().name == "Main")
            GoUndergroundUsingHiddenEntrance();
        else
        {
            if (DataManager.Instance._goUnderground)
            {
                DataManager.Instance._goUnderground = false;

                SceneManager.LoadScene("Main");
            }
            else
            {
                StartCoroutine("MoveShortCutRock");
                ShortCutRock1.GetComponent<AudioSource>().volume = DataManager.Instance.SoundVolume;
                ShortCutRock1.GetComponent<AudioSource>().Play();
                GameObject.Find("PlayerCameraRoot").GetComponent<CameraShake>().TriggerShake(6f, 0.02f);
            }
        }

    }

    IEnumerator MoveShortCutRock()
    {
        float DeltaTime = 0f;
        while (DeltaTime < 6f)
        {
            yield return null;
            ShortCutRock1.GetComponent<AudioSource>().volume = DataManager.Instance.SoundVolume / 2f;
            ShortCutRock1.transform.position -= Vector3.right * 3 * Time.unscaledDeltaTime;
            ShortCutRock2.transform.position += Vector3.up * 3 * Time.unscaledDeltaTime;
        }
    }

    private void GoUndergroundUsingHiddenEntrance()
    {
        // already start position is set
        SceneManager.LoadScene("UnderGround");
    }
}
