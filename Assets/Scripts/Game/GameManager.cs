using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private float m_yValueToDieFrom = -15.0f;
    private static GameManager instance = null;
    public static GameManager Instance {get => instance;}
    public float m_timeAtStartOfGame = 0.0f;
    public float m_lastTimeAverageUpdated = 0.0f;
    public float m_averageCheck = 1.0f;
    public int moneyEarntLastSecond = 0;
    public int averageMoneyEarnt = 0;

    public static float s_timeTaken = 0;

    private int m_currentMaxPlayers = 3;
    public int MaxPlayers {get => m_currentMaxPlayers; }

    public CharacterController m_currentCharacterController = null;
    public GameObject m_characterControllerPrefab = null;
    public Cinemachine.CinemachineVirtualCamera m_vCamera = null;
    public List<CharacterController> m_allCharacterControllers = new List<CharacterController>();

    public enum Milestone
    {
        InitialMilestone = 0,
        LastMilestone = 1,
    }

    private Milestone m_currentMileStone = Milestone.InitialMilestone;
    public GameObject m_playerSpawnPoint = null;
    public PlayerAccountTracker m_playerAccountTracker = new PlayerAccountTracker();
    public HUDView m_hudView = null;
    public PlayerInput m_playerInput = null;

    public UnityEngine.Events.UnityEvent m_onWinEvent = new UnityEngine.Events.UnityEvent();
    public bool m_showWin = false;


    public delegate void MilestoneEvent(Milestone milstone);
    public event MilestoneEvent OnMilestoneAchieved;
    public void OnEnable()
    {
        m_timeAtStartOfGame = Time.unscaledTime;
        instance = this;
        m_playerAccountTracker = new PlayerAccountTracker();
        RespawnPlayer(m_currentCharacterController);
    } 

    public CharacterController InstantiateNewCharacterController()
    {
        GameObject controller = Instantiate(m_characterControllerPrefab);
        CharacterController cc = controller.GetComponent<CharacterController>();
        RespawnPlayer(cc);
        return cc;
    }

    public void AddMoney(int amount)
    {
        #if DEVELOPMENT_BUILD || UNITY_EDITOR
            m_playerAccountTracker.IncrementMoney(amount);
        #endif
    }

    public bool CanUpgradeCurrentCharacter()
    {
        if(m_currentCharacterController == null)
        {
            return false;
        }

        if(m_playerAccountTracker.CanPurchase(GameValues.kCOST_OF_PLAYER_UPGRADE) == false)
        {
            return false;
        }

        return m_currentCharacterController.m_multiplier == 1.0f;
    }

    public void AttemptPurchaseCurrentPlayerUpgrade()
    {
        if(CanUpgradeCurrentCharacter() == true)
        {
            m_playerAccountTracker.SpendMoney(GameValues.kCOST_OF_PLAYER_UPGRADE);

            m_currentCharacterController.Upgrade();
        }
    }

    public bool CanPurchaseRandomCharacterSwap()
    {
        int aliveCharacters = CalculateNumberOfAliveCharacters();
        return (m_playerAccountTracker.CanPurchase(GameValues.kCOST_OF_SWITCHING_TO_RANDOM_CONTROLLER) == true && CalculateNumberOfAliveCharacters() > 1);
    }

    public void TryPurchaseRandomCharacterControllerSwap()
    {
        if(CanPurchaseRandomCharacterSwap() == true)
        {
            int aliveCharacters = CalculateNumberOfAliveCharacters();
            m_playerAccountTracker.SpendMoney(GameValues.kCOST_OF_SWITCHING_TO_RANDOM_CONTROLLER);
            // Oh no. list of character controllers contains alive and not alive characters for pooling.
            // Converting randomIndex to alive index is going to be messy unless everything is refactored. :(
            
            int randomIndex = Random.Range(0, aliveCharacters);
            
            CharacterController cc = null;
            int checkedIndex = 0;
            for(int i = 0, length = m_allCharacterControllers.Count; i < length * 2; i++)
            {
                CharacterController ccTemp = m_allCharacterControllers[i%length];

                if(ccTemp.m_dead == false)
                {
                    if(randomIndex <= checkedIndex && ccTemp != m_currentCharacterController)
                    {
                        cc = ccTemp;
                        break;
                    }

                    if(ccTemp != m_currentCharacterController)
                    {
                        cc = m_currentCharacterController;
                    }
                    checkedIndex++;
                }
            }
            // Stop moving if switched and input was given.
            m_currentCharacterController.SetDesiredVelocityDirection(Vector2.zero);
            // Update function should handle setting this up with the camera and player input next frame at most (depending on script execution order).
            m_currentCharacterController = cc;
        }
    }

    public bool CanPurchaseWin()
    {
        return (m_playerAccountTracker.CanPurchase(GameValues.kCOST_OF_WIN) == true && m_showWin == false);
    }

    public void TryPurchaseFinalUpgrade()
    {
        if(CanPurchaseWin() == true)
        {
            m_playerAccountTracker.SpendMoney(GameValues.kCOST_OF_WIN);
            s_timeTaken = Time.unscaledTime - m_timeAtStartOfGame;
            OnWin();
        }
    }

    public void OnWin()
    {
        m_showWin = true;

        m_onWinEvent?.Invoke();
    }

    public void IncreaseMaxPlayerCount(int amount)
    {
        m_currentMaxPlayers += amount;
    }

    public void TryPurchaseCharacterController()
    {
        AttemptPurchaseCharacterController();
    }

    public bool CanPurcshaseCharacterController()
    {
        return m_playerAccountTracker.CanPurchase(GameValues.kCOST_OF_CHARACTERCONTROLLER) == true && CalculateNumberOfAliveCharacters() < m_currentMaxPlayers;
    }

    public int CalculateNumberOfAliveCharacters()
    {
        int i = 0;
        foreach(CharacterController cc in m_allCharacterControllers)
        {
            if(cc.m_dead == false)
            {
                i++;
            }
        }

        return i;
    }
    public bool AttemptPurchaseCharacterController()
    {
        if(CanPurcshaseCharacterController() == true)
        {
            m_playerAccountTracker.SpendMoney(GameValues.kCOST_OF_CHARACTERCONTROLLER);

            CharacterController newCharacterController = null;

            // Find dead character in pool.
            foreach(CharacterController cc in m_allCharacterControllers)
            {
                if(cc.m_dead == true)
                {
                    newCharacterController = cc;
                    newCharacterController.Spawn(m_playerSpawnPoint.transform.position);
                    break;
                }
            }

            if(newCharacterController == null)
            {
                newCharacterController = InstantiateNewCharacterController();
            }
            if(m_currentCharacterController == null || m_currentCharacterController.m_dead == true)
            {
                m_currentCharacterController = newCharacterController;
            }
            return true;
        }
        return false;
    }

    public void KillCharacterController(CharacterController cc)
    {   
        cc.PrepareForPool();
        if(cc == m_currentCharacterController)
        {
            KillCurrentCharacterController();
        }
    }
    public void KillCurrentCharacterController()
    {
        m_currentCharacterController.PrepareForPool();
        m_currentCharacterController = null;
        foreach(CharacterController cc in m_allCharacterControllers)
        {
            if(cc.m_dead == false)
            {
                m_currentCharacterController = cc;
                break;
            }
        }

        if(m_currentCharacterController == null)
        {
            if(AttemptPurchaseCharacterController() == false)
            {
                Debug.Log("Game Over?");
            }
        }
    }
    
    public void Update()
    {
        if(m_lastTimeAverageUpdated + m_averageCheck < Time.unscaledTime)
        {
            m_lastTimeAverageUpdated = Time.unscaledTime;
            int moneyDiff = m_playerAccountTracker.totalMoneyEarnt - moneyEarntLastSecond;
            averageMoneyEarnt = Mathf.FloorToInt((float) moneyDiff / m_averageCheck);
            moneyEarntLastSecond = m_playerAccountTracker.totalMoneyEarnt;
        }

        m_hudView.RefreshDataUI(m_playerAccountTracker);

        if(m_currentMileStone != Milestone.LastMilestone)
        {
            if(NextMilestoneTargetAmount() <= m_playerAccountTracker.MoneyAmount)
            {
                OnMilestoneAchieved?.Invoke(m_currentMileStone);
                m_currentMileStone = ((Milestone)((int)m_currentMileStone + 1));
            }
        }
        
        foreach(CharacterController cc in m_allCharacterControllers)
        {
            if(cc.m_dead == true)
            {
                continue;
            }

            if(cc.transform.position.y < m_yValueToDieFrom)
            {
                KillCharacterController(cc);
                break;
            }
        }

        m_playerInput.m_characterController = m_currentCharacterController;
        if(m_currentCharacterController != null)
        {
            m_vCamera.m_Follow = m_playerInput.m_characterController.transform;
        }

    }

    private void RespawnPlayer(CharacterController cc)
    {
        cc.Spawn(m_playerSpawnPoint.transform.position);
    }

    public int NextMilestoneTargetAmount()
    {
        switch(m_currentMileStone)
        {
            case Milestone.InitialMilestone:
                return 25;
            case Milestone.LastMilestone:
                return 1000000;
        }

        return 0;
    }
}
