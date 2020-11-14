using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinTileModule : MonoBehaviour
{
    [SerializeField] private int m_moneyToGainOnLanded = 1;

    public void IncrementPlayerMoney()
    {
        GameManager.Instance.m_playerAccountTracker.IncrementMoney(m_moneyToGainOnLanded);
    }
}
