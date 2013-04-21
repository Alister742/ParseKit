/*
 * Copyright (c) 2007 Henri Sivonen
 * Copyright (c) 2008-2011 Mozilla Foundation
 * Copyright (c) 2012 Patrick Reisert
 *
 * Permission is hereby granted, free of charge, to any person obtaining a 
 * copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation 
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Diagnostics;


namespace Parse.DOM
{
	/// <summary>
	/// Be careful with this class. QName is the name in from HTML tokenization.
	/// Otherwise, please refer to the interface doc.
	/// </summary>
	public sealed class HtmlAttributes : IEquatable<HtmlAttributes> /* : Sax.IAttributes*/ {

		public static readonly HtmlAttributes EMPTY_ATTRIBUTES = new HtmlAttributes(AttributeName.HTML);

		private int mode;

		private int length;

		private AttributeName[] names;

		private string[] values;

		public HtmlAttributes(int mode)
		{
			this.mode = mode;
			this.length = 0;
			/*
			 * The length of 5 covers covers 98.3% of elements
			 * according to Hixie
			 */
			this.names = new AttributeName[5];
			this.values = new string[5];
		}
		

		/// <summary>
		/// Only use with a static argument
		/// </summary>
		public int GetIndex(AttributeName name)
		{
			for (int i = 0; i < length; i++)
			{
				if (names[i] == name)
				{
					return i;
				}
			}
			return -1;
		}

		public int Length
		{
			get
			{
				return length;
			}
		}

		
		public string GetLocalName(int index)
		{
			if (index < length && index >= 0)
			{
				return names[index].GetLocal(mode);
			}
			else
			{
				return null;
			}
		}

	

		public AttributeName GetAttributeName(int index)
		{
			if (index < length && index >= 0)
			{
				return names[index];
			}
			else
			{
				return null;
			}
		}

		
		public string GetURI(int index)
		{
			if (index < length && index >= 0)
			{
				return names[index].GetUri(mode);
			}
			else
			{
				return null;
			}
		}

		
		public string GetPrefix(int index)
		{
			if (index < length && index >= 0)
			{
				return names[index].GetPrefix(mode);
			}
			else
			{
				return null;
			}
		}

		public string GetValue(int index)
		{
			if (index < length && index >= 0)
			{
				return values[index];
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Only use with static argument.
		/// </summary>
		public string GetValue(AttributeName name)
		{
			int index = GetIndex(name);
			if (index == -1)
			{
				return null;
			}
			else
			{
				return GetValue(index);
			}
		}

		

		internal void AddAttribute(AttributeName name, string value)
		{
			

			if (names.Length == length)
			{
				int newLen = length << 1; // The first growth covers virtually
				// 100% of elements according to
				// Hixie
				AttributeName[] newNames = new AttributeName[newLen];
				Array.Copy(names, newNames, names.Length);
				names = newNames;
				string[] newValues = new string[newLen];
				Array.Copy(values, newValues, values.Length);
				values = newValues;
			}
			names[length] = name;
			values[length] = value;
			length++;
		}

		internal void Clear(int m)
		{
			for (int i = 0; i < length; i++)
			{
				names[i] = null;
				values[i] = null;
			}
			length = 0;
			mode = m;
			
		}

		/// <summary>
		/// This is only used for <code>AttributeName</code> ownership transfer
		/// in the isindex case to avoid freeing custom names twice in C++.
		/// </summary>
		internal void ClearWithoutReleasingContents()
		{
			for (int i = 0; i < length; i++)
			{
				names[i] = null;
				values[i] = null;
			}
			length = 0;
		}

		public bool Contains(AttributeName name)
		{
			for (int i = 0; i < length; i++)
			{
				if (name.EqualsAnother(names[i]))
				{
					return true;
				}
			}
			
			return false;
		}

		public void AdjustForMath()
		{
			mode = AttributeName.MATHML;
		}

		public void AdjustForSvg()
		{
			mode = AttributeName.SVG;
		}

		public HtmlAttributes CloneAttributes()
		{
			Debug.Assert((length == 0) || mode == 0 || mode == 3);
			HtmlAttributes clone = new HtmlAttributes(0);
			for (int i = 0; i < length; i++)
			{
				clone.AddAttribute(names[i].CloneAttributeName(), values[i]

				);
			}

			return clone; // XXX!!!
		}

		public bool Equals(HtmlAttributes other)
		{
			Debug.Assert(mode == 0 || mode == 3, "Trying to compare attributes in foreign content.");
			int otherLength = other.Length;
			if (length != otherLength)
			{
				return false;
			}
			for (int i = 0; i < length; i++)
			{
				// Work around the limitations of C++
				bool found = false;
				// The comparing just the local names is OK, since these attribute
				// holders are both supposed to belong to HTML formatting elements
				/**/
				string ownLocal = names[i].GetLocal(AttributeName.HTML);
				for (int j = 0; j < otherLength; j++)
				{
					if (ownLocal == other.names[j].GetLocal(AttributeName.HTML))
					{
						found = true;
						if (values[i] != other.values[j])
						{
							return false;
						}
					}
				}
				if (!found)
				{
					return false;
				}
			}
			return true;
		}
	}
}
