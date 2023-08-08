using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 게임 전반적인 로직 관리
// 레벨 전환, 퀘스트 진행, 이벤트 처리 등을 담당

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

}
