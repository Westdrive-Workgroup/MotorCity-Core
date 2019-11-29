using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "/Resources/Shared Data/Asset List", menuName = "CityAI/Asset List")]
public class AssetLists : ScriptableObject
{
    public List<GameObject> Cars;
    public List<GameObject> Pedestrians;
}
