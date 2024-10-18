using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private string[] checkbuttons = new string[] {
        "Fire1",
        "Fire2",
        "Fire3",
        "Jump",
        "Submit",
        "Cancel" };
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Fire1 Button Pressed");
        }
        if (Input.GetButtonDown("Fire2"))
        {
            Debug.Log("Fire2 Button Pressed");
        }
        if (Input.GetButtonDown("Fire3"))
        {
            Debug.Log("Fire3 Button Pressed");
        }
        if (Input.GetButtonDown("Jump"))
        {
            Debug.Log("Jump Button Pressed");
        }
        if (Input.GetButtonDown("Submit"))
        {
            Debug.Log("Submit Button Pressed");
        }
        if (Input.GetButtonDown("Cancel"))
        {
            Debug.Log("Cancel Button Pressed");
        }
    }
}
