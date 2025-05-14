using System.Collections;
using System.Collections.Generic;
using UnityEngine;

  public class prefabmanager : MonoBehaviour
    {
        private static int prefabCounter = 0;

        public static int GetNextPrefabID()
        {
            return prefabCounter++;
        }
    }


