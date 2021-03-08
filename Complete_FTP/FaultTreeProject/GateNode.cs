using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FaultTreeProject
{
    public class GateNode : FaultTreeNode
    {
        internal List<CutSet> CutSetList;
        public List<EventNode> EventChildNodeList;
        public List<GateNode> GateChildNodeList;
        public List<int> CutSetSummary;
        public int mLayer;

        public  GateNode(int pEventID, int pLayer)
        {
            CutSetList = new List<CutSet>();
            EventChildNodeList = new List<EventNode>();
            GateChildNodeList = new List<GateNode>();
            CutSetSummary = new List<int>();
            mLayer = pLayer + 1;
            mNodeID = pEventID;
        }

        public static void AssignParents(GateNode pParentNode)
        {
            foreach (GateNode pGateNode in pParentNode.GateChildNodeList)
            {
                pGateNode.mParent = pParentNode;

                AssignParents(pGateNode);
            }

            foreach (EventNode pEventNode in pParentNode.EventChildNodeList)
            {
                pEventNode.mParent = pParentNode;
            }
        }

        public static void PrintCutSets(GateNode pCurrentGateNode)
        {
            foreach (CutSet pCutSet in pCurrentGateNode.CutSetList)
            {
                Console.WriteLine("      CutSet");
                Console.WriteLine("      {");
                Console.WriteLine("         Unavailability: 0");
                Console.WriteLine("         UnavailabilitySort: 0");
                Console.WriteLine("         Events");
                Console.WriteLine("         {");

                foreach (int pEventID in pCutSet.EventList)
                {
                    Console.WriteLine("            EventID(" + pEventID.ToString() + ")");
                }

                Console.WriteLine("         }");
                Console.WriteLine("      }");
            }
        }
    }
}
