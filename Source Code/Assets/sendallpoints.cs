using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
public class sendallpoints : MonoBehaviour
{
    public void send()
    {
        HashSet<int> set = new HashSet<int>();
        CloudData data = CloudUpdater.instance.LoadCurrentStatus();
        foreach(int i in data.pointDataTable.Keys)
        {
            set.Add(i);
        }

        data.globalMetaData.FreeSelectionIDList = set;
        data.globalMetaData.FreeSelectionON = true;
        CloudUpdater.instance.UpdatePointSelection();
    }
}
