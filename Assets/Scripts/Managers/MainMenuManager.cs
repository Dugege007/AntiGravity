using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : Singleton<MainMenuManager>
{
    public Animator camAnim;

    public GameObject menuCanvas;
    public GameObject selectLevelCanvas;
    private GameObject functionCanvas;
    private GameObject leaderBoard;
    private GameObject settingsBoard;
    private GameObject helpBoard;
    private GameObject infoBoard;

    private Button backBtn;

    protected override void Awake()
    {
        base.Awake();

        menuCanvas = GameObject.Find("Menu Canvas");

        functionCanvas = GameObject.FindGameObjectWithTag("FunctionCanvas");
        leaderBoard = functionCanvas.transform.Find("Leader Board").gameObject;
        settingsBoard = functionCanvas.transform.Find("Settings Board").gameObject;
        helpBoard = functionCanvas.transform.Find("Help Board").gameObject;
        infoBoard = functionCanvas.transform.Find("Info Board").gameObject;

        backBtn = functionCanvas.transform.Find("Back Btn").GetComponent<Button>();
        backBtn.onClick.AddListener(OpenMainMenu);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenMainMenu();
        }
    }

    #region 主菜单按钮方法
    public void OpenSelectLevelCanvas()
    {
        CloseAllBoard();
        selectLevelCanvas.gameObject.SetActive(true);
        backBtn.gameObject.SetActive(true);
    }

    public void SelectLevelEasy()
    {
        GameManager.Instance.level = Level.Easy;
        StartGame();
    }

    public void SelectLevelHard()
    {
        GameManager.Instance.level = Level.Hard;
        StartGame();
    }

    public void SelectLevelChallenge()
    {
        GameManager.Instance.level = Level.Challenge;
        StartGame();
    }

    public void SelectLevelTest()
    {
        GameManager.Instance.level = Level.Test;
        StartGame();
    }

    public void StartGame()
    {
        TransitionManager.Instance.Transition("GamePlay Scene");
        // 切换音乐
        AudioManager.Instance.ChangeBgmMusic(AudioManager.Instance.startBgmClip);
        AudioManager.Instance.bgmMusic.loop = false;
        // 播放动画
        camAnim.SetTrigger("Start");
    }

    //public void ContinueGame()
    //{
    //    //TODO 继续游戏、加载数据
    //    //如果有已经进行但未完成的游戏
    //    if (true)
    //    {
    //    }
    //}

    public void OpenHelpBoard()
    {
        CloseAllBoard();
        helpBoard.gameObject.SetActive(true);
        backBtn.gameObject.SetActive(true);

        camAnim.SetBool("CamUp", true);
    }

    public void OpenLeaderboard()
    {
        CloseAllBoard();
        leaderBoard.gameObject.SetActive(true);
        backBtn.gameObject.SetActive(true);

        camAnim.SetBool("CamUp", true);
    }

    public void OpenSettingsBoard()
    {
        CloseAllBoard();
        settingsBoard.gameObject.SetActive(true);
        backBtn.gameObject.SetActive(true);

        camAnim.SetBool("CamUp", true);
    }

    public void OpenInfoBoard()
    {
        CloseAllBoard();
        infoBoard.gameObject.SetActive(true);
        backBtn.gameObject.SetActive(true);

        camAnim.SetBool("CamUp", true);
    }

    public void CloseAllBoard()
    {
        menuCanvas.SetActive(false);
        selectLevelCanvas.SetActive(false);
        helpBoard.gameObject.SetActive(false);
        leaderBoard.gameObject.SetActive(false);
        settingsBoard.gameObject.SetActive(false);
        infoBoard.gameObject.SetActive(false);

        backBtn.gameObject.SetActive(false);
    }

    public void OpenMainMenu()
    {
        CloseAllBoard();
        menuCanvas.SetActive(true);

        camAnim.SetBool("CamUp", false);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        //如果是在编辑器环境下
        UnityEditor.EditorApplication.isPlaying = false;
#else
        //在打包出来的环境下
        Application.Quit();
#endif
    }

    //public void OpenStrengthenBoard()
    //{
    //    //TODO 打开强化面板
    //}

    //public void OpenInformationBoard()
    //{
    //    //TODO 打开信息面板
    //}
    #endregion
}
