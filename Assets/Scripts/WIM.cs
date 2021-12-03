using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WIM : MonoBehaviour
{
    public GameObject parentWIMObject;
    private Vector3 lastPos;
    // Start is called before the first frame update
    void Start()
    {
        // this.parentWIMObject.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + .05f, this.transform.position.z);
        this.lastPos = this.transform.position;
        int i = 0;
        foreach (Transform Child in this.parentWIMObject.transform)
        {
            i++;
        }
        Debug.Log($"There are {i} children");
    }

    // Update is called once per frame
    void Update()
    {
        var deltaPos = this.lastPos - this.transform.position;
        foreach (Transform Child in this.parentWIMObject.transform)
        {
            var grabbable = Child.gameObject.GetComponent<Grabbable>();
            if (grabbable)
            {
                var orignalPos = grabbable.GetOriginalPosition();
                
                grabbable.MoveOrignalPosition(deltaPos, new Quaternion());

            }
        }

        this.lastPos = this.transform.position;
    }
}
