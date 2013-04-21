using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.Data;
using System.Text.RegularExpressions;
using ParseKit.CORE;
using ParseKit.Utils;

namespace ParseKit.ResourceClasses
{
    class TextsGiver
    {
        public void GenerateText()
        {
            /*
             * algorytm based on analyze many books and using insane algorytm
             * to make text basen on analyzed books 
             */
        }

        string RewriteText(string text)
        {

            return null;
        }

        string SynonimizeText(string text)
        {

            return null;
        }

        string TranslateGoogle(int count, string text)
        {
            return null;
        }

        public string[] ParseReferatsYandex(int themesCount = 0)
        {
            string refContentPattern = @"<h2>[^<]*</h2>\s*<h1 style=""color:black; margin-left:0;"">(?<theme>[^<]*)</h1>(?<text>(\n|.)*?)(?=</div></td>)";

            Regex refRx = new Regex(refContentPattern);

            List<string> themes = new List<string>{ "astronomy", 
                                     "geology", 
                                     "gyroscope", 
                                     "literature", 
                                     "marketing", 
                                     "mathematics", 
                                     "music", 
                                     "polit", 
                                     "agrobiologia", 
                                     "law", 
                                     "psychology", 
                                     "geography", 
                                     "physics", 
                                     "philosophy", 
                                     "chemistry", 
                                     "estetica" };

            if (themesCount==0) themesCount = GetGoodWeigetId(themes.Count);

            string themesPartOneReq = string.Empty;
            string themesPartTwoReq = string.Empty;
            for (int i = 0; i < themesCount; i++)
            {
                int indx = Rnd.Next(0, themes.Count);
                string theme = themes[indx];
                themes.RemoveAt(indx);
                themesPartOneReq += theme + "%2C";
                themesPartTwoReq += "&" + theme + "=on";
            }

            if (themesPartOneReq.Substring(themesPartOneReq.Length - 3, 3) == "%2C")
            {
                themesPartOneReq = themesPartOneReq.Remove(themesPartOneReq.Length - 3, 3);
            }

            Uri uri = new Uri("http://referats.yandex.ru/all.xml?mix=" + themesPartOneReq + themesPartTwoReq);
            DownloaderObj obj = new DownloaderObj(uri, null, true);
            Downloader.DownloadSync(obj);
            if (obj.DataStr == null) return null;

            Match dataMatch = refRx.Match(obj.DataStr);

            return new string[] { dataMatch.Groups["theme"].Value, dataMatch.Groups["text"].Value };    

            //http://referats.yandex.ru/all.xml?mix=astronomy%2Cgeology%2Cgyroscope%2Cliterature%2Cmarketing%2Cmathematics%2Cmusic%2Cpolit%2Cagrobiologia%2Claw%2Cpsychology%2Cgeography%2Cphysics%2Cphilosophy%2Cchemistry%2Cestetica&astronomy=on&geology=on&gyroscope=on&literature=on&marketing=on&mathematics=on&music=on&polit=on&agrobiologia=on&law=on&psychology=on&geography=on&physics=on&philosophy=on&chemistry=on&estetica=on
            //http://referats.yandex.ru/all.xml?mix=chemistry&chemistry=on
            //http://referats.yandex.ru/all.xml?mix=astronomy%2Cchemistry&astronomy=on&chemistry=on

            //оставить как есть
            //рерайт + синонимайз
            //перевести в гугл транслей 1-10 раз
            //рерайт + синонимайз + перевести в гугл транслей 1-10 раз
        }



        public void CopyFromTop10SE()
        {
            /*
             *  1. Используются без ссылок на источник
		        2. Используются со ссылкой
		        3. к 1 и 2 пункту прибавляется рерайт + синонимайз
		        4. к 1 и 2 + перевод их n раз
		        5. 3+4
             * */
        }

        //Берется N текстов и выдираются N/n предложений из 1..N, где n от 2 до 20 предложений.
        public void MixTexts()
        {
            /*
             *  1. Оставляется как есть
		        2. Прибавляется рерайт+ синонимайз
		        3. Прибавляется перевод
		        4. 1+2
             */
        }

        public void GetEngTextAndTranslate()
        {

        }

        public void UseUniqueText()
        {
            /*
             *  1. Пишутся самостоятельно, особенно если тематика интересна
		        2. Покупаются(вручную)
		        3. Уникальный скан книг + купленный уникальный скан.
             */
        }

        public void ParseBooks()
        {
            /*
                1.  тексты с книг тупо копируются и постятся на сайт.
		        2. тексты подвергаются рерайту+ синонимайзу/транслейту/выдергиваниям или всему вместе в рандомной степени.
            */

        }




        private static int GetGoodWeigetId(int maxId)
        {
            int sum = (1 + maxId) * maxId / 2;
            int rand = Rnd.Next(maxId * 2, sum + maxId * 2);
            int margin = sum - rand;
            int elementNumber = maxId;
            while (margin > 0)
            {
                margin -= elementNumber;
                elementNumber--;
            }

            int themesCount = maxId - elementNumber;
            if (themesCount == 0) themesCount = 1;
            return themesCount;
        }
    }
}
