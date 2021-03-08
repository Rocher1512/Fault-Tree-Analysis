using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaultTreeProject
{
    public abstract class FaultTreeNode
    {
        public int mNodeID;

        public GateNode mParent;

        public static void Indent(int pIndent)
        {
            for (int i = 0; i < pIndent; i++)
            {
                Console.Write(" ");
            }
        }
    }


}
