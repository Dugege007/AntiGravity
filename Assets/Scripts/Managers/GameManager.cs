using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using WeChatWASM;

public enum Level { Easy, Hard, Challenge, Test }
public enum OperationMode { GamePad, Touch, KeyBoard, MouseClick }

public class GameManager : Singleton<GameManager>
{
    public Level level;
    public OperationMode controlMode;

    private string dataPath;
    public List<int> scoreList;
    private int score;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);

#if UNITY_WEBGL
        dataPath = Application.persistentDataPath + "/LeaderBoard.json";
        scoreList = GetScoreListData();
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
        dataPath = Application.persistentDataPath + "/LeaderBoard.json";    //��ƽ̨Ĭ�����ݱ���·��
        scoreList = GetScoreListData();
#endif
    }

    private void OnEnable()
    {
        EventHandler.EndGameEvent += OnEndGameEvent;
        EventHandler.GetPointEvent += OnGetPointEvent;
    }

    private void OnDisable()
    {
        EventHandler.EndGameEvent -= OnEndGameEvent;
        EventHandler.GetPointEvent -= OnGetPointEvent;
    }

    public void OnEndGameEvent()
    {
        //���б�������µķ���������
        if (!scoreList.Contains(score))
        {
            scoreList.Add(score);
        }

        scoreList.Sort();
        scoreList.Reverse();

#if UNITY_WEBGL
        //WX.StorageSetStringSync(JsonConvert.SerializeObject(scoreList));

        //���б��浽��Ӧ·�����ļ�����
        File.WriteAllText(dataPath, JsonConvert.SerializeObject(scoreList));

#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
        //���б��浽��Ӧ·�����ļ�����
        File.WriteAllText(dataPath, JsonConvert.SerializeObject(scoreList));
#endif
    }

    private void OnGetPointEvent(int point)
    {
        score = point;
    }

    public List<int> GetScoreListData()
    {
#if UNITY_WEBGL
        //if (WX.StorageHasKeySync(JsonConvert.SerializeObject(scoreList)))
        //{
        //    string jsonData = WX.StorageGetStringSync(JsonConvert.SerializeObject(scoreList), "000");
        //    return JsonConvert.DeserializeObject<List<int>>(jsonData);
        //}

        if (File.Exists(dataPath))  //�ж������ļ��Ƿ����
        {
            string jsonData = File.ReadAllText(dataPath);
            return JsonConvert.DeserializeObject<List<int>>(jsonData);  //JsonConvert.DeserializeObject() ���ص���GameObject����Ҫ<>ȷ��ת������
        }

#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
        if (File.Exists(dataPath))  //�ж������ļ��Ƿ����
        {
            string jsonData = File.ReadAllText(dataPath);
            return JsonConvert.DeserializeObject<List<int>>(jsonData);  //JsonConvert.DeserializeObject() ���ص���GameObject����Ҫ<>ȷ��ת������
        }
#endif
        return new List<int>();
    }
}
