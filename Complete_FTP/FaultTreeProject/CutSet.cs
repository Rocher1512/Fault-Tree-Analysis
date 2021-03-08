using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FaultTreeProject
{
    public class CutSet
    {
        public List<int> EventList;
        
        public CutSet ()
        {
            EventList = new List<int>();
        }

        public void AddToCutSet(int EventID)
        {
            bool AlreadyIn = false;

            if (EventList.Count >= 1)
            {
                foreach (int number in EventList)
                {
                    if (number == EventID)
                    {
                        AlreadyIn = true;
                    }
                }
            }

            if (AlreadyIn == false)
            {
                EventList.Add(EventID);            
            }
        }

        //Returns a string form of a cutset to represent it
        public override string ToString()
        {
            string CutSetString = "";

            foreach (int pEventID in EventList)
            {
                CutSetString += "(" + pEventID + ")";
            }

            return CutSetString;
        }
    }
}
