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

public enum BallColor
{
	red,
	green,
	blue,
	yellow
}

public class MoveBalls : MonoBehaviour
{
	public GameObject redBall;
	public GameObject greenBall;
	public GameObject blueBall;
	public GameObject yellowBall;

	public float pathSpeed;
	public int ballCount;
	[SerializeField]
	private List<GameObject> ballList;
	private GameObject ballsContainerGO;
	private GameObject removedBallsContainer;

	private BGCurve bgCurve;
	private float distance;
	private List<ActiveBallList> activeBallList;
	private List<int> stopPositions;
	private int headballIndex;

	// Use this for initialization
	private void Start ()
	{
		headballIndex = 0;

		bgCurve = GetComponent<BGCurve>();
		ballList = new List<GameObject>();
		activeBallList = new List<ActiveBallList>();

		ballsContainerGO = new GameObject();
		ballsContainerGO.name = "Balls Container";

		removedBallsContainer = new GameObject();
		removedBallsContainer.name = "Removed Balls Container";

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
		distance += pathSpeed * Time.deltaTime;

		ballList[headballIndex].transform.position = GetComponent<BGCcMath>().CalcPositionAndTangentByDistance(distance, out tangent);
		ballList[headballIndex].transform.rotation = Quaternion.LookRotation(tangent);

		if (!ballList[headballIndex].activeSelf)
			ballList[headballIndex].SetActive(true);

		for (int i = headballIndex + 1; i < ballList.Count; i++)
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
		switch (GetRandomBallColor())
		{
			case BallColor.red:
				InstatiateBall(redBall);
				break;

			case BallColor.green:
				InstatiateBall(greenBall);
				break;

			case BallColor.blue:
				InstatiateBall(blueBall);
				break;

			case BallColor.yellow:
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

		if (index > ballList.Count)
			ballList.Add(go);
		else
			ballList.Insert(index, go);

		go.transform.parent = ballsContainerGO.transform;
		go.transform.SetSiblingIndex(index);

		RemoveMatchedBalls(index, go);
	}

	private void RemoveMatchedBalls(int index, GameObject go)
	{
		int front = index;
		int back = index;

		Color ballColor = go.GetComponent<Renderer>().material.GetColor("_Color");

		for (int i = index; i > 0 ; i--)
		{
			Color currrentBallColor = ballList[i].GetComponent<Renderer>().material.GetColor("_Color");
			if(ballColor == currrentBallColor)
				front = i;
			else
				break;
		}

		for (int i = index; i < ballList.Count ; i++)
		{
			Color currrentBallColor = ballList[i].GetComponent<Renderer>().material.GetColor("_Color");
			if(ballColor == currrentBallColor)
				back = i;
			else
				break;
		}

		if (back - front > 3)
		{
			if (!(back == ballList.Count - 1 || front == 0))
			{
				headballIndex = front;
				distance = distance - (back+1) * blueBall.transform.localScale.x;
			}

			RemoveBalls(front, back - front + 1);
		}
	}

	private void RemoveBalls(int atIndex, int range)
	{
		for (int i = 0; i < range; i++)
		{
			ballList[atIndex + i].transform.parent = removedBallsContainer.transform;
			ballList[atIndex + i].SetActive(false);
		}

		ballList.RemoveRange(atIndex, range);
	}

	public static BallColor GetRandomBallColor()
	{
		int rInt = Random.Range(0, 3);
		return (BallColor)rInt;
	}
}
