using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinView : MonoBehaviour
{
    public TMPro.TMP_Text m_timeTakenText = null;

    public void Update()
    {
        int minutes = Mathf.FloorToInt(GameManager.s_timeTaken / 60.0f);
        int seconds = (Mathf.FloorToInt(GameManager.s_timeTaken)) % 60;
        m_timeTakenText.text = $"Time Taken: {minutes} minutes and {seconds} seconds.";
    }

}
