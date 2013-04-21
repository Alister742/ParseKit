using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit;
using System.Text.RegularExpressions;
using ParseKit.DOMElements;
using System.Runtime.CompilerServices;

namespace ParseKit
{
    [Flags]
    enum DeclarationType
    {
        NoAny       = 1,
        Document    = 2,
        Comment     = 4,
        CDDATA      = 8,
    }

    enum TokenType
    {
        Text,
        Declaration,
        Close,
        Usual,
    }

    enum TokenStage
    {
        CDDATA,
        CDDATAEnd,
        Script,
        ScriptEnd,
        TagStart,
        MarkupDeclaration,
        OpenMarkupDeclaration,
        Text,
        BeforeDoctype,
        Doctype,
        Comment,
        CommentEnd,
        //SelfClosingFlag,
        Name,
        BeforeAttributeName,
        AttributeName,
        BeforeEqualChar,
        EqualChar,
        BeforeFirstQuotes,
        FirstQuotes,
        LastQuotes,
        AttributeValue,
        TagEnd,
        TagContent,
        PlainText,
    }








    //WTF???
    public enum OtherTagType
    {
        UserManageTag,
        TagWithoutInnerText = 1,
        Usual = 2,
        блочные,
        строчные,
        закрываемые_вручную,
        с_ручным_порядком_следования_элементов,
    }

    /// <summary>
    /// Type of element put in stack
    /// </summary>
    enum StackTokenType
    {
        Special,
        Formatting,
        Ordinary,
    }


    class StackOpenElements
    {
        List<IElement> _openElements = new List<IElement>();
        IElement _last 
        {
            get
            {
                if (_openElements.Count == 0)
                {
                    return null;
                }
                else
                {
                    return _openElements[_openElements.Count - 1];
                };
            } 
        }

        public bool ContainsTag(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return _openElements.Find((e) => { return e.tagName == name; }) != null;
        }

        public bool Contains(IElement element)
        {
            if (element == null)
                return false;

            return _openElements.Contains(element);
        }

        public bool Contains(string tagName)
        {
            if (tagName == null)
                return false;

            for (int i = _openElements.Count - 1; i > -1; i--)
            {
                if (_openElements[i].tagName == tagName)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Remove first match element from stack, but without removing elements after
        /// </summary>
        /// <param name="name">Element name</param>
        public bool RemoveOne(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            for (int i = _openElements.Count - 1; i > -1; i--)
            {
                if (_openElements[i].tagName == name)
                {
                    _openElements.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public bool RemoveUntil(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            for (int i = _openElements.Count - 1; i > -1; i--)
            {
                if (_openElements[i].tagName == name)
                {
                    for (int j = i; j < _openElements.Count; j++)
                    {
                        _openElements.RemoveAt(j);
                    }
                    return true;
                }
            }
            return false;
        }

        public void Add(IElement node)
        {
            if (node == null)
                return;

            _openElements.Add(node);
            _last = node;
        }

        public IElement Last()
        {
            return _last;
        }

        public void RemoveLast()
        {
            if (_openElements.Count < 1)
                return;
            
            _openElements.RemoveAt(_openElements.Count - 1);
        }
    }

    class FormattedElementsList
    {
        public FormattedElementsList(StackOpenElements associatedElements)
        {
            _associatedElements = associatedElements;
        }

        int _markersCount = 0;
        int _elementFromLastMarker = 0;
        List<IElement> _formattedElements = new List<IElement>();
        StackOpenElements _associatedElements;

        public void AddElement(IElement element)
        {
            if (element == null)
                return;

            if (_elementFromLastMarker >= 3)
            {
                for (int i = _formattedElements.Count - 1; i > -1; i--)
                {
                    if (_formattedElements[i] == null)
                        break;
                    if ((element as IHTMLElement).Equals(_formattedElements[i]))
                    {
                        _formattedElements.RemoveAt(i);
                    }
                }
            }

            _formattedElements.Add(element);
            _elementFromLastMarker++;
        }

        public void AddMarker()
        {
            _formattedElements.Add(null);
            _elementFromLastMarker = 0;
            _markersCount++;
        }

        public void Reconstruct()
        {
            // Step 1: stop the algorithm when there's nothing to do.
            if (_formattedElements.Count == 0)
                return;

            // Step 2 and step 3: we start with the last element. So i is -1.
            IElement clone;
            IElement entry;

            int i =_formattedElements.Count - 1;
            entry = _formattedElements[i];
            if (entry == null || _associatedElements.Contains(entry))
                return;

            // Step 6
            while (entry != null && !_associatedElements.Contains(entry))
	        {
	            if (i == 0)
	            {
		            i--;
                    break;
	            }
                i--;
                entry = _formattedElements[i];
	        }

            while (true)
	        {
	            i++;
                entry = _formattedElements[i];

                clone = entry.CloneElement();

                _associatedElements.Add(clone);

                _formattedElements[i] = clone;

                if (clone == _formattedElements.Last())
		            break;
	        }
        }

        public IElement Last()
        {
            return _formattedElements.Last();
        }

        public void IsLastMarker()
        {

        }

        public void ClearUpLastMarker()
        {
            for (int i = _formattedElements.Count - 1; i > -1 ; i--)
			{
                if (_formattedElements[i] == null)
                    break;
                _formattedElements.RemoveAt(i);
			}
        }
    }

    public class ParseError
    {
        public string Message { get; set; }
        public string TagName { get; set; }
        public int TokenIndex { get; set; }
        public List<HtmlAttribute> Atributes { get; set; }
        public string Content { get; set; }
        public TokenType TokenType { get; set; }
        public DeclarationType DeclarationType { get; set; }
        public bool EOFState { get; set; }
        public int DocumentPosition { get; set; }
        public INode ParentElement { get; set; }
        public INode LastElement { get; set; }
        public DOMParser.InsertionMode InsertionStage { get; set; }
        public DOMParser.InsertionMode OriginalInsertionStage { get; set; }
    }

    /// <summary>
    /// Not thread safe
    /// </summary>
    public class DOMParser
    {
        public DOMParser(HtmlDocument document, bool logging = false)
        {
            _document = document;
            _InsertionMode = InsertionMode.Initial;
            _openElements = new StackOpenElements();
            _attributes = new List<HtmlAttribute>();
            _content = new StringBuilder();
            _tokenType = TokenType.Usual;
            _declarationType = DeclarationType.Comment;
            Errors = new List<ParseError>();
            _openFormattingElements = new FormattedElementsList(_openElements);
            _tokenIndex = 0;
            _logging = logging;

            InitActionMaps();

            StartConstruction();
        }

        private void InitActionMaps()
        {
            _TokenActionMap = new Action[30];
            _TokenActionMap[(int)TokenStage.CDDATA] = ScriptAction;
            _TokenActionMap[(int)TokenStage.CDDATA] = CDDATAAction;
            _TokenActionMap[(int)TokenStage.CDDATAEnd] = CDDATAEndAction;
            _TokenActionMap[(int)TokenStage.TagStart] = TagStartAction;
            _TokenActionMap[(int)TokenStage.MarkupDeclaration] = MarkupDeclarationAction;
            _TokenActionMap[(int)TokenStage.OpenMarkupDeclaration] = OpenMarkupDeclarationAction;
            _TokenActionMap[(int)TokenStage.Text] = TextAction;
            _TokenActionMap[(int)TokenStage.BeforeDoctype] = SkipSpaces;
            _TokenActionMap[(int)TokenStage.Doctype] = DoctypeNameAction;
            _TokenActionMap[(int)TokenStage.Comment] = CommentAction;
            _TokenActionMap[(int)TokenStage.CommentEnd] = CommentEndAction;
            _TokenActionMap[(int)TokenStage.Name] = NameAction;
            _TokenActionMap[(int)TokenStage.BeforeAttributeName] = SkipSpaces;
            _TokenActionMap[(int)TokenStage.AttributeName] = AttributeNameAction;
            _TokenActionMap[(int)TokenStage.BeforeEqualChar] = SkipSpaces;
            _TokenActionMap[(int)TokenStage.EqualChar] = EqualCharAction;
            _TokenActionMap[(int)TokenStage.BeforeFirstQuotes] = SkipSpaces;
            _TokenActionMap[(int)TokenStage.FirstQuotes] = FirstQuotesAction;
            _TokenActionMap[(int)TokenStage.LastQuotes] = LastQuotesAction;
            _TokenActionMap[(int)TokenStage.AttributeValue] = AttributeValueAction;
            //_TokenActionMap[(int)TokenStage.SelfClosingFlag] = SelfClosingFlagAction;
            _TokenActionMap[(int)TokenStage.TagEnd] = TagEndAction;
            _TokenActionMap[(int)TokenStage.TagContent] = TagContentAction;
            _TokenActionMap[(int)TokenStage.PlainText] = PlainTextAction;
           
            _tokenTypeMap = new Action[10];
            _tokenTypeMap[(int)TokenType.Close] = CloseElement;
            _tokenTypeMap[(int)TokenType.Declaration] = AddDeclaration;
            _tokenTypeMap[(int)TokenType.Text] = AddText;
            _tokenTypeMap[(int)TokenType.Usual] = AddToken;

            _InsActiondMap = new Action[25];
            _InsActiondMap[(int)InsertionMode.Initial] = InitialAction;
            _InsActiondMap[(int)InsertionMode.BeforeHtml] = BeforeHtmlAction;
            _InsActiondMap[(int)InsertionMode.BeforeHead] = BeforeHeadAction;
            _InsActiondMap[(int)InsertionMode.InHead] = InHeadAction;
            _InsActiondMap[(int)InsertionMode.InHeadNoscript] = InHeadNoscriptAction;
            _InsActiondMap[(int)InsertionMode.AfterHead] = AfterHeadAction;
            _InsActiondMap[(int)InsertionMode.InBody] = InBodyAction;
            _InsActiondMap[(int)InsertionMode.Text] = TextTreeAction;
            _InsActiondMap[(int)InsertionMode.InTable] = InTableAction;
            _InsActiondMap[(int)InsertionMode.InTableText] = InTableTextAction;
            _InsActiondMap[(int)InsertionMode.InCaption] = InCaptionAction;
            _InsActiondMap[(int)InsertionMode.InColumnGroup] = InColumnGroupAction;
            _InsActiondMap[(int)InsertionMode.InTableBody] = InTableBodyAction;
            _InsActiondMap[(int)InsertionMode.InRow] = InRowAction;
            _InsActiondMap[(int)InsertionMode.InCell] = InCellAction;
            _InsActiondMap[(int)InsertionMode.InSelect] = InSelectAction;
            _InsActiondMap[(int)InsertionMode.InSelectInTable] = InSelectInTableAction;
            _InsActiondMap[(int)InsertionMode.AfterBody] = AfterBodyAction;
            _InsActiondMap[(int)InsertionMode.InFrameset] = InFramesetAction;
            _InsActiondMap[(int)InsertionMode.AfterFrameset] = AfterFramesetAction;
            _InsActiondMap[(int)InsertionMode.AfterAfterBody] = AfterAfterBodyAction;
            _InsActiondMap[(int)InsertionMode.AfterAfterFrameset] = AfterAfterFramesetAction;
        }

        /*
         * правила:
         * НАЧАЛЬНЫМ_ТЕГОМ: является любая последовательность символов начинающихся с "<", 
         * после ищется имя тега, игнорируя все пустые символы
         * если имя не найдено до символа ">" то последовательность символов является обычным текстом
         * 
         * КОНЕЧНЫМ_ТЕГОМ: является любая последовательность символов начинающихся с "<",
         * далее следуют любые пустые символы и символ "/"после следует название тега(все символы кроме пробела и ">")
         * далее следуют любые символы кроме ">" которые просто игнорируются и следует символ ">"
         * 
         * КОМЕНТАРИИ: если после нахождения символа "<"
         * далее следуют символы "!--" 
         * то пока не найдена последовательность "-->" 
         * все символы являются коментариями 
         * 
         * АТТРИБУТЫ: после того как найдено имя тэга и до символа ">" все слова являются атрибутами тега
         * символы " " и "/" не находящиеся в месте Value тега являются раделающими атрибуты символами
         * символ " " в добавок к вышесказанному является разделителем Value значения если оно не заключено в "\"" или "'" символы
         * в отличии от "/" который в местах value атрибутов входит в само значение
         * "=" разделяет название атрибута и его значение только если следует после имени атрибута (через любые пустые симвлы)
         * в противном случае сам является атрибутом
         * 
         * символы " " и "/" разделяют имена тегов, имена атрибутов, 
         */
        bool _originalParseMode;
        bool _logging;
        int _tokenIndex;
        HtmlDocument _document;
        public List<ParseError> Errors { get; private set; }

        #region Tokenization
        string _name;
        List<HtmlAttribute> _attributes;
        StringBuilder _content;
        TokenType _tokenType;
        DeclarationType _declarationType;

        bool EOF { get { return _pos + 1 >= _document.OriginalText.Length; } }
        int _pos = 0; //возможна проблема с самым последним символом до EOF, символ может быть не считан но конец уже наступил

        #region Token state machine
        int _tokenState = (int)TokenStage.Text;
        static Regex[] eaters;
        Action[] _TokenActionMap;

        void GoNextState()
        {
            if (_TokenActionMap.Length < 1)
                return;

            if (_tokenState < _TokenActionMap.Length - 1)
                _tokenState++;
            else
                _tokenState = 0;
        }
        void SwitchState(TokenStage state)
        {
            _tokenState = (int)state;
        }

        public Action CurrentState
        {
            get
            {
                if (_TokenActionMap.Length < 1)
                    return EndConstruction;
                else
                    return _TokenActionMap[_tokenState];
            }
        }
        #endregion

        #region Regexes
        static Regex _ignoreSpaces = new Regex(@"\s*", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        static Regex _nameRx = new Regex(@"[^\s^\/^>]*", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        static Regex _nameChar = new Regex(@"\w", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        static Regex _text = new Regex(@"[^<]*", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        static Regex _tagEnd = new Regex(@">", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        static Regex _tagContent = new Regex(@"[^<]*", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

        static Regex _attributeName = new Regex(@"(?<equals>=*)[^\/^\s^>^=]*", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        static Regex _equalChar = new Regex(@"=", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        static Regex _quotes = new Regex(@"[""']", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        static Regex _apostroph = new Regex(@"[^']*", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        static Regex _quote = new Regex(@"[^""]*", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        static Regex _attributeValue = new Regex(@"[^\s^>]*", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

        static Regex _declaration = new Regex(@"[^>]*", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

        static Regex _doctype = new Regex(@"document", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

        static Regex _cddata = new Regex(@"[CDATA[", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        static Regex _cddataDash = new Regex(@"^]*", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        static Regex _cddataEnd = new Regex(@"]]>", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

        static Regex _commentStart = new Regex(@"\s*-\s*-", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        static Regex _commentDash = new Regex(@"[^-]*", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        static Regex _commentEnd = new Regex(@"\s*-\s*-\s*>", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

        static Regex _plainText = new Regex(@"[\s\S]*", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        static Regex _scriptDash = new Regex(@"[^<]*", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        static Regex _scriptEnd = new Regex(@"</script\s*", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        #endregion


        private void ResetToken()
        {
            _tokenIndex++;
            _name = null;
            _content = null;
            _attributes.Clear();
            _tokenType = TokenType.Usual;
            _declarationType = DeclarationType.Comment;
        }
        private void AppendCharToContent()
        {
            if (!EOF)
            {
                _content.Append(_document.OriginalText[_pos]);
                _pos++;
            }
        }
        private void InitCloseToken(string name)
        {
            _name = name;
            _tokenType = TokenType.Close;
        }
        private bool TryGetCurChar(out char c)
        {
            if (!EOF)
            {
                c = _document.OriginalText[_pos];
                return true;
            }
            c = default(char);
            return false;
        }
        private bool Dash(Regex rx, out string value)
        {
            Match result = rx.Match(_document.OriginalText, _pos);
            _pos += result.Length;
            value = result.Value;
            return result.Success;
        }
        private void SkipSpaces()
        {
            Match result = _ignoreSpaces.Match(_document.OriginalText, _pos);
            _pos += result.Length;
            GoNextState();
        }

        public void ScriptEndAction()
        {
            Match result = _scriptEnd.Match(_document.OriginalText, _pos);
            if (result.Success)
            {
                _pos += result.Length;
                AddToken();
                InitCloseToken("script");
                SwitchState(TokenStage.AttributeName);
            }
            else
            {
                AppendCharToContent();
                SwitchState(TokenStage.Script);
            }
        }
        public void ScriptAction()
        {
            //Match result = _script.Match(_document.OriginalText, _pos);
            string val;
            if (Dash(_scriptDash, out val))
            {
                _content.Append(val);
                SwitchState(TokenStage.ScriptEnd);
            }
        }
        public void CDDATAAction()
        {
            Match result = _cddataDash.Match(_document.OriginalText, _pos);
            if (result.Success)
            {
                _pos += result.Length;
                _content.Append(result.Value);
                SwitchState(TokenStage.CDDATAEnd);
            }
        }
        public void CDDATAEndAction()
        {
            Match result = _cddataEnd.Match(_document.OriginalText, _pos);
            if (result.Success)
            {
                _pos += result.Length;
                SwitchState(TokenStage.Text);
                AddToken();
            }
            else
                SwitchState(TokenStage.CDDATA);
        }
        private void TagStartAction()
        {
            char c;
            if (TryGetCurChar(out c) && c == '<')
            {
                _pos++;
                if (TryGetCurChar(out c))
                {
                    if (c == '!')
                    {
                        _pos++;
                        _tokenType = TokenType.Declaration;
                        SwitchState(TokenStage.MarkupDeclaration);
                        return;
                    }
                    if (c == '<')
                    {
                        _content.Append("<");
                        _tokenType = TokenType.Text;
                        return;
                    }
                    if (!_nameChar.IsMatch(c.ToString()))
                    {
                        _pos++;
                        _content.Append("<" + c);
                        SwitchState(TokenStage.Text);
                        return;
                    }
                    if (c == '/')
                    {
                        _pos++;
                        _tokenType = TokenType.Close;
                    }
                    SwitchState(TokenStage.Name);
                }
            }
            //commented because <* eat ALL chars and there are only two ways: next char is > or EOF
            //else
                //SwitchState(TokenStage.Text);
        }
        private void MarkupDeclarationAction()
        {
            Match result = _doctype.Match(_document.OriginalText, _pos);
            if (result.Success)
            {
                _pos += result.Length;
                _declarationType = DeclarationType.Document;
                SwitchState(TokenStage.BeforeDoctype);
                return;
            }

            result = _cddata.Match(_document.OriginalText, _pos);
            if (result.Success)
            {
                _pos += result.Length;
                _declarationType = DeclarationType.CDDATA;
                SwitchState(TokenStage.CDDATA);
                return;
            }

            result = _commentStart.Match(_document.OriginalText, _pos);
            if (result.Success)
            {
                _pos += result.Length;
                SwitchState(TokenStage.Comment);
                return;
            }

            SwitchState(TokenStage.OpenMarkupDeclaration);
        }
        private void OpenMarkupDeclarationAction()
        {
            EatDeclarationContent();
            AddToken();
        }
        private void TextAction()
        {
            Match result = _text.Match(_document.OriginalText, _pos);

            _pos += result.Length;
            _content.Append(result.Value);
            _tokenType = TokenType.Text;
            if (_content.Length > 0)
            {
                _tokenType = TokenType.Text;
                AddToken();
            }
            SwitchState(TokenStage.TagStart);
        }
        private void EatDeclarationContent()
        {
            Match result = _declaration.Match(_document.OriginalText, _pos);
            if (result.Success)
            {
                _pos += result.Length;
                _content.Append(result.Value);
                AddToken();

                char c;
                if (TryGetCurChar(out c))
                {
                    if (c == '>')
                        _pos++;
                    SwitchState(TokenStage.Text);
                }
            }
        }
        private void DoctypeNameAction()
        {
            EatDeclarationContent();
            AddToken();
        }
        private void CommentAction()
        {
            Match result = _commentDash.Match(_document.OriginalText, _pos);
            if (result.Success)
            {
                _pos += result.Length;
                _content.Append(result.Value);
                SwitchState(TokenStage.CommentEnd);
            }
        }
        private void CommentEndAction()
        {
            Match result = _commentEnd.Match(_document.OriginalText, _pos);
            if (result.Success)
            {
                _pos += result.Length;
                SwitchState(TokenStage.Text);
                AddToken();
            }
            else
                SwitchState(TokenStage.Comment);
        }
        //private void CloseSlashAction()
        //{
        //    char c;
        //    if (TryGetCurChar(out c))
        //    {
        //        if (c == '/')
        //        {
        //            _pos++;
        //            _tokenType = TokenType.Close;
        //        }
        //        SwitchState(TokenStage.BeforeName);
        //    }
        //}
        private void NameAction()
        {
            Match result = _nameRx.Match(_document.OriginalText, _pos);
            if (result.Success)
            {
                _pos += result.Length;
                _name = result.Value;
                SwitchState(TokenStage.BeforeAttributeName);
                return;
            }
            else
            {
                _content.Append("<");
                SwitchState(TokenStage.Text);
            }
        }
        private void AttributeNameAction()
        {
            Match result = _attributeName.Match(_document.OriginalText, _pos);
            if (result.Success)
            {
                string name = null;
                if (result.Groups["equals"].Length <= 1)
                {
                    name = result.Value;
                }
                else if (result.Groups["equals"].Length > 1)
                {
                    name = result.Groups["equals"].Value[0].ToString();
                }
                _pos += name.Length;
                _attributes.Add(new HtmlAttribute(name, null));
                SwitchState(TokenStage.BeforeEqualChar);
            }
            else
            {
                char c;
                if (TryGetCurChar(out c))
                {
                    if (c == '>')
                    {
                        SwitchState(TokenStage.TagEnd);
                    }
                    else
                    {
                        _pos++;
                        SwitchState(TokenStage.BeforeAttributeName);
                    }
                }
            }
        }
        private void EqualCharAction()
        {
            char c;
            if (TryGetCurChar(out c))
            {
                if (c == '=')
                {
                    _pos++;
                    SwitchState(TokenStage.BeforeFirstQuotes);
                }
                else
                    SwitchState(TokenStage.BeforeAttributeName);
            }
        }
        private void FirstQuotesAction()
        {
            Match result = _quotes.Match(_document.OriginalText, _pos);
            if (result.Success)
            {
                _pos += result.Length;

                Match match = null;
                if (result.Value == "'")
                {
                    match = _apostroph.Match(_document.OriginalText, _pos);
                }
                else if (result.Value == "\"")
                {
                    match = _quote.Match(_document.OriginalText, _pos);
                }

                _attributes.Last().value = match.Value;
                SwitchState(TokenStage.LastQuotes);
            }
            else
                SwitchState(TokenStage.AttributeValue);
        }
        private void LastQuotesAction()
        {
            Match result = _quotes.Match(_document.OriginalText, _pos);
            _pos += result.Length;
            SwitchState(TokenStage.BeforeAttributeName);
        }
        private void AttributeValueAction()
        {
            Match result = _attributeValue.Match(_document.OriginalText, _pos);
            _pos += result.Length;

            _attributes.Last().value = result.Value;

            SwitchState(TokenStage.BeforeAttributeName);
        }
        //private void SelfClosingFlagAction()
        //{
        //    Match result = _ignoreSpaces.Match(_document.OriginalText, _pos);
        //    _pos += result.Length;
        //}
        private void TagEndAction()
        {
            char c;
            if (TryGetCurChar(out c))
            {
                if (c == '>')
                {
                    _pos++;
                    if (_tokenType == TokenType.Close)
                    {
                        AddToken();
                        SwitchState(TokenStage.Text);
                    }
                    else
                    {
                        if (_name.NoncaseEqual("script"))
                        {
                            SwitchState(TokenStage.Script);
                        }
                        else SwitchState(TokenStage.TagContent);
                    }
                }
                else
                    SwitchState(TokenStage.Text);
            }
        }
        private void TagContentAction()
        {
            Match result = _tagContent.Match(_document.OriginalText, _pos);
            _content.Append(result.Value);
            _pos += result.Length;
            AddToken();
            SwitchState(TokenStage.Text);
        }
        private void PlainTextAction()
        {
            Match result = _plainText.Match(_document.OriginalText, _pos);
            _name = "plaintext";
            _content.Append(result.Value);
            _pos += result.Length;
            AddToken();
        }
        #endregion

        #region Tree building
        StackOpenElements _openElements;
        IElement _currentNode { get { return _openElements.Last(); } }
        FormattedElementsList _openFormattingElements;
        InsertionMode _originalInsertionMode;
        InsertionMode _InsertionMode;
        //Action[] _tokenTypeMap;
        Action[] _InsActiondMap;
        INode _formPointer;
        bool _framesetOK; //UNUSABLE now and it allow frame element puts in tags like button(who cant have frames)
        INode _headPointer;

        List<HtmlScriptElement> _finishParsingScr = new List<HtmlScriptElement>();
        List<HtmlScriptElement> _pendingScr = new List<HtmlScriptElement>();
        List<HtmlScriptElement> _waitingExecScr = new List<HtmlScriptElement>();

        public enum InsertionMode
        {
            Initial, //Doctype

            BeforeHtml,
            BeforeHead,
            InHead,
            InHeadNoscript,
            AfterHead,
            InBody,
            Text,
            InTable,
            InTableText,
            InCaption,
            InColumnGroup,
            InTableBody,
            InRow,
            InCell,
            InSelect,
            InSelectInTable,
            AfterBody,
            InFrameset,
            AfterFrameset,
            AfterAfterBody,
            AfterAfterFrameset,
        }

        #region Tree building

        #region Quite simply

        void UniveraslBuild()
        {
            if (_tokenType == TokenType.Declaration)
            {
                if (_declarationType == DeclarationType.CDDATA)
                {
                    AddCDDATA(_content.ToString());
                    return;
                }
                if (_declarationType == DeclarationType.Comment)
                {
                    AddComment(_content.ToString());
                    return;
                }
                if (_declarationType == DeclarationType.Document)
                {
                    AddDoctype(_name);
                    return;
                }
            }

            if (_tokenType == TokenType.Text)
            {
                AddText(_content.ToString());
            }

            if (_tokenType == TokenType.Usual)
            {
                AddElement(new HtmlElement(_name, _attributes, _content.ToString()));
            }

            if (_tokenType == TokenType.Close)
            {
                CloseAllUntil(_name.ToLower());
            }
        }

        #endregion

        //#region W3C standart insertion
        //public void InitialAction()
        //{
        //    if (_tokenType == TokenType.Declaration)
        //    {
        //        if (_declarationType == DeclarationType.CDDATA)
        //        {
        //            _document.appendChild(new CDATAElement(_content.ToString()));
        //            return;
        //        }
        //        if (_declarationType == DeclarationType.Comment)
        //        {
        //            _document.appendChild(new HtmlComment(_content.ToString()));
        //            return;
        //        }
        //        if (_declarationType == DeclarationType.Document)
        //        {
        //            _document.appendChild(new HtmlDeclaration(_content.ToString()));
        //            _InsertionMode = InsertionMode.BeforeHtml;
        //            return;
        //        }
        //    }

        //    _InsertionMode = InsertionMode.BeforeHtml;
        //    AddToken();
        //}

        //public void BeforeHtmlAction()
        //{
        //    if (_tokenType == TokenType.Declaration)
        //    {
        //        if (_declarationType == DeclarationType.CDDATA)
        //        {
        //            _document.appendChild(new CDATAElement(_content.ToString()));
        //            return;
        //        }
        //        if (_declarationType == DeclarationType.Comment)
        //        {
        //            _document.appendChild(new HtmlComment(_content.ToString()));
        //            return;
        //        }
        //        if (_declarationType == DeclarationType.Document)
        //        {
        //            GenerateError("Unexpected DOCTYPE token");
        //            return;
        //        }
        //    }
        //    if (_tokenType == TokenType.Text || string.IsNullOrEmpty(_name))
        //    {
        //        _document.appendChild(new HtmlText(_content.ToString()));
        //        return;
        //    }
        //    if (_tokenType == TokenType.Usual || _tokenType == TokenType.Close && (_name.LowerEqual("head") || _name.LowerEqual("body") || _name.LowerEqual("html") || _name.LowerEqual("br")))
        //    {
        //        _document.Root = new HtmlHtmlElement(_name, _attributes);
        //        _lastElement = _document.Root;
        //        _lastElement = _document.Root;
        //        _openElements.Add(_document.Root);
        //        _InsertionMode = (int)InsertionMode.BeforeHead;

        //        if (_name.LowerEqual("html"))
        //        {
        //            //IAttribute attr = _attributes.Find((a) => { return a.name == "manifest" ? true : false; });
        //            //if (attr != null && !string.IsNullOrEmpty(attr.value))
        //            //{
        //            //    //Possibly to use application cache selection algorithm by W3C, for performance inc.    
        //            //}
        //        }
        //        else
        //        {
        //            AddToken();
        //        }
        //        return;
        //    }
        //    if (_tokenType == TokenType.Close)
        //    {
        //        GenerateError("Unexpected end token");
        //        return;
        //    }
        //}

        //public void BeforeHeadAction()
        //{
        //    if (_tokenType == TokenType.Declaration)
        //    {
        //        if (_declarationType == DeclarationType.CDDATA)
        //        {
        //            _lastElement.appendChild(new CDATAElement(_content.ToString()));
        //            return;
        //        }
        //        if (_declarationType == DeclarationType.Comment)
        //        {
        //            _lastElement.appendChild(new HtmlComment(_content.ToString()));
        //            return;
        //        }
        //        if (_declarationType == DeclarationType.Document)
        //        {
        //            GenerateError("Unexpected DOCTYPE token");
        //            return;
        //        }
        //    }
        //    if (_tokenType == TokenType.Text || string.IsNullOrEmpty(_name))
        //    {
        //        _lastElement.appendChild(new HtmlText(_content.ToString()));
        //        return;
        //    }
        //    if (_tokenType == TokenType.Usual)
        //    {
        //        if (_name.LowerEqual("html"))
        //        {
        //            foreach (var attribute in _attributes)
        //            {
        //                if (!string.IsNullOrEmpty(attribute.name))
        //                    _document.Root.setAttributeNode(attribute);
        //            }
        //            _InsertionMode = InsertionMode.InBody;
        //            return;
        //        }
        //        if (_name.LowerEqual("head"))
        //        {
        //            _headPointer = _lastElement.appendChild(new HtmlHeadElement(_attributes));
        //            _InsertionMode = InsertionMode.InHead;
        //            return;
        //        }
        //    }

        //    if (_tokenType == TokenType.Usual || _tokenType == TokenType.Close && (_name.LowerEqual("head") || _name.LowerEqual("body") || _name.LowerEqual("html") ||_name.LowerEqual("br")))
        //    {
        //        _lastElement.appendChild(new HtmlHeadElement());
        //        _InsertionMode = InsertionMode.InBody;
        //        AddToken();
        //        return;
        //    }

        //    if (_tokenType == TokenType.Close)
        //    {
        //        GenerateError("Unexpected end token");
        //        return;
        //    }
        //}

        //public void InHeadAction()
        //{
        //    if (_tokenType == TokenType.Declaration)
        //    {
        //        if (_declarationType == DeclarationType.CDDATA)
        //        {
        //            _lastElement.appendChild(new CDATAElement(_content.ToString()));
        //            return;
        //        }
        //        else if (_declarationType == DeclarationType.Comment)
        //        {
        //            _lastElement.appendChild(new HtmlComment(_content.ToString()));
        //            return;
        //        }
        //        if (_declarationType == DeclarationType.Document)
        //        {
        //            GenerateError("Unexpected DOCTYPE token");
        //            return;
        //        }
        //    }
        //    if (_name.LowerEqual("html"))
        //    {
        //        AddHtmlTagAttributes(); ////???????????????
        //        return;
        //    }


        //    /*
        //     * WARRING SOME OF NEXT ELEMENTS IS VOID AND AUTOCLOSING, NEED TO CHECK FOR IN AND CLOSE ELEMENTS IF REQUED
        //     * */
        //    /*
        //     * WARRING SOME OF NEXT ELEMENTS IS VOID AND AUTOCLOSING, NEED TO CHECK FOR IN AND CLOSE ELEMENTS IF REQUED
        //     * Acknowledge the token's self-closing flag, if it is set.
        //     * */
        //    if (_name.LowerEqual("base"))
        //    {
        //        _lastElement.appendChild(new HtmlBaseElement());
        //        CloseSingle();
        //        return;
        //    }
        //    if (_name.LowerEqual("basefont"))
        //    {
        //        _lastElement.appendChild(new HtmlBaseFontElement());
        //        CloseSingle();
        //        return;
        //    }
        //    if (_name.LowerEqual("link"))
        //    {
        //        _lastElement.appendChild(new HtmlLinkElement());
        //        CloseSingle();
        //        return;
        //    }
        //    if (_name.LowerEqual("bgsound") || _name.LowerEqual("command"))
        //    {
        //        _lastElement.appendChild(new HtmlElement());
        //        CloseSingle();
        //        return;
        //    }

        //    if (_name.LowerEqual("meta"))
        //    {
        //        /*
        //         * If the element has a charset attribute, 
        //         * and its value is either a supported ASCII-compatible character encoding or a UTF-16 encoding, 
        //         * and the confidence is currently tentative, 
        //         * then change the encoding to the encoding given by the value of the charset attribute.
        //         * 
        //         * Otherwise, 
        //         * if the element has an http-equiv attribute whose value is an ASCII case-insensitive match for the string "Content-Type", 
        //         * and the element has a content attribute, 
        //         * and applying the algorithm for extracting a character encoding from a meta element to that attribute's value returns 
        //         * a supported ASCII-compatible character encoding or a UTF-16 encoding, 
        //         * and the confidence is currently tentative, then change the encoding to the extracted encoding.
        //         */
        //        _lastElement.appendChild(new HtmlMetaElement());
        //        CloseSingle();
        //        return;
        //    }

        //    if ( _name.LowerEqual("style"))
        //    {
        //        _lastElement.appendChild(new HtmlStyleElement());
        //        _originalInsertionMode = _InsertionMode;
        //        _InsertionMode = (int)InsertionMode.Text;
        //        return;
        //    }
        //    if (_name.LowerEqual("noframes"))
        //    {
        //        _lastElement.appendChild(new HtmlElement());
        //        _originalInsertionMode = _InsertionMode;
        //        _InsertionMode = (int)InsertionMode.Text;
        //        return;
        //    }
        //    if (_name.LowerEqual("noscript"))
        //    {
        //        _lastElement.appendChild(new HtmlElement());
        //        _InsertionMode = (int)InsertionMode.InHeadNoscript;
        //        return;
        //    }

        //    if (_name.LowerEqual("title"))
        //    {
        //        _lastElement.appendChild(new HtmlTitleElement());
        //        _originalInsertionMode = _InsertionMode;
        //        _InsertionMode = (int)InsertionMode.Text;
        //    }
        //    if (_name.LowerEqual("script"))
        //    {
        //        //Mark the element as being "parser-inserted" and unset the element's "force-async" flag.
        //        //If the parser was originally created for the HTML fragment parsing algorithm, then mark the script element as "already started". (fragment case)
        //        HandleScript(_lastElement.appendChild(new HtmlScriptElement()) as HtmlScriptElement);
        //        _originalInsertionMode = _InsertionMode;
        //        _InsertionMode = (int)InsertionMode.Text;
        //        return;
        //    }
        //    if (_name.LowerEqual("head"))
        //    {
        //        GenerateError();
        //        return;
        //    }

        //    if (_tokenType == TokenType.Usual || _tokenType == TokenType.Close && (_name.LowerEqual("body") || _name.LowerEqual("html") || _name.LowerEqual("br")))
        //    {
        //        CloseSingle("head");
        //        _InsertionMode = (int)InsertionMode.AfterHead;
        //        AddToken();
        //        return;
        //    }

        //    if (_tokenType == TokenType.Close)
        //    {
        //        if (_name.LowerEqual("head"))
        //        {
        //            CloseSingle();
        //            _InsertionMode = (int)InsertionMode.AfterHead;
        //            return;
        //        }

        //        GenerateError("Unexpected end token");
        //        return;
        //    }
        //}

        //public void InHeadNoscriptAction()
        //{
        //    if (_tokenType == TokenType.Declaration)
        //    {
        //        if (_declarationType == DeclarationType.CDDATA)
        //        {
        //            _lastElement.appendChild(new CDATAElement(_content.ToString()));
        //            return;
        //        }
        //        else if (_declarationType == DeclarationType.Comment)
        //        {
        //            _lastElement.appendChild(new HtmlComment(_content.ToString()));
        //            return;
        //        }
        //        if (_declarationType == DeclarationType.Document)
        //        {
        //            GenerateError("Unexpected DOCTYPE token");
        //            return;
        //        }
        //    }
        //    if (_tokenType == TokenType.Usual)
        //    {
        //        if (_name.LowerEqual("html"))
        //        {
        //            AddHtmlTagAttributes(); ////???????????????
        //            return;
        //        }


        //        /*
        //         * WARRING SOME OF NEXT ELEMENTS IS VOID AND AUTOCLOSING, NEED TO CHECK FOR IN AND CLOSE ELEMENTS IF REQUED
        //         * */
        //        /*
        //         * WARRING SOME OF NEXT ELEMENTS IS VOID AND AUTOCLOSING, NEED TO CHECK FOR IN AND CLOSE ELEMENTS IF REQUED
        //         * Acknowledge the token's self-closing flag, if it is set.
        //         * */
        //        if (_name.LowerEqual("base"))
        //        {
        //            _lastElement.appendChild(new HtmlBaseElement());
        //            return;
        //        }
        //        if (_name.LowerEqual("basefont"))
        //        {
        //            _lastElement.appendChild(new HtmlBaseFontElement());
        //            return;
        //        }
        //        if (_name.LowerEqual("link"))
        //        {
        //            _lastElement.appendChild(new HtmlLinkElement());
        //            return;
        //        }
        //        if (_name.LowerEqual("bgsound") || _name.LowerEqual("command"))
        //        {
        //            _lastElement.appendChild(new HtmlElement());
        //            return;
        //        }

        //        if (_name.LowerEqual("head") || _name.LowerEqual("noscript"))
        //        {
        //            GenerateError("Unexpected tag");
        //            return;
        //        }
        //    }
        //    if (_tokenType == TokenType.Close)
        //    {
        //        if (_name.LowerEqual("noscript"))
        //        {
        //            CloseSingle();
        //            _InsertionMode = (int)InsertionMode.InHead;
        //            return;
        //        }

        //        GenerateError("Unexpected end tag");
        //        return;
        //    }

        //    GenerateError("Unexpected tag");
        //    CloseSingle("noscript");
        //    _InsertionMode = (int)InsertionMode.InHead;
        //    AddToken();
        //}

        ////good alg but need simplify text
        //public void AfterHeadAction()
        //{
        //    if (_tokenType == TokenType.Declaration)
        //    {
        //        if (_declarationType == DeclarationType.CDDATA)
        //        {
        //            _lastElement.appendChild(new CDATAElement(_content.ToString()));
        //            return;
        //        }
        //        else if (_declarationType == DeclarationType.Comment)
        //        {
        //            _lastElement.appendChild(new HtmlComment(_content.ToString()));
        //            return;
        //        }
        //        if (_declarationType == DeclarationType.Document)
        //        {
        //            GenerateError("Unexpected DOCTYPE token");
        //            return;
        //        }
        //    }
        //    if (_tokenType == TokenType.Usual)
        //    {
        //        if (_name.LowerEqual("html"))
        //        {
        //            AddHtmlTagAttributes(); ////???????????????
        //            return;
        //        }
        //        if (_name.LowerEqual("body"))
        //        {
        //            _lastElement.appendChild(new HtmlBodyElement());
        //             //Set the frameset-ok flag to "not ok".
        //            _InsertionMode = (int)InsertionMode.InBody;
        //            return;
        //        }
        //        if (_name.LowerEqual("frameset"))
        //        {
        //            _lastElement.appendChild(new HtmlFrameSetElement());
        //            _InsertionMode = (int)InsertionMode.InFrameset;
        //            return;
        //        }
        //        if (_name.LowerEqual("base") || _name.LowerEqual("basefont") || _name.LowerEqual("bgsound") || _name.LowerEqual("link") || _name.LowerEqual("meta") || _name.LowerEqual("noframes") || _name.LowerEqual("script") ||_name.LowerEqual("style") || _name.LowerEqual("title"))
        //        {
        //            GenerateError();

        //            INode temp = _lastElement;
        //            _lastElement = _headPointer;
        //            InHeadAction();
        //            CloseSingle(_name); //сразу закрыть этот токен чтобы он не висел в списке открытых тегов
        //            _lastElement = temp;

        //        }
        //        if (_name.LowerEqual("head"))
        //        {
        //            GenerateError();
        //            return;
        //        }
        //    }
        //    if (_tokenType == TokenType.Close)
        //    {
        //        if (!_name.LowerEqual("body") && !_name.LowerEqual("html") && !_name.LowerEqual("br"))
        //        {
        //            GenerateError("Unexpected end token");
        //            return;
        //        }
        //    }

        //    _lastElement.appendChild(new HtmlBodyElement());
        //    _InsertionMode = (int)InsertionMode.InBody;
        //    AddToken();
        //}

        //public void InBodyAction()
        //{
        //    if (_tokenType == TokenType.Declaration)
        //    {
        //        if (_declarationType == DeclarationType.CDDATA)
        //        {
        //            _lastElement.appendChild(new CDATAElement(_content.ToString()));
        //            return;
        //        }
        //        else if (_declarationType == DeclarationType.Comment)
        //        {
        //            _lastElement.appendChild(new HtmlComment(_content.ToString()));
        //            return;
        //        }
        //        if (_declarationType == DeclarationType.Document)
        //        {
        //            GenerateError("Unexpected DOCTYPE token");
        //            return;
        //        }
        //    }
        //    if (_tokenType == TokenType.Text)
        //    {
        //        ///добавить атрибуты в рут элемент
        //        _lastElement.appendChild(new HtmlText());
        //        return;
        //    }
        //    if (_tokenType == TokenType.Usual)
        //    {
        //        if (_name.LowerEqual("a"))
        //        {
                    


























        //        }
        //        if (_name.LowerEqual("html"))
        //        {
        //            AddHtmlTagAttributes(); ////???????????????
        //            return;
        //        }
        //        if (_name.LowerEqual("base") || _name.LowerEqual("basefont") || _name.LowerEqual("bgsound") || _name.LowerEqual("command") || _name.LowerEqual("link") || _name.LowerEqual("meta") || _name.LowerEqual("noframes") || _name.LowerEqual("script") || _name.LowerEqual("style") || _name.LowerEqual("title"))
        //        {
                    
        //            InHeadAction();
        //            return;
        //        }
        //        if (_name.LowerEqual("body"))
        //        {
        //            GenerateError();
        //            //забрать все НОВЫЕ не существующие аттрибуты
        //            return;
        //        }
        //        if (_name.LowerEqual("frameset"))
        //        {
        //            GenerateError();
        //            _lastElement.appendChild(new HtmlFrameSetElement());
        //            _InsertionMode = (int)InsertionMode.InFrameset;
        //            return;
        //        }
        //        if (true)//"address", "article", "aside", "blockquote", "center", "details", "dialog", "dir", "div", "dl", "fieldset", "figcaption", "figure", "footer", "header", "hgroup", "menu", "nav", "ol", "p", "section", "summary", "ul")
        //        {
        //            CloseSingle("p");
        //            _lastElement.appendChild(new HtmlElement());
        //            //open stack insert
        //            return;
        //        }
        //        if (true)//"h1", "h2", "h3", "h4", "h5", "h6")
        //        {
        //            CloseSingle("p");
        //            if (_lastElement.tagName == "")//"h1", "h2", "h3", "h4", "h5", "h6")
        //            {
        //                GenerateError("Double open <h> tokens");
        //                CloseSingle();//"h1", "h2", "h3", "h4", "h5", "h6")
        //            }
        //            _lastElement.appendChild(new HtmlHeadingElement());
        //            return;
        //        }
        //        if (true)//"pre", "listing")
        //        {
        //            CloseSingle("p");
        //            _lastElement.appendChild(new HtmlElement());
        //            return;
        //        }
        //        if (true)//"form")
        //        {
        //            if (_formPointer != null)
        //            {
        //                GenerateError();
        //                return;
        //            }
        //            CloseSingle("p");
        //            _formPointer = _lastElement.appendChild(new HtmlElement());
        //            return;
        //        }
        //        //возможно это элемент из списка самозакрываемых тегов и можно вынести в отдельный метод
        //        if (true)//"li")
        //        {
        //            for (int i = _openElements.Count - 1; i > -1; i--)
        //            {
        //                IHTMLElement temp = _openElements[i];
        //                if (temp.tagName.LowerEqual("li"))
        //                {
        //                    CloseSingle("li");
        //                    break;
        //                }
        //                else if (HTMLTagContainer.IsSpecial(temp.tagName) && !(_name.LowerEqual("address") || _name.LowerEqual("div") || _name.LowerEqual("p")))
        //                {
        //                    break;
        //                }
        //            }
        //            CloseSingle("p");
        //            _lastElement.appendChild(new HtmlLIElement());
        //            return;
        //        }
        //        //возможно это элемент из списка самозакрываемых тегов и можно вынести в отдельный метод
        //        if (true)//"dd", "dt"
        //        {
        //            for (int i = _openElements.Count - 1; i > -1; i--)
        //            {
        //                IHTMLElement temp = _openElements[i];
        //                if (temp.tagName.LowerEqual("dt"))
        //                {
        //                    CloseSingle("dt");
        //                    break;
        //                }
        //                else if (temp.tagName.LowerEqual("dd"))
        //                {
        //                    CloseSingle("dd");
        //                    break;
        //                }
        //                else if (HTMLTagContainer.IsSpecial(temp.tagName) && !(_name.LowerEqual("address") || _name.LowerEqual("div") || _name.LowerEqual("p")))
        //                {
        //                    break;
        //                }
        //            }
        //            CloseSingle("p");
        //            _lastElement.appendChild(new HtmlLIElement());
        //            return;
        //        }
        //        if (_name.LowerEqual("plaintext"))
        //        {
        //            if (_lastElement.tagName == "plaintext")
        //            {
        //                //ADD _content in _lastElement!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //            }
        //            else
        //            {
        //                CloseSingle("p");
        //                _lastElement.appendChild(new HtmlElement("plaintext"));
        //                SwitchState(TokenStage.PlainText);
        //            }
        //            return;
        //        }
        //        if (_name.LowerEqual("button"))
        //        {

        //            CloseSingle("button");
        //            //Reconstruct the active formatting elements, if any.
        //            _lastElement.appendChild(new HtmlButtonElement());
        //            //_framesetOK = false;
        //            return;
        //        }
        //    }
        //    if (_tokenType == TokenType.Close)
        //    {
        //        if (true)//An end tag whose tag name is one of: "address", "article", "aside", "blockquote", "button", "center", "details", "dialog", "dir", "div", "dl", "fieldset", "figcaption", "figure", "footer", "header", "hgroup", "listing", "menu", "nav", "ol", "pre", "section", "summary", "ul"
        //        {
        //            //Закрыть вместе со всеми вложенными -- алгоритм в W3C, возможно ли другой вариант, где закрывается только тэг из середины стека -- неизвестно.
        //            CloseSingle();
        //            return;
        //        }
        //        if (_name.LowerEqual("body"))
        //        {
        //            //do nothing
        //            return;
        //        }
        //        if (_name.LowerEqual("html"))
        //        {
        //            //do nothing
        //            return;
        //        }
        //        if (_name.LowerEqual("form"))
        //        {
        //            _formPointer = null;
        //            CloseSingle();
        //            return;
        //        }
        //        if (_name.LowerEqual("p"))
        //        {
        //            CloseSingle();
        //            return;
        //        }
        //        if (_name.LowerEqual("li"))
        //        {
        //            CloseSingle();
        //            return;
        //        }
        //        if (true)//"dd", "dt"
        //        {
        //            CloseSingle();
        //            return;
        //        }
        //        if (true)//"h1", "h2", "h3", "h4", "h5", "h6"
        //        {
        //            CloseSingle();
        //            return;
        //        }

        //    }

        //}

        //public void TextTreeAction()
        //{
        //    if (_tokenType == TokenType.Close)
        //    {
        //        _InsertionMode = _originalInsertionMode;
        //        CloseSingle();
        //        return;
        //    }
        //    AppendText();
        //}

        //public void InTableAction()
        //{
        //    if (_tokenType == TokenType.Declaration)
        //    {
        //        if (_declarationType == DeclarationType.CDDATA)
        //        {
        //            _lastElement.appendChild(new CDATAElement(_content.ToString()));
        //            return;
        //        }
        //        else if (_declarationType == DeclarationType.Comment)
        //        {
        //            _lastElement.appendChild(new HtmlComment(_content.ToString()));
        //            return;
        //        }
        //        if (_declarationType == DeclarationType.Document)
        //        {
        //            GenerateError("Unexpected DOCTYPE token");
        //            return;
        //        }
        //    }

        //    if (_tokenType == TokenType.Text)
        //    {
        //        _originalInsertionMode = _InsertionMode;
        //        _InsertionMode = InsertionMode.InTableText;
        //        AddToken();
        //        return;
        //    }

        //    if (_tokenType == TokenType.Usual)
        //    {
        //        if (_name.LowerEqual("caption"))
        //        {
                    
















        //        }
        //        return;
        //    }

        //    if (_tokenType == TokenType.Close)
        //    {

        //        return;
        //    }

        //}

        //public void InTableTextAction()
        //{
        //}

        //public void InCaptionAction()
        //{
        //}

        //public void InColumnGroupAction()
        //{
        //}

        //public void InTableBodyAction()
        //{
        //}

        //public void InRowAction()
        //{
        //    if (_tokenType == TokenType.Usual)
        //    {
        //        if (_name.LowerEqual("")) //"caption", "col", "colgroup", "tbody", "td", "tfoot", "th", "thead", "tr"
        //        {
        //            if (!(StackContains("td") || StackContains("th")))
        //            {
        //                GenerateError();
        //            }
        //            else
        //            {
        //                CloseTheCell();
        //                AddToken();
        //            }
        //            return;
        //        }
        //    }

        //    if (_tokenType == TokenType.Close)
        //    {
                
        //    }
        //}




        //public void InCellAction()
        //{
        //    if (_tokenType == TokenType.Usual)
        //    {
        //        if (_name.LowerEqual("")) //"caption", "col", "colgroup", "tbody", "td", "tfoot", "th", "thead", "tr"
        //        {
        //            if (!(StackContains("td") || StackContains("th")))
        //            {
        //                GenerateError();
        //            }
        //            else
        //            {
        //                CloseTheCell();
        //                AddToken();
        //            }
        //            return;
        //        }
        //    }

        //    if (_tokenType == TokenType.Close)
        //    {
        //        if (_name.LowerEqual("td") || _name.LowerEqual("th"))//"td", "th"
        //        {
        //            if (!StackContains(_name.ToLower()))
        //            {
        //                GenerateError();
        //            }
        //            else
        //            {
        //                while (HTMLTagContainer.IsImpliedEndTag(_currentNode.tagName))
        //                {
        //                    CloseLast();
        //                }
        //                if (!_name.LowerEqual(_currentNode.tagName))
        //                {
        //                    GenerateError("Document invalid");
        //                }
        //                CloseAllUntil(_name.ToLower());
        //                _openFormattingElements.ClearUpLastMarker();
        //                _InsertionMode = InsertionMode.InRow;

        //            }
        //            return;
        //        }
        //        if (_name.LowerEqual(""))//"body", "caption", "col", "colgroup", "html"
        //        {
        //            GenerateError();
        //            return;
        //        }
        //        if (_name.LowerEqual(""))//"table", "tbody", "tfoot", "thead", "tr"
        //        {
        //            if (!StackContains(_name.ToLower()))
        //            {
        //                GenerateError();
        //            }
        //            else
        //            {
        //                CloseTheCell();
        //                AddToken();
        //            }
        //            return;
        //        }
        //    }

        //    InBodyAction();
        //}
        //private void CloseTheCell()
        //{
        //    if (StackContains("td"))
        //    {
        //        CloseAllUntil("td");
        //    }
        //    else
        //    {
        //        CloseAllUntil("th");
        //    }
        //}





        //public void InSelectAction()
        //{
        //    if (_tokenType == TokenType.Declaration)
        //    {
        //        if (_declarationType == DeclarationType.CDDATA)
        //        {
        //            AddCDDATA(_content.ToString());
        //            return;
        //        }
        //        else if (_declarationType == DeclarationType.Comment)
        //        {
        //            AddComment(_content.ToString());
        //            return;
        //        }
        //        if (_declarationType == DeclarationType.Document)
        //        {
        //            GenerateError("Unexpected doctype");
        //            return;
        //        }
        //    }

        //    if (_tokenType == TokenType.Text)
        //    {
        //        AddText(_content.ToString());
        //        return;
        //    }

        //    if (_tokenType == TokenType.Usual)
        //    {
        //        if (_name.LowerEqual("html"))
        //        {
        //            InBodyAction();
        //            return;
        //        }
        //        if (_name.LowerEqual("option"))
        //        {
        //            if (_currentNode.nodeName == "option")
        //            {
        //                CloseAllUntil("option");
        //            }
        //            AddElement(new HtmlOptionElement());
        //            return;
        //        }
        //        if (_name.LowerEqual("optgroup"))
        //        {
        //            if (_currentNode.nodeName == "option")
        //            {
        //                CloseAllUntil("option");
        //            }
        //            if (_currentNode.nodeName == "optgroup")
        //            {
        //                CloseAllUntil("optgroup");
        //            }
        //            AddElement(new HtmlOptGroupElement());
        //            return;
        //        }
        //        if (_name.LowerEqual("select"))
        //        {
        //            _tokenType = TokenType.Close;
        //            AddToken();
        //            return;
        //        }
        //        if (_name.LowerEqual("input") || _name.LowerEqual("keygen") || _name.LowerEqual("textarea"))
        //        {
        //            GenerateError();
        //            if (StackContains("optgroup") || StackContains("option"))
        //            {
        //                //WARRNING: perhaps infinitile loop if have no select element!!!
        //                CloseAllUntil("select");
        //                AddToken();
        //                return;
        //            }
        //            return;
        //        }
        //        if (_name.LowerEqual("script"))
        //        {
        //            InHeadAction();
        //            return;
        //        }

        //        //AddElement(new HtmlElement());
        //        //return;
        //    }

        //    if (_tokenType == TokenType.Close)
        //    {
        //        if (_name.LowerEqual("select"))
        //        {
        //            CloseAllUntil("select");
        //            _InsertionMode = _originalInsertionMode;
        //            return;
        //        }

        //        //CloseAllUntil(_name.ToLower());
        //        //return;
        //    }

        //    GenerateError();
        //    /*
        //     * IN W3C STANDART RULE IS: ignore the token,
        //     * but in this case there are situation
        //     * when invalid html have no expected endtags 
        //     * and all next elements will be simply skipped!
        //     */
        //}





        //public void InSelectInTableAction()
        //{
        //    if (_tokenType == TokenType.Usual)
        //    {
        //        if (HTMLTagContainer.IsTableScopeTag(_name));
        //        {
        //            GenerateError();
        //            //Act as if an end tag with the tag name "select" had been seen, and reprocess the token.

        //            //CloseAllUntil("select");
        //            //AddToken();
        //            return;
        //        }
        //    }

        //    if (_tokenType == TokenType.Close)
        //    {
        //        if (HTMLTagContainer.IsTableScopeTag(_name))
        //        {
        //            GenerateError();

        //            if (StackContains(_name.ToLower()))
        //            {
        //                //Act as if an end tag with the tag name "select" had been seen, and reprocess the token.
        //                //CloseAllUntil("select");
        //                //AddToken();
        //            }
        //            return;
        //        }
        //    }

        //    InSelectAction();
        //}




        //public void AfterBodyAction()
        //{
        //    if (_tokenType == TokenType.Declaration)
        //    {
        //        if (_declarationType == DeclarationType.CDDATA)
        //        {
        //            _document.Root.appendChild(new CDATAElement(_content.ToString()));
        //            return;
        //        }
        //        else if (_declarationType == DeclarationType.Comment)
        //        {
        //            _document.Root.appendChild(new HtmlComment(_content.ToString()));
        //            return;
        //        }
        //        if (_declarationType == DeclarationType.Document)
        //        {
        //            GenerateError("Unexpected doctype");
        //            return;
        //        }
        //    }

        //    if (_tokenType == TokenType.Text)
        //    {
        //        InBodyAction();
        //        return;
        //    }

        //    if (_tokenType == TokenType.Usual)
        //    {
        //        if (_name.LowerEqual("html"))
        //        {
        //            InBodyAction();
        //            return;
        //        }
        //    }

        //    if (_tokenType == TokenType.Close)
        //    {
        //        if (_name.LowerEqual("html"))
        //        {
        //            if (_originalParseMode)
        //            {
        //                GenerateError();
        //            }
        //            else
        //            {
        //                _InsertionMode = InsertionMode.AfterAfterBody;
        //            }
        //            return;
        //        }
        //    }

        //    GenerateError();
        //    _InsertionMode = InsertionMode.InBody;
        //    AddToken();
        //}
        
        //public void InFramesetAction()
        //{
        //    if (_tokenType == TokenType.Declaration)
        //    {
        //        if (_declarationType == DeclarationType.CDDATA)
        //        {
        //            AddCDDATA(_content.ToString());
        //            return;
        //        }
        //        else if (_declarationType == DeclarationType.Comment)
        //        {
        //            AddComment(_content.ToString());
        //            return;
        //        }
        //        if (_declarationType == DeclarationType.Document)
        //        {
        //            GenerateError("Unexpected doctype");
        //            return;
        //        }
        //    }

        //    if (_tokenType == TokenType.Text)
        //    {
        //        AddText(_content.ToString());
        //        return;
        //    }

        //    if (_tokenType == TokenType.Usual)
        //    {
        //        if (_name.LowerEqual("html"))
        //        {
        //            InBodyAction();
        //            return;
        //        }
        //        if (_name.LowerEqual("frameset"))
        //        {
        //            AddElement(new HtmlFrameSetElement());
        //            return;
        //        }
        //        if (_name.LowerEqual("frame"))
        //        {
        //            AddElement(new HtmlFrameElement(), false);
        //            return;
        //        }
        //    }
        //    if (_tokenType == TokenType.Close)
        //    {
        //        if (_name.LowerEqual("frameset"))
        //        {
        //            if (_openElements.Last() == _document.Root)
        //            {
        //                GenerateError();
        //            }
        //            else
        //            {
        //                //W3C
        //                //_openElements.CloseLast(); 
        //                _openElements.RemoveUntil("frameset");
        //            }
        //            if (!_originalParseMode && _currentNode != null && _currentNode.tagName == "frameset")
        //            {
        //                _InsertionMode = InsertionMode.AfterFrameset;
        //            }                     
        //            return;
        //        }
        //    }

        //}

        //public void AfterFramesetAction()
        //{
        //    if (_tokenType == TokenType.Declaration)
        //    {
        //        if (_declarationType == DeclarationType.CDDATA)
        //        {
        //            AddCDDATA(_content.ToString());
        //            return;
        //        }
        //        else if (_declarationType == DeclarationType.Comment)
        //        {
        //            AddComment(_content.ToString());
        //            return;
        //        }
        //        if (_declarationType == DeclarationType.Document)
        //        {
        //            GenerateError("Unexpected doctype");
        //            return;
        //        }
        //    }

        //    if (_tokenType == TokenType.Text)
        //    {
        //        AddText(_content.ToString());
        //        return;
        //    }

        //    if (_tokenType == TokenType.Usual)
        //    {
        //        if (_name.LowerEqual("html"))
        //        {
        //            InBodyAction();
        //            return;
        //        }
        //        if (_name.LowerEqual("noframes"))
        //        {
        //            InHeadAction();
        //            return;
        //        }
        //    }

        //    if (_tokenType == TokenType.Close)
        //    {
        //        if (_name.LowerEqual("html"))
        //        {
        //            _InsertionMode = InsertionMode.AfterAfterFrameset;
        //            return;
        //        }
        //    }
        //    GenerateError();
        //}

        //public void AfterAfterBodyAction()
        //{
        //    if (_tokenType == TokenType.Declaration)
        //    {
        //        if (_declarationType == DeclarationType.CDDATA)
        //        {
        //            AddCDDATA(_content.ToString());
        //            return;
        //        }
        //        else if (_declarationType == DeclarationType.Comment)
        //        {
        //            AddComment(_content.ToString());
        //            return;
        //        }
        //        if (_declarationType == DeclarationType.Document)
        //        {
        //            InBodyAction();
        //            return;
        //        }
        //    }
        //    if (_tokenType == TokenType.Text)
        //    {
        //        InBodyAction();
        //        return;
        //    }
        //    if (_tokenType == TokenType.Usual)
        //    {
        //        if (_name.LowerEqual("html"))
        //        {
        //            InBodyAction();
        //            return;
        //        }
        //    }

        //    GenerateError();
        //    _InsertionMode = InsertionMode.InBody;
        //    AddToken();
        //}

        //public void AfterAfterFramesetAction()
        //{
        //    if (_tokenType == TokenType.Declaration)
        //    {
        //        if (_declarationType == DeclarationType.CDDATA)
        //        {
        //            AddCDDATA(_content.ToString());
        //            return;
        //        }
        //        else if (_declarationType == DeclarationType.Comment)
        //        {
        //            AddComment(_content.ToString());
        //            return;
        //        }
        //        if (_declarationType == DeclarationType.Document)
        //        {
        //            InBodyAction();
        //            return;
        //        }
        //    }
        //    if (_tokenType == TokenType.Text)
        //    {
        //        InBodyAction();
        //        return;
        //    }
        //    if (_tokenType == TokenType.Usual)
        //    {
        //        if (_name.LowerEqual("html"))
        //        {
        //            InBodyAction();
        //            return;
        //        }

        //        if (_name.LowerEqual("noframes"))
        //        {
        //            InHeadAction();
        //            return;
        //        }
        //    }
        //    GenerateError();
        //}
        //#endregion
        #endregion

        #region Script handling
        void HandleScript(HtmlScriptElement script)
        {
            bool haveSrc = _attributes.Find((a) => { return a.name == "src"; }) != null;
            bool haveDefer = _attributes.Find((a) => { return a.name == "defer"; }) != null;
            bool haveAsync = _attributes.Find((a) => { return a.name == "async"; }) == null;

            if (haveSrc && haveDefer && !haveAsync)
            {
                _finishParsingScr.Add(new HtmlScriptElement()); //BLANK
            }
            else if (haveSrc && !haveAsync)
            {
                _pendingScr.Add(new HtmlScriptElement()); //BLANK
            }
            else if (!haveSrc)
            {
                if (!TryExecuteScript())
                {
                    _pendingScr.Add(new HtmlScriptElement());
                }
            }
            else if (haveSrc && !haveAsync)
            {
                if (!TryExecuteScript())
                {
                    _pendingScr.Add(new HtmlScriptElement());
                }
            }
            else if (haveSrc)
            {
                if (!TryExecuteScript())
                {
                    _pendingScr.Add(new HtmlScriptElement());
                }
            }
            else
            {
                //The user agent must immediately execute the script block, even if other scripts are already executing.
                //нужно ли добавлять в лист скриптов если не возникла ошибка исполнения??
                if (!TryExecuteScript())
                {
                    _pendingScr.Add(new HtmlScriptElement());
                }
            }
            //в любом случае нужно добавить скрипт или в лист скриптов после загрузки документа для исполнения или добавить элемент в список скриптов к документу
        }

        private bool TryExecuteScript(HtmlScriptElement script)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Elements inserting
        void AddText(string text)
        {
            if (_lastElement is HtmlText)
            {
                (_lastElement as HtmlText).appendData(text);
            }
            else
                _lastElement.appendChild(new HtmlText(text));
        }
        void AddComment(string data)
        {
            AddTreeNode(new HtmlComment(data));
        }
        void AddCDDATA(string data)
        {
            AddTreeNode(new CDATAElement(_content.ToString()));
        }
        void AddDoctype(string name)
        {
            AddTreeNode(new HtmlDeclaration(name));
        }
        void AddDeclaration(DeclarationType forbidden)
        {
            if (_declarationType == DeclarationType.Document && !forbidden.HasFlag(DeclarationType.Document))
            {
                AddTreeNode(new HtmlDeclaration(_name));
            }
            else if (_declarationType == DeclarationType.CDDATA && !forbidden.HasFlag(DeclarationType.CDDATA))
            {
                AddTreeNode(new CDATAElement(_content.ToString()));
            }
            else if (_declarationType == DeclarationType.Comment && !forbidden.HasFlag(DeclarationType.Comment))
            {
                AddTreeNode(new HtmlComment(_content.ToString()));
            }

            if (!forbidden.HasFlag(DeclarationType.NoAny))
            {
                GenerateError("Unexpected declaration");
            }
        }

        void AddElement(IElement element, bool stackInsert = true)
        {
            AddTreeNode(element);

            if (!stackInsert || HTMLTagContainer.IsVoidTag(element.tagName))
            {
                if (_content.Length > 0)
                {
                    AddTreeNode(new HtmlText(_content.ToString()));
                }
            }
            else
            {
                _openElements.Add(element);
            }
        }
        void AddTreeNode(INode element)
        {
            _openElements.Last().appendChild(element);
        }

        bool CloseSingle(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;
            if (_attributes.Count > 0)
                GenerateError("End tag with attributes");

            return _openElements.RemoveOne(name);
        }
        bool CloseAllUntil(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;
            if (_attributes.Count > 0)
                GenerateError("End tag with attributes");

            return _openElements.RemoveUntil(name);
        }
        private void CloseLast()
        {
            _openElements.RemoveLast();
        }

        bool StackContains(string tagName)
        {
            return _openElements.Contains(tagName);
        }
        #endregion
        #endregion

        //DONT WORK
        private OtherTagType GetTagType(string _name)
        {
            throw new NotImplementedException();
            //if (LazyHTMLResourceContainer.IgnorTags.Contains(_name))
            //{
            //    return OtherTagType.Usual;
            //}
            //return OtherTagType.Usual;
        }

        private void StartConstruction()
        {
            _originalParseMode = true;

            while (!EOF)
            {
                CurrentState.Invoke();
            }
            EndConstruction();
        }

        void AddToken()
        {
            _InsActiondMap[(int)_InsertionMode].Invoke();

            ResetToken();
        }

        void EndConstruction()
        {
            //Pop all the nodes off the stack of open elements.
            //Execute all cripts in the 'list of scripts that will execute when the document has finished parsing' one by one
            //Fire a simple event that bubbles named 'DOMContentLoaded' at the Document.
            //Fire all elements who waiting document load event, выполнить все предшествующее load ивенту документа
            //fire a simple event named load at the Document's Window object, but with its target set to the Document object (and the currentTarget set to the Window object).
            //Fire a trusted event with the name pageshow at the Window object of the Document, but with its target set to the Document object (and the currentTarget set to the Window object)
            //ДОКУМЕНТ ГОТОВ!


            if (_InsertionMode == InsertionMode.InSelect || _InsertionMode == InsertionMode.InFrameset)
            {
                if (_currentNode != _document.Root)
                {
                    GenerateError("InFrameset mode EOF with not root node as current");
                }
            }


            if (!string.IsNullOrEmpty(_name))
            {
                ///
            }

            _originalParseMode = false;
        }

        public void ParseFragment(string shtml, INode tokenFor)
        {

        }

        void GenerateError(string message = null)
        {
            if (_logging)
            {
                ParseError e = new ParseError();
                e.Message = message ?? "Enexpected token";
                e.Atributes = _attributes;
                e.InsertionStage = (InsertionMode)_InsertionMode;
                e.Content = _content.ToString();
                e.DeclarationType = _declarationType;
                e.DocumentPosition = _pos;
                e.EOFState = EOF;
                e.LastElement = _lastElement;
                e.TagName = _name;
                e.ParentElement = _lastElement;
                e.TokenIndex = _tokenIndex;
                e.TokenType = _tokenType;
                e.OriginalInsertionStage = (InsertionMode)_originalInsertionMode;
                Errors.Add(e);
            }
        }
    }
}
