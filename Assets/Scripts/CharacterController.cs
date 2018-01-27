﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour {


	public States State;
	private MoveController Move; 
	[HideInInspector] public ViewArea View;
	private CharacterController TargetCtrl;
	public AngryablePerson AngryPerson;

	// hit
	private float HitRange;
	private bool HitOver;

	// rotate
	public int SearchRotate;
	public float RotateSpeed = 40f;
	private Facing Face;


	// Use this for initialization
	void Awake () 
	{
		Init ();


//		test();
	}

	void test()
	{
		State = States.WillAngry;
		SearchRotate = 1;
	}

	void Init()
	{
		State = States.Normal;
		SearchRotate = 0;

		Move = GetComponent<MoveController> ();
		HitRange = GetComponent<CircleCollider2D> ().radius * 2 + 0.1f;
		View = GetComponentInChildren<ViewArea> ();

		AdaptFace (View.transform.eulerAngles.z);
	}

	void Update()
	{
//		if (this.gameObject.name == "Character")
//			Debug.Log (View.transform.eulerAngles.z);

		// Search !
		if (SearchRotate != 0) {
			View.transform.Rotate (Vector3.forward * RotateSpeed * Time.deltaTime * SearchRotate);
			AdaptFace (View.transform.eulerAngles.z);

		} 

		// Hit others
		if (TargetCtrl != null && HitOver == false) 
		{
			float dist = (TargetCtrl.transform.position - transform.position).magnitude;
//			Debug.Log (dist);
			if (dist < HitRange)
			{
				HitOver = true;
				TargetCtrl.Hit (this.transform.position);
//				Debug.Log ("Fight");
				AngryPerson.TriggerFight ();
			}
		}




	}

	public void BecomeAngry(GameObject target)
	{
//		Debug.Log ("BecomeAngry");
		State = States.Angry;
		AngryPerson.TriggerRun ();
		SearchRotate = 0;
		Move.SetTarget (target);
		TargetCtrl = target.GetComponent<CharacterController> ();
	}

	public void Hit(Vector3 otherVec)
	{
//		Debug.Log ("Hit" + this.gameObject.name);

		if(State == States.Normal) // some condition
		{
			State = States.WillAngry;


			Vector3 direction = otherVec - transform.position;
			float deg = Mathf.Rad2Deg * Mathf.Atan2 (direction.y, direction.x); 


			float delta = deg - View.transform.eulerAngles.z;
			while (delta > 180)
				delta -= 360;
			while (delta < -180)
				delta += 360;

			Debug.Log (deg.ToString() + ',' + delta.ToString() );


			if (delta > 0)
				SearchRotate = 1;
			else
				SearchRotate = -1;

		}
	}

	public void AdaptFace(float deg)
	{

		deg -= 90;
		deg = -deg;
		while (deg >= 360)
			deg -= 360;
		while (deg < 0)
			deg += 360;
		
//		if (this.gameObject.name == "Character")
//			Debug.Log (deg);
		
		if (deg <= 315 && deg >= 225) 
		{
			if(Face != null || Face != Facing.left)
				AngryPerson.SetFacing (Facing.left);
		} 
		else if(deg <= 135 && deg >= 45)
		{
			if(Face != null || Face != Facing.right)
				AngryPerson.SetFacing (Facing.right);
		}
		else if(deg <= 225 && deg >= 135)
		{
			if(Face != null || Face != Facing.down)
				AngryPerson.SetFacing (Facing.down);
		}
		else 
		{
			if(Face != null || Face != Facing.up)
				AngryPerson.SetFacing (Facing.up);
		}
	}
}
