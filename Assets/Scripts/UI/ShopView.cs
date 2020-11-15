using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopView : MonoBehaviour
{
    public Button m_buyPlayerButton = null;
    public Button m_randomSwapButton= null;
    public Button m_playerUpgradeButton = null;
    public Button m_winbutton = null;

    // Update is called once per frame
    void Update()
    {
        m_buyPlayerButton.interactable = GameManager.Instance.CanPurcshaseCharacterController();
        m_randomSwapButton.interactable = GameManager.Instance.CanPurchaseRandomCharacterSwap();
        m_playerUpgradeButton.interactable = GameManager.Instance.CanUpgradeCurrentCharacter();
        m_winbutton.interactable = GameManager.Instance.CanPurchaseWin();
    }
}
