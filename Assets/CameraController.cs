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
    public bool enableGyro;
    public float smooth = 10f;

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

        if(enableGyro)
        {
            Input.gyro.enabled = true;
        }
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
        Vector3 angles = Input.gyro.attitude.eulerAngles;
        if(angles.x > 180f)
        {
            angles.x = angles.x - 360f;
        }
        angles.y = 90f - angles.y;
        Vector3 radians = angles * Mathf.Deg2Rad;
        Debug.Log("Angles: " + angles);
        Debug.Log("Radians: " + radians);
        cameraTrans.position = Vector3.Lerp(cameraTrans.position, new Vector3(radians.x, radians.y), Time.deltaTime * smooth);
    }

    public void SwitchCamera(int posType)
    {
        cameraTrans.position = cameraPosMap[(CameraPosType)posType].position;
        // cameraTrans.rotation = cameraPosMap[(CameraPosType)posType].rotation;
    }
}
