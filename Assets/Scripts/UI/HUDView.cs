using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDView : MonoBehaviour
{
    [SerializeField]
    private TMPro.TMP_Text m_moneyUI = null;
    public void RefreshDataUI(PlayerAccountTracker pat)
    {
        m_moneyUI.text = pat.MoneyAmount.ToString("n0");
    }
}
