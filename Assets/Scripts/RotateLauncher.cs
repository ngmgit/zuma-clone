using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLauncher : MonoBehaviour
{
	public GameObject dummyBall;
	public float ballSpeed = 10;

	private Vector3 lookPos;

	// Update is called once per frame
	void Update ()
	{
		RotatePlayerAlongMousePosition();
	}

 	void FixedUpdate()
	{
		ShootBall();
	}

	void RotatePlayerAlongMousePosition ()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, Camera.main.transform.position.y))
			lookPos = hit.point;

		Vector3 lookDir = lookPos - transform.position;
		lookDir.y = 0;

		transform.LookAt (transform.position + lookDir, Vector3.up);
	}

	void ShootBall()
	{
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			dummyBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
			dummyBall.transform.position = transform.position;
			dummyBall.transform.forward = transform.forward;
			dummyBall.GetComponent<Rigidbody>().AddForce(dummyBall.transform.forward * ballSpeed);
		}
	}
}
