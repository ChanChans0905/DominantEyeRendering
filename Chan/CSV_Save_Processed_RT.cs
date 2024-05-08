using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor.Build.Content;
using System.Data.Common;

public class CSV_Save_Processed_RT : MonoBehaviour
{
    [SerializeField] NoticeManager NM;
    [SerializeField] ExpManager_SliderTest ST;
    [SerializeField] ExpManager_RandomTest RT;
    [SerializeField] UserGazePostionAndAdjustFOV User;
    // 저장 데이터 : Condition, Task, 이미지 순서, foveation, 정답 여부, 
    string[] csvHeaders = new string[] { "Condition", "TaskCount", "ImageOrder", "PlayerAnswer", "IsCorrect", "DominantEye", "AnsweringTimer" };
    float[] PlayerData = new float[7];
    string csvFileName;
    string FilePath;

    public void Save_CSV_Processed()
    {
        // TaskCount, FoveaRegionSize, FoveationLevel, OriginOrder, PlayerAnswer, AnsweringTime
        PlayerData[0] = RT.ConditionList[RT.ConditionCount];
        PlayerData[1] = RT.TaskCount;
        PlayerData[2] = RT.ImageOrder; // 0 조정 먼저, 1 원본 먼저
        PlayerData[3] = RT.PlayerAnswer; // 0 먼저 나온 이미지 선택, 1 뒤에 나온 이미지 선택
        PlayerData[4] = RT.IsCorrect; // 1 정답, 0 오답
        PlayerData[5] = RT.DominantEye[RT.TaskCount]; // 0 왼눈, 1 오른눈
        PlayerData[6] = RT.AnsweringTimer;

        AppendToCsv(PlayerData);
    }

    public void AppendToCsv(float[] data)
    {
        using (StreamWriter sw = File.AppendText(FilePath))
        {
            string csvFinalString = "";
            for (int i = 0; i < data.Length; i++)
            {
                if (csvFinalString != "")
                {
                    csvFinalString += ",";
                }
                csvFinalString += data[i];
            }
            csvFinalString += ",";
            sw.WriteLine(csvFinalString);
        }
    }

    public void New_CSV_File()
    {
        string csvDirectoryName = "ProcessedData";
        string dir = Application.dataPath + "/" + csvDirectoryName;
        Directory.CreateDirectory(dir);

        csvFileName = "FM_SampleNumber_" + NM.SampleNumber + "_RandomTest.csv";

        FilePath = Application.dataPath + "/" + csvDirectoryName + "/" + csvFileName;

        using (StreamWriter sw = File.CreateText(FilePath))
        {
            string finalString = "";
            for (int i = 0; i < csvHeaders.Length; i++)
            {
                if (finalString != "")
                {
                    finalString += ",";
                }
                finalString += csvHeaders[i];
            }
            finalString += ",";
            sw.WriteLine(finalString);
        }
    }
}
