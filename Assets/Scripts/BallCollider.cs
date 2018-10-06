using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCollider : MonoBehaviour
{
	private MoveBalls moveBallsScript;
	private bool onceFlag;

	void Start()
	{
		onceFlag = true;
		moveBallsScript = GameObject.FindObjectOfType<MoveBalls>();
	}

	void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.tag == "ActiveBalls" && onceFlag)
		{
			onceFlag = false;

			this.GetComponent<Rigidbody>().velocity = Vector2.zero;
			this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
			this.gameObject.tag = "ActiveBalls";
			this.gameObject.layer = LayerMask.NameToLayer("ActiveBalls");

			// Get a vector from the center of the collided ball to the contact point
			ContactPoint contact = other.contacts[0];
			Vector3 CollisionDir = contact.point - other.transform.position;

			int currentIdx = other.transform.GetSiblingIndex();

			float angle  = Vector3.Angle(CollisionDir, other.transform.forward);
			if ( angle > 90)
				moveBallsScript.AddNewBallAt(this.gameObject, currentIdx + 1, currentIdx);
			else
				moveBallsScript.AddNewBallAt(this.gameObject, currentIdx, currentIdx + 1);

			this.gameObject.GetComponent<BallCollider>().enabled = false;
		}
	}
}
