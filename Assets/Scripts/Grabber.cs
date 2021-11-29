using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grabber : MonoBehaviour
{
    public float effectRadius; // needs to be smaller than collider radius
    public InputActionProperty grabAction;
    public InputActionProperty selectAction;

    private List<Grabbable> Grabbables = new List<Grabbable>();
    private GameObject grabbedObject;
    private GameObject selectedObject;
    private bool effectOn;

    // Start is called before the first frame update
    void Start()
    {
        grabbedObject = null;
        effectOn = true;

        grabAction.action.performed += Grab;
        grabAction.action.canceled += Release;


        selectAction.action.performed += Select;
    }

    // Update is called once per frame
    void Update()
    {
        var closestObject = this.GetClosestGrabbable(highlight: true);
        //this.effectRadius = closestObject.transform.lossyScale.
    }

    void OnTriggerEnter(Collider other)
    {
        var grabbable = other.GetComponent<Grabbable>();
        if (grabbable)
        {
            this.Grabbables.Add(grabbable);
        }
    }

    void OnTriggerExit(Collider other)
    {
        var grabbable = other.GetComponent<Grabbable>();
        if (grabbable)
        {
            this.Grabbables.Remove(grabbable);
        }
    }
        
    public void Grab(InputAction.CallbackContext context)
    {
        if (this.grabbedObject == null)
        {
            this.grabbedObject = GetClosestGrabbable();
            var grabbable = this.grabbedObject.GetComponent<Grabbable>();

            grabbable.Grab(parent: this.transform);
            this.effectOn = false;
        }
    }

    public void Release(InputAction.CallbackContext context)
    {
        if (this.grabbedObject)
        {
            var grabbable = this.grabbedObject.GetComponent<Grabbable>();

            grabbable.Release();
            this.grabbedObject = null;
            this.effectOn = this.selectedObject == null; // TODO: effect should only be on after sphere cast, work around bc cast is not implimented
        }
    }


    public void Select(InputAction.CallbackContext context)
    {
        if (this.selectedObject == null)
        {
            this.selectedObject = GetClosestGrabbable();

            var grabbable = this.selectedObject.GetComponent<Grabbable>(); // Will null ref is select is pressed without an object
            grabbable.Select();

            this.effectOn = false;
        }
        else
        {
            var grabbable = this.selectedObject.GetComponent<Grabbable>();
            
            grabbable.Select();

            this.selectedObject = null;
            this.effectOn = this.grabbedObject == null; // TODO: effect should only be on after sphere cast, work around bc cast is not implimented
        }
    }

    public bool IsEffectOn()
    {
        return this.effectOn;
    }

    private GameObject GetClosestGrabbable(bool highlight = false)
    {
        Grabbable closestGrabbable = null;
        var closestDistance = 0f;

        foreach (var grabbable in this.Grabbables)
        {
            grabbable.Unhighlight();
            var distance = Vector3.Distance(grabbable.GetOriginalPosition(), this.gameObject.transform.position);
            if (!closestGrabbable)
            {
                closestGrabbable = grabbable;
                closestDistance = distance;
            }
            else if (closestDistance > distance)
            {
                closestDistance = distance;
                closestGrabbable = grabbable;
            }
        }

        if (closestDistance <= this.effectRadius)
        {
            if (highlight)
            {
                closestGrabbable?.Highlight();
            }
            return closestGrabbable?.gameObject;
        }

        return null;
    }
}
