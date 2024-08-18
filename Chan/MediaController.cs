using System;
using System.Collections;
using System.Collections.Generic;
using RenderHeads.Media.AVProVideo;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements.Experimental;
using UnityEngine.Video;

public class MediaController : MonoBehaviour
{
    [SerializeField] EXP_Manager EXPM;
    [SerializeField] MediaPlayer MP_Origin;
    [SerializeField] MediaPlayer MP_Adjusted;

    public void PrepareMedia(int MediaCount)
    {
        int ScenarioNumber = EXPM.Scenario[EXPM.ScenarioCount];

        string OriginPath = EXPM.OriginMediaPath[ScenarioNumber];
        string AdjustedPath = EXPM.AdjustedMediaPath[ScenarioNumber, EXPM.AdjustedVideoOrder[MediaCount]];

        MP_Origin.OpenMedia(new MediaPath(OriginPath, MediaPathType.RelativeToStreamingAssetsFolder), autoPlay: false);
        MP_Adjusted.OpenMedia(new MediaPath(AdjustedPath, MediaPathType.RelativeToStreamingAssetsFolder), autoPlay: false);
    }

    public IEnumerator PlayMedia()
    {
        MP_Origin.Play();
        MP_Adjusted.Play();
        yield break;
    }

    public void AdjustMediaLocationToUser(Transform User)
    {
        transform.position = new Vector3(User.position.x, User.position.y, User.position.z + 1.5f);
    }

}

