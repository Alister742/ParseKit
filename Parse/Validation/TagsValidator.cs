using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Parse.Validation
{
    class TagsValidator : IPageValidator
    {
        List<string> _patterns;
        double _passScore;

        public TagsValidator(List<string> patterns, double passScore = 0.7)
        {
            _patterns = patterns;
            _passScore = passScore;
        }

        public bool Validate(string page)
        {
            int score = 0;

            List<string> tagSheme = PagePatternGrabber.GrabClassStructure(page);

            foreach (var tagClass in tagSheme)
            {
                if (_patterns.Contains(tagClass))
                    score++;
            }
            return (score / _patterns.Count) >= _passScore ? true : false;
        }
    }
}
