using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BansheeGz.BGSpline.Components;
using BansheeGz.BGSpline.Curve;


public struct ActiveBallList
{
	List<GameObject> ballList;
	bool isMoving;
	bool isInTransition;
}

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
	private List<ActiveBallList> activeBallList;
	private List<int> stopPositions;

	// Use this for initialization
	private void Start ()
	{
		bgCurve = GetComponent<BGCurve>();
		ballList = new List<GameObject>();
		activeBallList = new List<ActiveBallList>();

		ballsContainerGO = new GameObject();
		ballsContainerGO.name = "Balls Container";

		for (int i=0; i < ballCount; i++)
			CreateNewBall();
	}

	// Update is called once per frame
	private void Update ()
	{
		MoveAllBallsAlongPath();
	}

	private void MoveAllBallsAlongPath()
	{
		Vector3 tangent;
		distance += 5 * Time.deltaTime;

		ballList[0].transform.position = GetComponent<BGCcMath>().CalcPositionAndTangentByDistance(distance, out tangent);
		ballList[0].transform.rotation = Quaternion.LookRotation(tangent);

		if (!ballList[0].activeSelf)
			ballList[0].SetActive(true);

		for (int i = 1; i < ballList.Count; i++)
		{
			float currentBallDist = distance - i * greenBall.transform.localScale.x;
			ballList[i].transform.position = GetComponent<BGCcMath>().CalcPositionAndTangentByDistance(currentBallDist , out tangent);
			ballList[i].transform.rotation = Quaternion.LookRotation(tangent);

			if (!ballList[i].activeSelf)
				ballList[i].SetActive(true);
		}
	}

	private void CreateNewBall()
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

	private void InstatiateBall(GameObject ballGameObject)
	{
		GameObject go = Instantiate(ballGameObject,  bgCurve[0].PositionWorld, Quaternion.identity, ballsContainerGO.transform);
		go.SetActive(false);
		ballList.Add(go.gameObject);
	}

	public void AddNewBallAt(GameObject go, int index)
	{
		Debug.Log(index);
		if (index < 0)
		{
			int idx = 0;
			ballList.Insert(idx, go);
			SetBallIdxInContainer(go, idx);
			return;
		}

		if (index > ballList.Count)
		{
			int idx = ballList.Count + 1;
			ballList.Add(go);
			SetBallIdxInContainer(go, idx);
			return;
		}

		go.transform.parent = ballsContainerGO.transform;
		ballList.Insert(index, go);
		SetBallIdxInContainer(go, index);
	}

	private void SetBallIdxInContainer(GameObject go, int idx)
	{
		go.transform.parent = ballsContainerGO.transform;
		go.transform.SetSiblingIndex(idx);
	}
}
