using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FaultTreeProject
{
    class ChildNode : FaultTreeNode
    {
        public static void Print(XmlNode pCurrentNode, int indent)
        {
            
            Indent(indent);
            Console.WriteLine("Child");
            Indent(indent);
            Console.WriteLine("{");

            foreach(XmlNode pChildNode in pCurrentNode.ChildNodes)
            {
                switch(pChildNode.Name)
                {
                    case "Or":
                        OrGateNode.Print(pChildNode, indent + 3);
                        break;
                    case "And":
                        AndGateNode.Print(pChildNode, indent + 3);
                        break;
                    case "Event":
                        Indent(indent + 3);
                        break;
                }
            }

            Indent(indent);
            Console.WriteLine("}");
        }

        public static void Printxml(XmlNode pCurrentNode, XmlWriter xmlWriter)
        {
           
            foreach (XmlNode pChildNode in pCurrentNode.ChildNodes)
            {
                switch (pChildNode.Name)
                {
                    case "Or":
                        OrGateNode.Printxml(pChildNode, xmlWriter);
                        break;
                    case "And":
                        AndGateNode.Printxml(pChildNode, xmlWriter);
                        break;
                    case "Event":
                        xmlWriter.WriteElementString("EventID", pChildNode.Attributes[0].Value);
                        /*Console.WriteLine(pChildNode.ParentNode.ParentNode.FirstChild.InnerText);
                        EventNode.AddToEventList(int.Parse(pChildNode.Attributes[0].Value));*/
                        EventNode.AddToEventList(pChildNode);
                        break;
                }
            }
        }
        public CutSet GetCutSets(XmlNode pCurrentNode)
        {
            foreach (XmlNode pChildNode in pCurrentNode.ChildNodes)
            {
                switch (pChildNode.Name)
                {
                    case "Event":
                        CutSet NewCutSet = new CutSet();
                        NewCutSet.AddToCutSet(int.Parse(pChildNode.Attributes[0].Value));
                        return NewCutSet;
                }
            }
            return null;
        }

        public static void AddChildren(XmlNode pCurrentNode, GateNode pParentNode)
        {
            foreach (XmlNode pChildNode in pCurrentNode.ChildNodes)
            {
                switch (pChildNode.Name)
                {
                    case "Or":
                        OrGateNode NewOrGateNode = new OrGateNode(int.Parse(pChildNode.Attributes[0].Value), pParentNode.mLayer);
                        pParentNode.GateChildNodeList.Add(NewOrGateNode);
                        OrGateNode.AddChildren(pChildNode, NewOrGateNode);
                        break;
                    case "And":
                        AndGateNode NewAndGateNode = new AndGateNode(int.Parse(pChildNode.Attributes[0].Value), pParentNode.mLayer);
                        pParentNode.GateChildNodeList.Add(NewAndGateNode);
                        AndGateNode.AddChildren(pChildNode, NewAndGateNode);
                        break;
                    default: //Case "Child"
                        EventNode NewEventGateNode = new EventNode(int.Parse(pChildNode.Attributes[0].Value));
                        pParentNode.EventChildNodeList.Add(NewEventGateNode);
                        break;
                }
            }
        }
    }
}
