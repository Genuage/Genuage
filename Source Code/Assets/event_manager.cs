
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class event_manager : MonoBehaviour
{
    public pointmanager2 pointManager;
 


    public Button RemovePointButton;




    private void OnChangeCoordinatesButtonClicked()
    {
        pointManager.ChangeCoordinates();
    }

    private void OnAddPointButtonClicked()
    {
        pointManager.AddPoint();
    }

    private void OnRemovePointButtonClicked()
    {
        pointManager.RemovePoint();
    }


}


