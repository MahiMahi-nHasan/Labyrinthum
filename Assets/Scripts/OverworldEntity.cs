using System.Collections.Generic;
using UnityEngine;

public class OverworldEntity : MonoBehaviour
{
    [HideInInspector] public int id;
    public GameObject battlePrefab;
    public List<OverworldEntity> party;
}