using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerThirstController : MonoBehaviour
{
    [SerializeField] private float _maxThirst = 100f;
    [Range(0, 100)]
    [SerializeField] private float _currentThirst;

    [Range(0, 1)]
    [SerializeField] private float _thirstDecreaseRate = 0.33f;
    [Range(0, 1)]
    [SerializeField] private float _walkingThirstDecreaseRate = 0.33f;
    [Range(0, 1)]
    [SerializeField] private float _runningThirstDecreaseRate = 0.66f;

    [SerializeField] private float _lowThirstThreshold1 = 50f;
    [SerializeField] private float _lowThirstThreshold2 = 25f;
    [SerializeField] private float _lowThirstThreshold3 = 10f;
    public CameraController cameraController;

    private Image _thirstEffect;
    private PlayerInputs _input;
    private float _sprintDuration = 0;
    private Color _thirstEffectColor;
    private Image _thirstTextImg;
    private float _thirstMultiplier = 1;
    private TextMeshProUGUI _thirstText;

    private bool _resetThirstOnce = true;
    private bool _blockFastForward = false;
    private Vector3 _initialPosition = new Vector3(-19, -1.5f, 20);

    //private PostProcessVolume _postProcessVolume;

    void Start()
    {
        transform.GetComponent<CharacterController>().enabled = false;
        transform.GetComponent<PlayerInputs>().move = new Vector2(0f, 0f);
        transform.GetComponent<PlayerInputs>().look = new Vector2(0f, 0f);
        if (SceneManager.GetActiveScene().name == "Underground")
        {
            if (DataManager.Instance.GetStartPoint() == new Vector3(-360.16f, 79.5f, 57.97f)) // underground Ladder start position
                GameObject.Find("UnderGround").SetActive(false);
            else
                GameObject.Find("Maze_Total").SetActive(false);
        }

        transform.position = DataManager.Instance.GetStartPoint(); //_initialPosition;
        transform.GetComponent<CharacterController>().enabled = true;
        //wDataManager.Instance.SetStartPoint(new Vector3(-19, -1.5f, 20)); // always reset start point

        //transform.rotation = Quaternion.Euler(0, 0, 0);
        _currentThirst = DataManager.Instance.getPlayerThirst();
        _input = GetComponent<PlayerInputs>();
        //_postProcessVolume = GameObject.Find("PostProcessVolume").GetComponent<PostProcessVolume>();
        _thirstEffect = transform.Find("PlayerCanvas/ThirstEffect").GetComponent<Image>();
        _thirstEffectColor = _thirstEffect.color;
        _thirstTextImg = transform.Find("PlayerCanvas/Thirsty").GetComponent<Image>();
        _thirstText = transform.Find("PlayerCanvas/Thirsty/ThirstyTxt").GetComponent<TextMeshProUGUI>();

        if (DataManager.Instance._deathCount == 1) TriggerReviveWarning(JsonReader.Instance.IngameText("revive_01"));
        else if (DataManager.Instance._deathCount == 2) TriggerReviveWarning(JsonReader.Instance.IngameText("revive_02"));
        //if (SceneManager.GetActiveScene().name == "Underground") // this position is for official route of underground
        //    transform.position = new Vector3(111.3f, 67.33f, 236.64f);
        // if player enter grave robber's camp by using elevator, should start in different position
        // in that case, must using " new Vector3(517.84f, 68.41f, 351.89f); "
    }

    void FixedUpdate()
    {
        UpdateThirst();
        CheckThirst();
        FastForward();
        DataManager.Instance.setPlayerThirst(_currentThirst);
    }
    void FastForward()
    {
        if (_blockFastForward) return;
        if (Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale = 30;
            GameObject.FindWithTag("Player").GetComponent<PlayerInput>().enabled = false;
        }
        if (Input.GetKeyUp(KeyCode.P))
        {
            Time.timeScale = 1;
            GameObject.FindWithTag("Player").GetComponent<PlayerInput>().enabled = true;
        }
    }
    void UpdateThirst()
    {
        if (_input.sprint)
        {
            _sprintDuration += Time.fixedDeltaTime;
            _currentThirst -= _runningThirstDecreaseRate * Time.fixedDeltaTime * _thirstMultiplier;

        }
        else if (_input.move != Vector2.zero)
        {
            _sprintDuration = 0;
            _currentThirst -= _walkingThirstDecreaseRate * Time.fixedDeltaTime * _thirstMultiplier;
        }
        else
        {
            _sprintDuration = 0;
            _currentThirst -= _thirstDecreaseRate * Time.fixedDeltaTime * _thirstMultiplier;
        }
        _currentThirst = Mathf.Clamp(_currentThirst, 0, _maxThirst);
        if (_sprintDuration > 5)
        {
            TriggerRunWarning(JsonReader.Instance.IngameText("run_01"));
        }
    }

    void CheckThirst()
    {
        if (_currentThirst < _lowThirstThreshold3)
        {
            TriggerLowThirstWarning3(JsonReader.Instance.IngameText("thirst_01"));
        }
        else if (_currentThirst < _lowThirstThreshold2)
        {
            TriggerLowThirstWarning2(JsonReader.Instance.IngameText("thirst_02"));
        }
        else if (_currentThirst < _lowThirstThreshold1)
        {
            TriggerLowThirstWarning1(JsonReader.Instance.IngameText("thirst_03"));
        }


        if (_currentThirst <= 0 && _resetThirstOnce)
        {
            _blockFastForward = true;
            ResetThirst();
            Time.timeScale = 1;
            _resetThirstOnce = false;
        }
    }

    /// <summary>
    /// trigger a warning when the thirst is very low
    /// </summary>
    void TriggerLowThirstWarning1(string thirstText)
    {
        if (DataManager.Instance._thirstyOnce1)
        {
            _thirstText.text = thirstText;
            StopAllCoroutines();
            StartCoroutine(FadeCoroutine(0));
            DataManager.Instance._thirstyOnce1 = false;
        }
    }
    void TriggerLowThirstWarning2(string thirstText)
    {
        if (DataManager.Instance._thirstyOnce2)
        {
            _thirstText.text = thirstText;
            StopAllCoroutines();
            StartCoroutine(FadeCoroutine(0));
            DataManager.Instance._thirstyOnce2 = false;
        }
    }
    void TriggerLowThirstWarning3(string thirstText)
    {
        Color color = new Color(_thirstEffectColor.r, _thirstEffectColor.g, _thirstEffectColor.b, 1 - Mathf.Max(0, _currentThirst / _lowThirstThreshold3));
        GameObject.Find("Dying").GetComponent<PostProcessVolume>().weight = 1 - Mathf.Max(0, _currentThirst / _lowThirstThreshold3);
        _thirstEffect.color = color;
        if (DataManager.Instance._thirstyOnce3)
        {
            _thirstText.text = thirstText;
            StopAllCoroutines();
            StartCoroutine(FadeCoroutine(0));
            DataManager.Instance._thirstyOnce3 = false;
        }
    }
    public void TriggerRunWarning(string runText)
    {
        if (DataManager.Instance._runInfo)
        {
            DataManager.Instance._runInfo = false;
            _thirstText.text = runText;
            StopAllCoroutines();
            StartCoroutine(FadeCoroutine(0));
        }
    }
    public void TriggerReviveWarning(string reviveText)
    {
        if (DataManager.Instance._reviveInfo)
        {
            DataManager.Instance._reviveInfo = false;
            _thirstText.text = reviveText;
            StopAllCoroutines();
            StartCoroutine(FadeCoroutine(2f));
        }
    }
    public IEnumerator FadeCoroutine(float wait)
    {
        yield return new WaitForSeconds(wait);
        float duration = 0.5f;
        float holdTime = 3.0f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / duration);
            SetAlpha(alpha);
            yield return null;
        }

        yield return new WaitForSeconds(holdTime);

        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(1.0f - (elapsedTime / duration));
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(0);
    }
    private void SetAlpha(float alpha)
    {
        if (_thirstTextImg != null)
        {
            Color imageColor = _thirstTextImg.color;
            imageColor.a = alpha;
            _thirstTextImg.color = imageColor;
        }

        if (_thirstText != null)
        {
            Color textColor = _thirstText.color;
            textColor.a = alpha;
            _thirstText.color = textColor;
        }
    }

    public void ResetThirst()
    {
        //_thirstyOnce = true;
        //_thirstEffect.color = _thirstEffectColor;
        //GameObject.Find("TimeController").GetComponent<TimeController>().ResetTime();
        DataManager.Instance._deathCount++;
        DataManager.Instance._getGemstone = false;
        cameraController.OnPlayerDeath();
        GetComponent<PlayerFootstep>().StopFootsteps();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Collider가 "Area" 태그를 가지고 있는지 확인
        if (other.CompareTag("Area"))
        {
            // Area 내부에서는 _thirstMultiplier를 0.8로 설정
            _thirstMultiplier = 0.6f;
            Debug.Log("Entered Area: _thirstMultiplier set to " + _thirstMultiplier);
        }
        if (other.CompareTag("Outer"))
        {
            _thirstMultiplier = 4f;
            _thirstText.text = JsonReader.Instance.IngameText("thirst_04");
            StopAllCoroutines();
            StartCoroutine(FadeCoroutine(2f));
            Debug.Log("Exited Area: _thirstMultiplier set to " + _thirstMultiplier);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Collider가 "Area" 태그를 가지고 있는지 확인
        if (other.CompareTag("Area"))
        {
            // Area 외부에서는 _thirstMultiplier를 1.5로 설정
            _thirstMultiplier = 1.5f;
            Debug.Log("Exited Area: _thirstMultiplier set to " + _thirstMultiplier);
        }
        if (other.CompareTag("Outer"))
        {
            _thirstMultiplier = 1.5f;
            Debug.Log("Exited Area: _thirstMultiplier set to " + _thirstMultiplier);
        }
    }
}
