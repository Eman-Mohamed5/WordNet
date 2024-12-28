using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;namespace wordnet
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(" welcome in WordNet ");
            Console.WriteLine("__________________________________________________________________________");
        Continue_Program:
            Console.WriteLine("\n[1] Sample test cases\n[2] Complete test cases\n[3] Exit\n");
            Console.Write("Please enter your choice : ");
            char Choice = (char)Console.ReadLine()[0]; if (Choice == '1')
            {
                Console.WriteLine("\nwhich case you wanna run\n");
                Console.Write("Please pick up number between 1 to 6: ");
                char case_num = (char)Console.ReadLine()[0];
                if (case_num <= '6')
                {
                    if (case_num == '1' || case_num == '3' || case_num == '4')
                    {
                        string FileInfo = Read_Test_File("1synsets " + case_num + ".txt", "2hypernyms " + case_num + ".txt", "3RelationsQueries " + case_num + ".txt", "4OutcastQueries " + case_num + ".txt");
                    }
                    else
                    {
                        string FileInfo = Read_Test_File("1synsets " + case_num + ".txt", "2hypernyms " + case_num + ".txt", "3RelationsQueries " + case_num + ".txt", "");
                    }
                    goto Continue_Program;
                }
                else
                {
                    Console.Write("Please pick a right number\n");
                }
                goto Continue_Program;
            }
            else if (Choice == '2')
            {
                Console.WriteLine("\nwhich case you wanna run\n");
                Console.Write("Please pick up number between 1 to 7: "); char case_num = (char)Console.ReadLine()[0];
                if (case_num <= '6')
                {
                    string FileInfo = Read_Test_File("1synsets_case " + case_num + ".txt", "2hypernyms_case " + case_num + ".txt", "3RelationsQueries_case " + case_num + ".txt", "4OutcastQueries_case " + case_num + ".txt");
                    goto Continue_Program;
                }
                else if (case_num <= '7')
                {
                    string FileInfo = Read_Test_File("1synsets_case " + case_num + ".txt", "2hypernyms_case " + case_num + ".txt", "3RelationsQueries_case " + case_num + ".txt", "");
                    goto Continue_Program;
                }
                else
                {
                    Console.Write("Please pick a right number\n");
                }
                goto Continue_Program;
            }
            else if (Choice == '3')
            {
                goto EndProgram;
            }
            else
            {
                Console.WriteLine("Invalid choice, Please try agian\n");
                goto Continue_Program;
            }
        EndProgram:
            Console.WriteLine("__________________________________________________________________________");
        }
        public static int mdis = 100000;
        public static Dictionary<int, List<string>> vertices = new Dictionary<int, List<string>>();
        public static int size = 0;
        public static Dictionary<string, List<int>> Id = new Dictionary<string, List<int>>();
        public static Dictionary<int, List<int>> parents = new Dictionary<int, List<int>>();
        public static List<int> distance1;
        public static List<int> distance2;
        public static List<bool> local1;
        public static List<bool> local2;
        public static List<bool> global = new List<bool>();
        public static List<int> disGlobal = new List<int>();
        public static Stopwatch stopwatch = new Stopwatch();
        public static Stopwatch s_outcast = new Stopwatch();
        static List<int> ids1 = new List<int>();
        static List<int> ids2 = new List<int>();
        static string Read_Test_File(string FileName1, string FileName2, string FileName3, string FileName4)
        {
            ///sysnset
            StreamReader synsets = new StreamReader(FileName1);
            string synset = string.Empty;
            int myRoot;
            while ((synset = synsets.ReadLine()) != null)
            {
                size++;
                string[] split = synset.Split(',');
                string[] nouns = split[1].Split(' ');
                List<string> Verlist = new List<string>();
                foreach (string noun in nouns)
                {
                    if (!Id.ContainsKey(noun))
                    {
                        Id.Add(noun, new List<int>());
                    }
                    Id[noun].Add(int.Parse(split[0]));
                    Verlist.Add(noun);
                }
                vertices.Add(int.Parse(split[0]), Verlist);
                disGlobal.Add(-1);
                global.Add(false);
            }
            ////hyper
            StreamReader hypernyms = new StreamReader(FileName2);
            string hypernym = string.Empty;
            while ((hypernym = hypernyms.ReadLine()) != null)
            {
                string[] split = hypernym.Split(',');
                List<int> parent = new List<int>();
                if (split.Length == 1)
                {
                    myRoot = Int32.Parse(split[0]);
                    parents.Add(Int32.Parse(split[0]), null);
                }
                else
                {
                    for (int i = 1; i < split.Length; i++)
                    {
                        parent.Add(Int32.Parse(split[i]));
                        if (i + 1 == split.Length)
                        {
                            parents.Add(Int32.Parse(split[0]), parent);
                        }
                    }
                }
            }

            //relation
            FileStream relationoutput = new FileStream(@"D:\relation.txt", FileMode.OpenOrCreate);
            StreamWriter RelationOutPut = new StreamWriter(relationoutput);
            stopwatch.Start();
            int count = 0;
            StreamReader RelationsQueries = new StreamReader(FileName3);
            string relation = string.Empty;
            while ((relation = RelationsQueries.ReadLine()) != null)
            {
                List<int> ids1;
                List<int> ids2;
                string[] singleline = relation.Split(',');
                if (singleline.Length == 1)
                {
                    continue;
                }
                else
                {
                    ids1 = toId(singleline[0]);
                    ids2 = toId(singleline[1]);
                    int min = int.MaxValue;
                    int par = 0;
                    foreach (int x in ids1)
                    {
                        foreach (int y in ids2)
                        {
                            local1 = new List<bool>();
                            local2 = new List<bool>();
                            local1.AddRange(global);
                            local2.AddRange(global);
                            distance1 = new List<int>();
                            distance2 = new List<int>();
                            distance1.AddRange(disGlobal);
                            distance2.AddRange(disGlobal);
                            Tuple<int, int> q = get_PD(x, y, distance1, distance2, local1, local2);
                            if (q.Item2 < min)
                            {
                                min = q.Item2;
                                par = q.Item1;
                            }
                            RelationOutPut.Write(min + "," + string.Format("{0}", string.Join(" ", toNoun(par))));
                            //Console.WriteLine(min + "," + string.Format("{0}", string.Join(" ", toNoun(par))));
                        }
                        RelationOutPut.WriteLine();
                    }
                }
                distance1 = new List<int>();
                distance2 = new List<int>();
                local1 = new List<bool>();
                local2 = new List<bool>();
            }
            RelationOutPut.Close();
            stopwatch.Stop();
            Console.WriteLine("Time for relation is : " + stopwatch.Elapsed);
            s_outcast.Start();

            // outcast
            FileStream Outc = new FileStream(@"D:\outcast.txt", FileMode.Create);
            StreamWriter outd = new StreamWriter(Outc);
            StreamReader OutcastQueries = new StreamReader(FileName4);
            string Qutcast = string.Empty;
            while ((Qutcast = OutcastQueries.ReadLine()) != null)
            {
                string[] SingleLine = Qutcast.Split(',');
                if (SingleLine.Length == 1)
                {
                    continue;
                }
                List<string> words = new List<string>(); //list of pairs of the the test file each node of the pair in line
                for (int i = 0; i < SingleLine.Length; i++)
                    words.Add(SingleLine[i]); 
                //foreach (string s in words)
                //    Console.WriteLine(s);
                Console.WriteLine(outcast(words) + "     ");
                outd.Write(outcast(words));
                outd.WriteLine();
            }
            s_outcast.Stop();
          
            Console.WriteLine("Time for OutCast is : " + s_outcast.Elapsed);
            outd.Close();
            return " ";
        } 
        // All Functions
        public static List<string> toNoun(int node)
        {
            return vertices[node]; //m
        }
        public static List<int> toId(string node)
        {
            return Id[node]; //m
        }
        public static int di;
        public static string outcast(List<string> inputs)
        {
            List<int> ids = new List<int>();
            List<string> w = new List<string>();
            for (int i = 0; i < inputs.Count(); i++)
            {
                int temp = 0;
                for (int j = 0; j < inputs.Count(); j++)
                {
                    if (i != j)
                    {
                        Tuple<int, List<string>> q;
                        q = N_dist_noun(inputs[i], inputs[j]);
                        temp += q.Item1;
                    }
                }
                ids.Add(temp);
                w.Add(inputs[i]);
            }
            int index = 0;
            int maxdistance = 0;
            for (int i = 0; i < ids.Count; i++)
            {
                if (maxdistance < Math.Max(maxdistance, ids[i]))
                {
                    maxdistance = Math.Max(maxdistance, ids[i]);
                    index = i;
                }
            }
            return w[index];
        }
        public static Tuple<int, List<string>> N_dist_noun(string Noun1, string Noun2)
        {
           // get the list of ids of the secound noun
            List<string> CommonSynsets;
            int mmm = 100000;
            int SCA = 0;
            List<int> first = toId(Noun1); // get the list of ids of the first noun
            List<int> second = toId(Noun2);
            Tuple<int, int> q;
            for (int i = 0; i < first.Count; i++)
            {
                for (int j = 0; j < second.Count; j++)
                {
                    local1 = new List<bool>();
                    local2 = new List<bool>();
                    local1.AddRange(global);
                    local2.AddRange(global);
                    distance1 = new List<int>();
                    distance2 = new List<int>();
                    distance1.AddRange(disGlobal);
                    distance2.AddRange(disGlobal);
                    q = get_PD(first[i], second[j], distance1, distance2, local1, local2);
                    if (mmm > q.Item2)
                    {
                        mmm = q.Item2;
                        SCA = q.Item1;
                    }
                }
            }
            CommonSynsets = toNoun(SCA);
            return Tuple.Create(mmm, CommonSynsets);
        }
        public static int BFS(int start, List<int> d, List<bool> local, List<bool> general)
        {
            Queue<int> v = new Queue<int>();
            d[start] = 0;
            local[start] = true;
            if (parents[start] == null)
            {
                d[start] = 0;
                return start;
            }
            else
            {
                v.Enqueue(start);
                while (v.Count > 0)
                {
                    int a = v.Dequeue();
                    if (general != null)
                    {
                        if (general[a] == true)
                        {
                            return a;
                        }
                    }
                    if (parents[a] == null) { }
                    else
                    {
                        foreach (int x in parents[a])
                        {
                            if (local[x] == false)
                            {
                                v.Enqueue(x);
                                local[x] = true;
                                d[x] = d[a] + 1;
                            }
                        }
                    }
                }
                return 0;
            }
        }
        public static Tuple<int, int> get_PD(int id1, int id2, List<int> d1, List<int> d2, List<bool> local1, List<bool> local2)
        {
            BFS(id1, d1, local1, null);
            di = 0;
            int commonParent = BFS(id2, d2, local2, local1);
            di = d1[commonParent] + d2[commonParent];
            return Tuple.Create(commonParent, d1[commonParent] + d2[commonParent]);
        }
    }
}

