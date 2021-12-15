using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WIM : MonoBehaviour
{
    public GameObject parentWIMObject;
    public GameObject projectionSpace;
    public LineRenderer laserPointer;
    public Grabber controller;
    public Material seeThrough;
    private List<WIMMapping> mappings;
    private float smallestScaleModifier;
    private Vector3 lastPos;
    private bool inUpdateLoop;
    // Start is called before the first frame update
    void Start()
    {
        this.smallestScaleModifier = float.MaxValue;
        this.lastPos = this.transform.position;
        this.mappings = new List<WIMMapping>();

        //this.CastObjectsToWIM(parentWIMObject); // cheat until we actually start casting

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
        this.laserPointer.enabled = false;
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

            this.mappings.Add(new WIMMapping(child.gameObject, this.parentWIMObject, this.seeThrough));

            var gameObject = new GameObject($"Object_{i}");
            gameObject.transform.parent = this.projectionSpace.transform;
            gameObject.transform.localPosition = new Vector3(child.localPosition.x*.5f, child.localPosition.y * .5f, child.localPosition.z * .5f);
            gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);

            i++;
        }
        this.controller.SetEffectOn(true);
    }

    public Grabbable UnCastWIM(Grabbable targetClone)
    {
        var targetMapping = this.mappings.FirstOrDefault<WIMMapping>(_ => _.clone == targetClone.gameObject);
        Grabbable originalGrabbable = null;
        if (targetMapping != null)
        {
            originalGrabbable = targetMapping.original.GetComponent<Grabbable>();
            Debug.Log("Found target clone");

            foreach (Transform child in this.projectionSpace.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            GameObject.Destroy(this.parentWIMObject);
            this.parentWIMObject = null;
        }
        else
        {
            Debug.Log("Couldn't find target clone");
            throw new NullReferenceException();
        }
        foreach (WIMMapping mapping in this.mappings)
        {
            mapping.original.GetComponent<MeshRenderer>().material = mapping.originalMaterial;
            mapping.original.GetComponent<Grabbable>()?.Deselect();
        }
        this.mappings = new List<WIMMapping>();
        return originalGrabbable;
    }

    public float GetSmallestScaleModifier()
    {
        return this.smallestScaleModifier;
    }
    
    private class WIMMapping
    {
        public GameObject original;
        public GameObject clone;
        public Material originalMaterial;

        public WIMMapping(GameObject original, GameObject parentWIMObject, Material seeThrough)
        {
            this.original = original;
            this.clone = GameObject.Instantiate(original);
            var rigidBody = clone.GetComponent<Rigidbody>();
            rigidBody.isKinematic = true;
            rigidBody.useGravity = false;
            clone.transform.parent = parentWIMObject.transform;
            clone.transform.localScale = clone.transform.localScale * .5f;
            clone.transform.localPosition = new Vector3(original.transform.localPosition.x * .5f, original.transform.localPosition.y * .5f, original.transform.localPosition.z * .5f);
            MeshRenderer renderer = this.original.GetComponent<MeshRenderer>();
            this.originalMaterial = renderer.material;
            renderer.material = seeThrough;
        }
    }
}
