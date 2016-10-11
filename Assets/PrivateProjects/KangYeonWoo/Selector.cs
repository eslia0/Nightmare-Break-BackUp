//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//
//public class Selector : Node {
//
//	//"this class " lack variable so in the variable;
////	public enum NodeStates{
////		FAILURE,
////		SUCCESS,
////		RUNNING,
////		FALLURE
////	};
//
//	protected List<Node> m_nodes = new List<Node> ();
//
//	public Selector(List<Node> nodes)
//	{
//		m_nodes = nodes;
//	}
//
//
//
//	public override NodeStates Evealuate(){
//		foreach (Node node in m_nodes) {
//			switch (node.Evealuate ()) {
//			case NodeStates.FAILURE:
//				continue;
//			case NodeStates.SUCCESS: 
//				m_nodeState = NodeStates.SUCCESS;
//				return m_nodeState;
//			case NodeStates.RUNNING:
//				m_nodeState = NodeStates.RUNNING;
//				return m_nodeState;
//			default:
//				continue;
//			}
//		}
//		m_nodeState = NodeStates.FALLURE;
//		return m_nodeState;
//
//	}
//
//}
