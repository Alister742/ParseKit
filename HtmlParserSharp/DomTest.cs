//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace HtmlParserSharp
//{
//    class DomTest : TreeBuilder<T>
//    {
//        protected override void AccumulateCharacters(char[] buf, int start, int length)
//        {
//            int newLen = charBufferLen + length;
//            if (newLen > charBuffer.Length)
//            {
//                char[] newBuf = new char[newLen];
//                Array.Copy(charBuffer, newBuf, charBufferLen);
//                charBuffer = null; // release the old buffer in C++
//                charBuffer = newBuf;
//            }
//            Array.Copy(buf, start, charBuffer, charBufferLen, length);
//            charBufferLen = newLen;
//        }

//        override protected void AppendCharacters(T parent, char[] buf, int start, int length)
//        {
//            AppendCharacters(parent, new String(buf, start, length));
//        }


//        override protected void AppendIsindexPrompt(T parent)
//        {
//            AppendCharacters(parent, "This is a searchable index. Enter search keywords: ");
//        }

//        protected abstract void AppendCharacters(T parent, string text);

//        override protected void AppendComment(T parent, char[] buf, int start, int length)
//        {
//            AppendComment(parent, new String(buf, start, length));
//        }

//        protected abstract void AppendComment(T parent, string comment);

//        override protected void AppendCommentToDocument(char[] buf, int start, int length)
//        {
//            // TODO Auto-generated method stub
//            AppendCommentToDocument(new String(buf, start, length));
//        }

//        protected abstract void AppendCommentToDocument(string comment);

//        override protected void InsertFosterParentedCharacters(char[] buf, int start,
//                int length, T table, T stackParent)
//        {
//            InsertFosterParentedCharacters(new String(buf, start, length), table, stackParent);
//        }

//        protected abstract void InsertFosterParentedCharacters(string text, T table, T stackParent);
//    }
//}
