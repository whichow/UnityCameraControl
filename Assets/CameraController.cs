using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// [ExecuteInEditMode]
public class CameraController : MonoBehaviour
{
    [Tooltip("Camera rotate around and look at target")]
    public Transform target;
    [Tooltip("Smaller positive value means smoother rotation")]
    public float slerpSmoothValue = 100f;

    public Text text;

    [SerializeField]
    private Vector3 value;
    private Vector3 lastValue;
    private bool useGyro;
    private Vector3 originGyroValue;

    private Quaternion currentRot; // store the quaternion after the slerp operation
    private Quaternion targetRot;

    private Vector3 dir;

    private Vector2 touchStartPos;

    // Start is called before the first frame update
    void Start()
    {
        dir = new Vector3(0, 0, -Vector3.Distance(transform.position, target.position));
        Input.gyro.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (useGyro)
        {
            UpdateWithGyro();
        }
        else
        {
            UpdateWithTouch();
        }
        //LookAt一定要放下面，否则会出现抖动
        // if(value.y < 90 && value.y > -90 || value.y > 270 || value.y < -270)
        // {
            transform.LookAt(target, Vector3.up);
        // }
        // else
        // {
        //     transform.LookAt(target, Vector3.down);
        // }
    }

    private void UpdateWithGyro()
    {
        value = (Input.gyro.gravity - originGyroValue) * 90;
        RotateAround();
    }

    private void UpdateWithTouch()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    break;
                case TouchPhase.Moved:
                    value = (touch.position - touchStartPos) * 0.1f;
                    value = value + lastValue;
                    break;
                case TouchPhase.Ended:
                    lastValue= value;
                    break;
            }
        }
        RotateAround();
    }

    private void RotateAround()
    {
        // value.x = value.x % 360;
        // value.y = value.y % 360;
        value.x = Mathf.Clamp(value.x, -60, 60);
        value.y = Mathf.Clamp(value.y, -60, 60);
        Debug.Log(value);
        text.text = value.ToString();
        targetRot = Quaternion.Euler(-value.y, value.x, 0);
        currentRot = Quaternion.Slerp(currentRot, targetRot, Time.smoothDeltaTime * slerpSmoothValue);
        transform.position = target.position + currentRot * dir;
    }

    public void UseGyro(bool status)
    {
        if (status)
        {
            originGyroValue = Input.gyro.gravity;
            Debug.Log("Gyro Enabled");
        }
        value = Vector3.zero;
        useGyro = status;
    }
}
