using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using ParseKit.ResourceClasses;

namespace ParseKit
{
    class SiteSynthesizer
    {
        public void GetRosource(Hashtable resList)
        {
        }

        public void MakeSite(Hashtable resList)
        {
            string site = ResourceGiver.GetSitePattern((int)resList["SitePatternGiver"]);
            if (site != null)
            {
                string theme = ResourceGiver.GetTheme((int)resList["ThemeGiver"]);
                string[] keys = ResourceGiver.GetKeys((int)resList["KeysGiver"], theme);


                //"TextsGiver"
                //"ImagesGiver"
                //"FilesGiver"

                //"TitleGiver"
                //"DescriptionGiver"
                //"KeywordsTagGiver"
                
            }

        }

        void UploadSite(string ftpAdress)
        {

        }
    }
}
