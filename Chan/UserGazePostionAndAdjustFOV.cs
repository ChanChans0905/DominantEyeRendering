using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics;
using UnityEngine.Rendering;

public class UserGazePostionAndAdjustFOV : MonoBehaviour
{
    [SerializeField] ExpManager_RandomTest RT;
    [SerializeField] ExpManager_SliderTest ST;
    public Transform GazeTarget;
    public GameObject Notice_LookAtTheImage;
    RaycastHit hit;
    public Vector3 UserGazePoint;
    public float OutOfScreenTimer;

    void FixedUpdate()
    {
        // Dynamic Foveated Rendering ( + Eye Tracking )
        transform.LookAt(GazeTarget);

        if (Physics.Raycast(transform.position, transform.forward, out hit) && hit.collider.CompareTag("2D_Screen"))
        {
            Notice_LookAtTheImage.SetActive(false);
            RT.IsRestTime = false;
        }
        else
        {
            if (RT.Term_RT_ProceedTask || ST.Term_ST_ProceedTask)
            {
                Notice_LookAtTheImage.SetActive(true);
                RT.IsRestTime = true;
                OutOfScreenTimer += Time.deltaTime;
            }
        }

        UserGazePoint = hit.point;
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 20f;
        UnityEngine.Debug.DrawRay(transform.position, forward, Color.green);
    }
}
