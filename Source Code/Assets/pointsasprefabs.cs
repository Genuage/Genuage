using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class pointsasprefabs : MonoBehaviour
{

    private int ct = 0;


   public void pointasPrefab(Vector2 point)
    {
       
        GameObject gameobj = new GameObject();
        vector2asset asset = new vector2asset();
        asset.vector = point;
        asset.index = ct;
        //GameObject prefab = Instantiate(gameobj, point, Quaternion.identity) as GameObject;
        string prefabName = "Vector2Prefab_" + point.ToString();
        string prefabPath = "Assets/Prefabs1/" + prefabName + ".prefab";
        gameobj.name = prefabName;
        //PrefabUtility.SaveAsPrefabAsset(gameobj, prefabPath);
  //      DestroyImmediate(prefab);

        ct++;

    }

  
}
