using System;
using FxGqlLib;
using System.IO;
using System.Collections.Generic;

namespace FxGqlC
{
	class MainClass
	{
		static GqlEngine gqlEngine;
		
		public static void Main (string[] args)
		{
			bool nologo = false;
			bool help = false;
			bool license = false;
			bool prompt = false;
			string command = null;
			string gqlFile = null;
			string logFile = null;
			List<string> errors = new List<string> ();
			
			for (int i = 0; i < args.Length; i++) {
				if (string.Equals (args [i], "-nologo", StringComparison.InvariantCultureIgnoreCase))
					nologo = true;
				else if (string.Equals (args [i], "-help", StringComparison.InvariantCultureIgnoreCase)
				         || string.Equals (args [i], "-h", StringComparison.InvariantCultureIgnoreCase))
					help = true;
				else if (string.Equals (args [i], "-license", StringComparison.InvariantCultureIgnoreCase))
					license = true;
				else if (string.Equals (args [i], "-prompt", StringComparison.InvariantCultureIgnoreCase)
				         || string.Equals (args [i], "-p", StringComparison.InvariantCultureIgnoreCase))
					prompt = true;
				else if (string.Equals (args [i], "-command", StringComparison.InvariantCultureIgnoreCase)
				         || string.Equals (args [i], "-c", StringComparison.InvariantCultureIgnoreCase)) {
					i++;
					if (i < args.Length)
						command = args [i];
					else
						errors.Add ("Please specify a GQL command after '-command'");
				} else if (string.Equals (args [i], "-gqlfile", StringComparison.InvariantCultureIgnoreCase)) {
					i++;
					if (i < args.Length)
						gqlFile = args [i];
					else
						errors.Add ("Please specify a GQL file after '-file'");
				} else if (string.Equals (args [i], "-logfile", StringComparison.InvariantCultureIgnoreCase)) {
					i++;
					if (i < args.Length)
						logFile = args [i];
					else
						errors.Add ("Please specify an output file after '-logfile'");
				} else {
					errors.Add (string.Format ("Unknown parameter '{0}'", args [i]));
				}				
			}
						
			if (!nologo) {
				Console.WriteLine ("===========================================================================");
				Console.WriteLine ("FxGqlC - Fox Innovations Grep Query Language for Console");
				Console.WriteLine ("(c) Copyright 2006-2012 Fox Innovations / Wim Devos");
				Console.WriteLine ("===========================================================================");
			}
							
			if (license) {
				Console.WriteLine ("This program is free software: you can redistribute it and/or modify");
				Console.WriteLine ("it under the terms of the GNU General Public License as published by");
				Console.WriteLine ("the Free Software Foundation, either version 3 of the License, or");
				Console.WriteLine ("any later version.");
				Console.WriteLine ();
				Console.WriteLine ("This program is distributed in the hope that it will be useful,");
				Console.WriteLine ("but WITHOUT ANY WARRANTY; without even the implied warranty of");
				Console.WriteLine ("MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the");
				Console.WriteLine ("GNU General Public License for more details.");
				Console.WriteLine ();
				Console.WriteLine ("You should have received a copy of the GNU General Public License");
				Console.WriteLine ("along with this program.  If not, see <http://www.gnu.org/licenses/>.");
				Console.WriteLine ();
				Console.WriteLine ("FxGqlC depends on (and/or contains redistributables of) these open source");
				Console.WriteLine ("products:");
				Console.WriteLine ("  * SharpZipLib, licensed under GPLv2, Copyright 2001-2010 Mike Krueger, ");
				Console.WriteLine ("      John Reilly.");
				Console.WriteLine ("  * Antlr v3, Antlr3 license (BSD), Copyright (c) 2010 Terence Parr.");
				Console.WriteLine ();
				Console.WriteLine ("Contact Information: Wim Devos, wim AT obiwan DOT be");
				Console.WriteLine ("===========================================================================");
			} else if (!nologo) {
				Console.WriteLine ("This program is free software: you can redistribute it and/or modify");
				Console.WriteLine ("it under the terms of the GNU General Public License as published by");
				Console.WriteLine ("the Free Software Foundation, either version 3 of the License, or");
				Console.WriteLine ("any later version.  Run FxGqlC.exe -license for more information.");
				Console.WriteLine ("===========================================================================");
			}
			
			if (help || errors.Count > 0) {
				foreach (string error in errors) {
					Console.WriteLine (error);
				}
					
				ShowHelp ();
				return;
			}
			
			gqlEngine = new GqlEngine ();
			gqlEngine.OutputStream = Console.Out;
			if (logFile != null)
				gqlEngine.LogStream = new StreamWriter (logFile);
			
			try {
				if (prompt) {
					RunPrompt ();
				} else if (command != null) {
					ExecuteCommand (command);
				} else if (gqlFile != null) {
					ExecuteFile (gqlFile);
				} else {
					ShowHelp ();
				}
			} finally {
				gqlEngine.LogStream.Close ();
				gqlEngine.LogStream.Dispose ();
				gqlEngine.LogStream = null;
			}			
		}

		public static void ShowHelp ()
		{
			Console.WriteLine ("FxGqlC.exe usage");
			Console.WriteLine ("   -help or -h         Show this help");
			Console.WriteLine ("   -license            Shows license information");
			Console.WriteLine ("   -prompt or -p       Run in prompt mode");
			Console.WriteLine ("   -command <command>  Run a single command (usefull when running FxGqlC in");
			Console.WriteLine ("                         scripts)  Alternative: -c <command>");
			Console.WriteLine ("   -gqlfile <file>     Run the commands from a file");
			Console.WriteLine ("   -logfile <file>     Outputs all commands and output to a file");
			//Console.WriteLine ("   -loglevel <#>")
			Console.WriteLine ("===========================================================================");
		}

		public static void RunPrompt ()
		{
			while (true) {
				Console.Write ("FxGqlC> ");
				string command = Console.ReadLine ();
				if (command.Equals ("exit", StringComparison.InvariantCultureIgnoreCase))
					break; 
				try {
					ExecuteCommand (command);
				} catch (Exception x) {
					Console.WriteLine (x.ToString ());
				}
			}
		}

		public static void ExecuteCommand (string command)
		{
			gqlEngine.Execute (command);
		}

		public static void ExecuteFile (string file)
		{
			using (StreamReader reader = new StreamReader(file)) {
				string command = reader.ReadToEnd ();
				ExecuteCommand (command);
			}
		}
	}
}