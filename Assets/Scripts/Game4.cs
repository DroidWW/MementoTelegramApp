using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class Game4 : MonoBehaviour
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
    private Color _greenColor, _redColor, _blueColor;
    private int _live; //всего жизней
    private int _mistake; //ошибки на уровень, максимум 3 ошибки
    private int _level;
    private int _gridSize; //количество кнопок с каждой стороны


    private void Start()
    {
        Initialization();
        StartGame();
    }

    void StartGame()
    {
        _live = 3;
        _mistake = 3;
        _level = 1;
        _gridSize = 3;
        Grid.rotation = Quaternion.Euler(0, 0, 0);
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

    IEnumerator CorrectCells()
    {
        SetInactiveButtons();

        List<GameObject> tmp = new List<GameObject>(_buttons); //сделано для того, чтобы при выборе кнопки не повторялись
        for (int i = 0; i < _level + 2; i++)
        {
            GameObject randomButton = tmp[Random.Range(0, tmp.Count)];// таким образом не будет повторений при выборе
            tmp.Remove(randomButton);

            _correctButtons.Add(randomButton);
            randomButton.GetComponent<Image>().color = _greenColor;

        }
        tmp.Clear();

        yield return new WaitForSeconds(2f);
        foreach (GameObject button in _correctButtons)
            button.GetComponent<Image>().color = Color.white;

        //Поворот сетки
        
        //Grid.rotation=Quaternion.Euler(0, 0, 0);
        Quaternion start = Grid.rotation;
        float timer = 0f; ;
        float time_to_rotate = 1f;
        int rnd = Random.Range(0, 2);
        Quaternion targetRotation;

        if (rnd == 0)
            targetRotation = start*Quaternion.Euler(0, 0, 90);
        else
            targetRotation = start*Quaternion.Euler(0, 0, -90);
        
        while(timer<time_to_rotate)
        {
            timer += Time.deltaTime;
            Grid.rotation = Quaternion.Lerp(start, targetRotation, timer / time_to_rotate);
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);
        SetActiveButtons();
    }

    void MinusHeart()
    {
        _live--;
        if (_live == 0)
        {
            GameOver();
        }
        else
        {
            _mistake = 3;
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
        StartCoroutine(CorrectCells());
    }

    private void OnButtonClick(GameObject button)
    {
        button.GetComponent<Button>().interactable = false; //отключение нажатой кнопки
        if (_correctButtons.Contains(button))
        {
            if (_correctButtons.Count != 0)
                ASS.PlayOneShot(click);
            button.GetComponent<Image>().color = _greenColor;
            _correctButtons.Remove(button);
            if (_correctButtons.Count == 0)
            {
                ASS.PlayOneShot(next_lvl);
                _level++;
                if (_level % 3 == 0)
                    _gridSize++;
                _mistake = 3;
                CreateGrid();
            }
        }
        else
        {
            ASS.PlayOneShot(mistake);
            _mistake--;
            button.GetComponent<Image>().color = _redColor;
            int live = _live - 1;
            if (_mistake == 2)
                Hearts[live].GetComponent<Image>().fillAmount = 0.67f;
            else if (_mistake == 1)
                Hearts[live].GetComponent<Image>().fillAmount = 0.33f;

            if (_mistake == 0)
            {
                Hearts[live].GetComponent<Image>().fillAmount = 0f;
                MinusHeart();
            }
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
        OtherObjects[2].GetComponent<TextMeshProUGUI>().text = "Game 4";
    }

    void GameOver()
    {
        ASS.PlayOneShot(lose);//добавить логику со сравнением текущего счета и рекорда из бд
        SetInactiveButtons();
        StopAllCoroutines();
        SetAfterGameWindow();
    }
}
