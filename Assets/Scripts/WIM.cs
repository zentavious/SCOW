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

        this.CastObjectsToWIM(parentWIMObject); // cheat until we actually start casting

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

    public void CastObjectsToWIM(GameObject parentWIMObject)
    {
        int i = 0;
        this.parentWIMObject = new GameObject("Parent Container");

        this.parentWIMObject.transform.position = this.transform.position;
        foreach (Transform child in parentWIMObject.transform)
        {

            var clone = GameObject.Instantiate(child.gameObject);
            clone.transform.parent = this.parentWIMObject.transform;
            clone.transform.localPosition = new Vector3(child.transform.localPosition.x, child.transform.localPosition.y, child.transform.localPosition.z);

            this.miniObjects.Add(clone);

            var gameObject = new GameObject($"Object_{i}");
            gameObject.transform.parent = this.transform;
            gameObject.transform.localPosition = new Vector3(child.transform.localPosition.x, child.transform.localPosition.y, child.transform.localPosition.z);
            Debug.Log(gameObject.transform.position);

            i++;
        }
    }

}
