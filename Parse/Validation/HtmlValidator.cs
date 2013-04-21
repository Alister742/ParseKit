using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using Parse;

namespace Parse.PageValidation
{
    class HtmlValidator : IPageValidator
    {
        List<KeyValuePair<string, List<HtmlAttribute>>> _hasTagsWithAttributes;
        List<string> _hasTagsbyPathes;
        double _passScore;

        public HtmlValidator(double passScore = Setting)
        {
            //List<KeyValuePair<string, List<HtmlAttribute>>> hasTagsWithAttributes, List<string> hasTagsbyPathes, 
            //if ((hasTagsWithAttributes == null || hasTagsWithAttributes.Count == 0) && (hasTagsbyPathes == null || hasTagsbyPathes.Count == 0))
            //    throw new ArgumentException("Bad arguments, there are no arguments for matching");

            _hasTagsWithAttributes = hasTagsWithAttributes;
            _hasTagsbyPathes = hasTagsbyPathes;
            _passScore = passScore;
        }

        public bool Validate(string originalPage, string validatingPage)
        {



            int score = 0;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(page);

            if (_hasTagsbyPathes != null)
            {
                for (int i = 0; i < _hasTagsbyPathes.Count; i++)
                {
                    HtmlNodeCollection node = doc.DocumentNode.SelectNodes(_hasTagsbyPathes[i]);
                    if (node != null && node.Count > 0)
                    {
                        score++;
                    }
                }
            }

            if (_hasTagsWithAttributes != null)
            {
                for (int i = 0; i < _hasTagsWithAttributes.Count; i++)
                {
                    string tagname = _hasTagsWithAttributes[i].Key;
                    List<HtmlAttribute> attributes = _hasTagsWithAttributes[i].Value;

                    string attributesXPathString = string.Empty;
                    if (attributes != null && attributes.Count > 0)
                    {
                        for (int j = 0; j < attributes.Count; j++)
                        {
                            string attrName = attributes[0].Name;
                            string attrVal = string.Empty;
                            string and = string.Empty;

                            if (attributes[j].Value != null)
                            {
                                attrVal = string.Format(", '{0}'", attributes[j].Value);
                            }
                            if (j + 1 < attributes.Count)
                            {
                                and = " and ";
                            }

                            attributesXPathString += string.Format("contains(@{0}{1}){2}", attrName, attrVal, and);
                        }
                        attributesXPathString = string.Format("[{0}]", attributesXPathString);
                    }

                    HtmlNodeCollection node = doc.DocumentNode.SelectNodes(string.Format("//{0}{1}", tagname, attributesXPathString));
                    if (node != null && node.Count > 0)
                    {
                        score++;
                    }
                }
            }

            return score / (_hasTagsWithAttributes.Count + _hasTagsbyPathes.Count) >= _passScore;
        }
    }
}