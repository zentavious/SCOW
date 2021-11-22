using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    public float effectRadius; // needs to be smaller than collider radius
    public float viscocity; // propotion of effectRadius to use a movement inside collider

    private Vector3 originalPosition;
    private GameObject controller;
    private bool isHighlighted;

    // Start is called before the first frame update
    void Start()
    {
        isHighlighted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (controller)
        {
            if (!isHighlighted)
            {
                if (Vector3.Distance(this.originalPosition, this.controller.transform.position) <= effectRadius)
                {
                    Guid x = Guid.NewGuid();
                    var distanceFromController = Vector3.Distance(this.originalPosition, this.controller.transform.position);
                    var newDistance = distanceFromController / 2f - effectRadius / 2f; // TODO: needs to factor in size of selected object
                    Debug.Log($"{x} - dist: {newDistance}");
                    var newPos = Vector3.MoveTowards(this.originalPosition, this.controller.transform.position, newDistance);
                    newPos = Vector3.MoveTowards(this.transform.position, newPos, this.viscocity * this.effectRadius);
                    Debug.Log($"{x} - newPos: {newPos}");
                    this.transform.position = newPos;
                }
                else
                {
                    //this.transform.position = this.originalPosition;
                    this.transform.position = Vector3.MoveTowards(this.transform.position, this.originalPosition, this.viscocity * this.effectRadius);
                }
            }
            else
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, this.controller.transform.position, this.viscocity * this.effectRadius);
            }
        }
    }

    void OnTriggerEnter(Collider other) 
    {
        if (other.GetComponent<Grabber>())
        {
            this.originalPosition = this.transform.position;
            this.controller = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Grabber>())
        {
            this.transform.position = this.originalPosition;
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
    public Vector3 GetOriginalPosition()
    {
        return this.originalPosition;
    }
}
