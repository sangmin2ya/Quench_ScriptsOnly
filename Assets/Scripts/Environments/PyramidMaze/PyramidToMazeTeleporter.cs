using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PyramidToMazeTeleporter : MonoBehaviour
{
    [SerializeField]
    private GameObject startPositionBlock;
    [SerializeField]
    private Image image;
    [SerializeField]
    private GameObject Underground;
    [SerializeField]
    private GameObject Maze;

    private WaitForSecondsRealtime zpzo = new WaitForSecondsRealtime(0.01f);
    private WaitForSecondsRealtime zpzf = new WaitForSecondsRealtime(0.05f);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Player.transform.position += new Vector3(0.1f, 0.1f, 0.1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(FadeOut(other.gameObject));
            other.transform.GetComponent<CharacterController>().enabled = false;
            other.transform.GetComponent<PlayerInputs>().move = new Vector2(0f, 0f);
            other.transform.GetComponent<PlayerInputs>().look = new Vector2(0f, 0f);
            Debug.Log("Teleport On");
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
        TeleportPlayerToMaze(player);
    }

    private void TeleportPlayerToMaze(GameObject player)
    {
        Maze.gameObject.SetActive(true);
        player.transform.position = startPositionBlock.transform.position;
        player.transform.rotation = Quaternion.Euler(0f, 92f, 0f);
        player.transform.GetComponent<CharacterController>().enabled = true;
        StartCoroutine(FadeIn(player));
    }

    IEnumerator FadeIn(GameObject player)
    {
        yield return zpzf;

        float fadeIn = 1f;
        while (fadeIn >= 0f)
        {
            fadeIn -= 0.01f;
            yield return zpzo;
            image.color = new Color(0f, 0f, 0f, fadeIn);
        }

        HidePyramid();
    }

    private void HidePyramid()
    {
        Underground.gameObject.SetActive(false);
    }
}
