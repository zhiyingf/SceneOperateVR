using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZoomTips : MonoBehaviour
{
    public int N = 5;
    public Transform cameraTransform;
    public RadialMenu radialMenu;
    public LineRenderer lineRenderer;
    public Transform radialMenuPos;
    //public RectTransform rectTransform;
    //public Text label;

    private RectTransform rectTransform;
    private Text label;


    void Start()
    {
        rectTransform = GetComponentInChildren<RectTransform>();
        label = GetComponentInChildren<Text>();

        lineRenderer.positionCount = N + 1;
        lineRenderer.enabled = false;

    }

    public void StartZoom()
    {
        lineRenderer.enabled = true;
    }

    public void EndZoom()
    {
        label.enabled = false;
        lineRenderer.enabled = false;
        radialMenu.Show(false);
    }

    public void UpdateZoom(Vector3 primaryHand, Vector3 secondaryHand, bool zoomSuccess, Vector3 newScale)
    {

        Vector3[] positions = new Vector3[N + 1];
        float step = 1.0f / N;

        for (int i = 0; i <= N; i++)
        {
            float t = i * step;
            positions[i] = Vector3.Lerp(primaryHand, secondaryHand, t);
        }

        lineRenderer.SetPositions(positions);

        // Text
        label.enabled = true;
        label.text = "size " + newScale.ToString("F1");
        rectTransform.anchoredPosition3D = Vector3.Lerp(primaryHand, secondaryHand, 0.5f);
        rectTransform.LookAt(cameraTransform.position);

        radialMenu.Show(true);
        radialMenuPos.position = rectTransform.anchoredPosition3D;
        radialMenuPos.rotation = rectTransform.rotation;

        // Color
        if (!zoomSuccess)
            lineRenderer.material.color = Color.red;
        else
            lineRenderer.material.color = Color.yellow;
    }

}
