using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ToGame : MonoBehaviour
{
    public GameObject[] GameObjects;

    void Start()
    {
        SetOnButtonClick();
    }

    void SetOnButtonClick()
    {
        for (int i = 0; i < GameObjects.Length; i++)
        {
            int index = i;
            GameObjects[i].GetComponent<Button>().onClick.AddListener(() => OnButtonClick(index));
        }
    }

    void OnButtonClick(int buttonIndex)
    {
        GlobalItems.NextPage = buttonIndex;
        SceneManager.LoadScene(1);
    }
}
