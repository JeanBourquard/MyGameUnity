using UnityEngine;
using System.Collections;

public class CubeController : MonoBehaviour {

	public bool moveTo(Vector3 destination)
	{
		this.transform.position = destination;
		return true;
	}

//	Vector3 movement;								//Vector3 used to apply movement to players
//	private GameObject[] environmentElements;		//Array of GOs to get environment elements like obstacles from the scene
//	private bool allowMov;							//Boolean to determine if we allow the movement or not depending on obstacles
//	private bool didMove;							//Boolean used to give information on whether or not the player moved
//
//	// Use this for initialization
//	void Start () {
//
//		movement = new Vector3 (0f, 0f, 0f);
//		didMove = false;
//
//		//Get GOs with tag "EnvElement" from the scene and store them into an array
//		environmentElements = GameObject.FindGameObjectsWithTag ("EnvElement");
//
//	
//	}
//
//	public bool checkRay (Vector3 direction)
//	{
//		//RaycastHit used to collect info about the collision
//		RaycastHit hit;
//		allowMov = true;
//		//Set the movement vector for moving left
//		movement = direction;
//		//Ray used to test if there is any collision on the left
//		Ray collisionRay = new Ray (this.transform.position, movement);
//		
//		//Check if there is collision between the ray and any element and check if the tag is "EnvElement", if yes then don't allow the movement
//		if(Physics.Raycast(collisionRay, out hit, 1f) && (hit.collider.tag == "EnvElement" || hit.collider.tag == "Player"))
//			allowMov = false;
//		
//		Debug.Log("allowMov : " + allowMov);
//		//Call the applyMove function and get the result if the movement has been applied or not and store it into didMove boolean
//		didMove = applyMove (allowMov);
//		Debug.Log ("didMove : " + didMove);
//		return didMove;
//	}
//
//
//
//	//Apply the movement if the allowMov boolean is true
//	bool applyMove(bool allowMov)
//	{
//		if (allowMov == true) 
//		{
//			//Make the player forward vector = movement vector so he faces the right direction
//			this.transform.forward = movement;
//			//Make the player position vector = movement vector so he moves 1 unit in the right direction
//			this.transform.position += movement;
//			allowMov = false;
//			return true;
//		} 
//		else 
//		{
//			return false;
//		}
//	}
	
}
