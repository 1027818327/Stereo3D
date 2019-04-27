using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class DragRigidbody : MonoBehaviour
{
    public float spring = 50.0f;
    public float damper = 5.0f;
    public float drag = 10.0f;
    public float angularDrag = 5.0f;
    public float distance = 0.2f;
    public bool attachToCenterOfMass = false;
    private SpringJoint springJoint;

    public void Update()
    {
        RaycastHit hit = default(RaycastHit);
         // Make sure the user pressed the mouse down
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }
        Camera mainCamera = this.FindCamera();
        // We need to actually hit an object
        if (!Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, 100))
        {
            return;
        }
        // We need to hit a rigidbody that is not kinematic
        if (!hit.rigidbody || hit.rigidbody.isKinematic)
        {
            return;
        }
        if (!this.springJoint)
        {
            GameObject go = new GameObject("Rigidbody dragger");
            Rigidbody body = go.AddComponent<Rigidbody>();
            this.springJoint = go.AddComponent<SpringJoint>();
            body.isKinematic = true;
        }
        this.springJoint.transform.position = hit.point;
        if (this.attachToCenterOfMass)
        {
            Vector3 anchor = this.transform.TransformDirection(hit.rigidbody.centerOfMass) + hit.rigidbody.transform.position;
            anchor = this.springJoint.transform.InverseTransformPoint(anchor);
            this.springJoint.anchor = anchor;
        }
        else
        {
            this.springJoint.anchor = Vector3.zero;
        }
        this.springJoint.spring = this.spring;
        this.springJoint.damper = this.damper;
        this.springJoint.maxDistance = this.distance;
        this.springJoint.connectedBody = hit.rigidbody;
        this.StartCoroutine("DragObject", hit.distance);
    }

    public IEnumerator DragObject(float distance)
    {
        float oldDrag = this.springJoint.connectedBody.drag;
        float oldAngularDrag = this.springJoint.connectedBody.angularDrag;
        this.springJoint.connectedBody.drag = this.drag;
        this.springJoint.connectedBody.angularDrag = this.angularDrag;
        Camera mainCamera = this.FindCamera();
        while (Input.GetMouseButton(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            this.springJoint.transform.position = ray.GetPoint(distance);
            yield return null;
        }
        if (this.springJoint.connectedBody)
        {
            this.springJoint.connectedBody.drag = oldDrag;
            this.springJoint.connectedBody.angularDrag = oldAngularDrag;
            this.springJoint.connectedBody = null;
        }
    }

    public Camera FindCamera()
    {
        var tempCamera = GetComponent<Camera>();
        if (tempCamera)
        {
            return tempCamera;
        }
        else
        {
            return Camera.main;
        }
    }
}