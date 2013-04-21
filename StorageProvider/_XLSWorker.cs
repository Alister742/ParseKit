using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ParseKit.Data.DBWorkers
{
    /// <summary>
    /// Provide simple XLS document manipulations
    /// </summary>
    public class XLSWorker : IDisposable
    {
        /// <summary>
        /// Index starts from 0 and end by LinesCount - 1
        /// Use LinesCount value to know how mutch lines have current sheet
        /// </summary>
        /// <param name="idx"></param>
        /// <returns>
        /// Column values 
        /// (WARRING get_item works insanely slow, 
        /// better way is read all data and manipulate it in RAM and then save result)
        /// </returns>
        public string[] this[int idx]
        {
            get 
            {
                if (idx >= this.LinesCount)
                    throw new IndexOutOfRangeException("Index out of range");

                return SelectLine(idx + 1);
            }
            set 
            {
                if (idx >= this.LinesCount)
                    throw new IndexOutOfRangeException("Index out of range");

                WriteLine(value, 1, idx + 1); 
            }
        }

        /// <summary>
        /// Set current worksheet to worksheet with this number,
        /// or create new worksheets if not exist and select last with this number 
        /// </summary>
        public int WorksheetIdx { get { return _xlsSheet.Index; } set {
            if ((_xlsBook.Sheets[value] as Worksheet) == null)
            {
                CreateSheet(value);
            }
            else
            {
                (_xlsBook.Sheets[value] as Worksheet).Activate(); 
            }
        } }
        public int WorkSheetCount { get { return _xlsBook.Sheets.Count; } }
        public int LinesCount { get { return _xlsSheet.Rows.SpecialCells(XlCellType.xlCellTypeLastCell, XlSpecialCellsValue.xlTextValues).Row; } }
        public int ColumnsCount { get { return _xlsSheet.Rows.SpecialCells(XlCellType.xlCellTypeLastCell, XlSpecialCellsValue.xlTextValues).Column; } }

        string _filepath;

        static object _sync = new object();
        Application _xlsApp;
        Workbook _xlsBook;
        Worksheet _xlsSheet { get { return _xlsBook.ActiveSheet as Worksheet; } }
        

        public XLSWorker(string filepath, int startSheetNumb = 1)
        {
            if (!File.Exists(filepath))
                throw new FileNotFoundException("file not found");

            _filepath = filepath;
            object missing = Missing.Value;

            _xlsApp = new Application();
            _xlsBook = _xlsApp.Workbooks.Open(filepath, missing,false, missing, missing, missing, true, missing, missing, true, missing, missing, missing, missing, missing);
            
            WorksheetIdx = startSheetNumb;
        }

        private XLSWorker(Application xlapp)
        {
            _xlsApp = xlapp;
            _xlsBook = xlapp.ActiveWorkbook;
            //_xlsSheet = _xlsBook.Worksheets.get_Item(1) as Worksheet;
        }


        public static XLSWorker Create(string filepath, bool overwrite = true, /* XlFileFormat format = XlFileFormat.xlWorkbookNormal ,*/ Missing passwd = null)
        {
            if (string.IsNullOrEmpty(filepath))
                throw new ArgumentException("Bad argumenst");

            passwd = passwd ?? Missing.Value;
            object missing = Missing.Value;

            Application xlapp = new Application();
            xlapp.DisplayAlerts = false;
            xlapp.ScreenUpdating = false;

            Workbook workBook;
            if (xlapp.Workbooks.Count == 0)
            {
                workBook = xlapp.Workbooks.Add(missing);
            }
            else
            {
                workBook = xlapp.ActiveWorkbook;
            }

            if (File.Exists(filepath))
            {
                if (overwrite)
                {
                    File.Delete(filepath);
                }
                else
                {
                    throw new ArgumentException("File already exists and 'overwrite' flag is FALSE");
                }
            }

            workBook.SaveAs(filepath, XlFileFormat.xlWorkbookNormal /* format */ , passwd, missing, false, false, XlSaveAsAccessMode.xlExclusive, missing, missing, missing, missing, missing);

            return new XLSWorker(xlapp);
        }

        #region Select
        public List<string[]> SelectAll()
        {
            lock (_sync)
            {
                Range range = _xlsSheet.UsedRange;
                Array cells = range.get_Value() as Array;

                return ToStringArray(cells);
            }
        }

        public string[] SelectLine(int sLine)
        {
            if (sLine > this.LinesCount || sLine < 1)
		        throw new IndexOutOfRangeException("Index out of range");

            List<string[]> lines = SelectData(1, sLine, ColumnsCount, 1);

            if (lines == null || lines.Count == 0)
	            return null;

            return lines[0];
        }

        public List<string[]> SelectData(int sColumn, int sLine, int columnCount, int linesCount)
        {
            lock (_sync)
            {
                Range range = _xlsSheet.get_Range(_xlsSheet.Cells[sLine, sColumn] as Range, _xlsSheet.Cells[sLine + linesCount - 1, sColumn + columnCount - 1] as Range);
                Array val = range.get_Value() as Array;

                return ToStringArray(val);
            }
        }

        /// <summary>
        /// Get all values of specified column as collection
        /// </summary>
        /// <param name="column">Column numbers beginning from 1</param>
        /// <returns>Return column values in collection</returns>
        public List<string> SelectColumn(int column)
        {
            if (column < 1)
                throw new ArgumentException("Bad argumenst");

            lock (_sync)
            {
                Range range = _xlsSheet.get_Range(_xlsSheet.Cells[1, column] as Range, _xlsSheet.Cells[LinesCount, column] as Range);
                Array val = range.get_Value() as Array;

                List<string[]> values = ToStringArray(val);

                return values.ConvertAll<string>(x => { return x[0]; });
            }
        }

        private List<string[]> ToStringArray(Array ar)
        {
            List<string[]> values = new List<string[]>();

            int linesCount = ar.GetLength(0);
            int columnsCount = ar.GetLength(1);

            for (int i = 0; i < linesCount; i++)
            {
                string[] line = new string[columnsCount];
                for (int j = 0; j < columnsCount; j++)
                {
                    line[j] = ar.GetValue(i + 1, j + 1) as string ?? string.Empty;
                }
                values.Add(line);
            }
            return values;
        }
        #endregion

        #region Insert
        public void WriteLine(string[] values, int sColumn, int sLine, bool autosave = true)
        {
            lock (_sync)
            {
                if (sLine > LinesCount)
                    throw new IndexOutOfRangeException("Index out of range");

                for (int i = 0; i < values.Length; i++)
                {
                    _xlsSheet.Cells[sLine, sColumn + i] = values[i];
                }

                if (autosave)
                    Save();
            }
        }

        public void InsertLastLine(List<string> values, int columnShift = 0, bool autosave = true)
        {
            string[,] vals = new string[1, values.Count];

            for (int i = 0; i < values.Count; i++)
			{
			    vals[0,i] = values[i];
			}

            InsertLastLine(vals, columnShift, autosave);
        }

        public void InsertLastLine(string[,] values, int columnShift = 0, bool autosave = true)
        {
            if (values == null || values.Length == 0)
                throw new ArgumentException("Bad aruments");

            lock (_sync)
            {
                //int lastLine = _xlsSheet.Rows.SpecialCells(XlCellType.xlCellTypeLastCell, XlSpecialCellsValue.xlTextValues).Row;
                Range range = _xlsSheet.get_Range(_xlsSheet.Cells[LinesCount + 1, 1 + columnShift] as Range, _xlsSheet.Cells[LinesCount + 1, /* 1 + */ columnShift + values.GetLength(1) /* - 1 */] as Range);

                int line = range.Row;
                int column = range.Column;


                range.set_Value(Type.Missing, values);

                if (autosave)
                    Save();
            }
        }

        /// <summary>
        /// Insert column on specified position
        /// </summary>
        /// <param name="column">Enumeration starts from 1</param>
        public void InsertColumn(List<string> values, int sColumn, int sLine, bool autosave = true, bool autofit = true)
        {
            if (values == null)
                throw new ArgumentException("Bad aruments");

            if (values.Count == 0)
                return;

            string[] val_arr = values.ToArray();

            InsertColumn(val_arr, sColumn, sLine, autosave, autofit);
        }

        /// <summary>
        /// Insert column on specified position
        /// </summary>
        /// <param name="column">Enumeration starts from 1</param>
        public void InsertColumn(string[] values, int sColumn, int sLine, bool autosave = true, bool autofit = true)
        {
            if (values == null)
                throw new ArgumentException("Bad aruments");

            if (values.Length == 0)
                return;

            List<string[]> vals = new List<string[]>();
            for (int i = 0; i < values.Length; i++)
            {
                vals.Add(new string[] { values[i] });
            }

            InsertData(vals, sColumn, sLine, autosave, autofit);
        }

        /// <summary>
        /// Insert values in dataset, use this when you confident that all of array cells have less than 8203 symbols, otherwise use other method
        /// </summary>
        private void InsertAsRange(string[,] values, int sColumn, int sLine, bool autosave = true, bool autofit = true)
        {
            if (values == null || values.Length == 0)
                throw new ArgumentException("Bad aruments");

            lock (_sync)
            {
                Range range = _xlsSheet.get_Range(_xlsSheet.Cells[sLine, sColumn] as Range, _xlsSheet.Cells[sLine + values.GetLength(0) - 1, sColumn + values.GetLength(1) - 1] as Range);

                range.set_Value(Type.Missing, values);

                if (autofit)
                    AutoFitCells();

                if (autosave)
                    Save();
            }
        }

        public void InsertData(string[,] values, int sColumn, int sLine, bool autosave = true, bool autofit = true)
        {
            if (values == null)
                throw new ArgumentException("Bad aruments");

            if (values.Length == 0)
                return;

            List<string[]> vals = new List<string[]>();

            for (int i = 0; i < values.GetLength(0); i++)
            {
                string[] tmp_line = new string[values.GetLength(1)];
                for (int j = 0; j < values.GetLength(1); j++)
                {
                    tmp_line[j] = values[i, j];
                }
                vals.Add(tmp_line);
            }

            InsertData(vals, sColumn, sLine, autosave, autofit);
        }

        public void InsertData(string[][] values, int sColumn, int sLine, bool autosave = true, bool autofit = true)
        {
            if (values == null)
                throw new ArgumentException("Bad aruments");

            if (values.LongLength == 0)
                return;

            InsertData(values.ToList(), sColumn, sLine, autosave, autofit);
        }

        public void InsertData(List<string[]> values, int sColumn, int sLine, bool autosave = true, bool autofit = true)
        {
            if (values == null)
                throw new ArgumentException("Bad aruments");

            if (values.Count == 0)
                return;

            string[,] vals = new string[values.Count, values[0].Length];

            List<object[]> bigCells = new List<object[]>();

            for (int i = 0; i < values.Count; i++)
            {
                int columns = values[i].Length;
                for (int j = 0; j < columns; j++)
                {
                    string cell = values[i][j];
                    if (cell.Length > 8200) /* 8203..BUG? */
                    {
                        bigCells.Add(new object[] { sColumn + j, sLine /* - 1 */ + i /* + 1 */, cell });
                        vals[i, j] = string.Empty;
                    }
                    else
                    {
                        vals[i, j] = cell;
                    }
                }
            }

            InsertAsRange(vals, sColumn, sLine, autosave, autofit);

            /* write cells whos value lenght more than 8203 and can be Inserted by Range.set_value */
            for (int i = 0; i < bigCells.Count; i++)
            {
                InsertCell((int)bigCells[i][0], (int)bigCells[i][1], bigCells[i][2] as string, autosave);
            }
        }

        public void InsertCell(int column, int line, string val, bool autosave = true)
        {
            lock (_sync)
            {
                _xlsSheet.Cells[line, column] = val;

                if (autosave)
                    Save();
            }
        }
        #endregion

        #region Remove
        public void CleanLine(int line, bool autosave = true)
        {
            string[] lineValues = new string[ColumnsCount];

            for (int i = 0; i < lineValues.Length; i++)
			{
			    lineValues[i] = string.Empty;
			}

            WriteLine(lineValues, 1, line, autosave);
        }

        public void RemoveLine(int line, bool shiftUp = true, bool autosave = true)
        {
            lock (_sync)
            {
                if (line > LinesCount)
                    throw new IndexOutOfRangeException("Index out of range");

                XlDeleteShiftDirection shift = shiftUp ? XlDeleteShiftDirection.xlShiftUp : XlDeleteShiftDirection.xlShiftToLeft;

                _xlsSheet.get_Range(_xlsSheet.Cells[line, 1] as Range, _xlsSheet.Cells[line, ColumnsCount] as Range).Delete(shift);

                if (autosave)
                    Save();
            }
        }
        #endregion

        #region XLS Stuffs
        public void GetLastCoordinates(out int line, out int column)
        {
            line = _xlsSheet.UsedRange.Row;
            column = _xlsSheet.UsedRange.Column;
        }

        public void AutoFitCells()
        {
            _xlsSheet.Cells.EntireColumn.AutoFit();
        }

        public void Save()
        {
            /* asd */
            //_xlsSheet.UsedRange.NumberFormat = "@";
            _xlsBook.Save();
        }

        private void ReleaseComObj(object obj)
        {
            if (obj == null)
                return;

            try
            {
                Marshal.FinalReleaseComObject(obj);
                obj = null;
            }
            catch (Exception e)
            {
                obj = null;
                GlobalLog.Err(e, "Error when release xls com Obj");
            }
        }

        /// <summary>
        /// Create Worksheet and set current worksheet to it
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="name"></param>
        public void CreateSheet(int pos, string name = null)
        {
            int count = pos - _xlsBook.Sheets.Count;
            if (count <= 0)
            {
                (_xlsBook.Sheets[pos] as Worksheet).Activate();
                return;
            }

            _xlsBook.Sheets.Add(Missing.Value, _xlsSheet, count, Missing.Value);

            _xlsSheet.Name = name ?? string.Format("WorkSheet{0}", pos);
        }

        /// <summary>
        /// Create Worksheet after current and set current worksheet to it
        /// </summary>
        /// <param name="name"></param>
        public void CreateSheetAfter(string name = null)
        {
            _xlsBook.Sheets.Add(Missing.Value, _xlsSheet, 1, Missing.Value);
            _xlsSheet.Name = name ?? string.Format("WorkSheet{0}", _xlsSheet.Index);
        }

        /// <summary>
        /// Create Worksheet before current and set current worksheet to it
        /// </summary>
        /// <param name="name"></param>
        public void CreateSheetBefore(string name = null)
        {
            _xlsBook.Sheets.Add(_xlsSheet, Missing.Value, 1, Missing.Value);
            _xlsSheet.Name = name ?? string.Format("WorkSheet{0}", _xlsSheet.Index);
        }

        public bool TrySelectSheetByName(string name)
        {
            Worksheet sht = _xlsBook.Sheets[name] as Worksheet;
            if (sht == null)
                return false;

            sht.Activate();
            return true;
        }
        #endregion

        #region Члены IDisposable

        bool _disposed;

        public void Dispose()
        {
            if (!_disposed)
            {
                try
                {
                    if (_xlsBook != null)
                    {
                        _xlsBook.Close(false, Missing.Value, Missing.Value);
                        ReleaseComObj(_xlsBook);
                        _xlsBook = null;
                    }
                    if (_xlsApp != null)
                    {
                        _xlsApp.Quit();
                        ReleaseComObj(_xlsApp);
                        _xlsApp = null;
                    }

                    _disposed = true;

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                catch (Exception e)
                {
                    GlobalLog.Err(e, "Exception while dispose XLSWorker");
                }
            }
        }

        #endregion

        ~XLSWorker()
        {
            if (!_disposed)
            {
                Dispose();
            }
        }
    }
}
