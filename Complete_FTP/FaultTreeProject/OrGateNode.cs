using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace FaultTreeProject
{
    public class OrGateNode : GateNode
    {
        public OrGateNode (int pEventID, int pLayer) : base(pEventID, pLayer)
        {
        }

        public static void Print(XmlNode pCurrentNode, int indent)
        {
            Indent(indent);
            Console.WriteLine("Or(" + pCurrentNode.Attributes[0].Value + ")");
            Indent(indent);
            Console.WriteLine("{");

            foreach(XmlNode pChildNode in pCurrentNode.ChildNodes)
            {
                switch (pChildNode.Name)
                {
                    case "Name":
                        Indent(indent + 3);
                        Console.WriteLine("Name: " + pChildNode.InnerText);
                        break;
                    default: // Children":
                        ChildNode.Print(pChildNode, indent + 3);
                        break;
                }
            }

            Indent(indent);
            Console.WriteLine("}");
        }

        public static void Printxml(XmlNode pCurrentNode, XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Or");
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

        public static void AddChildren (XmlNode pCurrentNode, GateNode pParentNode)
        {
            foreach(XmlNode pChildNode in pCurrentNode.ChildNodes)
            {
                if (pChildNode.Name == "Children")
                {
                    ChildNode.AddChildren(pChildNode, pParentNode);
                }
            }
        }
     
        public static List<CutSet> GetCutSets (GateNode pCurrentObjectNode)
        {
            //Returns a List of the objects givens CutSets (And Makes them in the process)

            //Console.WriteLine(pCurrentObjectNode.ToString());

            //Get All Child Gate Nodes CutSets to generate this Gates Cutses
            foreach (GateNode pChildObjectNode in pCurrentObjectNode.GateChildNodeList)
            {
                List<CutSet> aChildCutSetList = null;
                switch (pChildObjectNode.ToString())
                {
                    case "FaultTreeProject.AndGateNode":
                        aChildCutSetList = AndGateNode.GetCutSets(pChildObjectNode);
                        break;
                    case "FaultTreeProject.OrGateNode":
                        aChildCutSetList = GetCutSets(pChildObjectNode);
                        break;
                }

                //Since or gate add all child gate node CutSets to own
                foreach (CutSet pChildCutSet in aChildCutSetList)
                {
                    pCurrentObjectNode.CutSetList.Add(pChildCutSet);
                }
            }

            //Make CutSets from each Child event Node after getting gate nodes cutsets
            foreach (EventNode pChildObjectNode in pCurrentObjectNode.EventChildNodeList)
            {
                CutSet NewCutSet = new CutSet();
                NewCutSet.AddToCutSet(pChildObjectNode.mNodeID);
                pCurrentObjectNode.CutSetList.Add(NewCutSet);
            }

            //Redudancy check With Cut Set Tree
            return pCurrentObjectNode.CutSetList;
        }

        public static void MakeCutSets(GateNode pCurrentObjectNode)
        {
            //Find base of tree
            foreach (GateNode pChildGateNode in pCurrentObjectNode.GateChildNodeList)
            {
                MakeCutSets(pChildGateNode);
            }

            foreach (EventNode pChildEventNode in pCurrentObjectNode.EventChildNodeList)
            {
                //Startworking up from event nodes
                EventNode.MakeCutSet(pChildEventNode);
            }

            //reverse up nodes
            if (pCurrentObjectNode.mParent != null)
            {
                switch (pCurrentObjectNode.mParent.ToString())
                {
                    case "FaultTreeProject.AndGateNode":
                        //MOCUS Algorithm for making CutSets in AND Gate from Child CutSet Gates
                        if (pCurrentObjectNode.mParent.CutSetList.Count == 0)
                        {
                            foreach (CutSet pChildCutSet in pCurrentObjectNode.CutSetList)
                            {
                                pCurrentObjectNode.mParent.CutSetList.Add(pChildCutSet);
                            }
                        }
                        else
                        {
                            List<CutSet> NewCutSetList = new List<CutSet>();

                            foreach (CutSet pParentObjectCutSet in pCurrentObjectNode.mParent.CutSetList)
                            {
                                foreach (CutSet pCurrentObjectCutSet in pCurrentObjectNode.CutSetList)
                                {
                                    CutSet NewCutSet = new CutSet();

                                    //Adding Current Gates Cut Sets
                                    foreach (int pEventID in pParentObjectCutSet.EventList)
                                    {
                                        NewCutSet.AddToCutSet(pEventID);
                                    }
                                    //Adding Childs Cutsets
                                    foreach (int pEventID in pCurrentObjectCutSet.EventList)
                                    {
                                        NewCutSet.AddToCutSet(pEventID);
                                    }
                                    NewCutSetList.Add(NewCutSet);
                                }
                            }

                            pCurrentObjectNode.mParent.CutSetList = NewCutSetList;
                        }
                        break;

                    case "FaultTreeProject.OrGateNode":
                        //Since or gate add all child gate node CutSets to own
                        foreach (CutSet pChildCutSet in pCurrentObjectNode.CutSetList)
                        {
                            pCurrentObjectNode.mParent.CutSetList.Add(pChildCutSet);
                        }
                        break;
                }
            }
        }
    }
}
