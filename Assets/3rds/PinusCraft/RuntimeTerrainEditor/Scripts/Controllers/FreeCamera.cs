using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class FreeCamera : MonoBehaviour
{
    public float m_ProjectionSize;
    private Camera _cam;
    /// <summary>
    /// 旋转速度
    /// </summary>
    public int rotateSpeed = 500;
    /// <summary>
    /// 移动速度
    /// </summary>
    public float moveSpeed = 50;
    /// <summary>
    /// 按住Shift加速倍数
    /// </summary>
    private int shiftRate;
    public void Init(Camera cam)
    {
        _cam = cam;
    }

    public void ListenInputs()
    {

        //if (Input.GetMouseButton(1))
        //{
        //    // 转相机朝向
        //    _cam.transform.RotateAround(_cam.transform.position, Vector3.up, Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime);

        //    float targetAngeTo = -Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;
        //    _cam.transform.RotateAround(_cam.transform.position, _cam.transform.right, targetAngeTo);

        //    // 倍速
        //    if (Input.GetKey(KeyCode.LeftShift))
        //        shiftRate = 2;
        //    else
        //        shiftRate = 1;

        //    float acceleration = Input.GetAxis("Mouse ScrollWheel");
        //    moveSpeed += acceleration * moveSpeed;

        //    // 移动
        //    _cam.transform.Translate(shiftRate * GetDirection() * moveSpeed * Time.deltaTime, Space.World);
        //}
    }

    /// <summary>
    /// 获取相机移动与方向
    /// </summary>
    private Vector3 GetDirection()
    {
        Vector3 dir = Vector3.zero;

        // 获取按键输入
        if (Input.GetKey(KeyCode.W))
        {
            dir += _cam.transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            dir -= _cam.transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            dir -= _cam.transform.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            dir += _cam.transform.right;
        }
        if (Input.GetKey(KeyCode.E))
        {
            dir += _cam.transform.up;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            dir -= _cam.transform.up;
        }

        return dir;
    }


}
