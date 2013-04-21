using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.Data.db;

namespace ParseKit.Data.DBWorkers
{
    public class DataWorkerConverter
    {
        public XLSWorker FromSQLToXLS(ADONETWorker adoWorker, string xlsPath)
        {
            List<string> tables = adoWorker.GetTablesList();

            XLSWorker xls = XLSWorker.Create(xlsPath);

            for (int i = 0; i < tables.Count; i++)
            {
                xls.CreateSheetAfter(tables[i]);

                string[] columns = adoWorker.GetColumnsNames(tables[i]).ToArray();
			    xls.WriteLine(columns, 1, 1);

                List<KeyValuePair<string, object>[]> rows = adoWorker.SelectAll(tables[i]);
                
                List<string[]> lines = rows.ConvertAll<string[]>(x => 
                { 
                    string[] val = new string[x.Length]; 
                    for (int j = 0; j < x.Length; j++)
			        {
			            val[j] = x[j].Value.ToString();
			        }
                    return val;
                });

                xls.InsertData(lines, 1, 2);
            }

            return xls;
        }

        public ADONETWorker FromXLSToSQL(XLSWorker adoWorker)
        {
            throw new NotImplementedException();
        }


    }
}
