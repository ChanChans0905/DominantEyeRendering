using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

public class RandomTest : MonoBehaviour
{
    [SerializeField] EXP_Manager EXPM;
    [SerializeField] MediaController MC;
    [SerializeField] NoticeManager NM;

    // public bool PlayerAnswer;
    // public bool IsCorrect;

    public IEnumerator ProceedTask()
    {
        EXPM.Process_RT_ProceedTask = false;

        NM.BlackScreen.SetActive(true);
        NM.Notice_RT_OpenFirstMedia.SetActive(true);
        MC.PrepareMedia(EXPM.RT_TaskCount);

        yield return new WaitForSeconds(1f);

        NM.Notice_RT_OpenFirstMedia.SetActive(false);
        NM.BlackScreen.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(MC.PlayMedia());

        yield return new WaitForSeconds(6f); // 이미지 제공 시간: 9초

        NM.BlackScreen.SetActive(true);
        NM.Notice_RT_OpenSecondMedia.SetActive(true);
        MC.PrepareMedia(EXPM.RT_TaskCount + 1);
        yield return new WaitForSeconds(1f);

        NM.Notice_RT_OpenSecondMedia.SetActive(false);
        NM.BlackScreen.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(MC.PlayMedia());

        yield return new WaitForSeconds(6f); // 두 번째 이미지 제공 시간: 9초

        NM.BlackScreen.SetActive(true);

        yield return new WaitForSeconds(1f); // 쉬는 시간: 1초

        NM.Notice_RT_SelectAnswer.SetActive(true);

        EXPM.Process_RT_InputAnswer = true;
        EXPM.Process_RT_AddAnsweringTimer = true;

        yield break;
    }


    public IEnumerator GetUserAnswer()
    {
        EXPM.Process_RT_InputAnswer = false;

        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha3))
            {
                EXPM.RT_PlayerAnswer = Input.GetKeyDown(KeyCode.Alpha1); // Alpha1이면 true, Alpha3이면 false
                NM.Notice_RT_SelectAnswer.SetActive(false);
                NM.Notice_RT_SelectAnswer.SetActive(true);
                EXPM.Process_RT_AddAnsweringTimer = false;
                yield return new WaitForSeconds(1f);
                yield break;
            }
        }
    }

    public IEnumerator CheckAnswerThenEndTask()
    {
        EXPM.Process_RT_CheckAnswerThenEndTask = false;

        float remainingWaitTime = 3.0f - EXPM.AnsweringTimer;
        if (remainingWaitTime > 0)
        {
            yield return new WaitForSeconds(remainingWaitTime);
        }

        // ASAP을 적용했을 때 정답이라는 게 있나?
        // if (PlayerAnswer)
        //     IsCorrect = true;
        // else
        //     IsCorrect = false;

        EXPM.Process_RT_LogTaskResult = true;
        EXPM.RT_TaskCount += 2; // 하나의 테스크에서 영상 2개를 비교하므로 2씩 증가.
        NM.Notice_RT_SelectAnswer.SetActive(false);

        if (EXPM.RT_TaskCount != 5) // 테스트 남았으면 바로 다음 테스크 진행
            EXPM.Process_RT_ProceedTask = true;
        else // 테스크 다 마쳤으면 슬라이더 테스트로 넘어감
        {
            EXPM.RT_TaskCount = 0;
            EXPM.Process_ST = true;
        }
    }

    public void ResetAfterTaskEnd()
    {
        EXPM.Process_RT_ResetAfterTaskEnd = false;
        EXPM.AnsweringTimer = 0;
    }
}
