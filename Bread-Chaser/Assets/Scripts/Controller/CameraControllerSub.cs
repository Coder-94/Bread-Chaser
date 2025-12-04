using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraControllerSub : MonoBehaviour
{
    private void Start()
    {
        Init();
    }

    void Init()
    {
        Camera cam = GetComponent<Camera>();

        float targetAspect = 9f / 16f;
        float currentAspect = (float)Screen.width / (float)Screen.height;

        if (currentAspect < targetAspect)
        {
            float constantWidthSize = cam.orthographicSize * (targetAspect / currentAspect);
            cam.orthographicSize = constantWidthSize;
        }
        cam.rect = new Rect(0, 0, 1, 1);
    }
}
