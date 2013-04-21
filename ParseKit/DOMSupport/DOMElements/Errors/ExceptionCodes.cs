using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParseKit.DOMElements._Classes.Errors
{
    class ExceptionCodes
    {
        public static short INDEX_SIZE_ERR = 1;
        public static short DOMSTRING_SIZE_ERR = 2; // historical
        public static short HIERARCHY_REQUEST_ERR = 3;
        public static short WRONG_DOCUMENT_ERR = 4;
        public static short INVALID_CHARACTER_ERR = 5;
        public static short NO_DATA_ALLOWED_ERR = 6; // historical
        public static short NO_MODIFICATION_ALLOWED_ERR = 7;
        public static short NOT_FOUND_ERR = 8;
        public static short NOT_SUPPORTED_ERR = 9;
        public static short INUSE_ATTRIBUTE_ERR = 10; // historical
        public static short INVALID_STATE_ERR = 11;
        public static short SYNTAX_ERR = 12;
        public static short INVALID_MODIFICATION_ERR = 13;
        public static short NAMESPACE_ERR = 14;
        public static short INVALID_ACCESS_ERR = 15;
        public static short VALIDATION_ERR = 16; // historical
        public static short TYPE_MISMATCH_ERR = 17; // historical; use TypeError instead
        public static short SECURITY_ERR = 18;
        public static short NETWORK_ERR = 19;
        public static short ABORT_ERR = 20;
        public static short URL_MISMATCH_ERR = 21;
        public static short QUOTA_EXCEEDED_ERR = 22;
        public static short TIMEOUT_ERR = 23;
        public static short INVALID_NODE_TYPE_ERR = 24;
        public static short DATA_CLONE_ERR = 25;
    }
}
