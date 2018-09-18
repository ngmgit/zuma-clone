using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BansheeGz.BGSpline.Components;
using BansheeGz.BGSpline.Curve;

public class MoveBalls : MonoBehaviour
{
	public GameObject redBall;
	public GameObject greenBall;
	public GameObject blueBall;
	public GameObject yellowBall;

	public int ballCount;

	[SerializeField]
	private List<GameObject> ballList;
	private GameObject ballsContainerGO;

	private BGCurve bgCurve;
	private float distance;

	// Use this for initialization
	void Start ()
	{

		bgCurve = GetComponent<BGCurve>();
		ballList = new List<GameObject>();
		ballsContainerGO = new GameObject();
		ballsContainerGO.name = "Balls Container";

		for (int i=0; i < ballCount; i++)
			CreateNewBall();
	}

	// Update is called once per frame
	void Update ()
	{
		Vector3 tangent;
		distance += 5 * Time.deltaTime;

		ballList[0].transform.position = GetComponent<BGCcMath>().CalcPositionAndTangentByDistance(distance, out tangent);
		ballList[0].transform.rotation = Quaternion.LookRotation(tangent);
		ballList[0].SetActive(true);

		for (int i = 1; i < ballCount; i++)
		{
			// update ball position on the path
			float currentBallDist = distance - i * greenBall.transform.localScale.x;
			ballList[i].transform.position = GetComponent<BGCcMath>().CalcPositionAndTangentByDistance(currentBallDist , out tangent);
			ballList[i].transform.rotation = Quaternion.LookRotation(tangent);
			ballList[i].SetActive(true);
		}
	}

	void CreateNewBall()
	{
		int rInt = Random.Range(0, 3);

		switch (rInt)
		{
			case 0:
				InstatiateBall(redBall);
				break;

			case 1:
				InstatiateBall(greenBall);
				break;

			case 2:
				InstatiateBall(blueBall);
				break;

			case 3:
				InstatiateBall(yellowBall);
				break;

			default:
				break;
		}
	}

	void InstatiateBall(GameObject ballGameObject)
	{
		GameObject go = Instantiate(ballGameObject,  bgCurve[0].PositionWorld, Quaternion.identity, ballsContainerGO.transform);
		go.SetActive(false);
		ballList.Add(go.gameObject);
	}
}
