using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraPosType
{
    Center,
    UpperLeft,
    UpperCenter,
    UpperRight,
    CenterLeft,
    CenterRight,
    BottomLeft,
    BottomCenter,
    BottomRight
}

// [ExecuteInEditMode]
public class CameraController : MonoBehaviour
{
    public Transform cameraTrans;
    public Transform cameraTarget;
    [SerializeField]
    private float smooth = 10f;
    [SerializeField]
    private float scale = 0.01f;

    private bool enableGyro;
    private Vector3 originGyroAngles;

    private List<Transform> cameraTransList;
    private Dictionary<CameraPosType, Transform> cameraPosMap;
    private int cameraIndex;

    // Start is called before the first frame update
    void Start()
    {
        cameraTransList = new List<Transform>();
        cameraPosMap = new Dictionary<CameraPosType, Transform>();

        int posIndex = 0;
        foreach (Transform child in transform)
        {
            cameraTransList.Add(child);
            cameraPosMap.Add((CameraPosType)posIndex++, child);
        }
        Input.gyro.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        LookAtTarget();
        if(enableGyro)
        {
            UpdateGyro();
        }
    }

    private void LookAtTarget()
    {
        cameraTrans.LookAt(cameraTarget);
    }

    private void UpdateGyro()
    {
        Debug.Log("Current Angles: " + Input.gyro.attitude.eulerAngles);
        Debug.Log("Origin Angles: " + originGyroAngles);
        Vector3 angles = ClampDegree(Input.gyro.attitude.eulerAngles) - ClampDegree(originGyroAngles);
        angles.z = 0f;
        Debug.Log("Angles: " + angles);
        cameraTrans.position = Vector3.Lerp(cameraTrans.position, angles * scale, Time.deltaTime * smooth);
    }

    //防止角度从0°->360°跳变
    private Vector3 ClampDegree(Vector3 angles)
    {
        if(angles.x > 180f)
        {
            angles.x = angles.x - 360f;
        }
        if(angles.y > 180f)
        {
            angles.y = angles.y - 360f;
        }
        return angles;
    }

    //在打开陀螺仪的一瞬间是获取不到值的，所以得预先打开
    public void EnableGyro(bool enable)
    {
        if(enable)
        {
            // Input.gyro.enabled = true;
            originGyroAngles = Input.gyro.attitude.eulerAngles;
            cameraTrans.position = Vector3.zero;
            Debug.Log("Gyro Enabled");
        }
        // else
        // {
        //     Input.gyro.enabled = false;
        // }
        enableGyro = enable;
    }

    public void SwitchCamera(int posType)
    {
        cameraTrans.position = cameraPosMap[(CameraPosType)posType].position;
        // cameraTrans.rotation = cameraPosMap[(CameraPosType)posType].rotation;
    }
}
