using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ParseKit.Data.DBWorkers
{
    public class CSVWorker : IDisposable
    {
        public const string Separator = "|&$%#|";

        public Encoding Encoding { get; set; }

        static object _sync = new object();

        FileStream _syncStream { get { lock (_sync) if (_stream != null) return _stream; else throw new ObjectDisposedException("Stream object was disposed, create new worker"); } }
        FileStream _stream;

        public CSVWorker(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
                throw new ArgumentException("Bad file path argument");

            this.Encoding = Encoding.UTF8;

            _stream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        }

        public void WriteLines(List<string[]> lines)
        {
            if (lines == null || lines.Count ==0)
                return;
            StringBuilder sb = new StringBuilder();
            foreach (var line in lines)
			{
                if (line.Length == 0)
                    continue;

			    sb.Append(line[0]);
                for (int j = 1; j < line.Length; j++)
			    {
                    sb.AppendFormat("{0}{1}",  Separator, line[j]);
			    }
                sb.Append("\r\n");
			}

            WriteData(sb.ToString());
        }

        public void WriteLine(params string[] columns)
        {
            if (columns == null || columns.Length ==0)
                return;

            StringBuilder sb = new StringBuilder(columns[0]);

            for (int i = 1; i < columns.Length; i++)
			{
			    sb.AppendFormat("{0}{1}", Separator, columns[i]);
			}
            sb.Append("\r\n");

            WriteData(sb.ToString());
        }

        public List<string[]> ReadAll()
        {
            string data = ReadData();
            string[] lines = data.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);

            List<string[]> splittedLines = new List<string[]>();

            for (int i = 0; i < lines.Length; i++)
            {
                splittedLines.Add(lines[i].Split(new string[] { Separator }, StringSplitOptions.None));
            }
            return splittedLines;
        }

        private void WriteData(string data)
        {
            byte[] buff = Encoding.GetBytes(data);
            _syncStream.Position = _stream.Length;
            _syncStream.Write(buff, 0, buff.Length);
        }

        private string ReadData()
        {
            byte[] data = null;
            byte[] buffer = new byte[4 * 1024];

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    //response.ContentLength
                    int readed;
                    while ((readed = _syncStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, readed);
                    }
                    data = ms.GetBuffer();
                }
                return Encoding.GetString(data);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #region Члены IDisposable

        public void Dispose()
        {
            if (_syncStream != null)
            {
                _stream.Close();
                _stream.Dispose();
                _stream = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        #endregion
    }
}
