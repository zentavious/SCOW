using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    public float viscocity; // propotion of effectRadius to use a movement inside collider

    private Vector3 originalPosition;
    private Grabber controller;
    private bool isHighlighted;
    private bool isGrabbed;

    private float scaleModifier;
    

    // Start is called before the first frame update
    void Start()
    {
        this.isHighlighted = false;
        this.isGrabbed = false;

        this.scaleModifier = Vector3.Distance(Vector3.zero, this.transform.lossyScale) / 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGrabbed)
        {
            if (controller?.IsEffectOn() ?? false)
            {
                if (!isHighlighted)
                {
                    if (Vector3.Distance(this.originalPosition, this.controller.transform.position) - this.scaleModifier <= this.controller.effectCollider.radius + this.scaleModifier) // TODO: Fix jitter effect on boundaries
                    {
                        var distanceFromController = Vector3.Distance(this.originalPosition, this.controller.transform.position) - this.scaleModifier;
                        var newDistance = distanceFromController / 2f - this.controller.effectCollider.radius / 2f; // TODO: needs to factor in size of selected object, or this object?
                        var newPos = Vector3.MoveTowards(this.originalPosition, this.controller.transform.position, newDistance);
                        newPos = Vector3.MoveTowards(this.transform.position, newPos, this.viscocity * this.controller.effectCollider.radius);
                        this.transform.position = newPos;
                    }
                    else
                    {
                        this.transform.position = Vector3.MoveTowards(this.transform.position, this.originalPosition, this.viscocity * this.controller.effectCollider.radius);
                    }
                }
                else
                {
                    this.transform.position = Vector3.MoveTowards(this.transform.position, this.controller.transform.position, this.viscocity * this.controller.effectCollider.radius);
                }
            }
            else if (!controller?.IsEffectOn() ?? false)
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, this.originalPosition, this.viscocity * this.controller.effectCollider.radius);
            }
        }
        else
        {
            this.originalPosition = this.transform.position;
        }
    }

    void OnTriggerEnter(Collider other) 
    {
        if (other.GetComponent<Grabber>())
        {
            this.originalPosition = this.transform.position;
            this.controller = other.GetComponent<Grabber>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Grabber>())
        {
            this.transform.position = this.originalPosition; // might not be needed
            this.controller = null;
        }
    }

    public void Highlight()
    {
        this.isHighlighted = true;
    }
    public void Unhighlight()
    {
        this.isHighlighted = false;
    }

    public bool IsHighlighted()
    {
        return this.isHighlighted;
    }
    public Vector3 GetOriginalPosition()
    {
        return this.originalPosition;
    }

    public void Grab(Transform parent)
    {
        this.isGrabbed = true;

        this.transform.position = parent.position;
        this.transform.parent = parent;
    }

    public void Release()
    {
        this.originalPosition = this.transform.position;
        this.transform.parent = null;

        this.isGrabbed = false;
    }

    public bool Select()
    {
        var outline = this.GetComponent<Outline>();
        outline.enabled = !outline.enabled;
        return outline.enabled;
    }
}
