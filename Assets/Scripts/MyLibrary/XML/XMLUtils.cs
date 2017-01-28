﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//Util class to interface with XMLParser.cs
public static class XMLUtils{
	
	/*
        Desc: This returns all the attributes of a node as a hashtable
    */
	public static Hashtable GetAttributes(IXMLNode node){
		Hashtable attributesHash = new Hashtable(); //Key: attribute name, Value: attribute value
		
		for(int i=0; i<node.Attributes.Count; i++){
			XMLAttribute attr = node.Attributes[i];
			string attrKey = attr.name;
			string attrValue = attr.value;
			attributesHash[attrKey] = attrValue;
		}
		return attributesHash;
	}
	
	/*
        Desc: This returns the children of an xml node as a hashtable. 
        DO NOT USE THIS if there can be more than one child with the same tag name.
    */
	public static Hashtable GetChildren(IXMLNode node){
		Hashtable childrenHash = new Hashtable(); //Key: name of the xml node, Value: xml node
		
		for(int i=0; i<node.Children.Count; i++){
			IXMLNode childNode = node.Children[i];
			string childNodeValue = childNode.value;
			childrenHash[childNodeValue] = childNode;
		}
		return childrenHash;
	}
	
	/*
        Desc: This returns the children of a xml node as a list. ONLY use this if 
        the children all share the same tag name
    */
	public static List<IXMLNode> GetChildrenList(IXMLNode node){
		List<IXMLNode> childrenList = new List<IXMLNode>();
		
		for(int i=0; i<node.Children.Count; i++){
			IXMLNode childNode = node.Children[i];
			
			childrenList.Add(childNode);
		}
		return childrenList;
	}
	
	//get the string value from a xml node. Default to empty string
	public static string GetString(IXMLNode node){
		return GetString(node, "");
	}
	
	public static string GetString(IXMLNode node, string defaultstring, string strError = null){
		string retVal = defaultstring;
		if(node != null){
			if(node.Children.Count != 0)
				retVal = node.Children[0].value;
			else
				Debug.LogError("Incoming element has more than one child...can't get value: " + node + "(" + node.Children.Count + ")");
		}
		else if ( strError != null )
			Debug.LogError(strError + "Required data is missing.");
		
		return retVal;
	}
	
	//get int value from a xml node. default to 0
	public static int GetInt(IXMLNode node){
		return GetInt(node, 0);
	}
	
	public static int GetInt(IXMLNode node, int defaultValue, string strError = null){
		int retVal = defaultValue;
		if(node != null){
			if(node.Children.Count == 1)
				retVal = Convert.ToInt32(node.Children[0].value);
			else
				Debug.LogError("Incoming element has more than one child...can't get value: " + node + "(" + node.Children.Count + ")");
		}
		else if ( strError != null )
			Debug.LogError(strError + "Required data is missing.");
		
		return retVal;
	}
	
	public static float GetFloat(IXMLNode node) {
		return GetFloat(node, 0);		
	}
	
	public static float GetFloat(IXMLNode node, float defaultValue, string strError = null){
		float retVal = defaultValue;
		if(node != null){
			if(node.Children.Count == 1)
				retVal = float.Parse(node.Children[0].value);
			else
				Debug.LogError("Incoming element has more than one child...can't get value: " + node + "(" + node.Children.Count + ")");
		}
		else if ( strError != null )
			Debug.LogError(strError + "Required data is missing.");
		
		return retVal;		
	}
	
	public static bool GetBool(IXMLNode node, bool defaultValue = true){
		bool retVal = defaultValue;
		if(node != null){
			if(node.Children.Count == 1)
				retVal = node.Children[0].value == "True";
			else
				Debug.LogError("Incoming element has more than one child...can't get value: " + node + "(" + node.Children.Count + ")");
		}
		return retVal;
		
	}
}