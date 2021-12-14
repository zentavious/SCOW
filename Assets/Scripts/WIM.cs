using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WIM : MonoBehaviour
{
    public GameObject parentWIMObject;
    public GameObject projectionSpace;
    private List<WIMMapping> mappings;
    private float smallestScaleModifier;
    private Vector3 lastPos;
    // Start is called before the first frame update
    void Start()
    {
        this.smallestScaleModifier = float.MaxValue;
        this.lastPos = this.transform.position;
        this.mappings = new List<WIMMapping>();

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
                var grabbable = this.mappings[index].clone.GetComponent<Grabbable>();
                var originalGrabbable = this.mappings[index].original.GetComponent<Grabbable>();
                originalGrabbable.Deselect(); // force deselect, we will select again if this is highlighted
                if (grabbable)
                {
                    if (!grabbable.IsGrabbed())
                    {
                        grabbable.SetOriginalPosition(child.transform.position, child.transform.rotation);
                    }
                    if (grabbable.IsHighlighted())
                    {
                        originalGrabbable.Select();
                    }
                }
                else // probablhy not necessary if only grabbable objects are projected
                {
                    this.mappings[index].clone.transform.position = child.transform.position;
                    this.mappings[index].clone.transform.rotation = child.transform.rotation;
                }
            }
        }

        this.lastPos = this.transform.position;
    }

    public void CastObjectsToWIM(GameObject parentWIMObject)
    {
        this.mappings = new List<WIMMapping>(); // Force clear any existing mappings
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
            /*
            var clone = GameObject.Instantiate(child.gameObject);
            clone.transform.parent = this.parentWIMObject.transform;
            clone.transform.localPosition = new Vector3(child.transform.localPosition.x, child.transform.localPosition.y, child.transform.localPosition.z);*/

            this.mappings.Add(new WIMMapping(child.gameObject, this.parentWIMObject));

            var gameObject = new GameObject($"Object_{i}");
            gameObject.transform.parent = this.projectionSpace.transform;
            gameObject.transform.localPosition = new Vector3(child.localPosition.x, child.localPosition.y, child.localPosition.z);
            Debug.Log(gameObject.transform.position);

            i++;
        }
    }

    public GameObject UnCastWIM(Grabbable targetClone)
    {
        var targetMapping = this.mappings.FirstOrDefault<WIMMapping>(_ => _.clone == targetClone.gameObject);
        if (targetMapping != null)
        {

            if (targetClone.IsGrabbed())
            {
                targetClone.Release();
            }
            else if (targetClone.IsS)
            Debug.Log("Found target clone");
        }
        else
        {
            Debug.Log("Couldn't find target clone");
        }
        //this.mappings = new List<WIMMapping>();
        return targetMapping.original;
    }

    public float GetSmallestScaleModifier()
    {
        return this.smallestScaleModifier;
    }
    
    private class WIMMapping
    {
        public GameObject original;
        public GameObject clone;

        public WIMMapping(GameObject original, GameObject parentWIMObject)
        {
            this.original = original;
            this.clone = GameObject.Instantiate(original);
            clone.transform.parent = parentWIMObject.transform;
            clone.transform.localPosition = new Vector3(original.transform.localPosition.x, original.transform.localPosition.y, original.transform.localPosition.z);
        }
    }
}
