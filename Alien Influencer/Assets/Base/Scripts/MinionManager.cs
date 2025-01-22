using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MinionManager : Singleton<MinionManager>
{
    public GameObject SelectedBuilding;
    public static event System.Action OnNewBuildingSelected; //Event for all minions to subscribe to
    public void NewBuildingSelection(GameObject newSelectionBuilding)
    {
        SelectedBuilding = newSelectionBuilding;
        OnNewBuildingSelected?.Invoke();
    }
    public bool CanAttackBuilding()
    {
        if(SelectedBuilding == null)
        {
            return false;
        }
        Building building = SelectedBuilding.transform.parent.GetComponent<Building>();
        if(building == null)
        {
            return false;
        }
        return building.CurrentState != Building.BuildingState.IsDestroyed;
    }
    public void AttackBuilding(int damage)
    {
        SelectedBuilding.transform.parent.GetComponent<Building>()?.AddDamage(damage);
    }
}
