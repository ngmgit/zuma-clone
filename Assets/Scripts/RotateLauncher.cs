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
			Color color = new Color(Random.Range(0F,1F), Random.Range(0, 1F), Random.Range(0, 1F));
			GameObject go  = Instantiate(dummyBall, transform.position, Quaternion.identity);
			go.GetComponent<Renderer>().material.SetColor("_Color", color);
			go.tag = "NewBall";
			go.SetActive(true);
			go.gameObject.layer = LayerMask.NameToLayer("Default");
			go.transform.position = transform.position;
			go.transform.forward = transform.forward;
			go.GetComponent<Rigidbody>().AddForce(go.transform.forward * ballSpeed);
		}
	}
}
