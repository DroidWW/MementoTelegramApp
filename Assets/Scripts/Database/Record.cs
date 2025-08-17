using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class Record : MonoBehaviour
{
    public TMP_Text[] RecordText;
    public TMP_Text Record1;
    public TMP_Text GameName;


    private void Start()
    {
        HttpWebRequest.SetMainRecord(GameName.text, Record1.text);
        StartCoroutine(FetchRecord());
    }

    private IEnumerator FetchRecord()
    {
        yield return StartCoroutine(HttpWebRequest.SendRequestRecord());

        string[] text = HttpWebRequest.SetRecord().Split(';', StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < RecordText.Length; i++)
        {
            RecordText[i].text = text[i];
        }
    }
}
