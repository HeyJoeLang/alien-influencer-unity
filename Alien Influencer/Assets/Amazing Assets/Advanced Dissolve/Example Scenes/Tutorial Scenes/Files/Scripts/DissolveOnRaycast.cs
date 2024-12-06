using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace AmazingAssets.AdvancedDissolve.ExampleScripts
{
    public class DissolveOnRaycast : MonoBehaviour
    {
        public LayerMask layer;


        void Update()
        {
            if (GetLeftMouseButtonDown())
            {
                Ray ray = Camera.main.ScreenPointToRay(GetMousePosition());

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layer))
                {
                    //Hit gamobject
                    GameObject target = hit.collider.gameObject;


                    //Assign dissolve animator script to the target object
                    AnimateCutoutAndDestroy script = target.GetComponent<AnimateCutoutAndDestroy>();   //Make sure there is no animator script attached
                    if (script == null)
                    {
                        //Add a little impulse to the rigidbody
                        if(hit.collider.attachedRigidbody != null)
                            hit.collider.attachedRigidbody.AddForce(ray.direction.normalized * Random.Range(4f, 8f), ForceMode.Impulse);



                        //Add dissolve properties animator script
                        script = target.AddComponent<AnimateCutoutAndDestroy>();


                        //Instantiate material and assign it to the script
                        script.material = target.GetComponent<Renderer>().material;
                    }
                }
            }
        }


        bool GetLeftMouseButtonDown()
        {
            return Input.GetMouseButtonDown(0);
        }

        Vector3 GetMousePosition()
        {
            return Input.mousePosition;
        }
    }
}