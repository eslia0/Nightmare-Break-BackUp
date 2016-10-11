using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class Node {

// test delegate 

	public delegate NodeStates ActionNodeDelegate();

	protected NodeStates m_nodeState;

	public delegate NodeStates NodeReturn ();

	public NodeStates nodeState{
		get {return m_nodeState;}
	}

	public Node(){}

	public abstract NodeStates Evealuate ();

	//"this class " lack variable so in the variable;

//	public enum NodeStates{
//		FAILURE,
//		SUCCESS,
//		RUNNING,
//		FALLURE
//	};

	//public delegate  void NodeStates (int state);

	public enum NodeStates {
		FAILURE=1 ,
		SUCCESS,
		RUNNING,
		FALLURE
	};

//	private ActionNodeDelegate m_action;
//
//	public ActionNodeDelegate(ActionNodeDelegate action){
//		m_action = action;
//		return m_action;
//	}
}




