using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;
    public GameObject[] stages;

    // UI를 담을 변수들
    public Image[] UI_Health;
    public Text UI_Point;
    public Text UI_Stage;
    public GameObject UI_RestartButton;

    void Update()
    {
        UI_Point.text = (totalPoint + stagePoint).ToString();
    }

    public void NextStage()
    {
        // Change Stage
        if (stageIndex < stages.Length - 1)
        {
            stages[stageIndex].SetActive(false);
            stageIndex++;
            stages[stageIndex].SetActive(true);
            PlayerReposition();

            UI_Stage.text = "STAGE " + (stageIndex + 1);
        }
        else // Game Clear
        {
            // Player Control Lock : 완주하게 되면 timeScale = 0으로 시간을 멈춰둠
            Time.timeScale = 0;

            // Restart Button UI
            Text buttonText = UI_RestartButton.GetComponentInChildren<Text>();
            buttonText.text = "Clear!";
            UI_RestartButton.SetActive(true);
        }

        // Calculate Point
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    public void HealthDown()
    {
        if (health > 1)
        {
            health--;
            // 해당 이미지 색상을 어둡게 변경
            UI_Health[health].color = new Color(1, 0, 0, 0.4f);
        }
        else
        {
            // All Health UI Off
            UI_Health[0].color = new Color(1, 0, 0, 0.4f);

            // Player Die Effect
            player.OnDie();
            // Result UI
            Debug.Log("플레이어 사망");
            // Retry Button UI : 게임이 끝났을때 활성화
            UI_RestartButton.SetActive(true);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // 플레이어 원위치 기능, 마지막 체력에서는 원위치 하지 않기
            if (health > 1)
                PlayerReposition();

            HealthDown();
        }
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(-15f, 3f, 0f);
        player.VelocityZero();
    }

    public void Restart()
    {
        // 재시작하게 되면 timeScale을 1로 복구
        Time.timeScale = 1;     
        SceneManager.LoadScene(0);
    }
}