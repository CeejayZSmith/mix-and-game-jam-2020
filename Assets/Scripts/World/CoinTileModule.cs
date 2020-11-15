using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinTileModule : MonoBehaviour
{
    [SerializeField] private int m_moneyToGainOnLanded = 1;

    private Tile m_tile = null;
    public void IncrementPlayerMoney(CharacterController cc)
    {
        GameManager.Instance.m_playerAccountTracker.IncrementMoney((int)(m_moneyToGainOnLanded * cc.m_multiplier));
        if(cc == GameManager.Instance.m_currentCharacterController)
        {
            AudioManager.Instance.PlayCoinGatherNoise();
        }
    }

    private void OnEnable()
    {
        m_tile = GetComponent<Tile>();

        m_tile.OnCharacterControllerLandedOnTile += IncrementPlayerMoney;
    }

    private void OnDisable()
    {
        m_tile.OnCharacterControllerLandedOnTile -= IncrementPlayerMoney;
    }
}
