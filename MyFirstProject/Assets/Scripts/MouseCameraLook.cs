using UnityEngine;
using System.Collections;

public class MouseCameraLook : MonoBehaviour {

    public enum RotationAxes {
        MouseXAndY = 0,
        MouseX = 1,
        MouseY = 2
    }

    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityHor = 9.0f;
    public float sensitivityVert = 9.0f;
    public float minimumVert = -60.0f;
    public float maximumVert = 60.0f;
    private float rotationX = 0;
    
    void Start() {
        
        Rigidbody body = GetComponent<Rigidbody>();
    
        if (body != null)
            body.freezeRotation = true;
        }

    void Update() {

        rotationX -= Input.GetAxis("Mouse Y") * sensitivityVert;
        rotationX = Mathf.Clamp(rotationX, minimumVert, maximumVert);

        float rotationY = transform.localEulerAngles.y;

        transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);

        /*
        if (axes == RotationAxes.MouseY) {
            rotationX -= Input.GetAxis("Mouse Y") * sensitivityVert;
            rotationX = Mathf.Clamp(rotationX, minimumVert, maximumVert);
            
            float rotationY = transform.localEulerAngles.y;
            
            transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);
        }
        */
    }
}
