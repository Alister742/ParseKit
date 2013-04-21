using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Storage
{
    interface IStorageManager
    {
        CommonResultFormat ToCommonFormat();

        #region Select

        List<string[]> SelectAll();

        string[] SelectLine(int sLine);

        List<string[]> SelectData(int sColumn, int sLine, int columnCount, int linesCount);

        List<string> SelectColumn(int column);
       
        #endregion

        #region Insert

        void WriteLine(string[] values, int sColumn, int sLine, bool autosave = true);

        void InsertLastLine(List<string> values, int columnShift = 0, bool autosave = true);

        void InsertLastLine(string[,] values, int columnShift = 0, bool autosave = true);

        void InsertColumn(List<string> values, int sColumn, int sLine, bool autosave = true, bool autofit = true);

        void InsertColumn(string[] values, int sColumn, int sLine, bool autosave = true, bool autofit = true);

        void InsertData(string[,] values, int sColumn, int sLine, bool autosave = true, bool autofit = true);

        void InsertData(string[][] values, int sColumn, int sLine, bool autosave = true, bool autofit = true);

        void InsertData(List<string[]> values, int sColumn, int sLine, bool autosave = true, bool autofit = true);

        void InsertCell(int column, int line, string val, bool autosave = true);

        #endregion

        #region Remove

        void CleanLine(int line, bool autosave = true);

        void RemoveLine(int line, bool shiftUp = true, bool autosave = true);

        #endregion
    }
}
