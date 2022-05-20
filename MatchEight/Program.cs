using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MatchEight
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please tipe the target heigth in inches ");
            int target = Convert.ToInt32(Console.ReadLine());
            bussyneslogic(target, GetItems("https://mach-eight.uc.r.appspot.com/"));

            Console.ReadLine();

        }
        public static void bussyneslogic(int target, List<Players> infoPlayers)
        {

            Console.WriteLine("//////////////////////////////////Method 1////////////////////////////////////////////");
            Stopwatch timeMeasure = new Stopwatch();
            timeMeasure.Start();

            var result = (from P in infoPlayers
                          from P2 in infoPlayers
                          where P.h_in+P2.h_in==target
                          select P.first_name +P.last_name +" & "+P2.first_name+P2.last_name).ToList();
            if (result.Count<=0)
                Console.WriteLine("No matches found");
            foreach (string r in result)
                Console.WriteLine(r);
            timeMeasure.Stop();
            Console.WriteLine($"Execution Time: {timeMeasure.Elapsed.TotalMilliseconds} ms");
            Console.WriteLine("//////////////////////////////////Method 2////////////////////////////////////////////");

            Stopwatch timeMeasure2 = new Stopwatch();
            timeMeasure2.Start();
            int counter = 0;
            for (int i = 0; i< infoPlayers.Count; i++)
                for (int j = i; j< infoPlayers.Count; j++)
                    if ((infoPlayers[i].h_in+infoPlayers[j].h_in)==target)
                    {
                        counter++; Console.WriteLine(infoPlayers[i].first_name+" "+infoPlayers[i].last_name+" && "+infoPlayers[j].first_name+" "+infoPlayers[j].last_name);
                    }

            if (counter<=0)
                Console.WriteLine("No matches found"); 

            timeMeasure2.Stop();
            Console.WriteLine($"Execution Time: {timeMeasure2.Elapsed.TotalMilliseconds} ms");

            Console.WriteLine("//////////////////////////////////Method 3////////////////////////////////////////////");

            Stopwatch timeMeasure3 = new Stopwatch();
            timeMeasure3.Start();
            counter=0;
            foreach (string s in GetCombinations(infoPlayers.Select(x => x.h_in).ToArray(), target, "", infoPlayers))
            {
                Console.WriteLine(s);
                counter++;
            }
            if (counter<=0)
                Console.WriteLine("No matches found");

            timeMeasure3.Stop();
            Console.WriteLine($"Execution Time: {timeMeasure3.Elapsed.TotalMilliseconds} ms");


            Console.WriteLine("//////////////////////////////////Method 4////////////////////////////////////////////");

            Stopwatch timeMeasure4 = new Stopwatch();
            timeMeasure4.Start();

            List<string> list = new List<string>();
            for (int i = 0; i< infoPlayers.Count; i++)
                list.AddRange(infoPlayers.Where(x => x.h_in==target-infoPlayers[i].h_in).Select(x => infoPlayers[i].first_name+infoPlayers[i].last_name+","+ x.first_name+x.last_name).ToList());

            foreach (string res in list)
                Console.WriteLine(res);

            timeMeasure4.Stop();
            Console.WriteLine($"Execution Time: {timeMeasure4.Elapsed.TotalMilliseconds} ms");


        }

        ///  recursive 
        static void findNumbers(List<List<int>> ans, List<int> arr, int sum, int index, List<int> temp)
        {
            if (sum == 0)
            {
                ans.Add(new List<int>(temp));
                return;
            }
            for (int i = index; i < arr.Count; i++)
            {
                if ((sum - arr[i]) >= 0)
                {

                    temp.Add(arr[i]);
                    findNumbers(ans, arr, sum - arr[i], i,
                                temp);
                    temp.Remove(arr[i]);
                }
            }
        }

        ///  tree search 
        static bool isSubsetSum(int[] set, int n, int sum)
        {
            if (sum == 0)
            {
                Console.WriteLine(set[n]);
                set[n]=0;
                n=set.Length;
                sum=139;
                if (n<=1)
                    return true;
            }

            if (n == 0)
                return false;

            if (set[n - 1] > sum)
                return isSubsetSum(set, n - 1, sum);


            return isSubsetSum(set, n - 1, sum)|| isSubsetSum(set, n - 1, sum - set[n - 1]);
        }

        public static IEnumerable<string> GetCombinations(int[] set, int sum, string values, List<Players> infoPlayers)
        {
            for (int i = 0; i < set.Length; i++)
            {
                int left = sum - set[i];
                string vals = infoPlayers[Convert.ToInt32(i)].first_name+" "+infoPlayers[Convert.ToInt32(i)].last_name + "," + values;
                if (left == 0)
                {
                    yield return vals;
                }
                else
                {
                    int[] possible = set.Take(i).Where(n => n <= sum).ToArray();
                    if (possible.Length > 0)
                    {
                        foreach (string s in GetCombinations(possible, left, i.ToString(), infoPlayers))
                        {
                            int index = Convert.ToInt32(Regex.Replace(s, "[^0-9]", string.Empty));
                            string name = Regex.Replace(s, @"[\d-]", string.Empty);

                            yield return name+infoPlayers[index].first_name+" "+infoPlayers[index].last_name;
                        }
                    }
                }
            }
        }

        private static List<Players> GetItems(string url)
        {

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            List<Players> data = new List<Players>();
            using (WebResponse response = request.GetResponse())
            {
                using (Stream strReader = response.GetResponseStream())
                {

                    using (StreamReader objReader = new StreamReader(strReader))
                    {
                        string responseBody = objReader.ReadToEnd();
                        var oMycustomclassname = JsonConvert.DeserializeObject<dynamic>(responseBody);


                        data = JsonConvert.DeserializeObject<List<Players>>(oMycustomclassname["values"].ToString());

                    }
                }
            }
            return data;

        }
    }
}
