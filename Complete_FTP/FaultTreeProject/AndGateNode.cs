using System;
using System.Collections.Generic;
using System.Xml;

namespace FaultTreeProject
{
    public class AndGateNode : GateNode
    {
        public AndGateNode(int pEventID, int pLayer) : base(pEventID, pLayer) { }

        public static void Print(XmlNode pCurrentNode, int indent)
        {
            Indent(indent);
            Console.WriteLine("And(" + pCurrentNode.Attributes[0].Value + ")");
            Indent(indent);
            Console.WriteLine("{");

            foreach (XmlNode pChildNode in pCurrentNode.ChildNodes)
            {
                switch (pChildNode.Name)
                {
                    case "Name":
                        Indent(indent + 3);
                        Console.WriteLine("Name: " + pChildNode.InnerText);
                        break;
                    default: // "Children":
                        ChildNode.Print(pChildNode, indent + 3);
                        break;
                }
            }

            Indent(indent);
            Console.WriteLine("}");
        }

        public static void Printxml(XmlNode pCurrentNode, XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("And");
            xmlWriter.WriteAttributeString("ID", pCurrentNode.Attributes[0].Value);
            foreach (XmlNode pChildNode in pCurrentNode.ChildNodes)
            {
                if (pChildNode.Name != "Name")
                {
                    ChildNode.Printxml(pChildNode, xmlWriter);
                }
            }
            xmlWriter.WriteEndElement();
        }

        public static void AddChildren(XmlNode pCurrentNode, GateNode pParrentNode)
        {
            foreach (XmlNode pChildNode in pCurrentNode.ChildNodes)
            {
                if (pChildNode.Name == "Children")
                {
                    ChildNode.AddChildren(pChildNode, pParrentNode);
                }
            }
        }

        public static List<CutSet> GetCutSets(GateNode pCurrentObjectNode)
        {
            //Returns a List of the objects givens CutSets (And Makes them in the process)

            //Get All Child Gate Nodes CutSets to generate this Gates Cutses
            foreach (GateNode pChildObjectNode in pCurrentObjectNode.GateChildNodeList)
            {
                List<CutSet> aChildCutSetList = null;
                switch (pChildObjectNode.ToString())
                {
                    case "FaultTreeProject.AndGateNode":
                        aChildCutSetList = GetCutSets(pChildObjectNode);
                        break;
                    case "FaultTreeProject.OrGateNode":
                        aChildCutSetList = OrGateNode.GetCutSets(pChildObjectNode);
                        break;
                }

                //MISCUP Algorithm for making CutSets in AND Gate from Child CutSet Gates
                if (pCurrentObjectNode.CutSetList.Count == 0)
                {
                    foreach (CutSet pChildCutSet in aChildCutSetList)
                    {
                        pCurrentObjectNode.CutSetList.Add(pChildCutSet);
                    }
                }
                else
                {
                    List<CutSet> NewCutSetList = new List<CutSet>();

                    foreach (CutSet pCurrentObjectCutSet in pCurrentObjectNode.CutSetList)
                    {
                        foreach (CutSet pChildObjectCutSet in aChildCutSetList)
                        {
                            CutSet NewCutSet = new CutSet();
                            
                            //Adding Current Gates Cut Sets
                            foreach (int pEventID in pCurrentObjectCutSet.EventList)
                            {
                                NewCutSet.AddToCutSet(pEventID);
                            }
                            //Adding Childs Cutsets
                            foreach (int pEventID in pChildObjectCutSet.EventList)
                            {
                                NewCutSet.AddToCutSet(pEventID);
                            }
                            NewCutSetList.Add(NewCutSet);
                        }
                    }

                    pCurrentObjectNode.CutSetList = NewCutSetList;
                }
            }

            //Make Cutsets for child event nodes
            foreach (EventNode pChildObjectNode in pCurrentObjectNode.EventChildNodeList)
            {
                if (pCurrentObjectNode.CutSetList.Count == 0)
                {
                    CutSet NewCutSet = new CutSet();
                    NewCutSet.AddToCutSet(pChildObjectNode.mNodeID);
                    pCurrentObjectNode.CutSetList.Add(NewCutSet);
                }
                else
                {
                    foreach (CutSet pCutSet in pCurrentObjectNode.CutSetList)
                    {
                        pCutSet.AddToCutSet(pChildObjectNode.mNodeID);
                    }
                }
            }

            //Redudancy check
            return pCurrentObjectNode.CutSetList;
        }
    }
}
