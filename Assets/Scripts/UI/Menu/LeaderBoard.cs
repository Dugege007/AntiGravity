using System.Collections.Generic;
using UnityEngine;

public class LeaderBoard : MonoBehaviour
{
    public List<ScoreRecord> scoreRecords;

    private List<int> scoreList;

    private void OnEnable() //OnEnable() 在物体启动时执行一次
    {
        scoreList = GameManager.Instance.GetScoreListData();
    }

    private void Start()    //Start() 在物体启动完成时执行一次
    {
        SetLeaderboardData();
    }

    public void SetLeaderboardData()
    {
        for (int i = 0; i < scoreRecords.Count; i++)
        {
            if (i < scoreList.Count)
            {
                scoreRecords[i].SetScoreText(scoreList[i]);
                scoreRecords[i].gameObject.SetActive(true);
            }
            else
            {
                scoreRecords[i].gameObject.SetActive(false);
            }
        }
    }
}
