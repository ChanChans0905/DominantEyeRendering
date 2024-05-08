using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

public class ExpManager_RandomTest : MonoBehaviour
{
    [SerializeField] NoticeManager NM;
    [SerializeField] ExpManager_SliderTest ST;
    [SerializeField] UserGazePostionAndAdjustFOV User;
    [SerializeField] CSV_Save_Processed_RT CSV_P_RT;
    [SerializeField] CSV_Save_Raw CSV_R;
    [SerializeField] ImageController IC;
    public int[] ConditionList = new int[4];
    public int[] ConditionOrder = new int[10];
    public int[] DominantEye = new int[10];
    public int ImageOrder;
    public float TaskTimer;
    public int PlayerAnswer;
    public int IsCorrect; // 사용자 선택이 정답인지 아닌지
    bool Term_InputAnswer; // 사용자 입력 가능 시기
    public int TaskCount, ConditionCount, RepetitionCount;
    public GameObject Notice_OpenFirstImage, Notice_OpenSecondImage; // 각 이미지 '1번입니다, 2번입니다' 안내
    public bool Term_RT_ProceedTask;
    public float AnsweringTimer;
    bool Term_AddAnsweringTimer;
    bool Term_ChangeImageOrder;
    public bool Term_RandomTest;
    public TextMeshProUGUI Num_C, Num_T, Num_R;
    public bool BlockEnd_RandomTest;
    public bool IsRestTime;
    public GameObject Notice_SelectAnswer;

    void Start()
    {
        ResetAtStart();

        // 0. Static1, 1. Static2, 2. Dynamic1, 3. Dynamic2 
        ConditionList = new int[] { 0, 1, 2, 3 };
        ShuffleArray(ConditionList);

        ConditionOrder = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        ShuffleArray(ConditionOrder);

        DominantEye = new int[] { 0, 0, 0, 0, 0, 1, 1, 1, 1, 1 };
        ShuffleArray(DominantEye);
    }

    void Update()
    {
        // 실험 조건 : 같은 컨디션에 대해 4번 반복, 2번 할 때마다 1분 휴식
        if (Term_RandomTest)
        {
            if (Term_RT_ProceedTask)
                ProceedTask();

            if (Term_InputAnswer)
                GetUserAnswer();
        }
    }

    void FixedUpdate()
    {
        if (Term_RT_ProceedTask)
            TaskTimer += Time.deltaTime;

        if (Term_AddAnsweringTimer)
            AnsweringTimer += Time.deltaTime;
    }


    public void ProceedTask()
    {
        if (TaskTimer < 2f)
        {
            if (Term_ChangeImageOrder)
            {
                CSV_R.New_CSV_File();
                ImageOrder = UnityEngine.Random.Range(0, 2);
                Term_ChangeImageOrder = false;
            }

            if (TaskTimer < 1.5f)
                Notice_OpenFirstImage.SetActive(true);
            else
                Notice_OpenFirstImage.SetActive(false);
        }
        else if (TaskTimer > 2f && TaskTimer < 12f)
        {
            if (ImageOrder == 0)
                IC.RT_TurnOnImage();
            else if (ImageOrder == 1)
                IC.RT_TurnOnOriginalImage();
        }
        else if (TaskTimer > 12f && TaskTimer < 12.5f)
            IC.RT_TurnOffImage();
        else if (TaskTimer > 12.5f && TaskTimer < 14f)
            if (TaskTimer < 13.5f)
                Notice_OpenSecondImage.SetActive(true);
            else
                Notice_OpenSecondImage.SetActive(false);
        else if (TaskTimer > 14f && TaskTimer < 24f)
        {
            if (ImageOrder == 0)
                IC.RT_TurnOnOriginalImage();
            else if (ImageOrder == 1)
                IC.RT_TurnOnImage();
        }
        else if (TaskTimer > 24f)
        {
            Notice_SelectAnswer.SetActive(true);
            IC.RT_TurnOffImage();
            Term_RT_ProceedTask = false;
            Term_InputAnswer = true;
            Term_AddAnsweringTimer = true;
            TaskTimer = 0;
        }
    }

    void GetUserAnswer()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Term_AddAnsweringTimer = false;
            PlayerAnswer = 0;
            CheckAnswerThenEndTask(PlayerAnswer);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Term_AddAnsweringTimer = false;
            PlayerAnswer = 1;
            CheckAnswerThenEndTask(PlayerAnswer);
        }
    }

    void CheckAnswerThenEndTask(int PlayerAnswer)
    {
        if (PlayerAnswer == ImageOrder)
            IsCorrect = 1;
        else
            IsCorrect = 0;

        Notice_SelectAnswer.SetActive(false);
        CSV_P_RT.Save_CSV_Processed();
        TaskCount += 10;
        ResetAfterEachTask();

        if (TaskCount < 10)
            Term_RT_ProceedTask = true;
        else
        {
            RepetitionCount++;
            TaskCount = 0;

            ShuffleArray(DominantEye);
            ShuffleArray(ConditionOrder);

            if (RepetitionCount == 1)
                Term_RT_ProceedTask = true;
            else if (RepetitionCount == 2)
            {
                ResetValue();
                NM.Term_BreakTime = true;
                BlockEnd_RandomTest = true;
            }
        }
    }

    void ShuffleArray(int[] array)
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

    void ResetAfterEachTask()
    {
        AnsweringTimer = 0;
        Term_InputAnswer = false;
        Term_ChangeImageOrder = true;
        Term_AddAnsweringTimer = false;
        User.OutOfScreenTimer = 0;
    }

    void ResetValue()
    {
        Term_InputAnswer = false;
        TaskCount = 0;
        AnsweringTimer = 0;
        Term_AddAnsweringTimer = false;
        Term_ChangeImageOrder = true;
        Term_RT_ProceedTask = false;
        RepetitionCount = 0;
    }

    void ResetAtStart()
    {
        ImageOrder = 0;
        PlayerAnswer = 0;
        IsCorrect = 0;
        Term_InputAnswer = false;
        TaskCount = 0;
        Term_RT_ProceedTask = false;
        AnsweringTimer = 0;
        Term_AddAnsweringTimer = false;
        Term_ChangeImageOrder = true;
        Term_RandomTest = true;
        RepetitionCount = 0;
        ConditionCount = 0;
    }

}
