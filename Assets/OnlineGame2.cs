using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class OnlineGame2 : MonoBehaviour
{
    public GameObject[] GameButtons;
    public GameObject[] OtherObjects;
    public GameObject ProgressBar;

    public AudioSource ASS;
    public AudioClip click, lose, mistake, new_record, next_lvl;

    private string _number;
    private int _levelGame;
    private Color _greenColor, _redColor;
    private bool _gameOn, _usersTurn;
    private const int ProgressBarTime = 4, TotalGameTime = 150, InputNumberTime = 15;

    void Start()
    {
        Initialization();
        StartGame();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (OtherObjects[4].GetComponent<TMP_InputField>().interactable)
                OnButtonClick(0);
        }
    }

    void Initialization()
    {
        SetGlobalColors();
        SetOnButtonClick();
    }

    void StartGame()
    {
        _number = "";
        _gameOn = true;
        _usersTurn = false;
        _levelGame = 2;
        StartCoroutine(Timer(TotalGameTime));
        StartCoroutine(GameBegins());
    }

    IEnumerator Timer(int numberTimer)
    {
        while (numberTimer > 0 && _gameOn)
        {
            OtherObjects[5].GetComponent<TextMeshProUGUI>().text = numberTimer.ToString();

            yield return new WaitForSeconds(1f);
            numberTimer--;

        }

        EndGame();
    }

    void EndGame()
    {
        _gameOn = false;
        ASS.PlayOneShot(lose);//добавить логику со сравнением текущего счета и рекорда из бд
        StopAllCoroutines();
        SetAfterGameWindow();
    }

    void SetGlobalColors()
    {
        UnityEngine.ColorUtility.TryParseHtmlString("#C5DD91", out _greenColor);
        UnityEngine.ColorUtility.TryParseHtmlString("#FF8A84", out _redColor);
    }

    void SetOnButtonClick()
    {

        for (int i = 0; i < GameButtons.Length; i++)
        {
            int index = i;
            GameButtons[i].GetComponent<Button>().onClick.AddListener(() => OnButtonClick(index));
        }
    }

    void OnButtonClick(int buttonIndex)
    {

        if (buttonIndex == 2)
        {
            OtherObjects[3].SetActive(false);
            StartGame();
            return;
        }
        else if (buttonIndex == 3)
        {
            EndGame();
            GlobalItems.PrevPage = 2;
            SceneManager.LoadScene(0);
            return;
        }
        else if (buttonIndex == 1)
        {
            OtherObjects[3].SetActive(true);
            EndGame();
            return;
        }

        if (OtherObjects[4].GetComponent<TMP_InputField>().text == _number.ToString())
        {
            ASS.PlayOneShot(next_lvl);
            ProgressBar.GetComponent<Image>().color = _greenColor;
            _usersTurn = false;
        }
        else
        {
            ProgressBar.GetComponent<Image>().color =  _redColor;
            _gameOn = false;
            EndGame();
        }
    }

    void SetAfterGameWindow()
    {
        OtherObjects[3].SetActive(true);
        OtherObjects[2].GetComponent<TextMeshProUGUI>().text = (_levelGame - 1).ToString();
        OtherObjects[1].GetComponent<TextMeshProUGUI>().text = "Numerical coverage";
    }

    IEnumerator LineTimer(int seconds)
    {
        SetInactiveButtons();

        OtherObjects[0].GetComponent<TextMeshProUGUI>().text = "LEVEL " + (++_levelGame).ToString();

        RandomNumber();

        yield return new WaitForSeconds(0.2f);
        ProgressBar.GetComponent<Image>().color = Color.white;
        float timer = 0f;
        float fillamount = 0f;

        while (timer<seconds)
        {
            timer += Time.deltaTime;
            ProgressBar.GetComponent<Image>().fillAmount = Mathf.Lerp(fillamount, 1, timer/seconds);
            yield return null;
        }

        OtherObjects[4].GetComponent<TMP_InputField>().text = "";
        SetActiveButtons();
    }

    IEnumerator InputTimer(int seconds)
    {
        ProgressBar.GetComponent<Image>().color = Color.white;
        float timer = 0f;
        float fillamount = 0f;

        while (timer<seconds)
        {
            if (!_usersTurn)
                break;
            timer += Time.deltaTime;
            ProgressBar.GetComponent<Image>().fillAmount = Mathf.Lerp(fillamount, 1, timer/seconds);
            yield return null;
        }

        if (_usersTurn)
            EndGame();
    }

    void SetActiveButtons()
    {
        OtherObjects[4].GetComponent<TMP_InputField>().interactable = true;
        OtherObjects[4].GetComponent<TMP_InputField>().ActivateInputField();
        GameButtons[0].gameObject.SetActive(true);
    }

    void SetInactiveButtons()
    {
        OtherObjects[4].GetComponent<TMP_InputField>().interactable = false;
        GameButtons[0].gameObject.SetActive(false);
    }

    void RandomNumber()
    {
        if (_levelGame == 0)
            _number += Random.Range(0, 10);
        else
        {
            _number += Random.Range(0, 10);
            for (int i = 1; i < _levelGame; i++)
                _number += Random.Range(1, 10);
        }

        Debug.LogWarning(_number);

        OtherObjects[4].GetComponent<TMP_InputField>().text = _number;
    }

    IEnumerator GameBegins()
    {
        while (_gameOn)
        {
            if (!_usersTurn)
            {
                if (_levelGame == 24)
                    EndGame();

                _number = "";
                yield return StartCoroutine(LineTimer(ProgressBarTime));

                _usersTurn = true;
                StartCoroutine(InputTimer(InputNumberTime));
                yield return new WaitUntil(() => !_usersTurn);
            }
        }
    }
}
