using System.Collections.Generic;
using UnityEngine;

public class HelpBoard : MonoBehaviour
{
    public List<GameObject> tipsBoardList = new List<GameObject>();
    private int tipsBoardIndex;

    public void PageUp()
    {
        tipsBoardList[tipsBoardIndex].gameObject.SetActive(false);
        if (tipsBoardIndex - 1 >= 0)
        {
            tipsBoardIndex--;
            tipsBoardList[tipsBoardIndex].gameObject.SetActive(true);
        }
        else
        {
            tipsBoardIndex = tipsBoardList.Count - 1;
            tipsBoardList[tipsBoardList.Count - 1].gameObject.SetActive(true);
        }
    }

    public void PageDown()
    {
        tipsBoardList[tipsBoardIndex].gameObject.SetActive(false);
        if (tipsBoardIndex + 1 == tipsBoardList.Count)
        {
            tipsBoardIndex = 0;
            tipsBoardList[tipsBoardIndex].gameObject.SetActive(true);
        }
        else
        {
            tipsBoardIndex++;
            tipsBoardList[tipsBoardIndex].gameObject.SetActive(true);
        }
    }

    public void CloseAllChildBoard()
    {
        //�رհ������������
        foreach (var board in tipsBoardList)
        {
            if (board.gameObject.activeInHierarchy)
                board.gameObject.SetActive(false);
        }
    }
}
