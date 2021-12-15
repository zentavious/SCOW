using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SphereCast : MonoBehaviour
{
    public InputActionProperty selectAction;
    public WIM worldsInMiniature;

    private List<GameObject> objects;

    // Start is called before the first frame update
    void Start()
    {
        this.objects = new List<GameObject>();

        this.selectAction.action.performed += Cast;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        var grabbable = other.GetComponent<Grabbable>();
        if (grabbable)
        {
            this.objects.Add(other.gameObject);
        }
    }
    void OnTriggerExit(Collider other)
    {
        var grabbable = other.GetComponent<Grabbable>();
        if (grabbable)
        {
            this.objects.Remove(other.gameObject);
        }
    }

    public void Cast(InputAction.CallbackContext context)
    {
        Debug.Log("trigger pressed");
        if (this.gameObject.GetComponent<SphereCollider>().enabled)
        {
            this.gameObject.GetComponent<SphereCollider>().enabled = false;
            this.gameObject.GetComponent<Renderer>().enabled = true;
            Vector3 averagePos = Vector3.zero;
            foreach (var gameObject in this.objects)
            {
                averagePos = new Vector3(averagePos.x + gameObject.transform.position.x / objects.Count, averagePos.y + gameObject.transform.position.y / objects.Count, averagePos.z + gameObject.transform.position.z / objects.Count);
            }

            var castContainer = new GameObject("Cast Container");
            castContainer.transform.position = averagePos;
            castContainer.transform.rotation = Quaternion.Euler(Vector3.zero);
            foreach (var gameObject in this.objects)
            {
                gameObject.transform.parent = castContainer.transform;
            }

            this.worldsInMiniature.CastObjectsToWIM(castContainer);
        }
    }
}
