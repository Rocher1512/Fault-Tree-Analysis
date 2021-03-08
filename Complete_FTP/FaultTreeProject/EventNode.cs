using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FaultTreeProject
{
    public class EventNode : FaultTreeNode
    {
        public EventNode(int pEventID)
        {
            mNodeID = pEventID;
        }

        public static void AddToEventList(XmlNode Eventnode)
        {
            bool AlreadyIn = false;

            if (Program.EventIDList.Count >= 1)
            {
                foreach (int number in Program.EventIDList)
                {
                    if (number == int.Parse(Eventnode.Attributes[0].Value))
                    {
                        AlreadyIn = true;
                    }
                }
            }

            if (AlreadyIn == false)
            {
                Program.EventIDList.Add(int.Parse(Eventnode.Attributes[0].Value));
                Program.FMEANames.Add(Eventnode.ParentNode.ParentNode.FirstChild.InnerText);
            }
        }

        public static void MakeCutSet(EventNode pEventNode)
        {
            //Find out if parents gate
            switch (pEventNode.mParent.ToString())
            {
                case "FaultTreeProject.AndGateNode":

                    if (pEventNode.mParent.CutSetList.Count == 0)
                    {
                        CutSet TempCutSet = new CutSet();
                        TempCutSet.AddToCutSet(pEventNode.mNodeID);
                        pEventNode.mParent.CutSetList.Add(TempCutSet);
                    }
                    else
                    {
                        foreach (CutSet pCutSet in pEventNode.mParent.CutSetList)
                        {
                            pCutSet.AddToCutSet(pEventNode.mNodeID);
                        }
                    }
                    break;

                case "FaultTreeProject.OrGateNode":

                    CutSet NewCutSet = new CutSet();
                    NewCutSet.AddToCutSet(pEventNode.mNodeID);
                    pEventNode.mParent.CutSetList.Add(NewCutSet);
                    break;
            }
        }
    }
}
