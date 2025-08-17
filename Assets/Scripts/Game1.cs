using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

public class CubeGameManager : MonoBehaviour
{
    public GameObject[] GameButtons;
    public GameObject[] OtherObjects;
    public AudioSource ASS;
    public AudioClip click, lose, mistake, new_record, next_lvl;
    private List<int> _sequenceOfMoves = new List<int>(); 
    private Color _greenColor, _redColor; 
    private bool _gameOn, _usersTurn; 
    private float _coloringDuration = 1f, _delayBetweenColorings = 0.5f;
    private int _userMoveIndex, _levelGame;

    void Start()
    {
        Initialization();
        StartGame();
    }

    void Initialization()
    {
        SetGlobalColors();
        SetOnButtonClick();
    }

    void StartGame()
    {
        _sequenceOfMoves.Clear();
        _gameOn = true;
        _usersTurn = false;
        _userMoveIndex = 0;
        _levelGame = 1;

        ResetButtonColor();
        StartCoroutine(GameBegins());
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
        ColorUtility.TryParseHtmlString("#C5DD91", out _greenColor);
        ColorUtility.TryParseHtmlString("#FF8A84", out _redColor);
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

        if (buttonIndex == 9)
        {
            OtherObjects[3].SetActive(false);
            StartGame();
            return;
        }
        else if(buttonIndex == 10)
        {
            EndGame();
            GlobalItems.PrevPage = 2;
            SceneManager.LoadScene(0);
            return;
        }
        else if (buttonIndex == 11)
        {
            OtherObjects[3].SetActive(true);
            EndGame();
            return;
        }

        if (buttonIndex == _sequenceOfMoves[_userMoveIndex])
        {
            GameButtons[buttonIndex].GetComponent<Image>().color = _greenColor;
            ASS.PlayOneShot(click);
            Invoke("ResetButtonColor", 0.2f);
            _userMoveIndex++;
        }
        else
        {
            GameButtons[buttonIndex].GetComponent<Image>().color = _redColor;
            Invoke("EndGame", 0.4f);
        }

        if (_userMoveIndex == _sequenceOfMoves.Count)
        {
            Invoke("ColorButtonsToGreen", 0.4f);
            _usersTurn = false;
        }
    }

    void SetAfterGameWindow()
    {
        OtherObjects[3].SetActive(true);
        OtherObjects[2].GetComponent<TextMeshProUGUI>().text = (_levelGame - 1).ToString();
        OtherObjects[1].GetComponent<TextMeshProUGUI>().text = "Game 1";
    }

    void ResetButtonColor()
    {
        for (int i = 0; i < GameButtons.Length - 3; i++)
            GameButtons[i].GetComponent<Image>().color = Color.white;
    }

    void ColorButtonsToGreen()
    {
        for (int i = 0; i < GameButtons.Length - 3; i++)
            GameButtons[i].GetComponent<Image>().color = _greenColor;
    }

    void SetInactiveButtons()
    {
        for (int i = 0; i < GameButtons.Length - 3; i++)
            GameButtons[i].GetComponent<Button>().interactable = false;
    }
    void SetActiveButtons()
    {
        for (int i = 0; i < GameButtons.Length - 3; i++)
            GameButtons[i].GetComponent<Button>().interactable = true;
    }

    IEnumerator GameBegins()
    {

        while (_gameOn)
        {
            if (!_usersTurn)
            {
                SetInactiveButtons();
                ResetButtonColor();

                _userMoveIndex = 0;
                yield return StartCoroutine(ColoringSequence());

                SetActiveButtons();
                _usersTurn = true;
                yield return new WaitUntil(() => !_usersTurn);
                yield return new WaitForSeconds(0.5f);

            }
            ASS.PlayOneShot(next_lvl);
            _levelGame++;
        }
    }

    IEnumerator ColoringSequence()
    {
        OtherObjects[0].GetComponent<TextMeshProUGUI>().text = "LEVEL " +_levelGame.ToString();

        _sequenceOfMoves.Add(Random.Range(0, GameButtons.Length - 3));

        for (int i = 0; i < _sequenceOfMoves.Count; i++)
        {
            GameButtons[_sequenceOfMoves[i]].GetComponent<Image>().color = _greenColor;
            yield return new WaitForSeconds(_coloringDuration);

            GameButtons[_sequenceOfMoves[i]].GetComponent<Image>().color = Color.white;
            yield return new WaitForSeconds(_delayBetweenColorings);
        }
    }
}