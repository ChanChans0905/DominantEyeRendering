using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class NoticeManager : MonoBehaviour
{
    [SerializeField] ExpManager_SliderTest ST;
    [SerializeField] ExpManager_RandomTest RT;
    [SerializeField] CSV_Save_Processed_RT CSV_P_RT;
    [SerializeField] CSV_Save_Processed_ST CSV_P_ST;
    [SerializeField] CSV_Save_Raw CSV_R;
    [SerializeField] ImageController IC;
    public GameObject Notice_SelectSample, Notice_Welcome, Notice_GameStart;
    public GameObject Notice_ST_BreakStart, Notice_ST_Start;
    public GameObject Notice_RT_BreakStart, Notice_RT_Start, Notice_RT_MiddleBreakStart;
    public TextMeshProUGUI Text_SampleNumber;
    float ThresholdTimer;
    public int SampleNumber;
    int Next;
    bool Term_SelectSampleNumber, Term_ExpStart;
    public bool Term_Notice_NewCondition;
    public bool Term_BreakTime;
    float BreakTimer;

    void Start()
    {
        ResetAtStart();
    }

    void Update()
    {
        if (Term_SelectSampleNumber)
            SelectSampleNumber();

        if (Term_ExpStart)
            ExpStart();

        if (Term_BreakTime)
            BreakTime();

        if (Term_Notice_NewCondition)
            Notice_NewCondition();
    }

    void SelectSampleNumber()
    {
        ThresholdTimer += Time.deltaTime;

        if (ThresholdTimer > 0.3f)
        {
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SampleNumber++;
                ThresholdTimer = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1) && SampleNumber > 0)
            {
                SampleNumber--;
                ThresholdTimer = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Term_ExpStart = true;
                Term_SelectSampleNumber = false;
                Notice_SelectSample.SetActive(false);
                Notice_Welcome.SetActive(true);
                ThresholdTimer = 0;
                CSV_P_RT.New_CSV_File();
                CSV_P_ST.New_CSV_File();
            }
            Text_SampleNumber.text = SampleNumber.ToString();
        }
    }

    void ExpStart()
    {
        ThresholdTimer += Time.deltaTime;

        if (ThresholdTimer > 0.1f && Input.GetKeyDown(KeyCode.Alpha2))
        {
            Next++;
            ThresholdTimer = 0;

            switch (Next)
            {
                case 1:
                    Notice_Welcome.SetActive(false);
                    Notice_GameStart.SetActive(true);
                    break;

                case 2:
                    Notice_GameStart.SetActive(false);
                    Term_Notice_NewCondition = true;
                    Term_ExpStart = false;
                    break;
            }
        }
    }

    void BreakTime()
    {
        BreakTimer += Time.deltaTime;

        if (RT.Term_RandomTest) // RT 쉬는 시간
        {
            if (RT.BlockEnd_RandomTest) // RT 종류 후 쉬는 시간
            {
                if (BreakTimer < 3)
                    Notice_RT_BreakStart.SetActive(true);
                else
                {
                    Notice_RT_BreakStart.SetActive(false);
                    BreakTimer = 0;
                    Term_BreakTime = false;
                    RT.Term_RandomTest = false;
                    ST.Term_SliderTest = true;
                    Term_Notice_NewCondition = true;
                    RT.BlockEnd_RandomTest = false;
                }
            }
            else // RT 중간 쉬는 시간
            {
                if (BreakTimer < 3)
                    Notice_RT_MiddleBreakStart.SetActive(true);
                else
                {
                    Notice_RT_MiddleBreakStart.SetActive(false);
                    BreakTimer = 0;
                    Term_BreakTime = false;
                    RT.Term_RT_ProceedTask = true;
                }
            }
        }
        else if (ST.Term_SliderTest) // ST 쉬는 시간
        {
            if (BreakTimer < 3)
                Notice_ST_BreakStart.SetActive(true);
            else
            {
                Notice_ST_BreakStart.SetActive(false);
                BreakTimer = 0;
                Term_BreakTime = false;
                ST.Term_SliderTest = false;
                RT.Term_RandomTest = true;
                Term_Notice_NewCondition = true;
                RT.ConditionCount++;
            }
        }
    }

    void Notice_NewCondition()
    {
        ThresholdTimer += Time.deltaTime;

        if (RT.Term_RandomTest)
        {
            Notice_RT_Start.SetActive(true);

            if (Input.GetKeyDown(KeyCode.Alpha2) && ThresholdTimer > 1.5f)
            {
                Notice_RT_Start.SetActive(false);
                RT.Term_RT_ProceedTask = true;
                Term_Notice_NewCondition = false;
                ThresholdTimer = 0;
            }
        }
        else if (ST.Term_SliderTest)
        {
            Notice_ST_Start.SetActive(true);

            if (Input.GetKeyDown(KeyCode.Alpha2) && ThresholdTimer > 1.5f)
            {
                Notice_ST_Start.SetActive(false);
                ST.DominantEye = Random.Range(0, 2);
                IC.ST_TurnOnOffImage(true, ST.SliderCount);
                ST.Term_ST_ProceedTask = true;
                Term_Notice_NewCondition = false;
                ThresholdTimer = 0;
            }
        }
    }

    void ResetAtStart()
    {
        ThresholdTimer = 0;
        SampleNumber = 0;
        BreakTimer = 0;
        Next = 0;
        Term_ExpStart = false;
        Term_Notice_NewCondition = false;
        Term_BreakTime = false;
        Term_SelectSampleNumber = true;
    }
}
