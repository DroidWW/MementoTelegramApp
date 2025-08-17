using UnityEngine;
using TMPro;

public class UserName : MonoBehaviour
{
    public TMP_Text UserNameText;

    private void Start()
    {
        UserNameText.text = HttpWebRequest.GetUserName();
    }
}
