using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class isClicked : MonoBehaviour
{
   public RawImage rawImagePrefab;
        private void Update()
        {

        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePosition = Input.mousePosition;

            // Instantiate the RawImage prefab at the mouse position
            RawImage newRawImage = Instantiate(rawImagePrefab, mousePosition, Quaternion.identity);
            // Make sure the RawImage is a child of the canvas or another appropriate parent object
            newRawImage.transform.SetParent(transform);
        }

        }
    }

