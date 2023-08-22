using System.Collections.Generic;
using UnityEngine;

public class LeaderBoard : MonoBehaviour
{
    public List<ScoreRecord> scoreRecords;

    private List<int> scoreList;

    private void OnEnable() //OnEnable() ����������ʱִ��һ��
    {
        scoreList = GameManager.Instance.GetScoreListData();
    }

    private void Start()    //Start() �������������ʱִ��һ��
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
