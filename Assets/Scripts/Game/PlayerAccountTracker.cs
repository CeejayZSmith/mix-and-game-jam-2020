using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerAccountTracker
{
    [SerializeField]
    private int m_moneyAmount = 0;
    public int MoneyAmount{get => m_moneyAmount;}

    public int totalMoneyEarnt = 0;
    public void IncrementMoney(int moneyAmount)
    {
        m_moneyAmount += moneyAmount;
        totalMoneyEarnt += moneyAmount;
        // Trigger sound of somethin here
    }

    public void SpendMoney(int amount)
    {
        m_moneyAmount -= amount;
        // Trigger sound or something here.
    }

    public bool CanPurchase(int price)
    {
        return price < m_moneyAmount;
    }
}
