using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Civilian : MonoBehaviour
{
    private Vector3 centerPosition;
    private float speed;
    private float radius;
    private float angle;

    public void Initialize(Vector3 spawnPosition, int meshIndex)
    {
        transform.GetChild(meshIndex).gameObject.SetActive(true);
        centerPosition = spawnPosition;
        speed = Random.Range(5.0f, 40.0f);
        radius = Random.Range(5.0f, 20.0f);
        angle = Random.Range(0, 360);
    }

    void Update()
    {
        /* *
        * Moving the person in a circular path around the center position
        * */

        angle += speed * Time.deltaTime;


        float radian = angle * Mathf.Deg2Rad;

        float x = centerPosition.x + Mathf.Cos(radian) * radius;
        float z = centerPosition.z + Mathf.Sin(radian) * radius;

        Vector3 destination = new Vector3(x, transform.position.y, z);

        transform.LookAt(destination);
        transform.position = destination;
    }
}