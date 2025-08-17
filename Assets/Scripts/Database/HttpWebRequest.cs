using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class HttpWebRequest : MonoBehaviour
{
    private static HttpWebRequest _instance;
    private static string _urlUserName = "http://82.202.138.171/php/get_username_data.php";
    private static string _urlScoresGames = "http://82.202.138.171/php/get_scores_data.php";
    private static string _urlRecord = "http://82.202.138.171/php/set_score_data.php";
    private static string _id;
    private static string _userName;
    private static string _scoresGames;
    private static int _record;
    private static string _game;
    private static string _recordsUserAndUsers;


    public void SetTelegramId(string id)
    {
        _id = id;
        Debug.Log("Получен Telegram ID: " + _id);
        StartCoroutine(SendRequestUserName());
        StartCoroutine(SendRequestScoresGames());
    }

    void Awake()
    {

        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }


    //private void Start()
    //{
    //    StartCoroutine(SendRequestUserName());
    //}
    //private void OnEnable()
    //{
    //    StartCoroutine(SendRequestScoresGames());
    //}

    private IEnumerator SendRequestUserName()
    {
        WWWForm form = new WWWForm();
        form.AddField("telegram", _id);

        using (UnityWebRequest request = UnityWebRequest.Post(_urlUserName, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                _userName = request.downloadHandler.text;
                Debug.Log("Ответ сервера: " + _userName);
            }
            else
            {
                Debug.LogError("Ошибка: " + request.error);
            }
        }
    }

    private static IEnumerator SendRequestScoresGames()
    {
        WWWForm form = new WWWForm();
        form.AddField("telegram", _id);

        using (UnityWebRequest request = UnityWebRequest.Post(_urlScoresGames, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                _scoresGames = request.downloadHandler.text;
                Debug.Log("Ответ сервера: " + _scoresGames);
            }
            else
            {
                Debug.LogError("Ошибка: " + request.error);
            }
        }
    }

    public static IEnumerator SendRequestRecord()
    {

        WWWForm form = new WWWForm();
        form.AddField("telegram", _id);
        form.AddField("score", _record);
        form.AddField("game", _game);

        using (UnityWebRequest request = UnityWebRequest.Post(_urlRecord, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                _recordsUserAndUsers = request.downloadHandler.text;
                Debug.Log("Ответ сервера: " + _recordsUserAndUsers);
            }
            else
            {
                Debug.LogError("Ошибка: " + request.error);
            }
        }
    }

    public static string GetUserName()
    {
        return _userName;
    }

    private static void UpdateRecords()
    {
        _instance.StartCoroutine(SendRequestScoresGames());
    }

    public static string GetScoresGames()
    {
        UpdateRecords();
        return _scoresGames;
    }

    public static string SetRecord()
    {
        return _recordsUserAndUsers;
    }
    public static void SetMainRecord(string gameName, string record)
    {
        _game = gameName;
        int.TryParse(record, out _record);
        Debug.Log(_game + _record);
    }

    public static IEnumerator RequestAndWaitScores()
    {
        yield return _instance.StartCoroutine(SendRequestScoresGames());
    }
}
