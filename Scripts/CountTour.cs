using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CountTour : MonoBehaviour {

	private const int Pm = 4;				//used to store how many case the player is allowed to move each tour
	private int nbPlayer = 2;
	private int tour = 0;					//tour number
	private GameObject[] players;			//array of GOs used to get players from the scene thanks to tags
	private CubeController[] playerScript;	//used to access functions of CubeControllerScript
	private bool didMove;					//boolean used to check if the player moved
	private Ray ray;						//ray used to do raycast
	private RaycastHit hit;					//raycasthit used to get info from raycast
	private Vector3 mousePositionOver;		//vector3 used to get the coordinated of the cube the mouse is over
	private GameObject currentCase;			//used to store the current case the mouse is over
	private Material highlightMaterial;		//material used to highlight a case
	private Material basicMaterial;			//material used to set case material back to default
	private AstarSearch aStar;				//used to access functions of AstarScript
	private List<NodeClass> aStarResult;	//used to store the result of the Astar algorithm
	private bool validPath;					//used to know if the path found by the Astar algorithm is valid

	// Use this for initialization
	void Start () {
		aStar = new AstarSearch ();
		mousePositionOver = new Vector3 (666f, 666f, 666f);
		tour = 1;
		didMove = false;
		currentCase = null;
		playerScript = new CubeController[nbPlayer];
		aStarResult = new List<NodeClass>();
		validPath = false;

		//Look in the scene for objects with tag "Player"
		players = GameObject.FindGameObjectsWithTag ("Player");

		//put CubeController script into array of CubeController scripts
		playerScript[0] = players [0].GetComponent<CubeController>();
		playerScript[1] = players [1].GetComponent<CubeController>();


		//load the materials used to highlight
		basicMaterial = Resources.Load("Material/Maison_tex", typeof(Material)) as Material;
		highlightMaterial = Resources.Load("Material/CubeHighlight", typeof(Material)) as Material;

	
	}
	
	// Update is called once per frame
	void Update () {
		if(playerScript[(1+tour)%nbPlayer].isMoving == false)
		{
			checkRaycast();
		}


		if (Input.GetMouseButtonDown (0) && mousePositionOver != new Vector3 (666f, 666f, 666f) && validPath == true && playerScript[(1+tour)%nbPlayer].isMoving == false)
		{
			//Debug.Log(mousePositionOver);
			//didMove = playerScript[tour%nbPlayer].moveTo (mousePositionOver);
			didMove = playerScript[tour%nbPlayer].moveTo (aStarResult);
			resetHilight();
			//Debug.Log(player1.transform.position);
			if(didMove)
				tour += 1;
		}
//		else if (Input.GetKeyDown (KeyCode.UpArrow))
//		{
//			didMove = playerScript[tour%2].checkRay(Vector3.forward);
//			if(didMove)
//				tour += 1;
//		}
//		else if (Input.GetKeyDown (KeyCode.DownArrow))
//		{
//			didMove = playerScript[tour%2].checkRay(Vector3.back);
//			if(didMove)
//				tour += 1;
//		}
//		else if (Input.GetKeyDown (KeyCode.LeftArrow))
//		{
//			didMove = playerScript[tour%2].checkRay(Vector3.left);
//			if(didMove)
//				tour += 1;
//		}
//		else if (Input.GetKeyDown (KeyCode.RightArrow))
//		{
//			didMove = playerScript[tour%2].checkRay(Vector3.right);
//			if(didMove)
//				tour += 1;
//		}
	
	}

	private void checkRaycast()
	{
		//create a ray with the mouse position
		ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		if (Physics.Raycast (ray, out hit))
		{
			//if the ray touch the ground
			if (hit.collider.tag == "Ground") 
			{
				//calculate the manhattan distance between the arriving position and the starting position
				float movLimit = Mathf.Abs(hit.collider.transform.position.x - players[tour%nbPlayer].transform.position.x) 
					+ Mathf.Abs(hit.collider.transform.position.z - players[tour%nbPlayer].transform.position.z);
				// if the case touched is closer than 4
				if (movLimit <= Pm)
				{
					//call the function corresponding to this event
					rayHitsGroundInLimit(hit);
				}
				else
				{
					Debug.Log("too far");
					rayNotValid();
				}
			}
			else //if the raycast does not touch the ground or is too far from the player
			{
				Debug.Log("Raycast no Ground");
				rayNotValid();
			}
		}
		else //if the raycast does not hit anything
		{
			Debug.Log ("Raycase failed");
			rayNotValid();
		}
	}

	//function called when ray hits the ground in the limits
	private void rayHitsGroundInLimit(RaycastHit hitInfo)
	{ 
		//if an other case was already highlighted then put it back to default material
		if (this.currentCase != null)
		{
			this.currentCase.renderer.material = basicMaterial;

			//if the hitted case is different from the previous one
			if(this.currentCase.GetInstanceID() != hitInfo.collider.gameObject.GetInstanceID())
			{
				//reset the previous hilighted path, set the new position of the mouse, run the aStar algorithm again and hilight the new path.
				resetHilight();
				mousePositionOver = new Vector3(hitInfo.collider.transform.position.x, 0.5f, hitInfo.collider.transform.position.z);
				aStarResult = aStar.init(players[tour%nbPlayer].transform.position, mousePositionOver); 
			}
		}
		else
		{
			//set the new position of the mouse, run the aStar algorithm and hilight the founded path
			mousePositionOver = new Vector3(hitInfo.collider.transform.position.x, 0.5f, hitInfo.collider.transform.position.z);
			this.aStarResult = aStar.init(players[tour%nbPlayer].transform.position, mousePositionOver);
			//hilightPath();
		}
		//if the path found is not too long
		if(aStarResult.Count <= Pm + 1 )
		{
			validPath = true;
			hilightPath();
			//set the new current case highlighted position
			this.currentCase = hitInfo.collider.gameObject;
			this.currentCase.renderer.material = highlightMaterial;
		}
		//else then invalid the path and set the mouse position to default vector
		else
		{
			validPath = false;
			mousePositionOver = new Vector3 (666f, 666f, 666f);
		}
	}

	//function called when the ray does not hit a correct object
	private void rayNotValid()
	{
		//set the current case position back to null and reset the material of the previous one
		if (this.currentCase != null)
		{
			this.currentCase.renderer.material = basicMaterial;
			this.currentCase = null;
			resetHilight();
			aStarResult.Clear();
		}
		mousePositionOver = new Vector3 (666f, 666f, 666f);
	}

	//function in charge of reseting the hilighting of the path (previous path)
	private void resetHilight()
	{
		Vector3 originRay = new Vector3();
		Vector3 directionRay = new Vector3();

		//browse all the list that contains the result of the aStar algorithm
		for(int i = 0; i < aStarResult.Count-1; i++)
		{
			//build a ray with the origin equals to the current node and direction equals to the next node
			originRay = new Vector3(aStarResult[i].getNodePosition().x, -0.15f, aStarResult[i].getNodePosition().z);
			directionRay = new Vector3(aStarResult[i+1].getNodePosition().x-aStarResult[i].getNodePosition().x,aStarResult[i+1].getNodePosition().y-aStarResult[i].getNodePosition().y,aStarResult[i+1].getNodePosition().z-aStarResult[i].getNodePosition().z);
			ray = new Ray(originRay,directionRay);

			//if the ray hits
			if (Physics.Raycast (ray, out hit))
			{
				//if the ray touch the ground
				if (hit.collider.tag == "Ground") 
				{	
					//set the material of this case to the basic one
					GameObject caseHitted = hit.collider.gameObject;
					caseHitted.renderer.material = basicMaterial;
				}
			}
		}
	}

	//function inf charge of hilighting the path
	private void hilightPath()
	{
		Vector3 originRay = new Vector3();
		Vector3 directionRay = new Vector3();

		//browse all the list that contains the result of the aStar algorithm
		for(int i = 0; i < aStarResult.Count-1; i++)
		{
			//build a ray with the origin equals to the current node and direction equals to the next node
			originRay = new Vector3(aStarResult[i].getNodePosition().x, -0.15f, aStarResult[i].getNodePosition().z);
			directionRay = new Vector3(aStarResult[i+1].getNodePosition().x-aStarResult[i].getNodePosition().x,aStarResult[i+1].getNodePosition().y-aStarResult[i].getNodePosition().y,aStarResult[i+1].getNodePosition().z-aStarResult[i].getNodePosition().z);
			ray = new Ray(originRay,directionRay);

			//if the ray hits
			if (Physics.Raycast (ray, out hit))
			{
				//if the ray touch the ground
				if (hit.collider.tag == "Ground") 
				{
					//set the material of this case to the hilight material
					GameObject caseHitted = hit.collider.gameObject;
					caseHitted.renderer.material = highlightMaterial;
				}
			}
		}
	}
}
