using UnityEngine;
using System.Collections;


[System.Serializable]
public abstract class Node {

	public delegate NodeStates NodeReturn();

	protected NodeStates m_nodeStates;

	protected NodeStates m_NodeState{
		get { return m_nodeStates;}
	}

	public abstract NodeStates Evaluate();

}


