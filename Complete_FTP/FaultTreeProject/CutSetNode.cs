using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaultTreeProject
{
    class CutSetNode
    {
        bool mRootComplete;
        int mTopEventID;
        int mLayer;
        List<CutSetNode> mCutSetNodeChildList;

        public CutSetNode(int pEventID, int pParentDepth)
        {
            //mRootComplete = pRootComplete;
            mTopEventID = pEventID;
            mLayer = pParentDepth+1;
            mCutSetNodeChildList = new List<CutSetNode>();
        }

        //Given cutset is stored (if not redundant) in CutSetNode tree
        public static void StoreCutSet(CutSetNode pCurrentCutSetNode, CutSet pCutSet, List<CutSet>[] pCutSetList)
        {

            //Redundancy check one
            CutSet TestCutSet = new CutSet();
            for (int i = 1; i < pCutSet.EventList.Count; i++)
            {
                TestCutSet.EventList.Add(pCutSet.EventList[i]);
            }

            if (RedundantCheck1(pCurrentCutSetNode, TestCutSet))
            {
                return;
            }

            //Redundancy Check two as well 
            //Check child nodes with cutsets to see if theres a match
            foreach (CutSetNode pChildCutSetNode in pCurrentCutSetNode.mCutSetNodeChildList)
            {
                foreach (int pEventID in pCutSet.EventList)
                {
                    //If Event in CutSetNode is in CutSet
                    if (pEventID == pChildCutSetNode.mTopEventID)
                    {
                        //If CutSet Makes Children Redundant (if the child node is final the cutset could be stored in and its not the end)
                        if (pChildCutSetNode.mRootComplete)
                        {
                            return;
                        }
                        else //Store Cut Set, if it completes the root then end
                        {
                            //Go deeper, to the child to store the cutset in the correct place
                            StoreCutSet(pChildCutSetNode, pCutSet, pCutSetList);
                            return;
                        }
                    }
                }
            }

            //If there is no match
            MakeChildNode(pCurrentCutSetNode, pCutSet, pCutSetList);
        }

        //Makes a new node for cutset
        public static void MakeChildNode(CutSetNode pCurrentCutSetNode, CutSet pCutSet, List<CutSet>[] pCutSetList)
        {
            CutSetNode NewCutSetNode = new CutSetNode(pCutSet.EventList[pCurrentCutSetNode.mLayer], pCurrentCutSetNode.mLayer);
            pCurrentCutSetNode.mCutSetNodeChildList.Add(NewCutSetNode);

            if (!false)
            {
                //If new child node is the end
                if (pCutSet.EventList.Count == NewCutSetNode.mLayer)
                {
                    NewCutSetNode.mRootComplete = true;
                    pCutSetList[pCutSet.EventList.Count-1].Add(pCutSet);
                }
                else
                {
                    MakeChildNode(NewCutSetNode, pCutSet, pCutSetList);
                }
            }
        }

        public static bool RedundantCheck1(CutSetNode pCurrentCutSetNode, CutSet pCutSet)
        {
            //Check child nodes with cutsets to see if theres a match
            foreach (CutSetNode pChildCutSetNode in pCurrentCutSetNode.mCutSetNodeChildList)
            {
                foreach (int pEventID in pCutSet.EventList)
                {
                    //If Event in CutSetNode is in CutSet
                    if (pEventID == pChildCutSetNode.mTopEventID)
                    {
                        //If CutSet Makes Children Redundant (if the child node is final the cutset could be stored in and its not the end)
                        if (pChildCutSetNode.mRootComplete)
                        {
                            //Console.WriteLine(pCutSet);
                            return true;
                        }
                        else //Store Cut Set, if it completes the root then end
                        {
                            //Go deeper, to the child to store the cutset in the correct place
                            return RedundantCheck1(pChildCutSetNode, pCutSet);
                        }
                    }
                }
            }
            return false;
        }

        public static void PrintSummary(CutSetNode pCurrentCutSetNode)
        {
            int[] pMagCoutArray = new int[4];
            CountChildrenMag(pCurrentCutSetNode, pMagCoutArray);

            //Console.WriteLine("FaultTree(" + pCurrentCutSetNode.mTopEventID + ") CutSet Summary:");

            for (int i = 0; i < 4;)
            {
                if (pMagCoutArray[i] > 0)
                {
                    Console.WriteLine("   Magnitude: " + (i + 1) + " | Amount: " + pMagCoutArray[i]);
                }
                i++;
            }
        }

        private static void CountChildrenMag(CutSetNode pCurrentCutSetNode, int[] pMagCoutArray)
        {
            foreach (CutSetNode pChildCutSetNode in pCurrentCutSetNode.mCutSetNodeChildList)
            {
                if (pChildCutSetNode.mRootComplete)
                {
                    pMagCoutArray[pChildCutSetNode.mLayer - 1] += 1;
                }
                CountChildrenMag(pChildCutSetNode, pMagCoutArray);
            }
        }

        //Call to indent a line n spaces
        public static void Indent(int pIndent)
        {
            for (int i = 0; i < pIndent; i++)
            {
                Console.Write(" ");
            }
        }

        //Start with 0 indent
        public static void Print(CutSetNode pCurrentCutSetNode)
        {
            PrintNodeTree(pCurrentCutSetNode, 0);
        }

        //Print out the tree
        static void PrintNodeTree (CutSetNode pCurrentCutSetNode, int pIndent)
        {

            Indent(pIndent);
            Console.WriteLine("[" + pCurrentCutSetNode.mLayer + "](" + pCurrentCutSetNode.mTopEventID + ")");

            if (pCurrentCutSetNode.mRootComplete)
            {
                Indent(pIndent);
                Console.WriteLine("END NODE");
            }

            foreach (CutSetNode pChildCutSetNode in pCurrentCutSetNode.mCutSetNodeChildList)
            {
                PrintNodeTree(pChildCutSetNode, pIndent + 3);
            }
            
        }

        //Order CutSets in size of magnitude TO BE REMOVED AT SOME POINT
        public static List<CutSet> OrderList (List<CutSet> pCutSetList)
        {
            List<CutSet>[] pCutSetListArray = new List<CutSet>[4];

            pCutSetListArray[0] = new List<CutSet>();
            pCutSetListArray[1] = new List<CutSet>();
            pCutSetListArray[2] = new List<CutSet>();
            pCutSetListArray[3] = new List<CutSet>();

            foreach (CutSet pCutSet in pCutSetList)
            {
                switch (pCutSet.EventList.Count)
                {
                    case 1:
                        pCutSetListArray[0].Add(pCutSet);
                        break;
                    case 2:
                        pCutSetListArray[1].Add(pCutSet);
                        break;
                    case 3:
                        pCutSetListArray[2].Add(pCutSet);
                        break;
                    default: //Size 4 the only other one in this case
                        pCutSetListArray[3].Add(pCutSet);
                        break;
                }
            }

            List<CutSet> pOrderedList = new List<CutSet>();

            pOrderedList.AddRange(pCutSetListArray[0]);
            pOrderedList.AddRange(pCutSetListArray[1]);
            pOrderedList.AddRange(pCutSetListArray[2]);
            pOrderedList.AddRange(pCutSetListArray[3]);

            return pOrderedList;
        }

        //Takes a list and returns a list with now redundant cutsets
        public static List<CutSet> RemoveRedundancysFromList(List<CutSet> pCutSetList)
        {
            List<CutSet> pRedudantCutSets = new List<CutSet>();

            //Mag1 Redudants
            //Compare everything to every other thing once
            int pCount = 0;
            for (int i = 0; i < pCutSetList.Count;)
            {
                for (int j = i + 1; j < pCutSetList.Count;)
                {
                    CutSet NewRedundantCutSet = RedundantCutSet(pCutSetList[i], pCutSetList[j]);
                    if (NewRedundantCutSet != null)
                    {
                        pRedudantCutSets.Add(NewRedundantCutSet);
                        //Console.WriteLine("New Redundate CutSet: " + (pCount));
                        pCount++;
                    }
                    j++;
                }
                i++;
            }
            //Remove Redundant CutSets
            foreach (CutSet pCutSet in pRedudantCutSets)
            {
                pCutSetList.Remove(pCutSet);
            }
            return pCutSetList;
        }

        //Take two CutSets to find if one makes the other redudant and return the redundant one
        public static CutSet RedundantCutSet(CutSet pCurrentCutSet, CutSet pCheckCutSet)
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
    }
}
