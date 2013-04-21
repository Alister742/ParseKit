using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Storage
{
    public class Field
    {
        public Field(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class Line
    {
        public Line(List<Field> fields)
        {
            Fields = fields;
        }

        public Line(IEnumerable<Field> fields)
        {
            Fields = fields.ToList();
        }

        public List<Field> Fields { get; set; }
    }

    public class CommonResultFormat
    {
        public List<Line> Lines { get; set; }
    }
}
