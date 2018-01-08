using UnityEngine;
using System.Collections;

public class CameraSplashEffect : MonoBehaviour {

    public MeshRenderer meshRender;
    private static Vector3[] corners = new Vector3[4];

    private void Awake()
    {
        if (meshRender == null)
        {
            meshRender = GetComponent<MeshRenderer>();
        }
    }

    private void LateUpdate()
    {
        var camera = Camera.main;
        var cameraTrans = camera.transform;

        float dist = camera.nearClipPlane + 0.01f;
        Vector3 position = camera.cameraToWorldMatrix.MultiplyPoint(Vector3.back * dist);

        transform.position = position;
        transform.LookAt(camera.cameraToWorldMatrix.MultiplyPoint(Vector3.zero));

        camera.useJitteredProjectionMatrixForTransparentRendering = false;
        if (camera.orthographic)
        {
            transform.localScale = new Vector3(
                camera.orthographicSize / camera.aspect,
                camera.orthographicSize, 1);
        }
        else
        {
            camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), dist, Camera.MonoOrStereoscopicEye.Mono, corners);

            float width = (corners[3] - corners[0]).magnitude;
            float height = (corners[1] - corners[0]).magnitude;

            transform.localScale = new Vector3(width, height, 1);
        }
    }
}
