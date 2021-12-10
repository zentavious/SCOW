using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WIM : MonoBehaviour
{
    public GameObject parentWIMObject;
    public GameObject projectionSpace;
    private List<GameObject> miniObjects;
    private float smallestScaleModifier;
    private Vector3 lastPos;
    // Start is called before the first frame update
    void Start()
    {
        this.smallestScaleModifier = float.MaxValue;
        this.lastPos = this.transform.position;
        this.miniObjects = new List<GameObject>();

        this.CastObjectsToWIM(parentWIMObject); // cheat until we actually start casting

    }

    // Update is called once per frame
    void Update()
    {
        var deltaPos = this.lastPos - this.transform.position;
        foreach (Transform child in this.projectionSpace.transform)
        {
            var name = child.name.Split('_');
            if (name[0].Equals("Object"))
            {
                var index = int.Parse(child.name.Split('_')[1]);
                var grabbable = miniObjects[index].GetComponent<Grabbable>();
                if (grabbable)
                {
                    if (!grabbable.IsGrabbed())
                    {
                        grabbable.SetOriginalPosition(child.transform.position, child.transform.rotation);
                    }
                }
                else
                {
                    miniObjects[index].transform.position = child.transform.position;
                    miniObjects[index].transform.rotation = child.transform.rotation;
                }
            }
        }

        this.lastPos = this.transform.position;
    }

    public void CastObjectsToWIM(GameObject parentWIMObject)
    {
        this.smallestScaleModifier = float.MaxValue; // reset on every new cast

        int i = 0;
        this.parentWIMObject = new GameObject("Parent Container");

        this.parentWIMObject.transform.position = this.projectionSpace.transform.position;
        foreach (Transform child in parentWIMObject.transform)
        {
            var scaleModifier = Vector3.Distance(Vector3.zero, child.transform.lossyScale) / 2;
            if (scaleModifier <= this.smallestScaleModifier)
            {
                this.smallestScaleModifier = scaleModifier;
            }
            var clone = GameObject.Instantiate(child.gameObject);
            clone.transform.parent = this.parentWIMObject.transform;
            clone.transform.localPosition = new Vector3(child.transform.localPosition.x, child.transform.localPosition.y, child.transform.localPosition.z);

            this.miniObjects.Add(clone);

            var gameObject = new GameObject($"Object_{i}");
            gameObject.transform.parent = this.projectionSpace.transform;
            gameObject.transform.localPosition = new Vector3(child.transform.localPosition.x, child.transform.localPosition.y, child.transform.localPosition.z);
            Debug.Log(gameObject.transform.position);

            i++;
        }
    }

    public float GetSmallestScaleModifier()
    {
        return this.smallestScaleModifier;
    }

}
