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

        //�첽���س���
        yield return SceneManager.LoadSceneAsync(sceneName);
        yield return Fade(0);
    }

    //Э��
    /// <summary>
    /// ����
    /// </summary>
    /// <param name="amount">1�Ǻڣ�0��͸��</param>
    /// <returns></returns>
    private IEnumerator Fade(int amount)
    {
        //�ڵ����߼��
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
            yield return null;  //��Э�̽�����һ��
        }
        canvasGroup.blocksRaycasts = false;

        GetComponent<Canvas>().worldCamera = FindObjectOfType<Camera>();
        GetComponent<Canvas>().planeDistance = 10;
        FunctionManager.Instance.GetComponent<Canvas>().worldCamera = FindObjectOfType<Camera>();
        FunctionManager.Instance.GetComponent<Canvas>().planeDistance = 10;
    }
}
