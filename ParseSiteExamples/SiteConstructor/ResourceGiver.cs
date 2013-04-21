using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.ResourceClasses;

namespace ParseKit
{
    static class ResourceGiver
    {

        public static string GetTheme(int actId)
        {
            if (actId == 0)
	        {
		        return new ThemeGiver().GetRandomTheme();
	        }
            if (actId == 1)
	        {
		        return new ThemeGiver().GetPopTheme();
	        }
            if (actId == 2)
	        {
		        return new ThemeGiver().GetBestTheme();
	        }
            return null;
        }

        public static string[] GetKeys(int actId, string theme)
        {
            if (actId == 0)
	        {
                return new KeysGiver().ParseAverageFK(theme);
	        }
            if (actId == 1)
	        {
                return new KeysGiver().ParseHightFK(theme);
	        }
            if (actId == 2)
	        {
                return new KeysGiver().ParseLowFK(theme);
	        }
            if (actId == 3)
            {
                return new KeysGiver().ParseMixedFK(theme);
            }
            return null;
        }

        public static void GetTexts(int actId)
        {

        }

        public static void GetImages(int actId)
        {

        }

        public static void GetTitle(int actId)
        {

        }

        public static void GetDescription(int actId)
        {

        }

        public static void GetKeywordsTag(int actId)
        {

        }

        public static string GetSitePattern(int actId)
        {
            if (actId == 0)
	        {
		        
	        }
            return null;
        }

        public static void GetFiles(int actId)
        {

        }
    }




    











}
