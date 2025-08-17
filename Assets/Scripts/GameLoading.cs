using UnityEngine;

public class test : MonoBehaviour
{
    public GameObject[] Games;

    void Start()
    {
        LoadingGame(GlobalItems.NextPage);
    }

    void LoadingGame(int game)
    {
        Games[game].SetActive(true);
    }
}
