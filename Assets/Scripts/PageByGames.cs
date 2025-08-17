using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PageByGames : MonoBehaviour
{
    public GameObject[] Pages;

    void Start()
    {
        //Debug.LogWarning(WhatsGame.SelectedGame);
        LoadingGame(GlobalItems.PrevPage);
    }

    void LoadingGame(int game)
    {
        Pages[game].SetActive(true);
    }

}
