//------------------------------------------------------------------------------ 
// <copyright file="HtmlElementErrorEventArgs.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------------------- 

using System; 
using System.ComponentModel;

namespace Browser_Emulator.zSource
{ 

    /// <include file='doc\HtmlElementErrorEventArgs.uex' path='docs/doc[@for="HtmlElementErrorEventArgs"]/*' />
    /// <devdoc>
    ///    <para>EventArgs for onerror event of HtmlElement</para> 
    /// </devdoc>
    public sealed class HtmlElementErrorEventArgs : EventArgs { 
        private string description; 
        private string urlString;
        private Uri url; 
        private int lineNumber;
        private bool handled;

        internal HtmlElementErrorEventArgs(string description, string urlString, int lineNumber) 
        {
            this.description = description; 
            this.urlString = urlString; 
            this.lineNumber = lineNumber;
        } 

        /// <include file='doc\HtmlElementErrorEventArgs.uex' path='docs/doc[@for="HtmlElementErrorEventArgs.Description"]/*' />
        /// <devdoc>
        ///    <para>Description of error</para> 
        /// </devdoc>
        public string Description 
        { 
            get
            { 
                return description;
            }
        }
 
        /// <include file='doc\HtmlElementErrorEventArgs.uex' path='docs/doc[@for="HtmlElementErrorEventArgs.Handled"]/*' />
        /// <devdoc> 
        ///    <para> 
        ///       Gets or sets a value indicating whether the <see cref='System.Windows.Forms.HtmlWindow.Error'/>
        ///       event was handled. 
        ///    </para>
        /// </devdoc>
        public bool Handled {
            get { 
                return handled;
            } 
            set { 
                handled = value;
            } 
        }

        /// <include file='doc\HtmlElementErrorEventArgs.uex' path='docs/doc[@for="HtmlElementErrorEventArgs.LineNumber"]/*' />
        /// <devdoc> 
        ///    <para>Line number where error occurred</para>
        /// </devdoc> 
        public int LineNumber 
        {
            get 
            {
                return lineNumber;
            }
        } 

        /// <include file='doc\HtmlElementErrorEventArgs.uex' path='docs/doc[@for="HtmlElementErrorEventArgs.Url"]/*' /> 
        /// <devdoc> 
        ///    <para>Url where error occurred</para>
        /// </devdoc> 
        public Uri Url
        {
            get
            { 
                if (url == null)
                { 
                    url = new Uri(urlString); 
                }
                return url; 
            }
        }
    }
} 


// File provided for Reference Use Only by Microsoft Corporation (c) 2007.