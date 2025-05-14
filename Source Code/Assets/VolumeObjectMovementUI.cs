using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using Data;
public class VolumeObjectMovementUI : MonoBehaviour
{

    public InputField PositionInputX;
    public InputField PositionInputY;
    public InputField PositionInputZ;

    public InputField RotationInputX;
    public InputField RotationInputY;
    public InputField RotationInputZ;

    public InputField ScaleInputX;
    public InputField ScaleInputY;
    public InputField ScaleInputZ;

    public InputField VoxelSizeInput;
    public InputField VoxelSizeZInput;

    private void Awake()
    {
        PositionInputX.onEndEdit.AddListener(ChangeCoordinates);
        PositionInputY.onEndEdit.AddListener(ChangeCoordinates);
        PositionInputZ.onEndEdit.AddListener(ChangeCoordinates);

        RotationInputX.onEndEdit.AddListener(ChangeCoordinates);
        RotationInputY.onEndEdit.AddListener(ChangeCoordinates);
        RotationInputZ.onEndEdit.AddListener(ChangeCoordinates);

        ScaleInputX.onEndEdit.AddListener(ChangeCoordinates);
        ScaleInputY.onEndEdit.AddListener(ChangeCoordinates);
        ScaleInputZ.onEndEdit.AddListener(ChangeCoordinates);

        VoxelSizeInput.onEndEdit.AddListener(ChangeVoxelSize);
        VoxelSizeZInput.onEndEdit.AddListener(ChangeVoxelSize);

        CloudUpdater.instance.OnVolumeRenderingActivated += UpdateFields;
        CloudSelector.instance.OnSelectionChange += UpdateFieldsSelection;
    }

    private void UpdateFieldsSelection(int i)
    {
        UpdateFields();
    }

    private void UpdateFields()
    {
        CloudData data = CloudUpdater.instance.LoadCurrentStatus();
        if (data.globalMetaData.volume_rendered_gameobject)
        {


            Transform t = data.globalMetaData.volume_rendered_gameobject.transform;
            PositionInputX.text = t.localPosition.x.ToString("F2", CultureInfo.InvariantCulture);
            PositionInputY.text = t.localPosition.y.ToString("F2", CultureInfo.InvariantCulture);
            PositionInputZ.text = t.localPosition.z.ToString("F2", CultureInfo.InvariantCulture);

            RotationInputX.text = t.localRotation.eulerAngles.x.ToString("F2", CultureInfo.InvariantCulture);
            RotationInputY.text = t.localRotation.eulerAngles.y.ToString("F2", CultureInfo.InvariantCulture);
            RotationInputZ.text = t.localRotation.eulerAngles.z.ToString("F2", CultureInfo.InvariantCulture);


            VoxelSizeInput.text = data.globalMetaData.voxel_size.ToString("F2", CultureInfo.InvariantCulture);
            VoxelSizeZInput.text = data.globalMetaData.voxel_size_z.ToString("F2", CultureInfo.InvariantCulture);
            //ScaleInputX.text = t.localScale.x.ToString("F2", CultureInfo.InvariantCulture);
            //ScaleInputY.text = t.localScale.y.ToString("F2", CultureInfo.InvariantCulture);
            //ScaleInputZ.text = t.localScale.z.ToString("F2", CultureInfo.InvariantCulture);
        }

    }

    private void ChangeVoxelSize(string Value)
    {
        float VoxelSize = float.Parse(VoxelSizeInput.text, CultureInfo.InvariantCulture);
        float VoxelSizeZ = float.Parse(VoxelSizeZInput.text, CultureInfo.InvariantCulture);

        CloudUpdater.instance.ChangeVolumeRenderedObjectVoxelSize(VoxelSize, VoxelSizeZ);
    }

    private void ChangeCoordinates(string uselessValue)
    {
        float VoxelSize = float.Parse(VoxelSizeInput.text, CultureInfo.InvariantCulture);

        float VoxelSizeZ = float.Parse(VoxelSizeZInput.text, CultureInfo.InvariantCulture);
        CloudUpdater.instance.ChangeVolumeRenderedObjectVoxelSize(VoxelSize, VoxelSizeZ);

        CloudUpdater.instance.ChangeVolumeRenderedObjectLocalCoordinates(new Vector3(float.Parse(PositionInputX.text, CultureInfo.InvariantCulture), float.Parse(PositionInputY.text, CultureInfo.InvariantCulture), float.Parse(PositionInputZ.text, CultureInfo.InvariantCulture)),
                                                                         new Vector3(float.Parse(RotationInputX.text, CultureInfo.InvariantCulture), float.Parse(RotationInputY.text, CultureInfo.InvariantCulture), float.Parse(RotationInputZ.text, CultureInfo.InvariantCulture)),
                                                                         new Vector3(float.Parse(ScaleInputX.text, CultureInfo.InvariantCulture), float.Parse(ScaleInputY.text, CultureInfo.InvariantCulture), float.Parse(ScaleInputZ.text, CultureInfo.InvariantCulture)));
            
    }

}
