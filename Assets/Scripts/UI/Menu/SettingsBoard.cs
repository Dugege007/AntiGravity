using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsBoard : Singleton<SettingsBoard>
{
    public Button gamePadBtn;
    public Button touchBtn;
    public Button keyBoardBtn;
    public Button mouseClick;

    protected override void Awake()
    {
#if UNITY_WEBGL || UNITY_ANDROID
        SelectGamePad();
#elif UNITY_STANDALONE_WIN
        SelectMouseClick();
#endif
    }

    #region 操作方式按钮方法
    public void SelectGamePad()
    {
        GameManager.Instance.controlMode = OperationMode.GamePad;
        if (SceneManager.GetActiveScene().name == "GamePlay Scene")
        {
            PlayerController.Instance.inputControls.PlayerKG.Enable();
            PlayerController.Instance.inputControls.PlayerMT.Disable();
        }

        CloseAllMask();
        gamePadBtn.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void SelectTouch()
    {
        GameManager.Instance.controlMode = OperationMode.Touch;
        if (SceneManager.GetActiveScene().name == "GamePlay Scene")
        {
            PlayerController.Instance.inputControls.PlayerKG.Disable();
            PlayerController.Instance.inputControls.PlayerMT.Enable();
        }

        CloseAllMask();
        touchBtn.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void SelectKeyBoard()
    {
        GameManager.Instance.controlMode = OperationMode.KeyBoard;
        if (SceneManager.GetActiveScene().name == "GamePlay Scene")
        {
            PlayerController.Instance.inputControls.PlayerKG.Enable();
            PlayerController.Instance.inputControls.PlayerMT.Disable();
        }

        CloseAllMask();
        keyBoardBtn.transform.GetChild(1).gameObject.SetActive(true);
    }
    public void SelectMouseClick()
    {
        GameManager.Instance.controlMode = OperationMode.MouseClick;
        if (SceneManager.GetActiveScene().name == "GamePlay Scene")
        {
            PlayerController.Instance.inputControls.PlayerKG.Disable();
            PlayerController.Instance.inputControls.PlayerMT.Enable();
        }

        CloseAllMask();
        mouseClick.transform.GetChild(1).gameObject.SetActive(true);
    }
    #endregion

    public void CloseAllMask()
    {
        gamePadBtn.transform.GetChild(1).gameObject.SetActive(false);
        touchBtn.transform.GetChild(1).gameObject.SetActive(false);
        keyBoardBtn.transform.GetChild(1).gameObject.SetActive(false);
        mouseClick.transform.GetChild(1).gameObject.SetActive(false);
    }
}
