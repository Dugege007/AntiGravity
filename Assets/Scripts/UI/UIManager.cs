using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class UIManager : Singleton<UIManager>
{
    //显示当前难度
    [SerializeField] private Level level;

    [Header("Default")]
    public Text time;
    public Text score;
    public Animator scoreAnim;
    //private int scoreTemp;
    public Text levelText;
    public Animator levelAnim;
    public Text health;
    public Animator healthAnim;
    public Button pauseBtn;

    [Header("GamePause")]
    public GameObject gamePadCanvas;
    public GameObject pauseCanvas;
    public GameObject nextLevelCanvas;
    public GameObject pausePanel;
    private GameObject functionCanvas;
    private GameObject leaderBoard;
    private GameObject settingsBoard;
    private GameObject helpBoard;
    private Button backBtn;
    public Button resumeBtn;
    public Button continueBtn;
    private bool isPause;

    [Header("LevelUp")]
    public GameObject levelUpCanvas;
    public Text levelUp;
    public List<GameObject> tipsList = new List<GameObject>();
    public List<GameObject> gameOverTipsList = new List<GameObject>();
    public GameObject easyLevelTipsBoard;
    public Animator easyLevelTipsAnim;
    private float currentHTLTipsTime, currentGBTipsTime, currentGHTipsTime;
    private float maxHTLTipsTime, maxGBTipsTime, maxGHTipsTime;

    public ParticleSystem congratulationsParticlePrefab;
    private ParticleSystem congratulationsParticle;

    [Header("GameOver")]
    public Text gameOver;

    [HideInInspector]
    public float countTime;
    private int tipsIndex;
    private ParentStarStats parentStarStats;
    private PlayerStats playerStats;

    private bool isWin;

    private float tempWaitTime;

    //private GameObject leaderboardCanvas;

    protected override void Awake()
    {
        base.Awake();

        functionCanvas = GameObject.FindGameObjectWithTag("FunctionCanvas");
        leaderBoard = functionCanvas.transform.Find("Leader Board").gameObject;
        settingsBoard = functionCanvas.transform.Find("Settings Board").gameObject;
        helpBoard = functionCanvas.transform.Find("Help Board").gameObject;

        backBtn = functionCanvas.transform.Find("Back Btn").GetComponent<Button>();
        backBtn.gameObject.SetActive(false);

        isPause = false;
        isWin = false;

        tempWaitTime = ParentStarController.Instance.waitTime;
    }

    private void OnEnable()
    {
        EventHandler.ArrivedParentEvent += OnArrivedParent;
        EventHandler.LeaveParentEvent += OnLeaveParentEvent;
        EventHandler.EndGameEvent += OnEndGameEvent;

        //传入level
        level = GameManager.Instance.level;
        switch (level)
        {
            case Level.Easy:
                //添加提示事件
                EventHandler.GetBuffByEjectorEvent += OnGetBuffByEjectorEvent;
                EventHandler.GetHitByEjectorEvent += OnGetHitByEjectorEvent;
                EventHandler.TooFarToParentEvent += OnTooFarToParentEvent;
                EventHandler.HealthTooLowEvent += OnHealthTooLowEvent;

                currentGBTipsTime = 0;
                currentGHTipsTime = 0;
                currentHTLTipsTime = 0;
                maxGBTipsTime = 5;
                maxGHTipsTime = 5;
                maxHTLTipsTime = 5;
                break;
            case Level.Hard:
            case Level.Challenge:
                break;
        }

        //操控方式
        switch (GameManager.Instance.controlMode)
        {
            case OperationMode.KeyBoard:
            case OperationMode.GamePad:
                gamePadCanvas.gameObject.SetActive(true);
                break;

            case OperationMode.MouseClick:
            case OperationMode.Touch:
                gamePadCanvas.gameObject.SetActive(false);
                break;
        }

#if UNITY_WEBGL || UNITY_ANDROID
        gamePadCanvas.gameObject.SetActive(true);
#elif UNITY_STANDALONE_WIN
        gamePadCanvas.gameObject.SetActive(false);
#endif
    }

    private void OnDisable()
    {
        EventHandler.ArrivedParentEvent -= OnArrivedParent;
        EventHandler.LeaveParentEvent -= OnLeaveParentEvent;
        EventHandler.EndGameEvent -= OnEndGameEvent;

        switch (level)
        {
            case Level.Easy:
                //移除提示事件
                EventHandler.GetBuffByEjectorEvent -= OnGetBuffByEjectorEvent;
                EventHandler.GetHitByEjectorEvent -= OnGetHitByEjectorEvent;
                EventHandler.TooFarToParentEvent -= OnTooFarToParentEvent;
                EventHandler.HealthTooLowEvent -= OnHealthTooLowEvent;
                break;
            case Level.Hard:
            case Level.Challenge:
                break;
        }
    }

    private void Start()
    {
        parentStarStats = ParentStarController.Instance.parentStarStats;
        playerStats = PlayerController.Instance.playerStats;

        switch (level)
        {
            case Level.Easy:
                easyLevelTipsAnim.SetTrigger("Goal");
                //送个分先
                playerStats.FinalPoint = 100;
                break;
            case Level.Hard:
            case Level.Challenge:
                break;
            case Level.Test:
                //送个分先
                playerStats.FinalPoint = 100;
                break;
        }

        UpdateHealth();
    }

    private void Update()
    {
        if (!isWin)
            countTime += Time.deltaTime;
        time.text = ((int)countTime).ToString() + " s";

        switch (level)
        {
            case Level.Easy:
                //如果玩家血量为1，开启提示
                if (playerStats.CurrentHealth == 1)
                    OnHealthTooLowEvent();
                break;
            case Level.Hard:
            case Level.Challenge:
                break;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPause)
                GamePause();
            else
                GameResume();
        }
    }

    #region 事件
    private void OnArrivedParent()
    {
        if (!PlayerController.Instance.isDead)
        {
            levelUpCanvas.gameObject.SetActive(true);
            tipsIndex = Random.Range(0, tipsList.Count);
            tipsList[tipsIndex].gameObject.SetActive(true);
        }
        else
        {
            tipsIndex = Random.Range(0, gameOverTipsList.Count);
            gameOverTipsList[tipsIndex].gameObject.SetActive(true);
        }

        playerStats.FinalPoint += parentStarStats.ScorePlus;
        UpdateScore();

        if (parentStarStats.parentStarData.currentLevel == parentStarStats.parentStarData.maxLevel)
        {
            isWin = true;
            //弹出暂停界面
            GamePause();
            //打开下一步提示框
            nextLevelCanvas.gameObject.SetActive(true);
            //保持游戏时间
            Time.timeScale = 1;
            //播放动画
            congratulationsParticle = Instantiate(congratulationsParticlePrefab, ParentStarController.Instance.transform.position, Quaternion.identity);
            congratulationsParticle.Play();
            //保存分数
            GameManager.Instance.OnEndGameEvent();
            //延长暂停时间
            ParentStarController.Instance.waitTime = 90;
            //切换音乐
            AudioManager.Instance.bgmMusic.clip = AudioManager.Instance.finalBgmClip;
            AudioManager.Instance.PlayBGMMusic();
            //恭喜字幕
            levelUp.text = "Congratulations!";
        }
    }

    private void OnLeaveParentEvent()
    {
        int lastLevel = parentStarStats.parentStarData.lastLevel;
        levelUpCanvas.gameObject.SetActive(false);
        levelText.text = "Level  " + parentStarStats.parentStarData.currentLevel + " / " + parentStarStats.parentStarData.maxLevel;
        CloseAllBoard();

        //播放动画
        PlayDefaultUIAnim(lastLevel, parentStarStats.parentStarData.currentLevel, levelAnim);
    }

    private void OnEndGameEvent()
    {
        gameOver.gameObject.SetActive(true);
        tipsIndex = Random.Range(0, gameOverTipsList.Count);
        gameOverTipsList[tipsIndex].gameObject.SetActive(true);
        //切换音乐
        AudioManager.Instance.bgmMusic.clip = AudioManager.Instance.gameOverBgmClip;
        AudioManager.Instance.PlayBGMMusic();
    }

    private void OnGetBuffByEjectorEvent()
    {
        if (currentGBTipsTime >= maxGBTipsTime)
            return;
        easyLevelTipsAnim.SetTrigger("GetBuff");
        currentGBTipsTime += Time.deltaTime;

    }

    private void OnGetHitByEjectorEvent()
    {
        if (currentGHTipsTime >= maxGHTipsTime)
            return;
        easyLevelTipsAnim.SetTrigger("LeaveObstacle");
        currentGHTipsTime += Time.deltaTime;
    }

    private void OnTooFarToParentEvent()
    {
        easyLevelTipsAnim.SetTrigger("TooFar");
    }

    private void OnHealthTooLowEvent()
    {
        if (currentHTLTipsTime >= maxHTLTipsTime)
            return;
        easyLevelTipsAnim.SetTrigger("HPTooLow");
        currentHTLTipsTime += Time.deltaTime;
    }
    #endregion

    public void UpdateScore()
    {
        int lastScore = playerStats.lastScore;
        score.text = playerStats.FinalPoint.ToString();

        //上传分数
        EventHandler.CallGetPointEvent(playerStats.FinalPoint);
        //播放动画
        PlayDefaultUIAnim(lastScore, playerStats.FinalPoint, scoreAnim);
    }

    //private void ScoreAnimation()
    //{
    //    if (scoreTemp == playerStats.FinalPoint)
    //        return;
    //    else if (scoreTemp < playerStats.FinalPoint)
    //        scoreTemp++;
    //    else if (scoreTemp > playerStats.FinalPoint)
    //        scoreTemp--;
    //}

    public void UpdateHealth()
    {
        int lastHealth = playerStats.lastHealth;
        health.text = "× " + playerStats.CurrentHealth.ToString();

        //播放动画
        PlayDefaultUIAnim(lastHealth, playerStats.CurrentHealth, healthAnim);
    }

    private void PlayDefaultUIAnim(int lastValue, int currentValue, Animator anim)
    {
        if (currentValue > lastValue)
            anim.SetTrigger("Up");
        else if (currentValue < lastValue)
            anim.SetTrigger("Down");
        else
            return;
    }

    #region GamePlay按钮方法
    public void GamePause()
    {
        Time.timeScale = 0;
        pauseCanvas.gameObject.SetActive(true);
        isPause = true;
        gamePadCanvas.gameObject.SetActive(false);
        pauseBtn.gameObject.SetActive(false);
        gameOver.gameObject.SetActive(false);

        // 音乐暂停
        AudioManager.Instance.bgmMusic.Pause();
    }

    public void GameResume()
    {
        Time.timeScale = 1;
        //恢复暂停时间
        ParentStarController.Instance.waitTime = tempWaitTime;
        pauseBtn.gameObject.SetActive(true);

        switch (level)
        {
            case Level.Easy:
                easyLevelTipsBoard.gameObject.SetActive(true);
                break;
            case Level.Hard:
            case Level.Challenge:
            case Level.Test:
                break;
        }

        //操控方式
        switch (GameManager.Instance.controlMode)
        {
            case OperationMode.KeyBoard:
            case OperationMode.GamePad:
                gamePadCanvas.gameObject.SetActive(true);
                break;
            case OperationMode.MouseClick:
            case OperationMode.Touch:
                gamePadCanvas.gameObject.SetActive(false);
                break;
        }

        isPause = false;
        isWin = false;
        if (congratulationsParticle!=null)
            congratulationsParticle.Stop();
        CloseAllBoard();
        pauseCanvas.gameObject.SetActive(false);

        // 音乐播放
        AudioManager.Instance.PlayBGMMusic();
    }

    public void OpenHelpBoard()
    {
        if (!helpBoard.activeInHierarchy)
        {
            CloseAllBoard();
            helpBoard.gameObject.SetActive(true);
        }
        else
        {
            helpBoard.gameObject.SetActive(false);
        }
    }

    public void OpenLeaderboard()
    {
        if (!leaderBoard.activeInHierarchy)
        {
            CloseAllBoard();
            leaderBoard.gameObject.SetActive(true);
        }
        else
        {
            leaderBoard.gameObject.SetActive(false);
        }
    }

    public void OpenSettingsBoard()
    {
        if (!settingsBoard.activeInHierarchy)
        {
            CloseAllBoard();
            settingsBoard.gameObject.SetActive(true);
        }
        else
        {
            settingsBoard.gameObject.SetActive(false);
        }
    }

    public void CloseAllBoard()
    {
        helpBoard.gameObject.SetActive(false);
        leaderBoard.gameObject.SetActive(false);
        settingsBoard.gameObject.SetActive(false);
        easyLevelTipsBoard.gameObject.SetActive(false);
        nextLevelCanvas.gameObject.SetActive(false);

        CloseAllChildBoard();
    }

    public void CloseAllChildBoard()
    {
        //关闭所有提示
        foreach (var tips in tipsList)
        {
            if (tips.gameObject.activeInHierarchy)
                tips.gameObject.SetActive(false);
        }
        foreach (var tips in gameOverTipsList)
        {
            if (tips.gameObject.activeInHierarchy)
                tips.gameObject.SetActive(false);
        }
    }

    public void NextLevel()
    {
        switch (GameManager.Instance.level)
        {
            case Level.Easy:
                GameManager.Instance.level = Level.Hard;
                RestartGame();
                break;
            case Level.Hard:
                GameManager.Instance.level = Level.Challenge;
                RestartGame();
                break;
            case Level.Challenge:
                BackToMenu();
                break;
            case Level.Test:
                GameManager.Instance.level = Level.Hard;
                RestartGame();
                break;
        }
    }

    public void RestartGame()
    {
        pauseCanvas.gameObject.SetActive(false);
        CloseAllBoard();

        //数据存储
        EventHandler.CallGetPointEvent(playerStats.FinalPoint);
        //EventHandler.CallEndGameEvent();

        //重载场景
        TransitionManager.Instance.Transition(SceneManager.GetActiveScene().name);

        //切换初始背景音乐
        AudioManager.Instance.ChangeBgmMusic(AudioManager.Instance.startBgmClip);
    }

    public void BackToMenu()
    {
        pauseCanvas.gameObject.SetActive(false);
        CloseAllBoard();

        //转到主界面
        TransitionManager.Instance.Transition("MainMenu Scene");

        //数据存储
        EventHandler.CallGetPointEvent(playerStats.FinalPoint);
        //EventHandler.CallEndGameEvent();

        //切换菜单背景音乐
        AudioManager.Instance.ChangeBgmMusic(AudioManager.Instance.MenuBgmClip);
    }
    #endregion
}
