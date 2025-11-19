using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
namespace HomingMissile
{
   public class shoot_missile_example : MonoBehaviour
   {
      public GameObject missile_prefab;
      public GameObject target;
      
      public static event Action<GameObject> MissileCreated;

      void Update()
      {
         transform.parent.LookAt(target.transform.position,Vector3.up);
      }
      public void shoot_missile()
      {
         GameObject missile= Instantiate(missile_prefab,new Vector3(200,200,200),transform.parent.rotation);
         homing_missile hm = missile.GetComponent<homing_missile>();
         hm.target=target;
         hm.targetpointer.GetComponent<homing_missile_pointer>().target=target;
         hm.shooter=this.gameObject;
         hm.usemissile();  
         MissileCreated?.Invoke(missile);
      
      }
   }
}