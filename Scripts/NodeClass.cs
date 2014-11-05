using UnityEngine;
using System.Collections;

public class NodeClass {

	private Vector3 nodePosition;
	private Vector3 parent;
	private int parentG;
	private int h;
	private int g;
	private int f;

	public NodeClass(Vector3 position)
	{
		nodePosition = position;
		parentG = 0;
	}

/* -----------------------------------------------------*/

	public void setNodePosition(Vector3 value)
	{
		nodePosition = value;
	}

	public void setParent(Vector3 value)
	{
		parent = value;
	}

	public void setParentG(int value)
	{
		parentG = value;
	}
	
	public void setF(int value)
	{
		f = value;
	}

	public void setG(int value)
	{
		g = value;
	}

	public void setH(int value)
	{
		h = value;
	}

	public Vector3 getNodePosition()
	{
		return nodePosition;
	}

	public Vector3 getParent()
	{
		return parent;
	}

	public int getParentG()
	{
		return parentG;
	}

	public int getH()
	{
		return h;
	}

	public int getG()
	{
		return g;
	}

	public int getF()
	{
		return f;
	}
}
