namespace Parse.DOM
{
	public enum InsertionMode
	{
		INITIAL = 0,

		BEFORE_HTML = 1,

		BEFORE_HEAD = 2,

		IN_HEAD = 3,            //Mode for Script execution

		IN_HEAD_NOSCRIPT = 4,

        AFTER_HEAD = 5,         //Mode for Script execution [DO LIKE IN_HEAD]

        IN_BODY = 6,            //Mode for Script execution [DO LIKE IN_HEAD]

        IN_TABLE = 7,           //Mode for Script execution [DO LIKE IN_HEAD]

		IN_CAPTION = 8,

		IN_COLUMN_GROUP = 9,

		IN_TABLE_BODY = 10,

		IN_ROW = 11,

		IN_CELL = 12,

        IN_SELECT = 13,     //Mode for Script execution [DO LIKE IN_HEAD]

		IN_SELECT_IN_TABLE = 14,

		AFTER_BODY = 15,

		IN_FRAMESET = 16,

		AFTER_FRAMESET = 17,

		AFTER_AFTER_BODY = 18,

		AFTER_AFTER_FRAMESET = 19,

        TEXT = 20,          //Mode for Script execution [DO LIKE IN_HEAD]

		FRAMESET_OK = 21
	}
}
