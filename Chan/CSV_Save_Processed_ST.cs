using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor.Build.Content;
using System.Data.Common;

public class CSV_Save_Processed_ST : MonoBehaviour
{
    [SerializeField] NoticeManager NM;
    [SerializeField] ExpManager_SliderTest ST;
    [SerializeField] ExpManager_RandomTest RT;
    [SerializeField] UserGazePostionAndAdjustFOV User;
    // 저장 데이터 : Condition, Task, 이미지 순서, foveation, 정답 여부, 
    string[] csvHeaders = new string[] { "Condition", "RepetitionCount", "SliderCount", "TotalTestTime", "DominantEye" };
    float[] PlayerData = new float[5];
    string csvFileName;
    string FilePath;

    public void Save_CSV_Analysis()
    {
        // TaskCount, SliderCount, CompleteTime, FOV
        PlayerData[0] = RT.ConditionList[RT.ConditionCount];
        PlayerData[1] = ST.RepetitionCount;
        PlayerData[2] = ST.SliderCount;
        PlayerData[3] = ST.TotalTestTime;
        PlayerData[4] = ST.DominantEye;
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

        csvFileName = "FM_SampleNumber_" + NM.SampleNumber + "_SliderTest.csv";

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
