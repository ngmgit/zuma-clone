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
			GameObject go  = Instantiate(dummyBall, transform.position, Quaternion.identity);
			go.SetActive(true);

			go.tag = "NewBall";
			go.gameObject.layer = LayerMask.NameToLayer("Default");

			go.transform.position = transform.position;
			go.transform.forward = transform.forward;

			SetBallColor(go);
			go.GetComponent<Rigidbody>().AddForce(go.transform.forward * ballSpeed);
		}
	}

	void SetRandomColor(GameObject go)
	{
		Color color = new Color(Random.Range(0F,1F), Random.Range(0, 1F), Random.Range(0, 1F));
		go.GetComponent<Renderer>().material.SetColor("_Color", color);
	}

	void SetBallColor(GameObject go)
	{
		BallColor randColor = MoveBalls.GetRandomBallColor();

		switch (randColor)
		{
			case BallColor.red:
				go.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
				break;

			case BallColor.green:
				go.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
				break;

			case BallColor.blue:
				go.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
				break;

			case BallColor.yellow:
				go.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
				break;

			default:
				break;
		}
	}
}
