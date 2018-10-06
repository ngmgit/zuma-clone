using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLauncher : MonoBehaviour
{
	public GameObject dummyBall;
	public float ballSpeed = 10;
	public GameObject instanceBall;

	private Vector3 lookPos;

	private void Start()
	{
		CreateBall();
	}

	// Update is called once per frame
	private void Update ()
	{
		RotatePlayerAlongMousePosition();
		SetBallPostion();

	}

 	private void FixedUpdate()
	{
		ShootBall();
	}

	// Rotate the launcher along the mouse position
	private void RotatePlayerAlongMousePosition ()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, Camera.main.transform.position.y))
			lookPos = hit.point;

		Vector3 lookDir = lookPos - transform.position;
		lookDir.y = 0;

		transform.LookAt (transform.position + lookDir, Vector3.up);
	}

	// Set balls postions and forward w.r.t to the launcher
	private void SetBallPostion()
	{
		instanceBall.transform.forward = transform.forward;
		instanceBall.transform.position = transform.position + transform.forward * transform.localScale.z;
	}

	private void ShootBall()
	{
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			instanceBall.GetComponent<Rigidbody>().AddForce(instanceBall.transform.forward * ballSpeed);
			CreateBall();
		}
	}

	private void CreateBall()
	{
		instanceBall = Instantiate(dummyBall, transform.position, Quaternion.identity);
		instanceBall.SetActive(true);

		instanceBall.tag = "NewBall";
		instanceBall.gameObject.layer = LayerMask.NameToLayer("Default");

		SetBallColor(instanceBall);
	}

	private void SetRandomColor(GameObject go)
	{
		Color color = new Color(Random.Range(0F,1F), Random.Range(0, 1F), Random.Range(0, 1F));
		go.GetComponent<Renderer>().material.SetColor("_Color", color);
	}

	private void SetBallColor(GameObject go)
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
		}
	}
}
