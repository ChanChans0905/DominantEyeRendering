using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Raycaster : MonoBehaviour
{
    [SerializeField] EXP_Manager EXPM;
    [SerializeField] NoticeManager NM;
    RaycastHit UserHeadHit, UserGazeHit;
    public GameObject RaycasterToGazeTarget;
    public Transform GazeTarget;

    public void GetUserHeadPosition(Vector3 UserHeadPosition)
    {
        if (Physics.Raycast(transform.position, transform.forward, out UserHeadHit) && UserHeadHit.collider.CompareTag("2D_Screen"))
            UserHeadPosition = UserHeadHit.point;
    }

    public void GetUserGazePosition(Vector3 UserGazePosition)
    {
        RaycasterToGazeTarget.transform.LookAt(GazeTarget);

        if (Physics.Raycast(RaycasterToGazeTarget.transform.position, transform.forward, out UserGazeHit) && UserGazeHit.collider.CompareTag("2D_Screen"))
            EXPM.UserGazeOutOfScreen = false;
        else
        {
            if (EXPM.Process_RT_ProceedTask || EXPM.Process_ST_ProceedTask)
            {
                EXPM.UserGazeOutOfScreen = true;
                StartCoroutine(NM.UserGazeOutOfScreen());
            }
        }

        UserGazePosition = UserGazeHit.point;
        Vector3 forward = RaycasterToGazeTarget.transform.TransformDirection(Vector3.forward) * 20f;
        UnityEngine.Debug.DrawRay(transform.position, forward, Color.green);
    }

}
