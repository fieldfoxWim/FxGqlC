using System;

namespace FxGqlLib
{
	public class FormatListFunction : Expression<string>
	{
		IExpression[] expression;
		string separator;
		
		public FormatListFunction (IExpression[] expressions, string separator)
		{
		}
				
		#region implemented abstract members of FxGqlLib.Expression[System.String]
		public override string Evaluate (GqlQueryState gqlQueryState)
		{
			string[] texts = new string[expression.Length];
			for (int i = 0; i < expression.Length; i++) {
				if (i > 0)
					texts [i * 2 - 1] = separator;
				texts [i * 2] = expression [i].EvaluateAsString (gqlQueryState);
			}
			
			return string.Concat (texts);
		}
		#endregion
	}
}
