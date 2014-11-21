// ===========================================================
// Copyright (C) 2014-2015 Kendar.org
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
// is furnished to do so, subject to the following conditions:
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS 
// BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF 
// OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ===========================================================


using System.Collections.Generic;

namespace SharpTemplate.Parsers
{
	/// <summary>
	/// Descriptor for a sharp parser result
	/// </summary>
	public class SharpClass
	{
		public SharpClass()
		{
			//Default using
			Using = new List<string>
			        {
				        "System", 
						"System.Collections.Generic",
						"SharpTemplate.Parsers"
			        };
		}

		/// <summary>
		/// Class name
		/// </summary>
		public string ClassName { get; set; }

		/// <summary>
		/// Namespace for the generateed class
		/// </summary>
		public string ClassNamespace { get; set; }

		/// <summary>
		/// Base class
		/// </summary>
		public string Base { get; set; }

		/// <summary>
		/// Type of the object that will be passed to execute function
		/// </summary>
		public string Model { get; set; }

		/// <summary>
		/// List of using
		/// </summary>
		public List<string> Using { get; set; }

		/// <summary>
		/// Source code for the Execute method
		/// </summary>
		public string Content { get; set; }

		public override string ToString()
		{
			var total = string.Empty;
			foreach (var item in Using)
			{
				total += "using " + item + ";\r\n";
			}
			total += "\r\n";
			total += "namespace " + ClassNamespace + "\r\n";
			total += "{\r\n";

			total += "public class " + ClassName;
			if (Base != null)
			{
				total += " : " + Base + ", ISharpResult";
			}
			else
			{
				total += " : ISharpResult";
			}
			total += "\r\n";
			total += "{\r\n";

			total += "public string Content { get; set; }\r\n";

			total += "public void Write(string toWrite, bool plusCrLf = false)\r\n";
			total += "{\r\n";
			total += "Content += toWrite;\r\n";
			total += "if (plusCrLf) Content += \"\\r\\n\";\r\n";
			total += "}\r\n";

			total += "public void Execute(object modelAsObject)\r\n";
			total += "{\r\n";
			if (Model != "object")
			{
				total += "var model = modelAsObject as " + Model + ";\r\n";
				total += "if(model==null) throw new InvalidCastException(\"Model must be of type '" + Model + "'\");\r\n";
			}
			else
			{
				total += "var model = modelAsObject;\r\n";
			}
			total += Content;
			total += "\r\n";
			total += "}\r\n";

			total += "}\r\n";

			total += "}\r\n";
			return total;
		}
	}
}