using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ScoreRecord : MonoBehaviour
{
    public Image playerHead;
    public Text playrName;
    public Text scoreText;

    public string testUrl;

    public void SetScoreText(int point)
    {
        scoreText.text = point.ToString();
#if UNITY_WEBGL
        GetImageByUnityWebRequest(playerHead, WXManager.Instance.playerHeadUrl);
        playrName.text = WXManager.Instance.playerName;

        //testUrl = "https://thirdwx.qlogo.cn/mmopen/vi_32/Q0j4TwGTfTJXo3Io0YuHa7bBBSo8K5q4VSV1rhXzyMsvRG0gqBlafTL0Jkv6Kn4FpiagYg8YXLBy0fZ58ybXeNg/132";
        //GetImageByUnityWebRequest(playerHead, testUrl);
        //playrName.text = "Loading...";
#endif
    }

    /// <summary>
    /// UnityWebRequestÇëÇó
    /// </summary>
    /// <param name="_imageComp">image×é¼þ</param>
    /// <param name="_url">URL</param>
    public void GetImageByUnityWebRequest(Image _imageComp, string _url)
    {
        StartCoroutine(UnityWebRequestGetData(_imageComp, _url));
    }

    IEnumerator UnityWebRequestGetData(Image _imageComp, string _url)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(_url))
        {
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.ProtocolError || uwr.result == UnityWebRequest.Result.ConnectionError) Debug.Log(uwr.error);
            else
            {
                if (uwr.isDone)
                {
                    int width = 132;
                    int height = 132;
                    Texture2D texture2d = new Texture2D(width, height);
                    texture2d = DownloadHandlerTexture.GetContent(uwr);
                    Sprite tempSprite = Sprite.Create(texture2d, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
                    _imageComp.sprite = tempSprite;
                    Resources.UnloadUnusedAssets();
                }
            }
        }
    }
}
