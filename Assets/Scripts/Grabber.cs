using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grabber : MonoBehaviour
{
    /*
     * defaultEffectRadius is currently tested for objects of scale .025 -> radius .15
     */
    public LineRenderer laserPointer;
    public GameObject sphereCast;
    public LayerMask defaultLayer;
    public float viscocity; // propotion of effectRadius to use a movement inside collider (0,1]
    public float defaultEffectRadius; 
    public GameObject effectBubble;
    public SphereCollider effectCollider; // needs to be bigger than modified collider radius by 1
    public InputActionProperty grabAction;
    public InputActionProperty selectAction;
    public WIM worldInMiniature;

    public float modifiendEffectRadius; // use this for all external radius functions

    private List<Grabbable> Grabbables = new List<Grabbable>();
    private GameObject grabbedObject;
    private GameObject selectedObject;
    private bool effectOn; // this is the Oil in Water effect. WHen off ray - sphere cast is "on"

    // Start is called before the first frame update
    void Start()
    {
        this.laserPointer.enabled = true;

        this.grabbedObject = null;
        this.effectOn = true;

        this.modifiendEffectRadius = this.defaultEffectRadius;
        this.effectCollider.radius = modifiendEffectRadius + 1;
        this.effectBubble.transform.localScale = new Vector3(this.modifiendEffectRadius * 2, this.modifiendEffectRadius * 2, this.modifiendEffectRadius * 2);

        this.grabAction.action.performed += Grab;
        this.grabAction.action.canceled += Release;

        this.selectAction.action.performed += Select;
    }

    // Update is called once per frame
    void Update()
    {
        if (laserPointer.enabled)
        {
            RaycastHit hit;
            if (Physics.Raycast(this.transform.position, this.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, this.defaultLayer))
            {
                this.laserPointer.SetPosition(1, new Vector3(0, 0, hit.distance));
                this.sphereCast.transform.position = hit.point;
                this.sphereCast.GetComponent<Renderer>().enabled = true;
                if (hit.collider.GetComponent<Grabbable>())
                {
                    //Debug.Log("hit");
                }
            }
            else
            {
                this.sphereCast.GetComponent<Renderer>().enabled = false;
                laserPointer.SetPosition(1, new Vector3(0, 0, 100));
            }
        }

        var closestObject = this.GetClosestGrabbable(highlight: true);
        if (closestObject)
        {
            modifiendEffectRadius = Math.Max(closestObject.transform.lossyScale.x, Math.Max(closestObject.transform.lossyScale.y, closestObject.transform.lossyScale.z)) + defaultEffectRadius - 0.025f;
            effectCollider.radius = modifiendEffectRadius + 1;
        }
        else
        {
            modifiendEffectRadius = defaultEffectRadius;
            effectCollider.radius = modifiendEffectRadius + 1;
        }
        this.effectBubble.transform.localScale = new Vector3(modifiendEffectRadius * 2, modifiendEffectRadius * 2, modifiendEffectRadius * 2);
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
            var grabbedObject = GetClosestGrabbable();
            var grabbable = grabbedObject.GetComponent<Grabbable>();

            grabbable = this.worldInMiniature.UnCastWIM(grabbable);// switch to original grabbabale
            this.grabbedObject = grabbable.gameObject;

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
            var selectedObject = GetClosestGrabbable();
            if (selectedObject)
            {
                var grabbable = selectedObject.GetComponent<Grabbable>(); // Will null ref is select is pressed without an object

                grabbable = this.worldInMiniature.UnCastWIM(grabbable); // switch to original grabbabale
                this.selectedObject = grabbable.gameObject;

                grabbable.Select();
                worldInMiniature.UnCastWIM(grabbable);
                this.effectOn = false;
            }
        }
        else
        {
            var grabbable = this.selectedObject.GetComponent<Grabbable>();
            
            grabbable.Deselect();

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
            if (distance - this.worldInMiniature.GetSmallestScaleModifier() <= this.modifiendEffectRadius + this.worldInMiniature.GetSmallestScaleModifier()) //
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

        if (closestGrabbable)
        {
            return closestGrabbable.gameObject;
        }
        else
        {
            return null;
        }
    }
}
