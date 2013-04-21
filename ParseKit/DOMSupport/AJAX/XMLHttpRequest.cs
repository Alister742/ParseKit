using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements._Classes.Events;

namespace ParseKit.DOM.AJAX
{

class XMLHttpRequest : XMLHttpRequestEventTarget {

    public XMLHttpRequest     (XMLHttpRequestOptions options = null)
    {

    }

  // event handler
  public EventHandler onreadystatechange;
  #region Interface members
  // states
  public const short UNSENT = 0;
  public const short OPENED = 1;
  public const short HEADERS_RECEIVED = 2;
  public const short LOADING = 3;
  public const short DONE = 4;
  public short readyState { get; private set; }

  // request
  public void open(string method, string url, bool async = true, string? user = null, string? password = null)
  {
      internalAbort();
      State previousState = m_state;
      m_state = State.UNSENT;
      m_error = false;
      m_uploadComplete = false;

      // clear stuff from possible previous load
      clearResponse();
      clearRequest();

      Debug.Assert(m_state == State.UNSENT);

      if (!isValidHTTPToken(method))
      {
          ec = SYNTAX_ERR;
          return;
      }

      if (!isAllowedHTTPMethod(method))
      {
          ec = SECURITY_ERR;
          return;
      }

      // FIXME: Convert this to check the isolated world's Content Security Policy once webkit.org/b/104520 is solved.
      bool shouldBypassMainWorldContentSecurityPolicy = false;
      if (scriptExecutionContext().isDocument())
      {
          Document document = (Document)(scriptExecutionContext());
          shouldBypassMainWorldContentSecurityPolicy = document.frame().script().shouldBypassMainWorldContentSecurityPolicy();
      }
      if (!shouldBypassMainWorldContentSecurityPolicy && !scriptExecutionContext().contentSecurityPolicy().allowConnectToSource(url))
      {
          // FIXME: Should this be throwing an exception?
          ec = SECURITY_ERR;
          return;
      }

      if (!async && scriptExecutionContext().isDocument())
      {
          if (document().settings() && !document().settings().syncXHRInDocumentsEnabled())
          {
              GlobalMembersXMLHttpRequest.logConsoleError(scriptExecutionContext(), "Synchronous XMLHttpRequests are disabled for this page.");
              ec = INVALID_ACCESS_ERR;
              return;
          }

          // Newer functionality is not available to synchronous requests in window contexts, as a spec-mandated
          // attempt to discourage synchronous XHR use. responseType is one such piece of functionality.
          // We'll only disable this functionality for HTTP(S) requests since sync requests for local protocols
          // such as file: and data: still make sense to allow.
          if (url.protocolIsInHTTPFamily() && m_responseTypeCode != ResponseTypeCode.ResponseTypeDefault)
          {
              GlobalMembersXMLHttpRequest.logConsoleError(scriptExecutionContext(), "Synchronous HTTP(S) requests made from the window context cannot have XMLHttpRequest.responseType set.");
              ec = INVALID_ACCESS_ERR;
              return;
          }
      }

      m_method = uppercaseKnownHTTPMethod(method);

      m_url = url;

      m_async = async;

      Debug.Assert(!m_loader);

      // Check previous state to avoid dispatching readyState event
      // when calling open several times in a row.
      if (previousState != State.OPENED)
          changeState(State.OPENED);
      else
          m_state = State.OPENED;
  }

  public void setRequestHeader(ByteString header, ByteString value);
  public long timeout { get; set; }
  public bool withCredentials { get; set; }
  public XMLHttpRequestUpload upload { get; private set; }
  public void send(ArrayBufferView? data = null);
  public void send(Blob data = null);
  public void send(Document data = null);
  public void send(string? data = null);
  public void send(FormData? data = null);
  public void abort();

  // response
  public short status { get; private set; }
  public ByteString statusText { get; private set; }
  public ByteString? getResponseHeader(ByteString header);
  public ByteString getAllResponseHeaders();
  public void overrideMimeType(string mime);
  public XMLHttpRequestResponseType responseType { get; set; }
  public object response { get; private set; }
  public string responseText { get; private set; }
  public Document? responseXML { get; private set; }
  #endregion

  private void internalAbort()
  {
      bool hadLoader = m_loader;

      m_error = true;

      // FIXME: when we add the support for multi-part XHR, we will have to think be careful with this initialization.
      m_receivedLength = 0;

      if (hadLoader)
      {
          m_loader.cancel();
          m_loader = 0;
      }

      m_decoder = 0;

      InspectorInstrumentation.didFailXHRLoading(scriptExecutionContext(), this);

      if (hadLoader)
          dropProtection();
  }

        private RefPtr<TextResourceDecoder> m_decoder = new RefPtr<TextResourceDecoder>();
        private long m_receivedLength;
    	private bool m_error;
        private RefPtr<ThreadableLoader> m_loader = new RefPtr<ThreadableLoader>();

    /*
    	private OwnPtr<XMLHttpRequestUpload> m_upload = new OwnPtr<XMLHttpRequestUpload>();

	private KURL m_url = new KURL();
	private string m_method;
	private HTTPHeaderMap m_requestHeaders = new HTTPHeaderMap();
	private RefPtr<FormData> m_requestEntityBody = new RefPtr<FormData>();
	private string m_mimeTypeOverride;
	private bool m_async;
	private bool m_includeCredentials;

	private uint m_timeoutMilliseconds;

	private RefPtr<Blob> m_responseBlob = new RefPtr<Blob>();


	private State m_state;

	private ResourceResponse m_response = new ResourceResponse();
	private string m_responseEncoding;



	private StringBuilder m_responseBuilder = new StringBuilder();
	private bool m_createdDocument;
	private RefPtr<Document> m_responseDocument = new RefPtr<Document>();

	private RefPtr<SharedBuffer> m_binaryResponseBuilder = new RefPtr<SharedBuffer>();
	private RefPtr<ArrayBuffer> m_responseArrayBuffer = new RefPtr<ArrayBuffer>();



	private bool m_uploadEventsAllowed;
	private bool m_uploadComplete;

	private bool m_sameOriginRequest;

	// Used for onprogress tracking
	

	private uint m_lastSendLineNumber;
	private string m_lastSendURL;
	private ExceptionCode m_exceptionCode = new ExceptionCode();

	private EventTargetData m_eventTargetData = new EventTargetData();

	private GlobalMembersXMLHttpRequest.XMLHttpRequestProgressEventThrottle m_progressEventThrottle = new GlobalMembersXMLHttpRequest.XMLHttpRequestProgressEventThrottle();

	// An enum corresponding to the allowed string values for the responseType attribute.
	private ResponseTypeCode m_responseTypeCode;

	private RefPtr<SecurityOrigin> m_securityOrigin = new RefPtr<SecurityOrigin>();

    */



}
}
