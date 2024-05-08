using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ImageController : MonoBehaviour
{
    [SerializeField] ExpManager_RandomTest RT;
    [SerializeField] ExpManager_SliderTest ST;
    [SerializeField] GameObject STI_S1_100, STI_S2_100, STI_D1_100, STI_D2_100;
    public GameObject[] RTI_S1, RTI_S2, RTI_D1, RTI_D2;
    public GameObject[] STI_S1, STI_S2, STI_D1, STI_D2;
    int Layer_Left, Layer_Right, Layer_Both;

    void Start()
    {
        Layer_Left = LayerMask.NameToLayer("Screen_Left");
        Layer_Right = LayerMask.NameToLayer("Screen_Right");
        Layer_Both = LayerMask.NameToLayer("Screen_Both");
    }

    public void RT_TurnOnImage()
    {
        int TO = RT.ConditionOrder[RT.TaskCount];
        int CO = RT.ConditionList[RT.ConditionCount];
        int DO = RT.DominantEye[RT.TaskCount];

        if (CO == 0)
            TurnOnImage(RTI_S1, DO, TO);
        else if (CO == 1)
            TurnOnImage(RTI_S2, DO, TO);
        else if (CO == 2)
            TurnOnImage(RTI_D1, DO, TO);
        else if (CO == 3)
            TurnOnImage(RTI_D2, DO, TO);
    }

    void TurnOnImage(GameObject[] ImageGroup, int DominantEyeOrder, int TaskOrder)
    {
        ImageGroup[0].layer = DominantEyeOrder == 0 ? Layer_Left : Layer_Right;

        for (int i = 1; i < 6; i++)
            ImageGroup[i].layer = DominantEyeOrder == 0 ? Layer_Right : Layer_Left;

        ImageGroup[0].SetActive(true);
        ImageGroup[TaskOrder / 2 + 1].SetActive(true);
    }

    public void RT_TurnOnOriginalImage()
    {
        int CO = RT.ConditionList[RT.ConditionCount];
        if (CO == 0)
        {
            RTI_S1[0].layer = Layer_Both;
            RTI_S1[0].SetActive(true);
        }
        else if (CO == 1)
        {
            RTI_S2[0].layer = Layer_Both;
            RTI_S2[0].SetActive(true);
        }
        else if (CO == 2)
        {
            RTI_D1[0].layer = Layer_Both;
            RTI_D1[0].SetActive(true);
        }
        else if (CO == 3)
        {
            RTI_D2[0].layer = Layer_Both;
            RTI_D2[0].SetActive(true);
        }
    }

    public void RT_TurnOffImage()
    {
        int CO = RT.ConditionList[RT.ConditionCount];

        if (CO == 0)
            TurnOffImage(RTI_S1);
        else if (CO == 1)
            TurnOffImage(RTI_S2);
        else if (CO == 2)
            TurnOffImage(RTI_D1);
        else if (CO == 3)
            TurnOffImage(RTI_D2);
    }

    void TurnOffImage(GameObject[] ImageGroup)
    {
        for (int i = 0; i < 6; i++)
            ImageGroup[i].SetActive(false);
    }

    public void ST_TurnOnOffImage(bool TurnOnOff, int SC)
    {
        int CO = RT.ConditionList[RT.ConditionCount];

        if (CO == 0)
        {
            ST_AdjustImageLayer(STI_S1_100, STI_S1, ST.DominantEye);
            STI_S1_100.SetActive(TurnOnOff);
            STI_S1[SC].SetActive(TurnOnOff);
        }
        else if (CO == 1)
        {
            ST_AdjustImageLayer(STI_S2_100, STI_S2, ST.DominantEye);
            STI_S2_100.SetActive(TurnOnOff);
            STI_S2[SC].SetActive(TurnOnOff);
        }
        else if (CO == 2)
        {
            ST_AdjustImageLayer(STI_D1_100, STI_D1, ST.DominantEye);
            STI_D1_100.SetActive(TurnOnOff);
            STI_D1[SC].SetActive(TurnOnOff);
        }
        else if (CO == 3)
        {
            ST_AdjustImageLayer(STI_D2_100, STI_D2, ST.DominantEye);
            STI_D2_100.SetActive(TurnOnOff);
            STI_D2[SC].SetActive(TurnOnOff);
        }
    }

    void ST_AdjustImageLayer(GameObject OriginalImage, GameObject[] ImageGroup, int LorR)
    {
        OriginalImage.layer = LorR == 0 ? Layer_Left : Layer_Right;
        for (int i = 0; i < 5; i++)
            ImageGroup[i].layer = LorR == 0 ? Layer_Right : Layer_Left;
    }
}
