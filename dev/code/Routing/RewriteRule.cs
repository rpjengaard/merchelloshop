namespace code.Routing
{
	public class RewriteRule
	{
		public RewriteRule(string pattern, string docTypeAlias)
		{
			Pattern = pattern;
			DocTypeAlias = docTypeAlias;
		}

		public string Pattern { get; set; }
		public string DocTypeAlias { get; set; }
	}
}
