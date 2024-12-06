using Sharklib.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOLaser : MonoBehaviour
{
    public LayerMask raycastLayer;
    public LayerMask terrainLayer;
    public Vector3 deltaPosition = Vector3.zero;
    public Vector3 deltaDirection = Vector3.forward;
    public float rayLength = 10f;
    public Color hitColor = Color.green;
    public Color missColor = Color.yellow;

    private LineRenderer lineRenderer;
    public GameObject crosshair;
    Material crosshairMat;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        crosshairMat = crosshair.GetComponent<MeshRenderer>().material;
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
            crosshairMat.color = hitColor;
            crosshair.transform.rotation = Quaternion.LookRotation(hit.normal);
            //Debug.Log(hit.collider.gameObject.name);
            endPos = hit.point;
            BuildingHighlighter.Instance.HighlightObject(hit.collider.gameObject);
        }
        else if (Physics.Raycast(startPos, rayDirection, out hit, rayLength, terrainLayer.value))
        {
            endPos = hit.point;
            crosshairMat.color = missColor;
            crosshair.transform.rotation = Quaternion.LookRotation(hit.normal);
        }
        else
        {
            crosshairMat.color = missColor;
        }
        if(Input.GetButton("Fire2") || Input.GetKey(KeyCode.P))
        {
            BuildingHighlighter.Instance.SelectObject();
        }
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
        crosshair.transform.position = endPos - (endPos - startPos).normalized * 0.5f;
    }
}
