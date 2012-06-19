using System;
using System.Collections.Generic;

namespace FxGqlLib
{
    public class GqlEngineState
    {
        public GqlEngineState (GqlEngineExecutionState executionState)
        {
            ExecutionState = executionState;
            Variables = new Dictionary<string, Variable> ();
        }
		
        public GqlEngineExecutionState ExecutionState { get; private set; }

        public string CurrentDirectory { get; set; }

        public enum HeadingEnum
        {
            Off,
            On,
            OnWithRule
        }
        public HeadingEnum Heading { get; set; }

        public Dictionary<string, Variable> Variables { get; private set; }
    }
}

