using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class ScoresGames : MonoBehaviour
{
    public TMP_Text[] ScoresGamesText;

    private void OnEnable()
    {
        StartCoroutine(UpdateScores());
    }

    private IEnumerator UpdateScores()
    {
        yield return HttpWebRequest.RequestAndWaitScores(); // ждём, пока получим данные

        string[] text = HttpWebRequest.GetScoresGames().Split(';', StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < ScoresGamesText.Length && i < text.Length; i++)
        {
            ScoresGamesText[i].text = text[i];
        }

        Debug.Log("Обновление завершено");
    }
}
