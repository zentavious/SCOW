using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WIM : MonoBehaviour
{
    public GameObject parentWIMObject;
    private List<GameObject> miniObjects;
    private Vector3 lastPos;
    // Start is called before the first frame update
    void Start()
    {
        // this.parentWIMObject.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + .05f, this.transform.position.z);
        this.lastPos = this.transform.position;
        this.miniObjects = new List<GameObject>();
        int i = 0;
        foreach (Transform child in this.parentWIMObject.transform) // TODO: we cheat here by getting the relative position of every object here at the start. This will need to be done later on cast
        {
            this.miniObjects.Add(child.gameObject);
            var gameObject = new GameObject($"Object_{i}");
            gameObject.transform.position = child.gameObject.transform.position;
            Debug.Log($"Created game object {gameObject.name}");
            gameObject.transform.parent = this.transform;
            i++;
        }
        Debug.Log($"There are {i} children");
    }

    // Update is called once per frame
    void Update()
    {
        var deltaPos = this.lastPos - this.transform.position;
        foreach (Transform child in this.transform)
        {
            var name = child.name.Split('_');
            if (name[0].Equals("Object"))
            {
                var index = int.Parse(child.name.Split('_')[1]);
                var grabbable = miniObjects[index].GetComponent<Grabbable>();
                if (grabbable)
                {
                    if (!grabbable.IsGrabbed())
                    {
                        grabbable.SetOriginalPosition(child.transform.position, child.transform.rotation);
                    }
                }
                else
                {
                    miniObjects[index].transform.position = child.transform.position;
                    miniObjects[index].transform.rotation = child.transform.rotation;
                }
            }
        }

        this.lastPos = this.transform.position;
    }

}
