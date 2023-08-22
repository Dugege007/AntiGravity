using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveBarUI : MonoBehaviour
{
    public GameObject moveUIPrefab;
    public GameObject coolingUIPrefab;
    public bool alwaysVisible;

    public float visbleTime;
    private float timeLeft;

    private Image moveFilleder;
    private Transform moveUIBar;

    private Image coolingFilleder;
    private Transform coolingUIBar;

    private void Start()
    {
        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.gameObject.name == "DefaultUI Canvas")
            {
                coolingUIBar = Instantiate(coolingUIPrefab, canvas.transform).transform;
                coolingFilleder = coolingUIBar.GetChild(0).GetComponent<Image>();
                coolingUIBar.gameObject.SetActive(alwaysVisible);

                moveUIBar = Instantiate(moveUIPrefab, canvas.transform).transform;
                moveFilleder = moveUIBar.GetChild(0).GetComponent<Image>();
                moveUIBar.gameObject.SetActive(alwaysVisible);
            }
        }
    }

    private void Update()
    {
        if (PlayerController.Instance.isPressed)
        {
            coolingUIBar.gameObject.SetActive(true);
            moveUIBar.gameObject.SetActive(true);

            timeLeft = visbleTime;
        }

        UpdateCoolingBar(PlayerController.Instance.moveCoolingTime, PlayerController.Instance.playerStats.MoveCoolDownTime);
        UpdateMoveBar(PlayerController.Instance.currentMoveTime, PlayerController.Instance.playerStats.MaxMoveTime);
    }

    public void UpdateCoolingBar(float currentMoveTime, float maxMoveTime)
    {
        float fillederPercent = currentMoveTime / maxMoveTime;
        coolingFilleder.fillAmount = fillederPercent;
    }

    public void UpdateMoveBar(float currentMoveTime, float maxMoveTime)
    {
        float fillederPercent = currentMoveTime / maxMoveTime;
        moveFilleder.fillAmount = fillederPercent;
    }

    private void LateUpdate()
    {
        moveUIBar.position = PlayerController.Instance.transform.position;
        coolingUIBar.position = PlayerController.Instance.transform.position;

        if (timeLeft <= 0 && !alwaysVisible)
        {
            moveUIBar.gameObject.SetActive(false);
            coolingUIBar.gameObject.SetActive(false);
        }
        else
        {
            timeLeft -= Time.deltaTime;
        }
    }
}
