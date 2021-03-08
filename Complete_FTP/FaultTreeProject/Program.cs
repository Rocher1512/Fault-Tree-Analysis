using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace FaultTreeProject
{
    public class Program
    {
        public static List<int> EventIDList = new List<int>();
        public static List<string> FMEANames = new List<string>();

        enum CutSetMethods
        {
            MISCUP = 0,
            MOCUS = 1
        }

        enum RedundancyMethods
        {
            CutSetNodeTree = 0,
            BruteForce = 1
        }

        static void Main(string[] args)
        {
            bool exit = true;

            do
            {

                #region Choose Options
                //Lets the user choose
                Console.WriteLine("[Open File] | (1) CompleteResults.xml | (2) SmallReslts.xml | (3) Custom.xml");
                Console.WriteLine("[CutSet Method] | (1) MISCUP | (2) MOCUS");
                Console.WriteLine("[Redundancy Method] | (1) CutSetNode Tree | (2) Brute Force");
                Console.WriteLine("[Save Name] | (1) CompleteResultsSave | (2) SmallResultsSave | (3) Custom");
                Console.WriteLine("[Run Times] (1-9) Run the program 1-9 times");
                Console.WriteLine("[Example] | 21123 | To select option 2, 1, 1, 2, 3, respectively");

                Console.Write("> ");

                string options = Console.ReadLine();
                bool validOptions = true;
                string[] optionsText = new string[5];

                //Open File
                XmlDocument FaultTreeDoc = new XmlDocument();
                switch (options[0])
                {
                    case '1':
                        FaultTreeDoc.Load("CompleteResults.xml");
                        optionsText[0] = "CompleteResults.xml";
                        break;
                    case '2':
                        FaultTreeDoc.Load("SmallResults.xml");
                        optionsText[0] = "SmallResults.xml";
                        break;
                    case '3':
                        Console.WriteLine("What document would you like to open?");
                        string opendoc = Console.ReadLine();

                        try
                        {
                            FaultTreeDoc.Load(opendoc + ".xml");
                            optionsText[0] = opendoc + ".xml";
                        }
                        catch
                        {
                            Console.WriteLine("Document does not exist");
                            validOptions = false;
                        }
                        break;
                    default:
                        validOptions = false;
                        break;
                }

                //CutSetMethod
                int CutSetMethod;

                switch (options[1])
                {
                    case '1':
                        CutSetMethod = (int)CutSetMethods.MISCUP;
                        optionsText[1] = "MISCUP";
                        break;
                    case '2':
                        CutSetMethod = (int)CutSetMethods.MOCUS;
                        optionsText[1] = "MOCUS";
                        break;
                    default:
                        CutSetMethod = 0;
                        validOptions = false;
                        break;
                }

                //Redundancy Method
                int RedundancyMethod;

                switch (options[2])
                {
                    case '1':
                        RedundancyMethod = (int)RedundancyMethods.CutSetNodeTree;
                        optionsText[2] = "CutSetNodeTree";
                        break;
                    case '2':
                        RedundancyMethod = (int)RedundancyMethods.BruteForce;
                        optionsText[2] = "BruteForce";
                        break;
                    default:
                        RedundancyMethod = 0;
                        validOptions = false;
                        break;
                }

                //Save File
                string savedoc;

                switch (options[3])
                {
                    case '1':
                        savedoc = "CompleteResultsSave";
                        optionsText[3] = "CompleteResultsSave";
                        break;
                    case '2':
                        savedoc = "SmallResultsSave";
                        optionsText[3] = "SmallResultsSave";
                        break;
                    case '3':
                        Console.WriteLine("What document would you like to Save too?");
                        savedoc = Console.ReadLine();
                        optionsText[3] = savedoc;
                        break;
                    default:
                        savedoc = "";
                        validOptions = false;
                        break;
                }

                int runTimes;

                try
                {
                    runTimes = int.Parse(options[4].ToString());
                    optionsText[4] = runTimes.ToString();
                }
                catch
                {
                    validOptions = false;
                    runTimes = 0;
                }

                #endregion

                if (validOptions == true)
                {
                    //| Opened file | CutSet Method | Redundancy Method | Save File |
                    Console.WriteLine("| [Opened] " + optionsText[0] + " | " + optionsText[1] + " | " + optionsText[2] + " | [Save] " + optionsText[3] + " | [Run's] " + runTimes);

                    //Entire Program Run Timer
                    Stopwatch sw = new Stopwatch();
                    Stopwatch CutSetTime = new Stopwatch();
                    Stopwatch RedundancyTime = new Stopwatch();
                    Stopwatch SaveTime = new Stopwatch();
                    sw.Start();

                    List<CutSet> OrderedCutSetList = new List<CutSet>();

                    for (int i = 0; i < runTimes; i++)
                    {
                        #region SetUp
                        //Setting Up XML File
                        XmlWriter xmlWriter;
                        XmlNodeList FaultTrees;
                        Xml_SetUp(FaultTreeDoc, savedoc, out xmlWriter, out FaultTrees);

                        List<List<CutSet>> xmllist = new List<List<CutSet>>();

                        //A list of lists for each Fault tree
                        List<GateNode> mFaultTreeNodesList = new List<GateNode>();
                        #endregion

                        #region Making Objects
                        //Make objects for each node
                        MakeObjectFaultTree(FaultTrees, mFaultTreeNodesList);
                        #endregion

                        #region Generating CutSets
                        //Generating CutSets then removes redundancys in objects
                        
                        CutSetTime.Start();

                        switch (CutSetMethod)
                        {
                            case (int)CutSetMethods.MISCUP:
                                MISCUP_CutSets(mFaultTreeNodesList);
                                break;
                            case (int)CutSetMethods.MOCUS:
                                MOCUS_CutSets(mFaultTreeNodesList);
                                break;
                        }

                        CutSetTime.Stop();
                        #endregion

                        #region RedundancyCheck
                        //REDUNDANCY
                        
                        RedundancyTime.Start();

                        switch (RedundancyMethod)
                        {
                            case (int)RedundancyMethods.CutSetNodeTree:
                                RemoveRedundancys_CutSetNodeTree(OrderedCutSetList, xmllist, mFaultTreeNodesList);
                                break;
                            case (int)RedundancyMethods.BruteForce:
                                RemoveRedundancys_BruteForce(OrderedCutSetList, xmllist, mFaultTreeNodesList);
                                break;
                        }

                        RedundancyTime.Stop();
                        #endregion

                        #region Store In Xml
                        
                        SaveTime.Start();

                        //Take results and store in XML doc
                        XMLPrintTree(xmlWriter, FaultTrees, xmllist, EventIDList);

                        SaveTime.Stop();
                        #endregion
                    }
                    sw.Stop();

                    #region TimesAndResults
                    //Results and Times
                    TimeSpan elapsedTime = sw.Elapsed;
                    TimeSpan elapsedCutSetTime = CutSetTime.Elapsed;
                    TimeSpan elapsedRedundancyTime = RedundancyTime.Elapsed;
                    TimeSpan elapsedSaveTime = SaveTime.Elapsed;
                    TimeSpan avgCutSetTime = new TimeSpan(elapsedCutSetTime.Ticks / runTimes);
                    TimeSpan avgRT = new TimeSpan(elapsedRedundancyTime.Ticks / runTimes);
                    TimeSpan avgST = new TimeSpan(elapsedSaveTime.Ticks / runTimes);
                    TimeSpan avgTotal = new TimeSpan(elapsedTime.Ticks / runTimes);
                    Console.WriteLine("Discrete CutSets Found: " + OrderedCutSetList.Count);
                    Console.WriteLine("Times Taken:");
                    Console.WriteLine("   CutSet Generation Time:     " + elapsedCutSetTime + " AVG " + (avgCutSetTime));
                    Console.WriteLine("   Removing Redundancys Time:  " + elapsedRedundancyTime + " AVG " + (avgRT));
                    Console.WriteLine("   Generate XML Save Time:     " + elapsedSaveTime + " AVG " + (elapsedSaveTime));
                    Console.WriteLine("--------------------------------------------------------------------");
                    Console.WriteLine("   Total Time:                 " + elapsedTime + " AVG " + avgTotal);
                    Console.WriteLine("--------------------------------------------------------------------");
                    #endregion
                }
                else
                {
                    Console.WriteLine("Invalid Options Chosen");
                }


                Console.WriteLine("Again? (y / n)");
                Console.Write("> ");
                string userInput = Console.ReadLine();
                
                if (userInput == "y")
                {
                    exit = false;
                }

            } while (!exit);
        }



        #region Class Program Methods

        private static void RemoveRedundancys_BruteForce(List<CutSet> OrderedCutSetList, List<List<CutSet>> xmllist, List<GateNode> mFaultTreeNodesList)
        {
            foreach (GateNode pCurrentObjectNode in mFaultTreeNodesList)
            {
                List<CutSet>[] CutSetList = new List<CutSet>[4];
                for (int i = 0; i < 4; i++)
                {
                    CutSetList[i] = new List<CutSet>();
                }

                List<CutSet> mRedudantCutSets = new List<CutSet>();
                for (int i = 0; i < pCurrentObjectNode.CutSetList.Count; i++)
                {
                    for (int j = i + 1; j < pCurrentObjectNode.CutSetList.Count; j++)
                    {
                        CutSet NewRedudantCutSet = RedudantCutSet(pCurrentObjectNode.CutSetList[i], pCurrentObjectNode.CutSetList[j]);
                        if (NewRedudantCutSet != null)
                        {
                            string Text = "";
                            foreach (int pEventID in pCurrentObjectNode.CutSetList[i].EventList)
                            {
                                Text += "(" + pEventID.ToString() + ")";
                            }
                            mRedudantCutSets.Add(NewRedudantCutSet);
                            // Console.WriteLine("Adding CutSet to Remove" + Text);
                        }
                    }
                }

                foreach (CutSet pRedudantCutSet in mRedudantCutSets)
                {
                    //Console.WriteLine("Removing a cutset");
                    pCurrentObjectNode.CutSetList.Remove(pRedudantCutSet);
                }

                foreach (CutSet pCutSet in pCurrentObjectNode.CutSetList)
                {
                    CutSetList[pCutSet.EventList.Count - 1].Add(pCutSet);
                }

                for (int i = 0; i < 4; i++)
                {
                    OrderedCutSetList.AddRange(CutSetList[i]);
                    xmllist.Add(CutSetList[i]);
                }
            }
        }

        private static void RemoveRedundancys_CutSetNodeTree(List<CutSet> OrderedCutSetList, List<List<CutSet>> xmllist, List<GateNode> mFaultTreeNodesList)
        {
            //Take Cutsets and put them in CutSetNode Tree
            List<CutSetNode> mCutSetNodeList = new List<CutSetNode>();

            //Console.WriteLine("CutSets Found | (Total) | (Mag 1)(Mag 2)(Mag 3)(Mag 4)");

            foreach (GateNode pCurrentObjectNode in mFaultTreeNodesList)
            {
                List<CutSet>[] CutSetList = new List<CutSet>[4];
                for (int i = 0; i < 4; i++)
                {
                    CutSetList[i] = new List<CutSet>();
                }

                CutSetNode pNewCutSetNode = new CutSetNode(pCurrentObjectNode.mNodeID, -1);
                mCutSetNodeList.Add(pNewCutSetNode);
                //Add all CutSets to a node
                pCurrentObjectNode.CutSetList = CutSetNode.OrderList(pCurrentObjectNode.CutSetList);
                foreach (CutSet pCutSet in pCurrentObjectNode.CutSetList)
                {
                    CutSetNode.StoreCutSet(pNewCutSetNode, pCutSet, CutSetList);
                }

                string CutSetMags = "";

                for (int i = 0; i < 4; i++)
                {
                    OrderedCutSetList.AddRange(CutSetList[i]);
                    xmllist.Add(CutSetList[i]);
                    CutSetMags += "(" + CutSetList[i].Count + ")";
                }
                //Console.WriteLine("FaultTree(" + pCurrentObjectNode.mNodeID + ")" + " | (" + OrderedCutSetList.Count + ") | " + CutSetMags);
            }
        }

        private static void MISCUP_CutSets(List<GateNode> mFaultTreeNodesList)
        {
            foreach (GateNode pCurrentObjectNode in mFaultTreeNodesList)
            {
                OrGateNode.GetCutSets(pCurrentObjectNode);
            }
        }

        private static void MOCUS_CutSets(List<GateNode> mFaultTreeNodesList)
        {
            foreach (GateNode pTopNode in mFaultTreeNodesList)
            {
                OrGateNode.MakeCutSets(pTopNode);
            }
        }

        private static void MakeObjectFaultTree(XmlNodeList FaultTrees, List<GateNode> mFaultTreeNodesList)
        {
            foreach (XmlNode pCurrentNode in FaultTrees)
            {
                if (pCurrentNode.Name == "FaultTree")
                {
                    foreach (XmlNode pChildNode in pCurrentNode.ChildNodes)
                    {
                        switch (pChildNode.Name)
                        {
                            case "TopNode":
                                OrGateNode NewOrGateNode = new OrGateNode(int.Parse(pChildNode.FirstChild.Attributes[0].Value), 0);
                                mFaultTreeNodesList.Add(NewOrGateNode);
                                OrGateNode.AddChildren(pChildNode.FirstChild, NewOrGateNode);
                                break;
                        }
                    }
                }
            }

            //Assign Parents
            foreach (GateNode pFaultTreeNode in mFaultTreeNodesList)
            {
                GateNode.AssignParents(pFaultTreeNode);
            }
        }
        
        private static void Xml_SetUp(XmlDocument FaultTreeDoc, string savedoc, out XmlWriter xmlWriter, out XmlNodeList FaultTrees)
        {
            xmlWriter = XmlWriter.Create(savedoc + ".xml");
            XmlNode FaultTreesNode = FaultTreeDoc.FirstChild.NextSibling.FirstChild;
            FaultTrees = FaultTreesNode.ChildNodes;
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("TIP-TOPS_Results");
            xmlWriter.WriteAttributeString("versionDate", "18 march 2019");
            xmlWriter.WriteAttributeString("version", "1");
            xmlWriter.WriteStartElement("FaultTrees");
        }

        private static void XMLPrintTree(XmlWriter xmlWriter, XmlNodeList FaultTrees, List<List<CutSet>> xmllist, List<int> EventIDList)
        {
            int mFaultTreeNumber = 0;
            int count = 0;
            int position = 0;
            bool start;
            bool end;
            bool cutsetpruned = false;
            string Unavailability = "";
            string Unavailabilitysort = "";
            string Name = "";
            string fmeaname = "";
            string beforefmeaname = "";
            List<CutSet> temp = new List<CutSet>();
            List<string> cutsetnames = new List<string>();
            List<string> cutsetID = new List<string>();
            foreach (XmlNode pNode in FaultTrees)
            {
                if (pNode.Name == "FaultTree")
                {
                    xmlWriter.WriteStartElement(pNode.Name);
                    xmlWriter.WriteAttributeString("ID", pNode.Attributes[0].Value);
                    cutsetID.Add(pNode.Attributes[0].Value);
                    //Console.WriteLine(pNode.Name + "(" + pNode.Attributes + ")");
                    foreach (XmlNode aNode in pNode.ChildNodes)
                    {
                        switch (aNode.Name)
                        {
                            case "Name":
                                xmlWriter.WriteElementString("Name", aNode.InnerText);
                                cutsetnames.Add(aNode.InnerText);
                                break;
                            case "SIL":
                                xmlWriter.WriteElementString("SIL", aNode.InnerText);
                                break;
                            case "Unavailability":
                                xmlWriter.WriteElementString("Unavailability", aNode.InnerText);
                                Unavailability = aNode.InnerText;
                                break;
                            case "UnavailabilitySort":
                                xmlWriter.WriteElementString("UnavailabilitySort", aNode.InnerText);
                                Unavailabilitysort = aNode.InnerText;
                                break;
                            case "Severity":
                                xmlWriter.WriteElementString("Severity", aNode.InnerText);
                                break;
                            case "TopNode":
                                xmlWriter.WriteStartElement("TopNode");
                                OrGateNode.Printxml(aNode.FirstChild, xmlWriter);
                                xmlWriter.WriteEndElement();
                                break;
                        }

                    }
                    xmlWriter.WriteStartElement("CutSetSummary");
                    int k = 1;
                    for (int j = count; j < count + 4; j++)
                    {
                        
                        if (xmllist[j].Count != 0)
                        {
                            xmlWriter.WriteStartElement("CutSets");
                            xmlWriter.WriteAttributeString("CutSetsSize", xmllist[j].Count.ToString());
                            xmlWriter.WriteAttributeString("CutSetsOrder", k.ToString());
                            xmlWriter.WriteAttributeString("CutSetsPruned", cutsetpruned.ToString());
                            xmlWriter.WriteEndElement();
                        }
                        k++;
                    }
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("AllCutSets");
                    for (int i = 0; i < 4; i++)
                    {
                        temp = xmllist[count];
                        
                        if (temp.Count != 0)
                        {
                            xmlWriter.WriteStartElement("CutSets");
                            xmlWriter.WriteAttributeString("Order", (i + 1).ToString());
                            foreach (CutSet cutSet in temp)
                            {

                                xmlWriter.WriteStartElement("Cutsets");
                                xmlWriter.WriteElementString("Unavailability", Unavailability);
                                xmlWriter.WriteElementString("UnavailabilitySort", Unavailabilitysort);
                                //xmlWriter.WriteElementString("EventID", cutSet.ToString());
                                xmlWriter.WriteStartElement("Events");
                                foreach (int ints in cutSet.EventList)
                                {
                                    xmlWriter.WriteElementString("EventID", ints.ToString());
                                }
                                xmlWriter.WriteEndElement();
                                xmlWriter.WriteEndElement();
                            }
                            xmlWriter.WriteEndElement();
                        }
                        
                        count++;
                    }

                    //xmlWriter.WriteElementString("EventID" , OrderedCutSetList[i].ToString());
                    mFaultTreeNumber += 1;
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();

                }
            }
            xmlWriter.WriteStartElement("FMEA");
            xmlWriter.WriteStartElement("Components");
            xmlWriter.WriteString("HardwarePP1");
            xmlWriter.WriteStartElement("Events");
            count = 0;
            int Counter = 0; 
            
            foreach (int cutsetid in EventIDList)
            {
                start = false;
                end = false;
                Name = FMEANames[count];
                position = 0;
                beforefmeaname = fmeaname;
                fmeaname = "";
                foreach(Char Character in Name)
                {
                    int i = (int)Character;
                    if (i == 93)
                    {
                        end = true;
                        break;
                    }
                    
                    if (start == true && end != true)
                    {
                        fmeaname += Character;
                    }
                    if (i == 45)
                    {
                        start = true;
                    }

                    position++;
                }
                if (fmeaname == "")
                {
                    fmeaname = beforefmeaname;
                }
                if (fmeaname != beforefmeaname)
                {
                    Counter = 0;
                }
                char character = (char)(65 + Counter);
                xmlWriter.WriteStartElement("BasicEventsID",cutsetid.ToString());
                xmlWriter.WriteElementString("Name",  fmeaname + "." + character.ToString());
                xmlWriter.WriteElementString("ShortName", character.ToString());
                xmlWriter.WriteElementString("Description","NA");
                xmlWriter.WriteElementString("Unavalability", Unavailability);
                xmlWriter.WriteStartElement("Effect");
                for (int i = 0; i < xmllist.Count / 4; i++)
                {
                    temp = xmllist[i * 4];
                    bool isafailure = false;
                    for (int j = 0; j < temp.Count; j++)
                    {
                        if (temp[j].ToString() == "(" + cutsetid.ToString() + ")")
                        {
                            isafailure = true;
                            break;
                        }
                    }
                    xmlWriter.WriteStartElement("EffectID", cutsetID[i]);
                    xmlWriter.WriteElementString("Name", cutsetnames[i]);
                    xmlWriter.WriteElementString("Singlepointfail", isafailure.ToString());
                    xmlWriter.WriteEndElement();

                }
                    // ves 1 added like 5 seconds so remaking
                    /*for (int i = 0; i < xmllist.Count/4; i++)
                    {
                        temp = xmllist[i*4];
                        foreach (CutSet cutset in temp)
                        {
                            Console.WriteLine(cutset);
                        }

                        for (int j = 0; j < temp.Count; j++)
                        {
                            if (temp[j].ToString() == "(" +cutsetid.ToString() + ")")
                            {
                                xmlWriter.WriteStartElement("EffectID", cutsetID[j]);
                                xmlWriter.WriteElementString("Name",cutsetnames[j]);
                                xmlWriter.WriteElementString("Singlepointfail", "True");
                                xmlWriter.WriteEndElement();
                            }
                            else
                            {

                                xmlWriter.WriteStartElement("EffectID", cutsetID[j]);
                                xmlWriter.WriteElementString("Name", cutsetnames[j]);
                                xmlWriter.WriteElementString("Singlepointfail", "False");
                                xmlWriter.WriteEndElement();
                            }
                        }
                    }*/
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
                count++;
                Counter++;
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }

        private static void PrintFaultTrees(XmlNodeList FaultTrees, List<GateNode> mFaultTreeNodesList, bool pIncludeCutSets)
        {
            int mFaultTreeNumber = 0;
            foreach (XmlNode pNode in FaultTrees)
            {
                if (pNode.Name == "FaultTree")
                {
                    Console.WriteLine(pNode.Name + "(" + pNode.Attributes[0].Value + ")");
                    Console.WriteLine("{");
                    foreach (XmlNode aNode in pNode.ChildNodes)
                    {
                        switch (aNode.Name)
                        {
                            case "Name":
                                Console.WriteLine("   " + aNode.Name + ": " + aNode.InnerText);
                                break;
                            case "SIL":
                                Console.WriteLine("   " + aNode.Name + ": " + aNode.InnerText);
                                break;
                            case "Unavailability":
                                Console.WriteLine("   " + aNode.Name + ": " + aNode.InnerText);
                                break;
                            case "UnavailabilitySort":
                                Console.WriteLine("   " + aNode.Name + ": " + aNode.InnerText);
                                break;
                            case "Severity":
                                Console.WriteLine("   " + aNode.Name + ": " + aNode.InnerText);
                                break;
                            case "TopNode":
                                Console.WriteLine("   {");
                                OrGateNode.Print(aNode.FirstChild, 6);
                                Console.WriteLine("   }");
                                break;
                            case "CutSetsSummary":
                                Console.WriteLine("   " + aNode.Name + ": ");
                                int Count = 0;
                                foreach (int pInt in mFaultTreeNodesList[mFaultTreeNumber].CutSetSummary)
                                {
                                    Count++;
                                    if (pInt > 0)
                                    {
                                        Console.WriteLine("      Magnitude: " + Count.ToString() + " | Ammount: " + pInt.ToString());
                                    }
                                }
                                break;
                            case "AllCutSets":
                                Console.WriteLine("   " + aNode.Name + ": ");
                                if (pIncludeCutSets)
                                {
                                    GateNode.PrintCutSets(mFaultTreeNodesList[mFaultTreeNumber]);
                                }
                                break;
                            default:
                                Console.WriteLine("   " + aNode.Name);
                                break;
                        }
                    }
                    Console.WriteLine("}");
                    mFaultTreeNumber += 1;
                }
                else
                {
                    Console.WriteLine(pNode.Name);
                }
            }
        }

        public static void RemoveCutSets(GateNode pCurrentObjectNode, List<CutSet> mRedudantCutSets)
        {
            for (int i = 0; i < mRedudantCutSets.Count; i++)
            {
                    Console.WriteLine("Removing a cutset");
                    pCurrentObjectNode.CutSetList.Remove(mRedudantCutSets[i]);
            }
        }

        public static List<CutSet> ReducingCutSets(GateNode pCurrentObjectNode, List<CutSet> mRedudantCutSets)
        {
            for (int i = 0; i < pCurrentObjectNode.CutSetList.Count; i++)
            {
                for (int j = i + 1; j < pCurrentObjectNode.CutSetList.Count; j++)
                {
                    CutSet NewRedundantCutSet = RedudantCutSet(pCurrentObjectNode.CutSetList[i], pCurrentObjectNode.CutSetList[j]);
                    if (NewRedundantCutSet != null)
                    {
                        string Text = "";
                        foreach (int pEventID in pCurrentObjectNode.CutSetList[i].EventList)
                        {
                            Text += "(" + pEventID.ToString() + ")";
                        }
                        mRedudantCutSets.Add(NewRedundantCutSet);
                        Console.WriteLine("-> " + pCurrentObjectNode.mLayer + " <- Adding CutSet to Remove" + Text);
                    }
                }
            }
            return mRedudantCutSets;
        }

        public static CutSet RedudantCutSet(CutSet pCurrentCutSet, CutSet pCheckCutSet)
        {
            if (pCurrentCutSet.EventList.Count < pCheckCutSet.EventList.Count)
            { 
                foreach (int pCurrentEventID in pCurrentCutSet.EventList)
                {
                    bool IsIn = false;

                    foreach (int pCheckEventID in pCheckCutSet.EventList)
                    {
                        if (pCurrentEventID == pCheckEventID)
                        {
                            IsIn = true;
                        }
                    }

                    if (IsIn == false)
                    {
                        return null;
                    }
                }
                return pCheckCutSet;
            }
            else
            {
                foreach (int pCurrentEventID in pCheckCutSet.EventList)
                {
                    bool IsIn = false;

                    foreach (int pCheckEventID in pCurrentCutSet.EventList)
                    {
                        if (pCurrentEventID == pCheckEventID)
                        {
                            IsIn = true;
                        }
                    }

                    if (IsIn == false)
                    {
                        return null;
                    }
                }
                return pCurrentCutSet;
            }
        }

        private static void Test_val(GateNode pCurrentNode)
        {
            if (pCurrentNode.ToString() == "FaultTreeProject.OrGateNode")
            {
                Console.WriteLine("Or(" + pCurrentNode.mNodeID + ") | CutSet Count: " + pCurrentNode.CutSetList.Count);
            }
            else
            {
                Console.WriteLine("And(" + pCurrentNode.mNodeID + ") | CutSet Count: " + pCurrentNode.CutSetList.Count);
            }

            foreach (GateNode pChildGateNode in pCurrentNode.GateChildNodeList)
            {
                Test_val(pChildGateNode);
            }
        }

        #endregion
    }
}
