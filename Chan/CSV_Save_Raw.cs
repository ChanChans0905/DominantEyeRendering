using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor.Build.Content;
using System.Data.Common;

public class CSV_Save_Raw : MonoBehaviour
{
    [SerializeField] NoticeManager NM;
    [SerializeField] ExpManager_SliderTest ST;
    [SerializeField] ExpManager_RandomTest RT;
    [SerializeField] UserGazePostionAndAdjustFOV User;
    public GameObject GazePoint, UserHMD;
    string[] csvHeaders = new string[] { "TaskTime", "GazePos_X", "GazePos_Y", "GazePos_Z", "HMDpos_X", "HMDpos_Y", "HMDpos_Z", "HMDrot_X", "HMDrot_Y", "HMDrot_Z", "OutOfScreenTime" };
    float[] PlayerData = new float[11];
    string csvFileName;
    string FilePath;

    void FixedUpdate()
    {
        // TaskTime, GazePoint, HeadPosition, HeadRotation
        if (RT.Term_RT_ProceedTask && !RT.IsRestTime)
            UpdatePlayerData(RT.TaskTimer);
        if (ST.Term_ST_ProceedTask)
            UpdatePlayerData(ST.TotalTestTime);
    }

    void UpdatePlayerData(float TaskTime)
    {
        PlayerData[0] = TaskTime;
        PlayerData[1] = GazePoint.transform.position.x;
        PlayerData[2] = GazePoint.transform.position.y;
        PlayerData[3] = GazePoint.transform.position.z;
        PlayerData[4] = UserHMD.transform.position.x;
        PlayerData[5] = UserHMD.transform.position.y;
        PlayerData[6] = UserHMD.transform.position.z;
        PlayerData[7] = UserHMD.transform.rotation.x;
        PlayerData[8] = UserHMD.transform.rotation.y;
        PlayerData[9] = UserHMD.transform.rotation.z;
        PlayerData[10] = User.OutOfScreenTimer;
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
        string csvDirectoryName = "RawData";
        string dir = Application.dataPath + "/" + csvDirectoryName;
        Directory.CreateDirectory(dir);

        if (RT.Term_RandomTest)
            csvFileName = "FM_Sample_" + NM.SampleNumber + "_Con_" + RT.ConditionList[RT.ConditionCount] + "_RT_TaskCount" + RT.TaskCount + ".csv";

        if (ST.Term_SliderTest)
            csvFileName = "FM_Sample_" + NM.SampleNumber + "_Con_" + RT.ConditionList[RT.ConditionCount] + "_ST_TaskCount" + ST.RepetitionCount + ".csv";

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
