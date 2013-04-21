using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using ParseKit.Data;
using ParseKit.ResourceClasses;
using System.Reflection;
using System.IO;

namespace ParseKit
{
    class ResourceScheme
    {
        const string DefaultResourceWeight = "0";
        const string DefaultMethodWeight = "0";

        public static void ReBuildResourceScheme()
        {
            XmlDocument XmlBase = new XmlDocument();

            try
            {
                XmlBase.Load(PATH.ResourceBase);
                CheckAttribute(XmlBase.DocumentElement, "Count", "0");
            }
            catch (Exception e)
            {
                GlobalLog.WriteError(e, "ResourceBase file error: creating new ResourceBase file content");
                XmlBase.LoadXml("<ResourceBase Count=\"0\"></ResourceBase>");
            }

            Type[] clasesTypes = { 
                                     typeof(ThemeGiver), 
                                     typeof(KeysGiver),
                                     typeof(TextsGiver),
                                     typeof(ImagesGiver),
                                     typeof(TitleGiver),
                                     typeof(DescriptionGiver),
                                     typeof(KeywordsTagGiver),
                                     typeof(FilesGiver),
                                     typeof(SitePatternGiver)
                                 };
            AddResInBase(XmlBase, clasesTypes);

            XmlBase.Save(PATH.ResourceBase);
        }

        private static void CheckAttribute(XmlElement xmlElement, string attrName, string attrDefaultValue)
        {
            XmlAttribute atr = xmlElement.GetAttributeNode(attrName);
            if (atr == null)
                xmlElement.SetAttributeNode(attrName, xmlElement.BaseURI).Value = attrDefaultValue;
            else if (string.IsNullOrEmpty(atr.Value))
                atr.Value = attrDefaultValue;
        }

        private static void AddResInBase(XmlDocument XmlBase, Type[] types)
        {
            foreach (Type type in types)
            {
                XmlElement resourceNode = XmlBase.GetElementsByTagName(type.Name)[0] as XmlElement;
                if (resourceNode == null)
                {
                    resourceNode = XmlBase.CreateElement(type.Name);
                    resourceNode.SetAttributeNode("ResourceWeight", resourceNode.BaseURI).Value = DefaultResourceWeight;
                }

                MethodInfo[] mList = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                foreach (MethodInfo mi in mList)
                {
                    XmlElement methodNode = XmlBase.GetElementsByTagName(mi.Name)[0] as XmlElement;
                    if (methodNode == null)
                    {
                        methodNode = XmlBase.CreateElement(mi.Name);
                        methodNode.SetAttributeNode("Weight", methodNode.BaseURI).Value = DefaultMethodWeight;
                        methodNode.SetAttributeNode("CalledCount", methodNode.BaseURI).Value = "0";
                    }
                    else
                    {
                        CheckAttribute(methodNode, "Weight", DefaultMethodWeight);
                        CheckAttribute(methodNode, "CalledCount", "0");
                    }
                    resourceNode.AppendChild(methodNode);
                }
                XmlBase.DocumentElement.AppendChild(resourceNode);
            }
        }

        public static void BackupBase()
        {
            if (File.Exists(PATH.ResourceBase))
                File.Copy(PATH.ResourceBase, PATH.ResourceBase + ".bak", true);
        }

        public static List<ResourceStat> ReadScheme()
        {
            List<ResourceStat> resStatList = new List<ResourceStat>();
            XmlDocument XmlBase = new XmlDocument();
            try
            {
                XmlBase.Load(PATH.ResourceBase);
            }
            catch (Exception e)
            {
                GlobalLog.WriteError(e, "error while load resourseBase file");
                ReBuildResourceScheme();
                ReadScheme();
            }

            resStatList.Add(GetResourceStat(XmlBase));
            return resStatList;
        }

        private static ResourceStat GetResourceStat(XmlDocument XmlBase)
        {
            XmlElement node = XmlBase.DocumentElement.SelectSingleNode("ThemeGiver") as XmlElement;
            int resourceWeight = Int32.Parse(node.GetAttributeNode("ResourceWeight").Value);

            return new ResourceStat("ThemeGiver", resourceWeight, GetActionStatList(node.ChildNodes));
        }

        private static List<ActionStat> GetActionStatList(XmlNodeList actionStatList)
        {
            List<ActionStat> actionsStat = new List<ActionStat>();
            foreach (XmlElement actStat in actionStatList)
	        {
                actionsStat.Add(new ActionStat(
                        actStat.Name, 
                        Int32.Parse(actStat.GetAttributeNode("Weight").Value), 
                        Int32.Parse(actStat.GetAttributeNode("CalledCount").Value)));
	        }
            return actionsStat;
        }
    }
}
