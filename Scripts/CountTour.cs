using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CountTour : MonoBehaviour {

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

	// Use this for initialization
	void Start () {
		aStar = new AstarSearch ();
		mousePositionOver = new Vector3 (666f, 666f, 666f);
		tour = 1;
		didMove = false;
		currentCase = null;
		playerScript = new CubeController[nbPlayer];
		aStarResult = new List<NodeClass>();

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

		//create a ray with the mouse position
		ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		if (Physics.Raycast (ray, out hit))
		{
			float movLimit = Mathf.Abs(hit.collider.transform.position.x - players[tour%nbPlayer].transform.position.x) 
								+ Mathf.Abs(hit.collider.transform.position.z - players[tour%nbPlayer].transform.position.z);
			//if the ray touch the ground and the case touch is closer than 4
			if (hit.collider.tag == "Ground" && movLimit <= 4) 
			{
				//if an other case was already highlighted then put it back to default material
				if (this.currentCase != null)
					this.currentCase.renderer.material = basicMaterial;

				//set the new current case highlighted position
				this.currentCase = hit.collider.gameObject;
				this.currentCase.renderer.material = highlightMaterial;
				mousePositionOver = new Vector3(hit.collider.transform.position.x, 0.5f, hit.collider.transform.position.z);
				aStarResult = aStar.init(players[tour%nbPlayer].transform.position, mousePositionOver);
				//hilightPath();
				//Debug.Log (mousePositionOver);
			}
			else //if the raycast does not touch the ground or is too far from the player
			{
				Debug.Log("Raycast no Ground or too far");
				//set the current case position back to null and reset the material of the previous one
				if (this.currentCase != null)
				{
					this.currentCase.renderer.material = basicMaterial;
					this.currentCase = null;
				}
				mousePositionOver = new Vector3 (666f, 666f, 666f);
			}
		}
		else //if the raycast does not hit anything
		{
			//set the current case position back to null and reset the material of the previous one
			if (this.currentCase != null)
			{
				this.currentCase.renderer.material = basicMaterial;
				this.currentCase = null;
			}
			mousePositionOver = new Vector3 (666f, 666f, 666f);
			Debug.Log ("Raycase failed");
		}


//		if (tour % 2 == 1) 
//		{
//			playerScript = player1.GetComponent<CubeController>();
//
//		}
//		else if (tour % 2 == 0) 
//		{
//			playerScript = player2.GetComponent<CubeController>();
//
//		}

		if (Input.GetMouseButtonDown (0) && mousePositionOver != new Vector3 (666f, 666f, 666f))
		{
			//Debug.Log(mousePositionOver);
			didMove = playerScript[tour%nbPlayer].moveTo (mousePositionOver);
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

	private void hilightPath()
	{
//		Debug.Log("hilightPath");
//		Vector3 testRay = new Vector3();
//		for(int i = 0; i < aStarResult.Count-1; i++)
//		{
//			//testRay = new Vector3(aStarResult[i].getNodePosition().x, aStarResult[i].getNodePosition().y + 1, aStarResult[i].getNodePosition().z);
//			//ray = Camera.main.ScreenPointToRay (aStarResult[i].getNodePosition());
//			if (Physics.Raycast (aStarResult[i].getNodePosition(),aStarResult[i+1].getNodePosition(), out hit))
//			{
//				Debug.Log("hilightPath raycast hit" );
//				//if the ray touch the ground
//				if (hit.collider.tag == "Ground") 
//				{				
//					Debug.Log("Raycast True : " + i);
//					//set the new current case highlighted position
//					this.currentCase = hit.collider.gameObject;
//					this.currentCase.renderer.material = highlightMaterial;
//				}
//			}
//		}
	}
}
