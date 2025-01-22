using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Move : MonoBehaviour
{
    public GameObject player;
    public float cameraHeight = 10f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            // 플레이어의 X, Z 좌표에 기반하여 카메라의 Y좌표 설정
            Vector3 newPosition = new Vector3(player.transform.position.x, cameraHeight, player.transform.position.z - 10);
            transform.position = newPosition;

            transform.LookAt(player.transform.position);
        }
    }
}