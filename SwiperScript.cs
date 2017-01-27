using UnityEngine;
using System.Collections;

public class SwiperScript : MonoBehaviour {

	private Touch initTouch = new Touch();
	public GameObject go;
	public float rotSpeed = 0.5f;
	public float rotDir = -1f;

	private float rotX = 0f;
	private float rotY = 0f;
	private Vector3 originalRot;


	// Use this for initialization
	void Start ()
	{
		originalRot = go.transform.eulerAngles;//storing the original rotation
		rotX = originalRot.x;
		rotY = originalRot.y;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	foreach(Touch t in Input.touches)
		{
			if(t.phase == TouchPhase.Began)
			{
				initTouch = t;
			}
			else if(t.phase == TouchPhase.Moved)
			{
				//swiping
				float deltaX = initTouch.position.x - t.position.x; // calculating the distance between the current touch and and starting touch position.
				float deltaY = initTouch.position.y - t.position.y;
				rotX -= deltaY * Time.deltaTime * rotSpeed * rotDir; // adding or substracking the swiped distance from the original position. so it moves.
				rotY += deltaX * Time.deltaTime * rotSpeed * rotDir;

				//rotates on X and Y axes.
				go.transform.eulerAngles = new Vector3(rotX, rotY, 0f);

			}

			else if(t.phase==TouchPhase.Ended)
			{
				//touch ended need to reset initTouch
				initTouch = new Touch();
			}
		}

	}
}
