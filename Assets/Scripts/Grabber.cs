using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabber : MonoBehaviour
{
    public float effectRadius; // needs to be smaller than collider radius

    private List<Grabbable> Grabbables = new List<Grabbable>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Grabbable closestGrabbable = null;
        var closestDistance = 0f;
        foreach (var grabbable in Grabbables)
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
            closestGrabbable?.Highlight();
        }
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
}
