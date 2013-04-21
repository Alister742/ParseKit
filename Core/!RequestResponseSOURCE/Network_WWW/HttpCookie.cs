﻿using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Security.Permissions;
using System.Web.Configuration;
using System;

namespace SE_Feed_AI.Network_WWW
{
    public sealed class HttpCookie
    {
        private String _name;
        private String _path = "/";
        private bool _secure;
        private bool _httpOnly;
        private String _domain;
        private bool _expirationSet;
        private DateTime _expires;
        private String _stringValue;
        private HttpValueCollection _multiValue;
        private bool _changed;
        private bool _added;

        internal HttpCookie()
        {
            _changed = true;
        }

        /* 
         * Constructor - empty cookie with name
         */

        /// <devdoc> 
        ///    <para>
        ///       Initializes a new instance of the <see cref="System.Web.HttpCookie"> 
        ///       class. 
        ///    </see></para>
        /// </devdoc> 
        public HttpCookie(String name)
        {
            _name = name;

            SetDefaultsFromConfig();
            _changed = true;
        }

        /*
         * Constructor - cookie with name and value 
         */

        /// <devdoc>
        ///    <para> 
        ///       Initializes a new instance of the <see cref="System.Web.HttpCookie">
        ///       class. 
        ///    </see></para> 
        /// </devdoc>
        public HttpCookie(String name, String value)
        {
            _name = name;
            _stringValue = value;

            SetDefaultsFromConfig();
            _changed = true;
        }

        private void SetDefaultsFromConfig()
        {
            HttpCookiesSection config = RuntimeConfig.GetConfig().HttpCookies;
            _secure = config.RequireSSL;
            _httpOnly = config.HttpOnlyCookies;

            if (config.Domain != null && config.Domain.Length > 0)
                _domain = config.Domain;
        }

        /*
         * Whether the cookie contents have changed 
         */
        internal bool Changed
        {
            get { return _changed; }
            set { _changed = value; }
        }

        /* 
         * Whether the cookie has been added
         */
        internal bool Added
        {
            get { return _added; }
            set { _added = value; }
        }

        /* 
         * Cookie name 
         */

        /// <devdoc>
        ///    <para>
        ///       Gets
        ///       or sets the name of cookie. 
        ///    </para>
        /// </devdoc> 
        public String Name
        {
            get { return _name; }
            set
            {
                _name = value;
                _changed = true;
            }
        }

        /* 
         * Cookie path 
         */

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the URL prefix to transmit with the
        ///       current cookie. 
        ///    </para>
        /// </devdoc> 
        public String Path
        {
            get { return _path; }
            set
            {
                _path = value;
                _changed = true;
            }
        }

        /* 
         * 'Secure' flag 
         */

        /// <devdoc>
        ///    <para>
        ///       Indicates whether the cookie should be transmitted only over HTTPS.
        ///    </para> 
        /// </devdoc>
        public bool Secure
        {
            get { return _secure; }
            set
            {
                _secure = value;
                _changed = true;
            }
        }

        /// <devdoc>
        ///    <para> 
        ///       Indicates whether the cookie should have HttpOnly attribute 
        ///    </para>
        /// </devdoc> 
        public bool HttpOnly
        {
            get { return _httpOnly; }
            set
            {
                _httpOnly = value;
                _changed = true;
            }
        }

        /* 
         * Cookie domain
         */

        /// <devdoc> 
        ///    <para>
        ///       Restricts domain cookie is to be used with. 
        ///    </para> 
        /// </devdoc>
        public String Domain
        {
            get { return _domain; }
            set
            {
                _domain = value;
                _changed = true;
            }
        }

        /*
         * Cookie expiration 
         */

        /// <devdoc>
        ///    <para> 
        ///       Expiration time for cookie (in minutes).
        ///    </para> 
        /// </devdoc> 
        public DateTime Expires
        {
            get
            {
                return (_expirationSet ? _expires : DateTime.MinValue);
            }

            set
            {
                _expires = value;
                _expirationSet = true;
                _changed = true;
            }
        }

        /*
         * Cookie value as string
         */

        /// <devdoc> 
        ///    <para> 
        ///       Gets
        ///       or 
        ///       sets an individual cookie value.
        ///    </para>
        /// </devdoc>
        public String Value
        {
            get
            {
                if (_multiValue != null)
                    return _multiValue.ToString(false);
                else
                    return _stringValue;
            }

            set
            {
                if (_multiValue != null)
                {
                    // reset multivalue collection to contain
                    // single keyless value 
                    _multiValue.Reset();
                    _multiValue.Add(null, value);
                }
                else
                {
                    // remember as string
                    _stringValue = value;
                }
                _changed = true;
            }
        }

        /* 
         * Checks is cookie has sub-keys
         */

        /// <devdoc> 
        ///    <para>Gets a
        ///       value indicating whether the cookie has sub-keys.</para> 
        /// </devdoc> 
        public bool HasKeys
        {
            get { return Values.HasKeys(); }
        }

        private bool SupportsHttpOnly(HttpContext context)
        {
            if (context != null && context.Request != null)
            {
                HttpBrowserCapabilities browser = context.Request.Browser;
                return (browser != null && (browser.Type != "IE5" || browser.Platform != "MacPPC"));
            }
            return false;
        }

        /*
         * Cookie values as multivalue collection
         */

        /// <devdoc> 
        ///    <para>Gets individual key:value pairs within a single cookie object.</para> 
        /// </devdoc>
        public NameValueCollection Values
        {
            get
            {
                if (_multiValue == null)
                {
                    // create collection on demand
                    _multiValue = new HttpValueCollection();

                    // convert existing string value into multivalue 
                    if (_stringValue != null)
                    {
                        if (_stringValue.IndexOf('&') >= 0 || _stringValue.IndexOf('=') >= 0)
                            _multiValue.FillFromString(_stringValue);
                        else
                            _multiValue.Add(null, _stringValue);

                        _stringValue = null;
                    }
                }

                _changed = true;

                return _multiValue;
            }
        }

        /*
         * Default indexed property -- lookup the multivalue collection 
         */

        /// <devdoc> 
        ///    <para>
        ///       Shortcut for HttpCookie$Values[key]. Required for ASP compatibility.
        ///    </para>
        /// </devdoc> 
        public String this[String key]
        {
            get
            {
                return Values[key];
            }

            set
            {
                Values[key] = value;
                _changed = true;
            }
        }

        /*
         * Construct set-cookie header 
         */
        internal HttpResponseHeader GetSetCookieHeader(HttpContext context)
        {
            StringBuilder s = new StringBuilder();

            // cookiename=
            if (!String.IsNullOrEmpty(_name))
            {
                s.Append(_name);
                s.Append('=');
            }

            // key=value&...
            if (_multiValue != null)
                s.Append(_multiValue.ToString(false));
            else if (_stringValue != null)
                s.Append(_stringValue);

            // domain
            if (!String.IsNullOrEmpty(_domain))
            {
                s.Append("; domain=");
                s.Append(_domain);
            }

            // expiration
            if (_expirationSet && _expires != DateTime.MinValue)
            {
                s.Append("; expires=");
                s.Append(HttpUtility.FormatHttpCookieDateTime(_expires));
            }

            // path
            if (!String.IsNullOrEmpty(_path))
            {
                s.Append("; path=");
                s.Append(_path);
            }

            // secure
            if (_secure)
                s.Append("; secure");

            // httponly, Note: IE5 on the Mac doesn't support this
            if (_httpOnly && SupportsHttpOnly(context))
            {
                s.Append("; HttpOnly");
            }

            // return as HttpResponseHeader
            return new HttpResponseHeader(HttpWorkerRequest.HeaderSetCookie, s.ToString());
        }
    }

    ///////////////////////////////////////////////////////////////////////////// 
    /////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////// 

    public enum HttpCookieMode
    {

        UseUri,          // cookieless=true

        UseCookies,      // cookieless=false

        AutoDetect,      // cookieless=AutoDetect; Probe if device is cookied

        UseDeviceProfile // cookieless=UseDeviceProfile; Base decision on caps 
    }
}
