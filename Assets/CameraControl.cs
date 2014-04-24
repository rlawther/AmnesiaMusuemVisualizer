using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

    public float _rotationSpeed = 1.0f;
    public float _moveSpeed = 4.0f; 

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.A))
        {
            Vector3 mainCameraDirection = Vector3.right;
            mainCameraDirection = mainCameraDirection * -_moveSpeed;
            this.transform.Translate(mainCameraDirection);
        }
        if (Input.GetKey(KeyCode.D))
        {
			Vector3 mainCameraDirection = Vector3.right;
            mainCameraDirection = mainCameraDirection * _moveSpeed;
            this.transform.Translate(mainCameraDirection);
        }
        if (Input.GetKey(KeyCode.S))
        {
			Vector3 mainCameraDirection = Vector3.forward;
            mainCameraDirection = mainCameraDirection * -_moveSpeed;
            this.transform.Translate(mainCameraDirection);
        }
        if (Input.GetKey(KeyCode.W))
        {            
			Vector3 mainCameraDirection = Vector3.forward;
            mainCameraDirection = mainCameraDirection * _moveSpeed; 
            this.transform.Translate(mainCameraDirection);
        }
		if (Input.GetKey(KeyCode.Z))
		{
			this.transform.Translate(Vector3.up * -_moveSpeed);
		}
		if (Input.GetKey(KeyCode.X))
		{
			this.transform.Translate(Vector3.up * _moveSpeed);
		}
		if (Input.GetKey(KeyCode.Q))
		{
			this.transform.Rotate(Vector3.up, -_rotationSpeed);
		}
		if (Input.GetKey(KeyCode.E))
        {
            this.transform.Rotate(Vector3.up, _rotationSpeed);
        }
	}
}
