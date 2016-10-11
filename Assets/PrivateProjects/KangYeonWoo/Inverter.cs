using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inverter : Node {

	//	"this class " lack variable so in the variable;
//	public enum NodeStates{
//		FAILURE,
//		SUCCESS,
//		RUNNING,
//		FALLURE
//	};

	protected Node m_nodes;

	public Node node{
		get {return m_nodes;}
	}

	public Inverter(Node node)
	{
		m_nodes = node;
	}

	public override NodeStates Evealuate ()
	{
		switch (node.Evealuate ()) {
			case NodeStates.FAILURE:
				m_nodeState = NodeStates.SUCCESS;
				return m_nodeState;
			case NodeStates.SUCCESS: 
				m_nodeState = NodeStates.FALLURE;
				return m_nodeState;
			case NodeStates.RUNNING:
				m_nodeState = NodeStates.RUNNING;
				return m_nodeState;
			}
			m_nodeState = NodeStates.SUCCESS;
			return m_nodeState;
	}

	//"this class " lack variable so in the variable;



}
