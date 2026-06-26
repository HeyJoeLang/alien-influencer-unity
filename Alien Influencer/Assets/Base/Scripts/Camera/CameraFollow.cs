using UnityEngine;
using Unity.Cinemachine;

public class CameraFollow : MonoBehaviour
{

    public CinemachineCamera[] cameras;

    public CinemachineCamera startCamera;
    private CinemachineCamera currentCam;

    private void Start()
    {
        currentCam = startCamera;

        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i] == currentCam)
            {
                cameras[i].Priority = 20;
            }
            else
            {
                cameras[i].Priority = 10;
            }

        }
    }
    public void SwitchCamera(CinemachineCamera newCam)
    {
        currentCam = newCam;

        currentCam.Priority = 20;

        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i] != currentCam)
            {
                cameras[i].Priority = 10;
            }
        }
    }
}
