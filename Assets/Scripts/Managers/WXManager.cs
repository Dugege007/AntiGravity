using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeChatWASM;

public class WXManager : Singleton<WXManager>
{
    public WXRewardedVideoAd ad;
    public WXInnerAudioContext inneraudio;
    public WXFileSystemManager fs = new WXFileSystemManager();
    public WXEnv env = new WXEnv();
    private WXUserInfoButton infoButton;

    public string playerHeadUrl;
    public string playerName;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    void Start()
    {
#if UNITY_WEBGL
        WX.InitSDK((code) =>
        {
            // 打印屏幕信息
            var systemInfo = WX.GetSystemInfoSync();
            Debug.Log($"{systemInfo.screenWidth}:{systemInfo.screenHeight}, {systemInfo.windowWidth}:{systemInfo.windowHeight}, {systemInfo.pixelRatio}");

            // 预先创建广告实例
            Debug.Log("初始化成功！");
            ad = WX.CreateRewardedVideoAd(new WXCreateRewardedVideoAdParam()
            {
                adUnitId = "xxxxxxxx" //自己申请广告单元ID
            });
            ad.OnError((r) =>
            {
                Debug.Log("ad error:" + r.errMsg);
            });
            ad.OnClose((r) =>
            {
                Debug.Log("ad close:" + r.isEnded);
            });

            // 登陆账号信息
            LoginOption loginOption = new LoginOption();
            loginOption.success = (res) =>
            {
                Debug.Log("登陆成功：" + res);
            };
            WX.Login(loginOption);

            // 创建用户信息获取按钮，创建一个透明区域
            // 首次获取会弹出用户授权窗口, 可通过右上角-设置-权限管理用户的授权记录
            var canvasWith = (int)(systemInfo.screenWidth * systemInfo.pixelRatio);
            var canvasHeight = (int)(systemInfo.screenHeight * systemInfo.pixelRatio);
            infoButton = WX.CreateUserInfoButton(0, 0, canvasWith, canvasHeight, "zh_CN", true);
            Debug.Log("infoButton Created");

            infoButton.OnTap((res) =>
            {
                Debug.Log(JsonUtility.ToJson(res.userInfo));
                GlobalWechat.OnRegisterUser(res.userInfoRaw);

                playerHeadUrl = res.userInfo.avatarUrl;
                playerName = res.userInfo.nickName;
                infoButton.Destroy();
            });
        });
#endif
    }

    public void OnAdClick()
    {
        ad.Show();
    }

    /// <summary>
    /// [Object wx.getEnterOptionsSync()]
    /// 获取小游戏打开的参数（包括冷启动和热启动）
    /// **返回有效 referrerInfo 的场景**
    /// | 场景值 | 场景                            | appId含义  |
    /// | ------ | ------------------------------- | ---------- |
    /// | 1020   | 公众号 profile 页相关小程序列表 | 来源公众号 |
    /// | 1035   | 公众号自定义菜单                | 来源公众号 |
    /// | 1036   | App 分享消息卡片                | 来源App    |
    /// | 1037   | 小程序打开小程序                | 来源小程序 |
    /// | 1038   | 从另一个小程序返回              | 来源小程序 |
    /// | 1043   | 公众号模板消息                  | 来源公众号 |
    /// **注意**
    /// 部分版本在无`referrerInfo`的时候会返回 `undefined`，建议使用 `options.referrerInfo && options.referrerInfo.appId` 进行判断。
    /// </summary>
    public void OnEnterOptionsClick()
    {
        var options = WX.GetEnterOptionsSync();
        Debug.Log("GetEnterOptionsSync scene:" + options.scene);
    }



    /// <summary>
    /// [wx.shareAppMessage(Object object)]
    /// 主动拉起转发，进入选择通讯录界面。
    /// </summary>
    public void OnShareClick()
    {
#if UNITY_WEBGL

        WX.ShareAppMessage(new ShareAppMessageOption()
        {
            title = "我正在玩一款很新的游戏，快来看看~",
            imageUrl = "https://mmocgame.qpic.cn/wechatgame/9e4JbNYBSIFdUduet4rxZBG6NItAFMHaDs9NlJZqAnW9gdeTEkVyXG2KwmXndpfW/0",
        });
#endif
    }

    /// <summary>
    /// [wx.requestMidasPayment(Object object)]
    /// 发起米大师支付
    /// **buyQuantity 限制说明**
    /// 购买游戏币的时候，buyQuantity 不可任意填写。需满足 buyQuantity * 游戏币单价 = 限定的价格等级。如：游戏币单价为 0.1 元，一次购买最少数量是 10。
    /// 有效价格等级如下：
    /// 价格等级（单位：人民币）详见 WX.cs
    /// </summary>
    public void OnPayClick()
    {
        WX.RequestMidasPayment(new RequestMidasPaymentOption()
        {
            mode = "game",
            env = 0,
            offerId = "xxxx", //在米大师侧申请的应用 id
            currencyType = "CNY",
            success = (res) =>
            {
                Debug.Log("pay success!");
            },
            fail = (res) =>
            {
                Debug.Log("pay fail:" + res.errMsg);
            }
        });
    }

    /// <summary>
    /// [wx.reportEvent(string eventId, object data)](https://developers.weixin.qq.com/minigame/dev/api/data-analysis/wx.reportEvent.html)
    /// 事件上报
    /// </summary>
    public void OnReportEventClick()
    {
        Dictionary<string, int> videoReport = new Dictionary<string, int>();
        videoReport.Add("video_maidian", 1);
        Debug.Log("video maidian 1");
        WX.ReportEvent("exptnormal", videoReport);
    }

    /// <summary>
    /// InnerAudioContext 实例，可通过 WX.CreateInnerAudioContext 接口获取实例。注意，音频播放过程中，可能被系统中断，可通过 WX.OnAudioInterruptionBegin、WX.OnAudioInterruptionEnd事件来处理这种情况。
    /// 使用参考文档：https://github.com/wechat-miniprogram/minigame-unity-webgl-transform/blob/main/Design/AudioOptimization.md
    /// </summary>
    public void OnPlayInnerAudio()
    {
        inneraudio = WX.CreateInnerAudioContext(new InnerAudioContextParam()
        {
            src = "Sounds/Seagull 002.wav",
            needDownload = true,//表示等下载完之后再播放，便于后续复用，无延迟
        });
        inneraudio.OnEnded(() =>
        {
            Debug.Log("OnEnded called, play again");
            inneraudio.Play();
        });
        inneraudio.OnCanplay(() =>
        {
            Debug.Log("OnCanplay called");
            inneraudio.Play();
        });
    }

    public void OnStopInnerAudio()
    {
        inneraudio.Stop();
    }


    public void OnFileSystemManagerClick()
    {
        // 扫描文件系统目录
        fs.Stat(new WXStatOption
        {
            path = env.USER_DATA_PATH + "/__GAME_FILE_CACHE",
            recursive = true,
            success = (succ) =>
            {
                Debug.Log($"stat success");
                foreach (var file in succ.stats)
                {
                    Debug.Log($"stat info. {file.path}, " +
                        $"{file.stats.size}，" +
                        $"{file.stats.mode}，" +
                        $"{file.stats.lastAccessedTime}，" +
                        $"{file.stats.lastModifiedTime}");
                }
            },
            fail = (fail) =>
            {
                Debug.Log($"stat fail {fail.errMsg}");
            }
        });

        // 同步接口创建目录（请勿在游戏过程中频繁调用同步接口）
        var errMsg = fs.MkdirSync(env.USER_DATA_PATH + "/mydir", true);

        // 异步写入文件
        fs.WriteFile(new WriteFileParam
        {
            filePath = env.USER_DATA_PATH + "/mydir/myfile.txt",
            encoding = "utf8",
            data = System.Text.Encoding.UTF8.GetBytes("Test FileSystemManager"),
            success = (succ) =>
            {
                Debug.Log($"WriteFile succ {succ.errMsg}");
                // 异步读取文件
                fs.ReadFile(new ReadFileParam
                {
                    filePath = env.USER_DATA_PATH + "/mydir/myfile.txt",
                    encoding = "utf8",
                    success = (succ) =>
                    {
                        Debug.Log($"ReadFile succ. stringData(utf8):{succ.stringData}");
                    },
                    fail = (fail) =>
                    {
                        Debug.Log($"ReadFile fail {fail.errMsg}");
                    }
                });

                // 异步以无编码(二进制)方式读取
                fs.ReadFile(new ReadFileParam
                {
                    filePath = env.USER_DATA_PATH + "/mydir/myfile.txt",
                    encoding = "",
                    success = (succ) =>
                    {
                        Debug.Log($"ReadFile succ. data(binary):{succ.binData.Length}, stringData(utf8):{System.Text.Encoding.UTF8.GetString(succ.binData)}");
                    },
                    fail = (fail) =>
                    {
                        Debug.Log($"ReadFile fail {fail.errMsg}");
                    }
                });

            },
            fail = (fail) =>
            {
                Debug.Log($"WriteFile fail {fail.errMsg}");
            },
            complete = null
        });
    }

    public void OnPlayerPrefClick()
    {
        // 注意！！！ PlayerPrefs为同步接口，iOS高性能模式下为"跨进程"同步调用，会阻塞游戏线程，请避免频繁调用
        PlayerPrefs.SetString("mystringkey", "myestringvalue");
        PlayerPrefs.SetInt("myintkey", 123);
        PlayerPrefs.SetFloat("myfloatkey", 1.23f);

        Debug.Log($"PlayerPrefs mystringkey:{PlayerPrefs.GetString("mystringkey")}");
        Debug.Log($"PlayerPrefs myintkey:{PlayerPrefs.GetInt("myintkey")}");
        Debug.Log($"PlayerPrefs myfloatkey:{PlayerPrefs.GetFloat("myfloatkey")}");
    }

    protected override void OnDestroy()
    {
        Debug.Log("OnDestroy");
        if (infoButton != null)
        {
            infoButton.Destroy();
            Debug.Log("infoButton Destroy");
        }
    }
}
