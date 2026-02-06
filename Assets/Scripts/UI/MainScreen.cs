using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MainScreen : MonoBehaviour
{
    public TimeController TimeController;
    public ParticleSystem ParticleSystem;
    public GameObject IntroScene;
    private void Awake()
    {
        TimeController = GameObject.Find("TimeController")
            .GetComponent<TimeController>();
        ParticleSystem = transform.Find("TornadoEfc").
            GetComponent<ParticleSystem>();
        IntroScene = GameObject.Find("Canvas").transform
            .Find("IntroScene").gameObject;
        
        if (DataManager.Instance.IsStarted())
        {
            Destroy(IntroScene.gameObject);
            Destroy(ParticleSystem.gameObject);
            return;
        }
        ScreenManager.Instance.PauseGame();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        RenderSettings.fogStartDistance = 10f;
        RenderSettings.fogEndDistance = 70f;
        TimeController.TimeStop();
    }
    private void LateUpdate()
    {
        if (DataManager.Instance.IsStarted()) return;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        ParticleSystem.Simulate(Time.unscaledDeltaTime, true, false);
    }
    
    public void TriggerStartGame()
    {
        StartCoroutine(StartGame());
    }
    public IEnumerator StartGame()
    {
        float currentTime = 0f;
        float duration =  7f;
        ParticleSystem.EmissionModule em = ParticleSystem.emission;
        AudioSource audso = ParticleSystem.gameObject.GetComponent<AudioSource>();
        while (currentTime < duration)
        {
            yield return null;
            currentTime += Time.unscaledDeltaTime;

            float ratio = currentTime / duration;

            em.rateOverTimeMultiplier = 1 - ratio;
            audso.volume = 1 - ratio;

            RenderSettings.fogStartDistance = Mathf.Lerp(10f, 300f, ratio);
            RenderSettings.fogEndDistance = Mathf.Lerp(70f, 700f, ratio);
        }

        em.rateOverTimeMultiplier = 0;

        RenderSettings.fogStartDistance = 300f;
        RenderSettings.fogEndDistance = 700f;
        ScreenManager.Instance.ResumeGame();
        TimeController.TimeRun();
        ParticleSystem.
            Stop(withChildren: false, stopBehavior: ParticleSystemStopBehavior.StopEmitting);
        IntroScene.SetActive(false);
        gameObject.SetActive(false);
        DataManager.Instance.GameStarted();
        yield return null;
    }
}