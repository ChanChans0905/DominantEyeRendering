using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Data.Common;

public class CSV_Manager : MonoBehaviour
{
    [SerializeField] EXP_Manager EXPM;
    string FilePath_RT, FilePath_ST, FilePath_Raw;

    string[] CsvHeader_RT = new string[] { "Condition", "TaskCount" };
    string[] CsvHeader_ST = new string[] { };
    string[] CsvHeader_Raw = new string[] { };

    float[] Data_RT = new float[5];
    float[] Data_ST = new float[5];
    float[] Data_Raw = new float[5];

    string dir = Application.dataPath + "/ExpData";

    public void LogData(string Name)
    {
        if (Name == "RT")
        {
            // 저장 데이터 : 시나리오, 시나리오 카운트(몇번째 블록인지), 틀어진 영상(첫번째, 두번째), 몇번째 테스크인지, 원본은 어느쪽 눈에 나왔는지, 사용자 답, 답 입력까지 걸린 시간
            EXPM.Process_RT_LogTaskResult = false;
            Data_RT[0] = EXPM.Scenario[EXPM.ScenarioCount];
            Data_RT[1] = EXPM.ST_RepetitionCount;
            Data_RT[2] = EXPM.ST_SliderCount;
            Data_RT[3] = EXPM.TaskTimer;
            Data_RT[4] = EXPM.UserGazePosition.x;
            WriteToCsv(Data_RT, false, FilePath_RT);
        }
        else if (Name == "ST")
        {
            // 저장 데이터: 시나리오, 시나리오 카운트, 반복횟수, 슬라이더 카운트, 카운트에 해당하는 영상 정보, 총 테스크 시간 
            EXPM.Process_ST_LogTaskResult = false;
            Data_ST[0] = EXPM.Scenario[EXPM.ScenarioCount];
            Data_ST[1] = EXPM.ST_RepetitionCount;
            Data_ST[2] = EXPM.ST_SliderCount;
            Data_ST[3] = EXPM.TaskTimer;
            Data_ST[4] = EXPM.UserGazePosition.x;
            WriteToCsv(Data_ST, false, FilePath_ST);
        }
        else if (Name == "Raw")
        {
            // 저장 데이터: 해당 테스트에서의 시간, 시나리오, 시나리오 카운트, RT인지 ST인지, 당시에 보고 있는 영상 정보, 사용자 시선 위치, 사용자 머리 위치, 회전값, Hitpoint 시선이랑 머리 각각, 화면의 어느 부분에 닿았는지, 스크린을 안보고 있는지
            Data_Raw[0] = EXPM.Scenario[EXPM.ScenarioCount];
            Data_Raw[1] = 1;
            Data_Raw[2] = 1;
            Data_Raw[3] = 1;
            Data_Raw[4] = 1;
            WriteToCsv(Data_Raw, false, FilePath_Raw);
        }
    }

    public void CreateCSV()
    {
        Directory.CreateDirectory(dir);

        FilePath_RT = GenerateCsvFilePath("RT");
        FilePath_ST = GenerateCsvFilePath("ST");
        FilePath_Raw = GenerateCsvFilePath("Raw");

        WriteToCsv(CsvHeader_RT, true, FilePath_RT);
        WriteToCsv(CsvHeader_ST, true, FilePath_ST);
        WriteToCsv(CsvHeader_Raw, true, FilePath_Raw);
    }

    string GenerateCsvFilePath(string Name)
    {
        string CsvFileName = "DER_SampleNumber_" + EXPM.SampleNumber + "_" + Name + ".csv";
        return dir + "/" + CsvFileName;
    }

    public void WriteToCsv<T>(T[] Data, bool IsNewFile, string FilePath)
    {
        using (StreamWriter sw = IsNewFile ? File.CreateText(FilePath) : File.AppendText(FilePath))
        {
            string CsvString = string.Join(",", Data) + ",";
            sw.WriteLine(CsvString);
        }
    }
}
