using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapDataTable", menuName = "MapDataTable/Add MapDataTable", order = 1)]
public class MapDataTable : ScriptableObject
{
    public List<string> mapDatas = new List<string>();
}
