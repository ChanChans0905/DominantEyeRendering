using System.Collections;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NoticeManager : MonoBehaviour
{
    [SerializeField] EXP_Manager EXPM;
    [SerializeField] CSV_Manager CSV;

    public GameObject Notice_SelectSample, Notice_Welcome, Notice_GameStart;
    public TextMeshProUGUI Text_SampleNumber;
    public GameObject Notice_RT_Start, Notice_ST_Start;
    public GameObject Notice_RT_OpenFirstMedia, Notice_RT_OpenSecondMedia;
    public GameObject Notice_RT_SelectAnswer, Notice_RT_Wait;
    public GameObject Notice_ST_MaxSliderCount, Notice_ST_MinSliderCount;
    public GameObject Notice_ST_TaskEnded;
    public GameObject BlackScreen;
    public GameObject Notice_UserGazeOutOfScreen;
    public GameObject Notice_BreakStart;
    public GameObject Notice_ExpEnd;


    public IEnumerator UserGazeOutOfScreen()
    {
        Notice_UserGazeOutOfScreen.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        Notice_UserGazeOutOfScreen.SetActive(false);
        yield break;
    }

    public IEnumerator SelectSampleNumber()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                EXPM.SampleNumber++;
                yield return new WaitForSeconds(0.3f);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1) && EXPM.SampleNumber > 0)
            {
                EXPM.SampleNumber--;
                yield return new WaitForSeconds(0.3f);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Notice_SelectSample.SetActive(false);
                Notice_Welcome.SetActive(true);
                // rt, st 각각 raw 파일이랑 processed 파일 생성해야 함.
                CSV.CreateCSV();
                yield return new WaitForSeconds(2f);
                Notice_Welcome.SetActive(false);
                Notice_GameStart.SetActive(true);

                yield return new WaitForSeconds(2f);
                Notice_GameStart.SetActive(false);
                EXPM.Porcess_NoticingNewCondition = true;
                EXPM.Process_RT = true;
                yield break;
            }
            Text_SampleNumber.text = EXPM.SampleNumber.ToString();
        }
    }

    public IEnumerator BreakTime()
    {
        EXPM.Process_BreakTime = false;

        Notice_BreakStart.SetActive(true);
        yield return new WaitForSeconds(3f);
        Notice_BreakStart.SetActive(false);
        EXPM.Porcess_NoticingNewCondition = true;

        // ST 끝나면 다음 시나리오로 넘어가기
        if (EXPM.Process_ST)
            EXPM.ScenarioCount++;

        EXPM.Process_RT = !EXPM.Process_RT;
        EXPM.Process_ST = !EXPM.Process_ST;

        yield break;
    }

    public IEnumerator NewCondition()
    {
        EXPM.Porcess_NoticingNewCondition = false;

        while (true)
        {
            if (EXPM.Process_RT)
                Notice_ST_Start.SetActive(true);
            else if (EXPM.Process_ST)
                Notice_RT_Start.SetActive(true);

            yield return new WaitForSeconds(1.5f);
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (EXPM.Process_RT)
                {
                    Notice_RT_Start.SetActive(false);
                    EXPM.Process_RT_ProceedTask = true;
                }
                else if (EXPM.Process_ST)
                {
                    Notice_ST_Start.SetActive(false);
                    EXPM.Process_ST_ProceedTask = true;
                }
                yield break;
            }
        }
    }

}
