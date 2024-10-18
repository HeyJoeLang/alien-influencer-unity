using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOLaser : MonoBehaviour
{
    public LayerMask raycastLayer;
    public Vector3 deltaPosition = Vector3.zero;
    public Vector3 deltaDirection = Vector3.forward;
    public float rayLength = 10f;
    public Color hitColor = Color.green;
    public Color missColor = Color.yellow;

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    }

    void FixedUpdate()
    {
        Vector3 rayDirection = transform.TransformDirection(deltaDirection);

        RaycastHit hit;
        Vector3 startPos = transform.position + deltaPosition;
        Vector3 endPos = startPos + rayDirection * rayLength;

        if (Physics.Raycast(startPos, rayDirection, out hit, rayLength, raycastLayer.value))
        {
            Debug.Log(hit.collider.gameObject.name);
            endPos = hit.point;
            lineRenderer.startColor = hitColor;
            lineRenderer.endColor = hitColor;
            BuildingHighlighter.Instance.HighlightObject(hit.collider.gameObject);
        }
        else
        {
            lineRenderer.startColor = missColor;
            lineRenderer.endColor = missColor;
        }
        if(Input.GetButton("Fire2") || Input.GetKey(KeyCode.X))
        {
            BuildingHighlighter.Instance.SelectObject();
        }
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }
}
