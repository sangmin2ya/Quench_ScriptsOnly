using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChangeToOverWorld : MonoBehaviour
{
    [SerializeField]
    private Image image;
    private WaitForSecondsRealtime zpzo = new WaitForSecondsRealtime(0.01f);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.transform.GetComponent<CharacterController>().enabled = false;
            other.transform.GetComponent<PlayerInputs>().move = new Vector2(0f, 0f);
            other.transform.GetComponent<PlayerInputs>().look = new Vector2(0f, 0f);
            StartCoroutine(FadeOut(other.gameObject));
        }
    }

    IEnumerator FadeOut(GameObject player)
    {
        float fadeOut = 0f;
        while (fadeOut <= 1f)
        {
            fadeOut += 0.01f;
            yield return zpzo;
            image.color = new Color(0f, 0f, 0f, fadeOut);
        }
        //player.transform.GetComponent<CharacterController>().gameObject.SetActive(true);
        // Scene Change Code Here
        DataManager.Instance.SetStartPoint(new Vector3(624.2f, -1.3f, -134f));

        SceneManager.LoadScene("Main");
        //
    }
}
