using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseTile : MonoBehaviour
{
    [SerializeField] private int m_price = 25;
    [SerializeField] private TMPro.TMP_Text m_uiPriceText = null;
    [SerializeField] private GameObject m_mainUIElement = null;
    public UnityEngine.Events.UnityEvent m_onPurchasedEvent = new UnityEngine.Events.UnityEvent();
    private bool m_purchased = false;
    public bool Purchased { get=> m_purchased; }

    private void Start()
    {
        m_uiPriceText.text = m_price.ToString();
    }
    public void AttemptPurchase()
    {
        if(Purchased == false && CanPurchase() == true)
        {
            GameManager.Instance.m_playerAccountTracker.SpendMoney(m_price);
            m_onPurchasedEvent?.Invoke();
            AudioManager.Instance.PlayPlatformPurchase();
            // Free up a little memory and reduce amount of ui world canvases.
            Destroy(m_mainUIElement);
            m_purchased = true;
        }
    }

    public bool CanPurchase()
    {
        return GameManager.Instance.m_playerAccountTracker.CanPurchase(m_price);
    }
}
