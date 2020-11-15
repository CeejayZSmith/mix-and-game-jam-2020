using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDView : MonoBehaviour
{
    [SerializeField]
    private TMPro.TMP_Text m_moneyUI = null;
    [SerializeField]
    private TMPro.TMP_Text m_playerCount = null;
    [SerializeField]
    private TMPro.TMP_Text m_moneyAverage = null;
    public void RefreshDataUI(PlayerAccountTracker pat)
    {
        m_moneyUI.text = pat.MoneyAmount.ToString("n0");
        m_moneyAverage.text = GameManager.Instance.averageMoneyEarnt.ToString() + " p/s";
        m_playerCount.text = GameManager.Instance.CalculateNumberOfAliveCharacters() + "/"+ GameManager.Instance.MaxPlayers;
    }
}
