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
    /// ��ת�ٶ�
    /// </summary>
    public int rotateSpeed = 500;
    /// <summary>
    /// �ƶ��ٶ�
    /// </summary>
    public float moveSpeed = 50;
    /// <summary>
    /// ��סShift���ٱ���
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
        //    // ת�������
        //    _cam.transform.RotateAround(_cam.transform.position, Vector3.up, Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime);

        //    float targetAngeTo = -Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;
        //    _cam.transform.RotateAround(_cam.transform.position, _cam.transform.right, targetAngeTo);

        //    // ����
        //    if (Input.GetKey(KeyCode.LeftShift))
        //        shiftRate = 2;
        //    else
        //        shiftRate = 1;

        //    float acceleration = Input.GetAxis("Mouse ScrollWheel");
        //    moveSpeed += acceleration * moveSpeed;

        //    // �ƶ�
        //    _cam.transform.Translate(shiftRate * GetDirection() * moveSpeed * Time.deltaTime, Space.World);
        //}
    }

    /// <summary>
    /// ��ȡ����ƶ��뷽��
    /// </summary>
    private Vector3 GetDirection()
    {
        Vector3 dir = Vector3.zero;

        // ��ȡ��������
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
