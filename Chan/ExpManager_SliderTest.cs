using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEditor;
using UnityEngine;

public class ExpManager_SliderTest : MonoBehaviour
{
    [SerializeField] NoticeManager NM;
    [SerializeField] ExpManager_RandomTest RT;
    [SerializeField] CSV_Save_Processed_ST CSV_P_ST;
    [SerializeField] CSV_Save_Raw CSV_R;
    [SerializeField] UserGazePostionAndAdjustFOV User;
    [SerializeField] ImageController IC;

    public TextMeshProUGUI T_MinCondition, T_MaxCondition;
    public GameObject Notice_MinCondition, Notice_MaxCondition;
    public GameObject Notice_ExpEnd;
    public GameObject Notice_TaskEnd;
    public bool Term_SliderTest;
    public int PlayerAnswer;
    public float ST_MinimumFRS;
    float ThresholdTimer;
    public bool Term_ST_ProceedTask;
    public float TotalTestTime;
    public int SliderCount;
    public float RT_AverageFRS;
    public TextMeshProUGUI T_FOV;
    float MinMaxNoticeTimer;
    bool Bool_MinMaxNoticeTimer;
    public int RepetitionCount;
    bool Term_ST_MovetoNextTask;
    public int DominantEye; // 0이면 왼눈, 1이면 오른눈

    void Start()
    {
        ResetAtStart();
    }

    void Update()
    {
        if (Term_SliderTest && Term_ST_ProceedTask)
            AdjustFoveation();

        if (Term_ST_MovetoNextTask)
            MoveToNextTask();
    }

    void FixedUpdate()
    {
        if (Term_SliderTest && Term_ST_ProceedTask)
        {
            ThresholdTimer += Time.deltaTime;
            TotalTestTime += Time.deltaTime;
        }
    }

    // public void SetSliderTestCondition()
    // {
    //     // foreach (float value in RT.LastFiveAnswers)
    //     //     RT_AverageFRS += value;

    //     // ST_MinimumFRS = RT_AverageFRS / 5;

    //     User.CameraFOV = 20;
    // }

    void AdjustFoveation()
    {
        Debug.Log("RT 컨디션 : " + RT.ConditionList[RT.ConditionCount]);

        if (ThresholdTimer >= 0.3f)
        {
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (SliderCount < 29)
                {
                    IC.ST_TurnOnOffImage(false, SliderCount);
                    SliderCount++;
                    IC.ST_TurnOnOffImage(true, SliderCount);
                    ThresholdTimer = 0;
                }
                else
                {
                    Bool_MinMaxNoticeTimer = true;
                    Notice_MaxCondition.SetActive(true);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (SliderCount > 0)
                {
                    IC.ST_TurnOnOffImage(false, SliderCount);
                    SliderCount--;
                    IC.ST_TurnOnOffImage(true, SliderCount);
                    ThresholdTimer = 0;
                }
                else
                {
                    Bool_MinMaxNoticeTimer = true;
                    Notice_MinCondition.SetActive(true);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2)) // save
            {
                IC.ST_TurnOnOffImage(false, SliderCount);
                CSV_P_ST.Save_CSV_Analysis();
                RepetitionCount++;
                Term_ST_ProceedTask = false;

                ResetAfterEachTask();

                if (RepetitionCount == 2)
                    BlockEnd_SliderTest();
                else
                    Term_ST_MovetoNextTask = true;
            }

            if (Bool_MinMaxNoticeTimer)
                MinMaxNoticeTimer += Time.deltaTime;

            if (MinMaxNoticeTimer > 2)
            {
                Notice_MaxCondition.SetActive(false);
                Notice_MinCondition.SetActive(false);
                MinMaxNoticeTimer = 0;
                Bool_MinMaxNoticeTimer = false;
            }
        }
    }

    void MoveToNextTask()
    {
        // 주시안, 보조시안 지정
        DominantEye = (DominantEye == 0) ? 1 : 0;

        ThresholdTimer += Time.deltaTime;
        Notice_TaskEnd.SetActive(true);

        if (Input.GetKeyDown(KeyCode.Alpha2) && ThresholdTimer > 1.5f)
        {
            CSV_R.New_CSV_File();
            IC.ST_TurnOnOffImage(true, SliderCount);
            Notice_TaskEnd.SetActive(false);
            ThresholdTimer = 0;
            Term_ST_ProceedTask = true;
            Term_ST_MovetoNextTask = false;
        }
    }

    public void BlockEnd_SliderTest()
    {
        if (RT.ConditionCount == 3)
        {
            Notice_MaxCondition.SetActive(false);
            Notice_MinCondition.SetActive(false);
            ExpEnd();
        }
        else
        {
            NM.Term_BreakTime = true;
            RepetitionCount = 0;
        }
    }

    void ResetAfterEachTask()
    {
        ThresholdTimer = 0;
        TotalTestTime = 0;
        Bool_MinMaxNoticeTimer = false;
        MinMaxNoticeTimer = 0;
        SliderCount = 0;
        Notice_MaxCondition.SetActive(false);
        Notice_MinCondition.SetActive(false);
        User.OutOfScreenTimer = 0;
    }

    void ExpEnd()
    {
        Notice_ExpEnd.SetActive(true);
        Term_SliderTest = false;
    }

    void ResetAtStart()
    {
        Term_SliderTest = false;
        Term_ST_ProceedTask = false;
        Bool_MinMaxNoticeTimer = false;
        PlayerAnswer = 0;
        ST_MinimumFRS = 0;
        RT_AverageFRS = 0;
        ThresholdTimer = 0;
    }
}
