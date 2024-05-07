using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class AdjustImageLocation : MonoBehaviour
{
    public GameObject User;
    float UserPositionY;
    float UserPositionX;

    private void Start()
    {
        UserPositionX = User.transform.position.x;
        UserPositionY = User.transform.position.y;
    }

    void FixedUpdate()
    {
        UserPositionY = User.transform.position.y;
        UserPositionX = User.transform.position.x;
        transform.position = new Vector3(UserPositionX, UserPositionY, User.transform.position.z + 1.5f);
    }
}
