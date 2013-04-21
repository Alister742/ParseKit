using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parse.DOM
{
    public interface ITreeBuilder
    {
        /// <summary>
        /// This method is called at the start of tokenization before any other
        /// methods on this interface are called. Implementations should hold the
        /// reference to the <code>Tokenizer</code> in order to set the content
        /// model flag and in order to be able to query for <code>Locator</code> data.
        /// </summary>
        /// <param name="self">The Tokenizer.</param>
        void StartTokenization(Tokenizer self);

        /// <summary>
        /// Receive a doctype token.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="publicIdentifier">The public identifier.</param>
        /// <param name="systemIdentifier">The system identifier.</param>
        /// <param name="forceQuirks">Whether the token is correct.</param>
        void Doctype(string name, string publicIdentifier, string systemIdentifier, bool forceQuirks);

        /// <summary>
        /// Receive a start tag token.
        /// </summary>
        /// <param name="eltName">The tag name.</param>
        /// <param name="attributes">The attributes.</param>
        /// <param name="selfClosing">TODO</param>
        void StartTag(ElementName eltName, HtmlAttributes attributes, bool selfClosing);

        /// <summary>
        /// Receive an end tag token.
        /// </summary>
        /// <param name="eltName">The tag name.</param>
        void EndTag(ElementName eltName);

        /// <summary>
        /// Receive a comment token. The data is junk if the<code>wantsComments()</code>
        /// returned <code>false</code>.
        /// </summary>
        /// <param name="buf">The buffer holding the data.</param>
        /// <param name="start">The offset into the buffer.</param>
        /// <param name="length">The number of code units to read.</param>
        void Comment(char[] buf, int start, int length);

        /// <summary>
        /// Receive character tokens. This method has the same semantics as the SAX
        /// method of the same name.
        /// </summary>
        /// <param name="buf">A buffer holding the data.</param>
        /// <param name="start">The offset into the buffer.</param>
        /// <param name="length">The number of code units to read.</param>
        void Characters(char[] buf, int start, int length);

        /// <summary>
        /// Reports a U+0000 that's being turned into a U+FFFD.
        /// </summary>
        void ZeroOriginatingReplacementCharacter();

        /// <summary>
        /// The end-of-file token.
        /// </summary>
        void Eof();

        /// <summary>
        /// The perform final cleanup.
        /// </summary>
        void EndTokenization();

        /// <summary>
        /// Checks if the CDATA sections are allowed.
        /// </summary>
        /// <returns><c>true</c> if CDATA sections are allowed</returns>
        bool IsCDataSectionAllowed { get; }
    }
}
