using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 cameraDistance;

    public void SetCameraDistance()
    {
        cameraDistance = new Vector3(11f, 6.5f, 0);
    }
    
    public IEnumerator CameraCtrl(Transform character)
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, transform.position + cameraDistance, Time.deltaTime * 10);
        }
    }
}
