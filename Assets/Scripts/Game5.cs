using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class Game5 : MonoBehaviour
{
    public GameObject ButtonPrefab; //кнопка
    public Transform Grid; //сетка
    public GameObject[] GameButtons;
    public GameObject[] OtherObjects;
    public GameObject[] Hearts;

    public AudioSource ASS;
    public AudioClip click, lose, mistake, new_record, next_lvl;

    private List<GameObject> _buttons = new List<GameObject>();
    private List<GameObject> _correctButtons = new List<GameObject>();
    private List<GameObject> _clone = new List<GameObject>();
    private Color _greenColor, _redColor, _blueColor;
    private int _live; //всего жизней
    //private int _mistake; //ошибки на уровень, максимум 3 ошибки
    private int _level;
    private int _numbers;
    private int _gridSize=5; //количество кнопок с каждой стороны
    private bool _ispressed=false;
    private int _indexBut = 1;
    private void Start()
    {
        Initialization();
        StartGame();
    }

    void StartGame()
    {
        _live = 3;
        //_mistake = 3;
        _level = 1;
        //_gridSize = 3;
        _numbers = 3;
        CreateGrid();
    }
    void Initialization()
    {
        SetGlobalColors();
        SetOnButtonClick();
    }

    void SetGlobalColors()
    {
        UnityEngine.ColorUtility.TryParseHtmlString("#C5DD91", out _greenColor);
        UnityEngine.ColorUtility.TryParseHtmlString("#FF8A84", out _redColor);
        UnityEngine.ColorUtility.TryParseHtmlString("#323F65", out _blueColor);
    }

    void SetOnButtonClick()
    {
        for (int i = 0; i < GameButtons.Length; i++)
        {
            int index = i;
            GameButtons[i].GetComponent<Button>().onClick.AddListener(() => OnButtonClick2(index));
        }
    }

    void OnButtonClick2(int buttonIndex)
    {
        if (buttonIndex == 2)
        {
            GameOver();
            GlobalItems.PrevPage = 2;
            SceneManager.LoadScene(0);
            return;
        }
        else if (buttonIndex == 1)
        {
            OtherObjects[0].SetActive(false);
            StartGame();
            return;
        }
        else if (buttonIndex == 0)
        {
            GameOver();
            return;
        }
    }

    void CorrectCells()
    {
        SetInactiveButtons();

        List<GameObject> tmp = new List<GameObject>(_buttons); //сделано для того, чтобы при выборе кнопки не повторялись
        foreach (GameObject button in _buttons)
        {
            //button.SetActive(false);
            button.GetComponent<Image>().color=new Color(0,0,0,0);
        }
        for (int i = 0; i < _numbers + 1; i++)
        {
            GameObject randomButton = tmp[UnityEngine.Random.Range(0, tmp.Count)];// таким образом не будет повторений при выборе
            tmp.Remove(randomButton);
            _correctButtons.Add(randomButton);
            randomButton.GetComponentInChildren<TextMeshProUGUI>().text = Convert.ToString(i+1);
            randomButton.GetComponent<Button>().interactable = true;
        }
        tmp.Clear();
        _clone = _correctButtons;
        //foreach (GameObject button in _correctButtons)
        //{
        //    button.SetActive(true);
        //}

        //yield return ;
        //SetActiveButtons();
    }

    void MinusHeart()
    {
        
        _live--;
        _indexBut = 1;
        _ispressed = false;
        Hearts[_live].GetComponent<Image>().fillAmount = 0f;
        if (_live == 0)
        {
            GameOver();
        }
        else
        {
            //_mistake = 3;
            CreateGrid();
        }
    }

    private void ClearGrid() //Очистка поля
    {
        if (_buttons.Count > 0)
        {
            foreach (var item in _buttons)
                Destroy(item);

            _buttons.Clear();
        }
        _correctButtons.Clear();
    }

    void SetActiveButtons()
    {
        foreach (GameObject button in _buttons)
            button.GetComponent<Button>().interactable = true;
    }
    void SetInactiveButtons()
    {
        foreach (GameObject button in _buttons)
            button.GetComponent<Button>().interactable = false;
    }

    void CreateGrid()
    {
        ClearGrid();
        OtherObjects[1].GetComponent<TextMeshProUGUI>().text = "Level " + _level.ToString();
        FillHearts();
        //вычисление размера кнопок
        Vector2 ssize = new Vector2(420f / _gridSize, 420f / _gridSize);
        Grid.GetComponent<GridLayoutGroup>().cellSize = ssize;

        //заполнение сетки кнопками
        for (int i = 0; i < _gridSize; i++)
        {
            for (int j = 0; j < _gridSize; j++)
            {
                GameObject button = Instantiate(ButtonPrefab, Grid);
                button.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(button));

                _buttons.Add(button);
            }
        }
        CorrectCells();
    }

    private void OnButtonClick(GameObject button)
    {
        _ispressed = true;
        string aa = button.GetComponentInChildren<TextMeshProUGUI>().text;
        button.GetComponent<Button>().interactable = false; //отключение нажатой кнопки
        if(_ispressed==true)
        {
            foreach (GameObject s in _correctButtons)
            { 
                s.GetComponent<Image>().color = new Color(255, 255, 255, 255);
                s.GetComponentInChildren<TextMeshProUGUI>().color=new Color(0,0,0,0);
            }

        }
        if (aa == Convert.ToString(_indexBut))
        {
            _indexBut++;
            if (_correctButtons.Count != 0)
                ASS.PlayOneShot(click);
            button.GetComponent<Image>().color = new Color(0,0,0,0);
            _correctButtons.Remove(button);
            if (_correctButtons.Count == 0)
            {
                ASS.PlayOneShot(next_lvl);
                _level++;
                _indexBut = 1;
                _ispressed=false;
                //if (_level % 3 == 0)
                //    _gridSize++;
                _numbers++;
                //_mistake = 3;
                CreateGrid();
            }
        }
        else
        {
            ASS.PlayOneShot(mistake);
            MinusHeart();
            //_mistake--;
            //button.GetComponent<Image>().color = _redColor;
            //int live = _live - 1;
            //if (_mistake == 2)
            //    Hearts[live].GetComponent<Image>().fillAmount = 0.67f;
            //else if (_mistake == 1)
            //    Hearts[live].GetComponent<Image>().fillAmount = 0.33f;

            //if (_mistake == 0)
            //{
            //    Hearts[live].GetComponent<Image>().fillAmount = 0f;
            //    MinusHeart();
            //}
        }
    }

    void FillHearts()
    {
        for (int i = 0; i < _live; i++)
            Hearts[i].GetComponent<Image>().fillAmount = 1f;
    }

    void SetAfterGameWindow()
    {

        OtherObjects[0].SetActive(true);
        OtherObjects[3].GetComponent<TextMeshProUGUI>().text = (_level - 1).ToString();
        OtherObjects[2].GetComponent<TextMeshProUGUI>().text = "Game 5";
    }

    void GameOver()
    {
        ASS.PlayOneShot(lose);//добавить логику со сравнением текущего счета и рекорда из бд
        SetInactiveButtons();
        StopAllCoroutines();
        SetAfterGameWindow();
    }
}
