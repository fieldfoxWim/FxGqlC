using System;
using System.IO;

namespace FxGqlLib
{
    public class SetVariableCommand : IGqlCommand
    {
        string variable;
        IExpression expression;

        public SetVariableCommand (Tuple<string, IExpression> setVariable)
        {
            this.variable = setVariable.Item1;
            this.expression = setVariable.Item2;
        }

        #region IGqlCommand implementation
        public void Execute (TextWriter outputStream, TextWriter logStream, GqlEngineState gqlEngineState)
        {
            Variable variable;
            if (!gqlEngineState.Variables.TryGetValue (this.variable, out variable))
                throw new InvalidOperationException (string.Format ("Variable '{0}' not declared.", this.variable));

            GqlQueryState gqlQueryState = new GqlQueryState (gqlEngineState.ExecutionState, gqlEngineState.Variables);
            gqlQueryState.CurrentDirectory = gqlEngineState.CurrentDirectory;
            variable.Value = this.expression.EvaluateAsComparable (gqlQueryState);
        }
        #endregion
    }
}

