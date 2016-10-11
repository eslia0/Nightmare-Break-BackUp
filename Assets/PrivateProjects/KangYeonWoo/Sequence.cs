//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//
//public class Sequnce : Node {
//
////	"this class " lack variable so in the variable;
////	public enum NodeStates{
////		FAILURE,
////		SUCCESS,
////		RUNNING,
////		FALLURE
////	};
//
//	protected List<Node> m_nodes = new List<Node> ();
//
//	public Sequnce(List<Node> nodes)
//	{
//		m_nodes = nodes;
//	}
//
//	public override NodeStates Evealuate(){
//		bool anyChildRunning = false;
//
//		foreach (Node node in m_nodes) {
//			switch (node.Evealuate ()) {
//
//			case NodeStates.FAILURE:
//				m_nodeState = NodeStates.FALLURE;
//				return m_nodeState;
//			case NodeStates.SUCCESS: 
//				continue;
//			case NodeStates.RUNNING:
//				anyChildRunning = true;
//				continue;
//			default:
//				m_nodeState = NodeStates.SUCCESS;
//				return m_nodeState;
//			}
//		}
//		m_nodeState = anyChildRunning ? NodeStates.RUNNING : 
//			NodeStates.SUCCESS;
//		return m_nodeState;
//
//	}
//
//}
