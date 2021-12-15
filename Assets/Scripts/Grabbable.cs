using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{

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
        //this.scaleModifier = this.transform.lossyScale.x / 2;
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
                    var distanceFromController = Vector3.Distance(this.originalPosition, this.controller.transform.position);

                    var normalizedScaleModifier = (this.controller.modifiendEffectRadius - distanceFromController) / this.controller.modifiendEffectRadius * scaleModifier; // Without this there is jitter on the boundaries, we mornalize the scale modifier based on the interval 0 - modifiedEffectRadius

                    distanceFromController = distanceFromController - normalizedScaleModifier;

                    if (distanceFromController <= this.controller.modifiendEffectRadius + normalizedScaleModifier) 
                    {
                        var newDistance = distanceFromController / 2f - (this.controller.modifiendEffectRadius + normalizedScaleModifier) / 2f; // TODO: all distance changes shoul duse this.viscocity
                        var newPos = Vector3.MoveTowards(this.originalPosition, this.controller.transform.position, newDistance);
                        newPos = Vector3.MoveTowards(this.transform.position, newPos, this.controller.viscocity * this.controller.defaultEffectRadius);
                        //this.transform.position = newPos;
                        this.transform.position = Vector3.MoveTowards(this.transform.position, newPos, this.controller.viscocity * this.controller.defaultEffectRadius);
                    }
                    else
                    {
                        this.transform.position = Vector3.MoveTowards(this.transform.position, this.originalPosition, this.controller.viscocity * this.controller.defaultEffectRadius);
                    }
                }
                else
                {
                    this.transform.position = Vector3.MoveTowards(this.transform.position, this.controller.transform.position, this.controller.viscocity * this.controller.defaultEffectRadius);
                }
            }
            else if (!controller?.IsEffectOn() ?? false)
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, this.originalPosition, this.controller.viscocity * this.controller.defaultEffectRadius);
            }
            else
            {

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

    public void MoveOriginalPosition(Vector3 posDelta, Quaternion newRotation)
    {
        this.originalPosition = this.originalPosition - posDelta;
        this.transform.position = this.transform.position - posDelta;
        this.transform.rotation = newRotation;
    }

    public void SetOriginalPosition(Vector3 pos, Quaternion newRotation)
    {
        var posDelta = this.originalPosition - pos;
        this.originalPosition = pos;
        if (!this.isHighlighted) // position will already be set if we are highlighted so we don't need to worry about this
        {
            this.transform.position = this.transform.position - posDelta;
        }
        this.transform.rotation = newRotation;
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

    public bool IsGrabbed()
    {
        return this.isGrabbed;
    }

    public void Select()
    {
        var outline = this.GetComponent<Outline>();
        outline.enabled = true;
    }

    public void Deselect()
    {
        var outline = this.GetComponent<Outline>();
        outline.enabled = false;
    }
}
