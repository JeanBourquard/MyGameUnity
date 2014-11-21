using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CubeController : MonoBehaviour {
	public bool isMoving = false;		//boolean used to disable the on clic event until the player is moving
	public float speed = 1.2f;

	public bool moveTo(List<NodeClass> aStarResult)
	{
		Debug.Log("in moveTo function");
		StartCoroutine(applyMove (aStarResult));
		return true;
	}
		
	IEnumerator applyMove(List<NodeClass> path)
	{
		//notify the player is moving
		isMoving = true;
		//get the child of the Gameobject, it is the model containing the animations
		GameObject thisChild = new GameObject();
		foreach(Transform child in transform)
		{
			if(child.name == "samuzai")
			{
				thisChild = child.gameObject;
			}
		}
		//play the walk animation
		thisChild.animation.Play("Walk");
		
		//travelled distance from starting point
		float travelDistance = 0;
		
		//starting point
		Vector3 startPosition = transform.position;
		
		//loop on every node of the path
		for(int i = path.Count-2 , count = path.Count, lastIndex = 0 ; i >= 0; i--)
		{
			
			//distance between starting point and arriving point (current node, next node)
			float distance = Vector3.Distance(startPosition, path[i].getNodePosition());
			
			//oriantation vector between those 2 points
			Vector3 direction = (path[i].getNodePosition() - startPosition).normalized;

			//set the orientation of the player according to the direction he is moving to
			Quaternion lookAt = Quaternion.LookRotation(path[i].getNodePosition() - transform.position);
			transform.rotation = Quaternion.Lerp(transform.rotation, lookAt, 1f);
			
			//loop until we did not pass the position of the next node
			while(travelDistance < distance)
			{
				
				//we go forward according to the moving speed and the time past
				travelDistance += (speed * Time.deltaTime);
				
				//if we passed or reached the arriving node position
				if(travelDistance >= distance)
				{
					
					//if we have still nodes left in the path list 
					if(i > lastIndex)
					{
						//we go further in the path 
						//between the 2 next nodes, substractign the distance already travelled beyond the current arriving node
						float distanceNext = Vector3.Distance(path[i-1].getNodePosition() , path[i].getNodePosition());
						float ratio = (travelDistance - distance) / distanceNext;
						
						//if the ration is greater than 1, then the distance travelled is also greater than the distance between the 2 next nodes (so we move too fast)
						//this loop will skip all the nodes that we are supposed to have already gone through by moving very fast (not really usefull in my case)
						while(ratio > 1)
						{
							i--;
							if(i == lastIndex)
							{
								//we reached the last node
								transform.position = path[i].getNodePosition();
								//go out of the loop
								break;
							}
							else
							{
								travelDistance -= distance;
								distance = distanceNext;
								distanceNext = Vector3.Distance(path[i-1].getNodePosition() , path[i].getNodePosition());
								ratio = (travelDistance - distance) / distanceNext;
							}
						}
						
						if(i == lastIndex)
						{
							//we reached the last node of the path in the previous while loop
							break;
						}
						else
						{
							transform.position = Vector3.Lerp(path[i].getNodePosition() , path[i-1].getNodePosition(), ratio);
						}
					}else{
						//we reached the last node of the path
						transform.position = path[i].getNodePosition();
						break;
					}
				}
				else
				{
					//else we go forward in the direction of the arriving point
					transform.position += direction * (speed * Time.deltaTime);
				}
				
				yield return null;
			}
			
			//we substract the distance that we had to travel between the 2 previous nodes on retire la distance qu'il y avait à parcourir entre les deux nodes précédents
			travelDistance -= distance;
			
			//update of the starting point for the next iteration
			startPosition = path[i].getNodePosition();
		}
		//stop the walk animation and play the idle one
		thisChild.animation.Stop("Walk");
		thisChild.animation.Play("idle");
		//notify the player is not moving anymore
		isMoving = false;
	}

}
