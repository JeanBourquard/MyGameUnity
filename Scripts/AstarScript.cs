using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AstarScript {
	private Vector3 currentPosition;			//position the player
	private Vector3 arrivalPosition;			//position the algorithme will try to reach
	private List<NodeClass> openList;			//list of nodes the algorithme will analyze
	private List<NodeClass> closeList;			//list of nodes the algorithme has already analyzed
	private NodeClass node;						//node to put all infos about one case together and then add it to the openList
	private int cheapestElementIndex;			//int used to store the index of the cheapest element of the openList
	private int cheapestElementCost;			//int used to store the cost of the cheapest element of the openList
	private Vector3[] movement = new [] {Vector3.forward, Vector3.back, Vector3.left, Vector3.right};
	
	//function called to initialize the algorithme
	public void init (Vector3 currPosition, Vector3 arrPosition)
	{
		//initialization of the variables
		openList = new List<NodeClass> ();
		closeList = new List<NodeClass>();
		currentPosition = currPosition;
		arrivalPosition = arrPosition;

		//gathering of the starting point informations
		node = new NodeClass (currentPosition);
		node.setH (System.Convert.ToInt32 (Mathf.Abs (arrivalPosition.x - node.getNodePosition().x) + Mathf.Abs (arrivalPosition.z - node.getNodePosition().z)));
		node.setG (0);
		node.setF (node.getH () + node.getG ());

		//make sure the openList and the closeList are empty at the beginning of the algorithme
		openList.Clear ();
		closeList.Clear();

		//add it to the openList
		openList.Add(node);

		//launch of the algorithme
		Astar ();
	}

	//main function of the algorithme
	private void Astar()
	{
		//while the openList is not empty
		do 
		{
			//find the cheapest element in the openList
			findCheapestInOpenList ();

			//add it to the closeList and remove it from the openList
			closeList.Add (openList [cheapestElementIndex]);
			openList.RemoveAt (cheapestElementIndex);
			//if it is the arrivalPosition then stop the algorithme
			if (closeList.Last ().getNodePosition () == arrivalPosition)
			{
				Debug.Log ("Found solution : " + closeList.Last ().getNodePosition ());
				break;
			}
			else 
			{
				Debug.Log("closeList last : " + closeList.Last().getNodePosition());
				Debug.Log("List.Count : " + closeList.Count);
//				Debug.Log("openList last : " + openList.Last().getNodePosition());
				//else develop its children
				developChildren ();
			}
		} while(openList.Count != 0);


	}

	//develop children of the last element in the closeList
	private void developChildren()
	{
		bool[] allowMov;
		//fill the table to know which movements are allowed or not
		allowMov = checkRay ();
		Debug.Log (allowMov [0].ToString () + allowMov [1].ToString () + allowMov [2].ToString () + allowMov [3].ToString ());
		//then compute informations for all children allowed
		computeCostsChildren (allowMov);

	}

	//find the cheapest element in the openList
	private void findCheapestInOpenList ()
	{
		int h;				//heuristic from element to arrivalPostion
		int g;				//cost of movement from currentPosition to element
		int f;				//cost of path from currentPosition to arrivalPosition going through element
		cheapestElementCost = 1000;			//set the cheapest cost to 1000 to start then algo
		cheapestElementIndex = 1000;		//set the cheapest index to 1000 (not usefull)

		//watch every element in the openList
		for (int i = 0; i < openList.Count; i++)
		{
//			//compute informations about the element
//			//heuristic is equal to manhattan distance
//			h = System.Convert.ToInt32(Mathf.Abs(arrivalPosition.x - openList[i].getNodePosition().x) + Mathf.Abs(arrivalPosition.z - openList[i].getNodePosition().z));
//			openList[i].setH(h);
//			//every movements cost 1 so G is just the number of movements made to get from the currentPosition to this case
//			g = openList[i].getParentG() +1;
//			openList[i].setG(g);
//			//cost of path is equal to addition of heuristic and cost of movement
//			f = h + g;
//			openList[i].setF(f);

			//if it is cheaper than previous ones
			if(openList[i].getF() < cheapestElementCost)
			{
				//then we store its cost and its index
				cheapestElementCost = openList[i].getF();
				cheapestElementIndex = i;
			}
		}
	}

	//compute informations of children ofthe last element of the closeList
	private bool computeCostsChildren (bool[] allowMov)
	{
		int h;				//heuristic from element to arrivalPostion
		int g;				//cost of movement from currentPosition to element
		int f;				//cost of path from currentPosition to arrivalPosition going through element

		//watch every movement
		for (int i = 0; i < allowMov.Length; i++)
		{
			//heuristic is equal to manhattan distance
			h = System.Convert.ToInt32(Mathf.Abs(arrivalPosition.x - closeList.Last().getNodePosition().x) + Mathf.Abs(arrivalPosition.z - closeList.Last().getNodePosition().z));
			//every movements cost 1 so G is just the number of movements made to get from the currentPosition to this case
			g = closeList.Last().getParentG() +1;
			//cost of path is equal to addition of heuristic and cost of movement
			f = h + g;
			//gather all the informations of the element in the node structure
			node.setNodePosition(closeList.Last().getNodePosition() + movement[i]);
			node.setH(h);
			node.setG(g);
			node.setF(f);
			node.setParentG(closeList.Last().getG());
			node.setParent(closeList.Last().getNodePosition());

			Debug.Log("node value : " + node.getNodePosition());
			//if the movement is allowed and the node is not already in the openList or in the closeList
			if(allowMov[i] == true && openList.Contains(node) == false && isInCloseList(node) == false)
			{
				//then add it to the openList
				Debug.Log("blabla");
				openList.Add(node);
			}
		}
		return true;
	}

	private bool isInCloseList ( NodeClass node)
	{
		bool inList = false;
		for (int i = 0; i < closeList.Count; i++)
		{
			if (closeList [i].getNodePosition () == node.getNodePosition ())
					inList = true;
		}
		return inList;

	}

	//check if the movements are possible or not
	private bool[] checkRay ()
	{
		//RaycastHit used to collect info about the collision
		RaycastHit hit;
		bool[] allowMovement =  new [] {true, true, true, true};

		//watch every element of the table
		for (int i = 0; i < allowMovement.Length; i++)
		{
						//Ray used to test if there is any collision on the left
						Ray collisionRay = new Ray (closeList.Last ().getNodePosition (), movement [i]);
			
						//Check if there is collision between the ray and any element and check if the tag is "EnvElement", if yes then don't allow the movement
						if (Physics.Raycast (collisionRay, out hit, 1f) && (hit.collider.tag == "EnvElement" || hit.collider.tag == "Player"))
								allowMovement [i] = false;
		}

		return allowMovement;
	}
}
