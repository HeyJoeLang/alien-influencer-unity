using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingType", menuName = "Building/Building Type")]
public class BuildingType : ScriptableObject
{

    [HideInInspector]
    public BuildingCategory category { get { return m_category; } }
    [SerializeField]
    private BuildingCategory m_category;

}
