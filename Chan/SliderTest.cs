using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class SliderTest : MonoBehaviour
{
    [SerializeField] EXP_Manager EXPM;
    [SerializeField] MediaController MC;
    [SerializeField] CSV_Manager CSV;
    [SerializeField] NoticeManager NM;


    public IEnumerator ProceedTask()
    {
        EXPM.Process_ST_ProceedTask = false;

        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                StartCoroutine(ChangeMedia(1, 7, NM.Notice_ST_MaxSliderCount));
                yield return new WaitForSeconds(1.5f);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                StartCoroutine(ChangeMedia(-1, 0, NM.Notice_ST_MinSliderCount));
                yield return new WaitForSeconds(1.5f);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2)) // save
            {
                EXPM.Process_ST_LogTaskResult = true;
                EXPM.ST_RepetitionCount++;

                ResetAfterTaskEnd();

                if (EXPM.ST_RepetitionCount == 1)
                    EXPM.Process_ST_BlockFinished = true;
                else
                    EXPM.Process_ST_MoveToNextTask = true;

                yield break;
            }

            if (EXPM.MediaLoopTimer > 8)
            {
                StartCoroutine(MC.PlayMedia());
                EXPM.MediaLoopTimer = 0;
            }
        }
    }

    IEnumerator ChangeMedia(int increment, int limit, GameObject Notice)
    {
        if ((increment == 1 && EXPM.ST_SliderCount < limit) || (increment == -1 && EXPM.ST_SliderCount > limit))
        {
            NM.BlackScreen.SetActive(true);
            MC.PrepareMedia(EXPM.ST_SliderCount);
            EXPM.ST_SliderCount += increment;
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(MC.PlayMedia());
            EXPM.MediaLoopTimer = 0;
            NM.BlackScreen.SetActive(false);
        }
        else
            StartCoroutine(MinMaxNotice(Notice));

        yield break;
    }

    IEnumerator MinMaxNotice(GameObject Notice)
    {
        Notice.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        Notice.SetActive(false);
        yield break;
    }

    public IEnumerator MoveToNextTask()
    {
        EXPM.Process_ST_MoveToNextTask = false;
        NM.Notice_ST_TaskEnded.SetActive(true);
        yield return new WaitForSeconds(1.5f);

        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                NM.Notice_ST_TaskEnded.SetActive(false);
                EXPM.Process_ST_ProceedTask = true;
                yield break;
            }
        }
    }

    public void FinishBlock()
    {
        if (EXPM.ScenarioCount == 3)
            EXPM.Process_ExpEnd = true;
        else
        {
            EXPM.Process_BreakTime = true;
            EXPM.ST_RepetitionCount = 0;
        }
    }

    void ResetAfterTaskEnd()
    {
        EXPM.ST_SliderCount = 0;
    }
}
