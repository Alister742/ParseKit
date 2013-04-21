using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Storage;

namespace Parse
{
    class ParseScheme
    {
        public string FieldName { get; set; }
        public object FielParseParametter { get; set; }
        public IParseAction ParseFieldAction { get; set; }

        public object ParseField()
        {
            try
            {
                return ParseFieldAction.ParseField(FielParseParametter);
            }
            catch (Exception ex)
            {
                Core.GlobalLog.Err(ex);
            }
            return null;
        }

        //CommonResultFormat 
    }
}
