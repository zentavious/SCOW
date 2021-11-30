using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grabber : MonoBehaviour
{
    /*
     * defaultEffectRadius is currently tested for objects of scale .025 -> radius .15
     */
    public float defaultEffectRadius; // needs to be smaller than collider radius
    public GameObject effectBubble;
    public SphereCollider effectCollider;
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

        this.effectBubble.transform.localScale = new Vector3(effectCollider.radius * 2, effectCollider.radius * 2, effectCollider.radius * 2);
        effectCollider.radius = defaultEffectRadius;

        grabAction.action.performed += Grab;
        grabAction.action.canceled += Release;

        selectAction.action.performed += Select;
    }

    // Update is called once per frame
    void Update()
    {
        var closestObject = this.GetClosestGrabbable(highlight: true);
        if (closestObject)
        {
            effectCollider.radius = closestObject.transform.lossyScale.x + .125f;
        }
        else
        {
            effectCollider.radius = defaultEffectRadius;
        }
        this.effectBubble.transform.localScale = new Vector3(effectCollider.radius * 2, effectCollider.radius * 2, effectCollider.radius * 2);
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
            var scaleModifier = Vector3.Distance(Vector3.zero, grabbable.transform.lossyScale) / 2;
            if (distance - scaleModifier <= this.defaultEffectRadius + scaleModifier)
            {
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
        }

        if (highlight)
        {
            closestGrabbable?.Highlight();
        }

        return closestGrabbable?.gameObject;
    }
}
