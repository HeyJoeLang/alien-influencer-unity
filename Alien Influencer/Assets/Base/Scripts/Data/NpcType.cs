using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NpcType", menuName = "NPC/NPC Type")]
public class NpcType : ScriptableObject
{


    [HideInInspector]
    public NpcCategory category { get { return m_category; } }
    [SerializeField]
    private NpcCategory m_category;

}
