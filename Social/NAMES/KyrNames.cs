using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Thirdname_Maker
{
    public class KyrNames
    {
        string[] man = new [] {"ович", "евич", "ич"};
        string[] woman = new[] { "овна", "евна", "ична"};
        string gl = "уефыаоэяиюё";
        string sogl = "цкнгшщзхъвпрлджчсмтб";
        static Random rn = new Random();

        List<string> manList = new List<string>();
        List<string> womanList = new List<string>();
        List<string> womanFamilList = new List<string>();
        string content = string.Empty;

        public void GetThirdName(string secondNamePath)
        {
            StreamReader sr = new StreamReader(secondNamePath, Encoding.Default);

            while (!sr.EndOfStream)
            {
                string s = sr.ReadLine();
                s.Trim();

                if ((s[s.Length - 1] == 'й' & s[s.Length - 2] == 'и') | (s[s.Length - 2] == 'ь') | ((s[s.Length - 1] == 'й' & s[s.Length - 2] == 'е')))
                {
                    manList.Add(s.Substring(0, s.Length - 1) + man[1]);
                    womanList.Add(s.Substring(0, s.Length - 1) + woman[1]);
                    continue;
                }

                if (gl.Contains(s[s.Length - 1]))
                {
                    manList.Add(s.Substring(0, s.Length - 1) + man[2]);
                    womanList.Add(s.Substring(0, s.Length - 1) + woman[2]);
                    continue;
                }

                if (sogl.Contains(s[s.Length - 1]))
                {
                    manList.Add(s + man[0]);
                    womanList.Add(s + woman[0]);
                    continue;
                }
            }
            sr.Close();

            content = string.Empty;
            foreach (string s in manList)
            {
                content += s + "\r\n";
            }
            File.WriteAllText(@"c:\!Работа\!SE Feeds\Отчества Муж.txt", content);

            content = string.Empty;
            foreach (string s in womanList)
            {
                content += s + "\r\n";
            }
            File.WriteAllText(@"c:\!Работа\!SE Feeds\Отчества Жен.txt", content);
            

        }

        public void MakeWomanFamil(string familFilePath)
        {
            StreamReader sr = new StreamReader(familFilePath, Encoding.Default);

            while (!sr.EndOfStream)
            {
                string s = sr.ReadLine();
                s.Trim();

                if (s[s.Length - 1] == 'в' | s[s.Length - 1] == 'н')
                {
                    womanFamilList.Add(s + 'а');
                    continue;
                }
                if (s[s.Length - 1] == 'й' | s[s.Length - 2] == 'и')
	            {
                    womanFamilList.Add(s.Substring(0, s.Length - 2) + 'о');
                    continue;
	            }
            }
            sr.Close();

            content = string.Empty;
            foreach (var s in womanFamilList)
            {
                content += s + "\r\n";
            }
            File.WriteAllText(@"c:\!Работа\!SE Feeds\Фамилии Жен.txt", content);
        }

        public string GetFIO()
        {
            string namePath;
            string familPath;
            string surnamePath;

            int gender = rn.Next(0, 2);
            if (gender == 0)
            {
                namePath = @"c:\!Работа\!SE Feeds\Фамилии\Имена Муж.txt";
                familPath = @"c:\!Работа\!SE Feeds\Фамилии\Фамилии Муж.txt";
                surnamePath = @"c:\!Работа\!SE Feeds\Фамилии\Отчества Муж.txt";
            }
            else
            {
                namePath = @"c:\!Работа\!SE Feeds\Фамилии\Имена Жен.txt";
                familPath = @"c:\!Работа\!SE Feeds\Фамилии\Фамилии Жен.txt";
                surnamePath = @"c:\!Работа\!SE Feeds\Фамилии\Отчества Жен.txt";
            }

            StreamReader name = new StreamReader(namePath, Encoding.Default);
            StreamReader famil = new StreamReader(familPath, Encoding.Default);
            StreamReader surname = new StreamReader(surnamePath, Encoding.Default);

            List<string> nameList = new List<string>();
            List<string> familList = new List<string>();
            List<string> surnameList = new List<string>();

            while (!name.EndOfStream)
            {
                nameList.Add(name.ReadLine());
            }

            while (!famil.EndOfStream)
            {
                familList.Add(famil.ReadLine());
            }

            while (!surname.EndOfStream)
            {
                surnameList.Add(surname.ReadLine());
            }

            name.Close();
            famil.Close();
            surname.Close();

            long pregressCount = familList.Count * nameList.Count * surnameList.Count;
            long elapsed = pregressCount;

            int nameID = rn.Next(0, nameList.Count);
            int surnameID = rn.Next(0, surnameList.Count);
            int familID = rn.Next(0, familList.Count);

            return familList[familID] + " " + nameList[nameID] + " " + surnameList[surnameID];

            //UNusable, because 2% of combinations is 1,5 gb
            //foreach (string fml in familList)
            //{
            //    foreach (string nm in nameList)
            //    {
            //        foreach (string srnm in surnameList)
            //        {
            //            string fio = fml + " " + nm + " " + srnm + "\r\n";
            //            byte[] fioBytes = Encoding.Default.GetBytes(fio);
            //            fs.Write(fioBytes, 0, fioBytes.Length);
            //            elapsed--;
            //        }
            //    }
            //    Console.Clear();
            //    Console.WriteLine((double)(pregressCount - elapsed) / (double)pregressCount);
            //}
            //fs.Close();
        }
    }
}
