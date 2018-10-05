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
	private List<int> stopPositions;
	private int headballIndex;
	public SortedDictionary<int, int> ballSections;

	// Use this for initialization
	private void Start ()
	{
		headballIndex = 0;

		bgCurve = GetComponent<BGCurve>();
		ballList = new List<GameObject>();

		ballsContainerGO = new GameObject();
		ballsContainerGO.name = "Balls Container";

		removedBallsContainer = new GameObject();
		removedBallsContainer.name = "Removed Balls Container";

		for (int i=0; i < ballCount; i++)
			CreateNewBall();

		ballSections = new SortedDictionary<int, int>();
		ballSections.Add(int.MaxValue, 0);
	}

	// Update is called once per frame
	private void Update ()
	{
		if (ballList.Count > 0)
			MoveAllBallsAlongPath();

		if (headballIndex != 0)
			JoinStoppedSections(headballIndex, headballIndex - 1);
	}

	/*
	 * Public Section
	 * =============
	 */
	public void AddNewBallAt(GameObject go, int index)
	{
		if (index > ballList.Count)
			ballList.Add(go);
		else
			ballList.Insert(index, go);

		go.transform.parent = ballsContainerGO.transform;
		go.transform.SetSiblingIndex(index);

		if (index < headballIndex)
			headballIndex++;

		// adjust distance  for added ball
		OnAddModifySections(index, 1);
		RemoveMatchedBalls(index, go);
	}

	public static BallColor GetRandomBallColor()
	{
		int rInt = Random.Range(0, 3);
		return (BallColor)rInt;
	}

	/*
	 * Private Section
	 * =============
	 */
	private void MoveAllBallsAlongPath()
	{
		Vector3 tangent;
		int movingBallCount = 1;
		distance += pathSpeed * Time.deltaTime;

		// use a head index value which leads the balls on the path
		// This value will be changed when balls are delected from the path
		ballList[headballIndex].transform.position = GetComponent<BGCcMath>().CalcPositionAndTangentByDistance(distance, out tangent);
		ballList[headballIndex].transform.rotation = Quaternion.LookRotation(tangent);

		if (!ballList[headballIndex].activeSelf)
			ballList[headballIndex].SetActive(true);

		for (int i = headballIndex + 1; i < ballList.Count; i++)
		{
			float currentBallDist = distance - movingBallCount * greenBall.transform.localScale.x;
			ballList[i].transform.position = GetComponent<BGCcMath>().CalcPositionAndTangentByDistance(currentBallDist , out tangent);
			ballList[i].transform.rotation = Quaternion.LookRotation(tangent);

			if (!ballList[i].activeSelf)
				ballList[i].SetActive(true);

			movingBallCount++;
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
		}
	}

	private void InstatiateBall(GameObject ballGameObject)
	{
		GameObject go = Instantiate(ballGameObject,  bgCurve[0].PositionWorld, Quaternion.identity, ballsContainerGO.transform);
		go.SetActive(false);
		ballList.Add(go.gameObject);
	}

	// Join the sections which were divided when balls were removed
	// Just check the current head with the next value if they are close
	private void JoinStoppedSections(int currentIdx, int nextSectionIdx)
	{
		float nextSecdist;
		GetComponent<BGCcMath>().CalcPositionByClosestPoint(ballList[nextSectionIdx].transform.position, out nextSecdist);
		if (nextSecdist - distance <= blueBall.transform.localScale.x)
		{
			int nextSectionKeyVal;
			ballSections.TryGetValue(nextSectionIdx, out nextSectionKeyVal);
			headballIndex = nextSectionKeyVal;
			MergeSections(currentIdx, nextSectionKeyVal);
			RemoveMatchedBalls(nextSectionIdx, ballList[nextSectionIdx]);

			if (ballList.Count > 0)
			{
				GetComponent<BGCcMath>().CalcPositionByClosestPoint(ballList[headballIndex].transform.position, out nextSecdist);
				distance = nextSecdist;
			}
		}
	}

	private void MergeSections(int currentIdx, int nextSectionKeyVal)
	{
		ballSections.Remove(currentIdx - 1);
		ballSections[int.MaxValue] = nextSectionKeyVal;
	}

	// Called by the collided new ball on collision with the active balls on the path
	private void RemoveMatchedBalls(int index, GameObject go)
	{
		int front = index;
		int back = index;

		Color ballColor = go.GetComponent<Renderer>().material.GetColor("_Color");

		int sectionKey = GetSectionKey(index);
		int sectionKeyVal;
		ballSections.TryGetValue(sectionKey, out sectionKeyVal);

		// Check if any same color balls towards the front side
		for (int i = index - 1; i >= sectionKeyVal; i--)
		{
			Color currrentBallColor = ballList[i].GetComponent<Renderer>().material.GetColor("_Color");
			if(ballColor == currrentBallColor)
				front = i;
			else
				break;
		}

		// Check if any same color balls towards the back side
		int end  = sectionKey == int.MaxValue ? ballList.Count - 1: sectionKey;
		for (int i = index + 1; i <= end ; i++)
		{
			Color currrentBallColor = ballList[i].GetComponent<Renderer>().material.GetColor("_Color");
			if(ballColor == currrentBallColor)
				back = i;
			else
				break;
		}

		// If atleast 3 balls in a row are found at the new balls position
		if (back - front >= 2)
		{
			// Modify the headballIndex only if the remove section is in the moving section
			if (back > headballIndex)
			{
				// whole back section will be removed change the headIndex to the front section value
				if (front == sectionKeyVal && back == ballList.Count - 1)
				{
					if (ballSections.Count > 1)
					{
						int nextSectionFront;
						ballSections.TryGetValue(front - 1, out nextSectionFront);
						headballIndex = nextSectionFront;
					}
				}
				// if the remove section is less that the back i.e front and middle part of the moving section
				else
				{
					if(front >= sectionKeyVal && back != ballList.Count - 1)
					{
						headballIndex = front;
					}
				}
			}
			else
			{

				headballIndex -= (back - front + 1);
			}

			Debug.Log("HEAD INDEX:" + headballIndex);

			RemoveBalls(front, back - front + 1);

			if (back > headballIndex)
				GetComponent<BGCcMath>().CalcPositionByClosestPoint(ballList[headballIndex].transform.position, out distance);
		}
	}

	// Remove balls from the list also from the ballsContainer in scene
	private void RemoveBalls(int atIndex, int range)
	{
		for (int i = 0; i < range; i++)
		{
			ballList[atIndex + i].transform.parent = removedBallsContainer.transform;
			ballList[atIndex + i].SetActive(false);
		}
		ballList.RemoveRange(atIndex, range);

		OnDeleteModifySections(atIndex, range);
	}

	private int GetSectionKey(int front)
	{
		int key = int.MaxValue;

		foreach(KeyValuePair<int, int> entry in ballSections)
		{
			if (front >= entry.Value && front <= entry.Key)
				key =  entry.Key;
		}

		return key;
	}

	private void OnAddModifySections(int atIndex, int range)
	{
		List<KeyValuePair<int, int>> modSectionList = new List<KeyValuePair<int, int>>();
		int sectionKey = GetSectionKey(atIndex);

		int sectionKeyVal;
		ballSections.TryGetValue(sectionKey, out sectionKeyVal);

		// Range will be 1 only when a new ball is added
		// else it will be >= 3 when balls are to be removed
		if (sectionKey != int.MaxValue) {
			int newSectionKey = sectionKey + 1;
			ballSections.Add(newSectionKey, sectionKeyVal);
			ballSections.Remove(sectionKey);

			// Get the keys which are to be updated
			// Since changing the value in the loop itself will add new entries which are always greater than the current one
			// There by looping it infinitely and incorrect
			foreach (KeyValuePair<int, int> entry in ballSections)
			{
				if (entry.Key > newSectionKey)
					modSectionList.Add(entry);
			}

			// Update the sections seperatly to avoid wrong calulation when done in above loop
			// For all the value other than the end of chain modifiy both their key and value
			// For the end of chain section only update the value i.e its front
			foreach(KeyValuePair<int, int> entry in modSectionList)
			{
				if (entry.Key != int.MaxValue)
				{
					if (entry.Value == 0)
						ballSections.Add(entry.Key + 1, entry.Value);
					else
						ballSections.Add(entry.Key + 1, entry.Value + 1);

					ballSections.Remove(entry.Key);
				}
				else
					ballSections[entry.Key] = entry.Value + 1;
			}
		}
	}

	private void OnDeleteModifySections(int atIndex, int range)
	{
		int sectionKey = GetSectionKey(atIndex);

		int sectionKeyVal;
		ballSections.TryGetValue(sectionKey, out sectionKeyVal);

		// completely remove the section
		if (atIndex == sectionKeyVal && atIndex + range == ballList.Count + range)
		{
			DeleteEntireSection(atIndex, range, sectionKey);
		}
		else
		{
			DeletePartialSection(atIndex, range, sectionKey, sectionKeyVal);
		}
	}

	private void DeleteEntireSection(int atIndex, int range, int sectionKey)
	{
		List<KeyValuePair<int, int>> modSectionList = new List<KeyValuePair<int, int>>();

		// If section is last but one i.e before the moving section
		if (atIndex + range != ballList.Count + range)
		{
			Debug.Log("Entire: ");
			// modify the back values for the immediate next section.
			// modify the back and front values for other higher ones
			ballSections.Remove(sectionKey);

			foreach (KeyValuePair<int, int> entry in ballSections)
			{
				if (entry.Key > sectionKey)
					modSectionList.Add(entry);
			}

			ballSections.Add(modSectionList[0].Key - range, atIndex);
			ballSections.Remove(modSectionList[0].Key);
			modSectionList.RemoveAt(0);

			foreach(KeyValuePair<int, int> entry in modSectionList)
			{
				if (entry.Key != int.MaxValue)
				{
					ballSections.Add(entry.Key - range, entry.Value - range);
					ballSections.Remove(entry.Key);
				}
				else
					ballSections[entry.Key] = entry.Value - range;
			}
		}
		else
		{
			KeyValuePair<int, int> getLastButOne = new KeyValuePair<int, int>();

			if (ballSections.Count > 1)
			{
				foreach (KeyValuePair<int, int> entry in ballSections)
				{
					if (entry.Key < sectionKey)
						getLastButOne = entry;
				}

				ballSections.Remove(getLastButOne.Key);
				ballSections[int.MaxValue] = getLastButOne.Value;
			}
		}
	}

	private void DeletePartialSection(int atIndex, int range, int sectionKey, int sectionKeyVal)
	{
		List<KeyValuePair<int, int>> modSectionList = new List<KeyValuePair<int, int>>();

		// Handle cases
		// when delection takes place at the front or back of the section
		int end = sectionKey == int.MaxValue? ballList.Count + range - 1: sectionKey;
		if (atIndex == sectionKeyVal || atIndex + range - 1 == end)
		{
			Debug.Log("Partial: Front/back");
			if (sectionKey == int.MaxValue)
				return;

			int newSectionKey = sectionKey - range;
			ballSections.Add(newSectionKey, sectionKeyVal);
			ballSections.Remove(sectionKey);

			foreach (KeyValuePair<int, int> entry in ballSections)
			{
				if (entry.Key > newSectionKey)
					modSectionList.Add(entry);
			}
		}
		// when delection takes place in middle of the section which creates a new section
		else
		{
			Debug.Log("Partial: Middle");
			// new section front
			int newSectionKey = atIndex - 1;
			ballSections.Add(newSectionKey, sectionKeyVal);

			if (sectionKey != int.MaxValue)
			{
				int nextSectionKey = sectionKey - range;

				ballSections.Remove(sectionKey);

				// new section back
				ballSections.Add( nextSectionKey, atIndex);

				foreach (KeyValuePair<int, int> entry in ballSections)
				{
					if (entry.Key > nextSectionKey)
						modSectionList.Add(entry);
				}
			}
			else
			{
				ballSections[int.MaxValue] = atIndex;
				return;
			}
		}

		// Sort keys just to avoid bug due if a lower key is modified before higher key
		modSectionList.Sort((entryA, entryB) => entryA.Key.CompareTo(entryB.Key));


		// Get the keys to be changed and modify them
		foreach(KeyValuePair<int, int> entry in modSectionList)
		{
			if (entry.Key != int.MaxValue)
			{
				if (entry.Value == 0)
					ballSections.Add(entry.Key - range, entry.Value);
				else
					ballSections.Add(entry.Key - range, entry.Value - range);

				ballSections.Remove(entry.Key);
			}
			else
				ballSections[entry.Key] = entry.Value - range;
		}
	}
}