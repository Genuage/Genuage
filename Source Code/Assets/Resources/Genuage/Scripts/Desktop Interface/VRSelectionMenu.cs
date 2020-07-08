using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VR_Interaction;
using Data;


namespace DesktopInterface
{

    public class VRSelectionMenu : MonoBehaviour
    {
        public GameObject ButtonPrefab;
        public GameObject SphereViewPort;
        public GameObject ConvexHullViewPort;

        public Dictionary<string, Dictionary<int, GameObject>> ButtonDicts;
        public Dictionary<int, GameObject> SphereButtonsDict;
        public Dictionary<int, GameObject> ConvexHullButtonsDict;

        // Start is called before the first frame update
        void Awake()
        {
            ButtonDicts = new Dictionary<string, Dictionary<int, GameObject>>();

            SphereButtonsDict = new Dictionary<int, GameObject>();
            ConvexHullButtonsDict = new Dictionary<int, GameObject>();
            ButtonDicts.Add("Sphere", SphereButtonsDict);
            ButtonDicts.Add("ConvexHull", ConvexHullButtonsDict);

            CreateAllButtons();
            VRObjectsManager.instance.OnContainerCreated += ContainerCreated;
            VRObjectsManager.instance.OnContainerDeleted += ContainerDeleted;

            CloudSelector.instance.OnSelectionChange += SelectionChanged;
        }

        private void SelectionChanged(int id)
        {
            CreateAllButtons();
        }
        public void CreateAllButtons()
        {
            DeleteAllButtons();
            CloudData data = CloudUpdater.instance.LoadCurrentStatus();

            foreach (KeyValuePair<int, GameObject> kvp in data.globalMetaData.sphereList)
            {
                ContainerCreated(kvp.Value.GetComponent<VRContainerSelectionSphere>().id, kvp.Value, "Sphere");
            }
            foreach (KeyValuePair<int, GameObject> kvp in data.globalMetaData.convexHullsList)
            {
                ContainerCreated(kvp.Value.GetComponent<VRContainerConvexHull>().id, kvp.Value, "ConvexHull");
            }


        }

        private void DeleteAllButtons()
        {
            foreach (string s in ButtonDicts.Keys)
            {
                foreach (int i in ButtonDicts[s].Keys)
                {
                    Destroy(ButtonDicts[s][i]);
                }
                ButtonDicts[s].Clear();
            }

        }


        public void ContainerCreated(int id, GameObject container, string type)
        {
            GameObject newbutton = CreateButton(id, container);
            switch (type)
            {
                case "Sphere":
                    newbutton.transform.SetParent(SphereViewPort.transform);
                    newbutton.transform.localPosition = new Vector3(newbutton.transform.localPosition.x, newbutton.transform.localPosition.y, 0f);
                    newbutton.transform.localScale = Vector3.one;
                    newbutton.transform.rotation = this.transform.rotation;
                    SphereButtonsDict.Add(id, newbutton);

                    break;
                case "ConvexHull":
                    newbutton.transform.SetParent(ConvexHullViewPort.transform);
                    newbutton.transform.localPosition = new Vector3(newbutton.transform.localPosition.x, newbutton.transform.localPosition.y, 0f);
                    newbutton.transform.localScale = Vector3.one;
                    newbutton.transform.rotation = this.transform.rotation;

                    ConvexHullButtonsDict.Add(id, newbutton);
                    break;
                default:
                    Debug.Log("VRO Menu Button Creation Error");
                    break;
            }
        }

        private GameObject CreateButton(int id, GameObject container)
        {
            GameObject go = Instantiate(ButtonPrefab) as GameObject;
            go.transform.GetChild(0).GetComponent<Text>().text = id.ToString();
            go.AddComponent<GameObjectActivationButton>();
            go.GetComponent<GameObjectActivationButton>().obj = container;
            return go;
        }

        public void ContainerDeleted(int id, string type)
        {
            switch (type)
            {
                case "Sphere":
                    if (SphereButtonsDict.ContainsKey(id))
                    {
                        GameObject go = SphereButtonsDict[id];
                        SphereButtonsDict.Remove(id);
                        Destroy(go);
                    }
                    break;
                case "ConvexHull":
                    if (ConvexHullButtonsDict.ContainsKey(id))
                    {
                        GameObject go = ConvexHullButtonsDict[id];
                        ConvexHullButtonsDict.Remove(id);
                        Destroy(go);
                    }
                    break;
                default:
                    Debug.Log("VRO Menu Button Deletion Error");
                    break;

            }

        }


        private void OnEnable()
        {
            CreateAllButtons();
        }

    }
}