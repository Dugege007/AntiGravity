using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class TransitionManager : Singleton<TransitionManager>
{
    private CanvasGroup canvasGroup;
    public float fadeScaler;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);

        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }
        canvasGroup = GetComponent<CanvasGroup>();
        GetComponent<Canvas>().worldCamera = FindObjectOfType<Camera>();
    }

    private void Start()
    {
        StartCoroutine(Fade(0));
    }

    public void Transition(string sceneName)
    {
        Time.timeScale = 1;
        StartCoroutine(TransitionToScene(sceneName));
    }

    private IEnumerator TransitionToScene(string sceneName)
    {
        yield return Fade(1);

        //异步加载场景
        yield return SceneManager.LoadSceneAsync(sceneName);
        yield return Fade(0);
    }

    //协程
    /// <summary>
    /// 渐变
    /// </summary>
    /// <param name="amount">1是黑，0是透明</param>
    /// <returns></returns>
    private IEnumerator Fade(int amount)
    {
        //遮挡射线检测
        canvasGroup.blocksRaycasts = true;

        while (canvasGroup.alpha != amount)
        {
            switch (amount)
            {
                case 0:
                    canvasGroup.alpha -= Time.deltaTime * fadeScaler;
                    break;
                case 1:
                    canvasGroup.alpha += Time.deltaTime * fadeScaler;
                    break;
            }
            yield return null;  //让协程进行下一步
        }
        canvasGroup.blocksRaycasts = false;

        GetComponent<Canvas>().worldCamera = FindObjectOfType<Camera>();
        GetComponent<Canvas>().planeDistance = 10;
        FunctionManager.Instance.GetComponent<Canvas>().worldCamera = FindObjectOfType<Camera>();
        FunctionManager.Instance.GetComponent<Canvas>().planeDistance = 10;
    }
}
