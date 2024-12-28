using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wordnet
{
    internal class Class1
    {

            public int value;
            public List<int> next;
            public int levelFromParent1, levelFromParent2;
            public bool check;
            public Class1(int id)
            {
                value = id;
                levelFromParent1 = 0;
                levelFromParent2 = 0;
                next = new List<int>();
                check = false;
            }
            public Class1()
            {
                value = 0;
                levelFromParent1 = 0;
                levelFromParent2 = 0;
                next = new List<int>();   // to fill it with the list of the parents
                check = false;             // to check if the node is vsisited 
            }


        }
    }
