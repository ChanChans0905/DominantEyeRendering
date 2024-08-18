using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EXP_Manager : MonoBehaviour
{
    [SerializeField] RandomTest RT;
    [SerializeField] SliderTest ST;
    [SerializeField] CSV_Manager CSV;
    [SerializeField] NoticeManager NM;
    [SerializeField] MediaController MC;
    [SerializeField] Raycaster RC;

    public GameObject UserCamera;
    public Vector3 UserGazePosition, UserHeadPosition;

    public int[] Scenario;
    public int[] AdjustedVideoOrder;

    public string[] OriginMediaPath;
    public string[,] AdjustedMediaPath;
    public int[] RT_MediaComparisonOrder;

    public bool Process_SelectSampleNumber;

    public bool Process_RT;
    public bool Process_RT_BlockEnd;
    public bool Process_RT_InputAnswer;
    public bool Process_RT_AddAnsweringTimer;
    public bool Process_RT_ProceedTask;
    public bool Process_RT_CheckAnswerThenEndTask;
    public bool Process_RT_ResetAfterTaskEnd;
    public bool Process_RT_LogTaskResult;

    public bool Process_ST;
    public bool Process_ST_ProceedTask;
    public bool Process_ST_MoveToNextTask;
    public bool Process_ST_LogTaskResult;
    public bool Process_ST_BlockFinished;

    public bool Process_ExpEnd;
    public bool Process_BreakTime;
    public bool Porcess_NoticingNewCondition;

    public bool IsRestTime;
    public bool UserGazeOutOfScreen;

    public int ScenarioCount;
    public int SampleNumber;

    public float TaskTimer;
    public float AnsweringTimer;
    public float MediaLoopTimer;

    public int ST_RepetitionCount;
    public int ST_SliderCount;

    public int RT_TaskCount;
    public bool RT_PlayerAnswer;

    public bool OriginIsDisplayedInLeftEye;


    void Start()
    {
        Scenario = new int[] { 0, 1, 2, 3 };

        AdjustedVideoOrder = new int[] { 0, 1, 2, 3, 4 };
        ShuffleArray(AdjustedVideoOrder);

        // RT는 순서가 ASAP으로 정해진 것이므로 각 영상 주소 순번에 맞게 어레이 설정
        // ST는 주소 순서 차례대로 Slidercount 적용
        RT_MediaComparisonOrder = new int[] { 0, 1 };

        SetMediaPath();

        StartCoroutine(NM.SelectSampleNumber());
    }

    void Update()
    {
        MC.AdjustMediaLocationToUser(UserCamera.transform);

        RC.GetUserHeadPosition(UserHeadPosition);
        RC.GetUserGazePosition(UserGazePosition);

        if (Process_BreakTime)
            StartCoroutine(NM.BreakTime());

        if (Porcess_NoticingNewCondition)
            StartCoroutine(NM.NewCondition());


        if (Process_RT)
        {
            if (Process_RT_ProceedTask)
                StartCoroutine(RT.ProceedTask());


            if (Process_RT_InputAnswer)
                StartCoroutine(RT.GetUserAnswer());

            if (Process_RT_CheckAnswerThenEndTask)
                StartCoroutine(RT.CheckAnswerThenEndTask());

            if (Process_RT_LogTaskResult)
                CSV.LogData("RT");

            if (Process_RT_ResetAfterTaskEnd)
                RT.ResetAfterTaskEnd();
        }

        if (Process_ST)
        {
            if (Process_ST_ProceedTask)
                StartCoroutine(ST.ProceedTask());


            if (Process_ST_LogTaskResult)
                CSV.LogData("ST");

            if (Process_ST_MoveToNextTask)
                StartCoroutine(ST.MoveToNextTask());

            if (Process_ST_BlockFinished)
                ST.FinishBlock();
        }

        if (Process_ExpEnd)
            NM.Notice_ExpEnd.SetActive(true);
    }

    void FixedUpdate()
    {
        if (Process_RT_ProceedTask || Process_ST_ProceedTask)
        {
            TaskTimer += Time.deltaTime;
            CSV.LogData("Raw");
        }

        if (Process_RT_AddAnsweringTimer)
            AnsweringTimer += Time.deltaTime;

        if (Process_ST_ProceedTask)
            MediaLoopTimer += Time.deltaTime;
    }

    public void ShuffleArray(int[] array)
    {
        System.Random rng = new System.Random();
        int n = array.Length;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            int value = array[k];
            array[k] = array[n];
            array[n] = value;
        }
    }

    void SetMediaPath()
    {
        OriginMediaPath = new string[] { "Path" };
        AdjustedMediaPath = new string[4, 12]
        {
            // H264
            { "Media_05_H264_3840x2160_1.mp4", "Media_05_H264_3840x2160_2.mp4", "Media_05_H264_3840x2160_3.mp4", "Media_05_H264_3840x2160_4.mp4" ,"Media_05_H264_1920x1080_1.mp4", "Media_05_H264_1920x1080_2.mp4", "Media_05_H264_1920x1080_3.mp4", "Media_05_H264_1920x1080_4.mp4",  "Media_05_H264_960x540_1.mp4", "Media_05_H264_960x540_2.mp4", "Media_05_H264_960x540_3.mp4", "Media_05_H264_960x540_4.mp4"},
            { "Media_12_H264_3840x2160_1.mp4", "Media_12_H264_3840x2160_2.mp4", "Media_12_H264_3840x2160_3.mp4", "Media_12_H264_3840x2160_4.mp4","Media_12_H264_1920x1080_1.mp4", "Media_12_H264_1920x1080_2.mp4", "Media_12_H264_1920x1080_3.mp4", "Media_12_H264_1920x1080_4.mp4",  "Media_12_H264_960x540_1.mp4", "Media_12_H264_960x540_2.mp4", "Media_12_H264_960x540_3.mp4", "Media_12_H264_960x540_4.mp4"},
            { "Media_14_H264_3840x2160_1.mp4", "Media_14_H264_3840x2160_2.mp4", "Media_14_H264_3840x2160_3.mp4", "Media_14_H264_3840x2160_4.mp4","Media_14_H264_1920x1080_1.mp4", "Media_14_H264_1920x1080_2.mp4", "Media_14_H264_1920x1080_3.mp4", "Media_14_H264_1920x1080_4.mp4",  "Media_14_H264_960x540_1.mp4", "Media_14_H264_960x540_2.mp4", "Media_14_H264_960x540_3.mp4", "Media_14_H264_960x540_4.mp4" },
            { "Media_20_H264_3840x2160_1.mp4", "Media_20_H264_3840x2160_2.mp4", "Media_20_H264_3840x2160_3.mp4", "Media_20_H264_3840x2160_4.mp4","Media_20_H264_1920x1080_1.mp4", "Media_20_H264_1920x1080_2.mp4", "Media_20_H264_1920x1080_3.mp4", "Media_20_H264_1920x1080_4.mp4",  "Media_20_H264_960x540_1.mp4", "Media_20_H264_960x540_2.mp4", "Media_20_H264_960x540_3.mp4", "Media_20_H264_960x540_4.mp4" }

            // VP9
            // { "Media_05_VP9_3840x2160_1.mp4", "Media_05_VP9_3840x2160_2.mp4", "Media_05_VP9_3840x2160_3.mp4", "Media_05_VP9_3840x2160_4.mp4",  "Media_05_VP9_1920x1080_1.mp4", "Media_05_VP9_1920x1080_2.mp4", "Media_05_VP9_1920x1080_3.mp4", "Media_05_VP9_1920x1080_4.mp4",  "Media_05_VP9_960x540_1.mp4", "Media_05_VP9_960x540_2.mp4", "Media_05_VP9_960x540_3.mp4", "Media_05_VP9_960x540_4.mp4"},
            // { "Media_12_VP9_3840x2160_1.mp4", "Media_12_VP9_3840x2160_2.mp4", "Media_12_VP9_3840x2160_3.mp4", "Media_12_VP9_3840x2160_4.mp4", "Media_12_VP9_1920x1080_1.mp4", "Media_12_VP9_1920x1080_2.mp4", "Media_12_VP9_1920x1080_3.mp4", "Media_12_VP9_1920x1080_4.mp4",  "Media_12_VP9_960x540_1.mp4", "Media_12_VP9_960x540_2.mp4", "Media_12_VP9_960x540_3.mp4", "Media_12_VP9_960x540_4.mp4"},
            // { "Media_14_VP9_3840x2160_1.mp4", "Media_14_VP9_3840x2160_2.mp4", "Media_14_VP9_3840x2160_3.mp4", "Media_14_VP9_3840x2160_4.mp4","Media_14_VP9_1920x1080_1.mp4", "Media_14_VP9_1920x1080_2.mp4", "Media_14_VP9_1920x1080_3.mp4", "Media_14_VP9_1920x1080_4.mp4",  "Media_14_VP9_960x540_1.mp4", "Media_14_VP9_960x540_2.mp4", "Media_14_VP9_960x540_3.mp4", "Media_14_VP9_960x540_4.mp4" },
            // { "Media_20_VP9_3840x2160_1.mp4", "Media_20_VP9_3840x2160_2.mp4", "Media_20_VP9_3840x2160_3.mp4", "Media_20_VP9_3840x2160_4.mp4", "Media_20_VP9_1920x1080_1.mp4", "Media_20_VP9_1920x1080_2.mp4", "Media_20_VP9_1920x1080_3.mp4", "Media_20_VP9_1920x1080_4.mp4",  "Media_20_VP9_960x540_1.mp4", "Media_20_VP9_960x540_2.mp4", "Media_20_VP9_960x540_3.mp4", "Media_20_VP9_960x540_4.mp4"}

        };
    }
}
