﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpTemplate.Compilers;
using SharpTemplate.Utils;

namespace SharpTemplate.Test.Compilers
{
	[TestClass]
	public class SourceCompilerTest
	{
		private readonly List<string> _files = new List<string>();

		private void AddFile(string path)
		{
			_files.Add(path);
		}

		[TestInitialize]
		public void Initialize()
		{
			_files.Clear();
		}
		[TestCleanup]
		public void CleanUp()
		{
			foreach (var file in _files)
			{
				if (File.Exists(file))
				{
					try
					{
						File.Delete(file);
					}
					catch
					{
						Console.WriteLine("Unable to delete "+file);
					}
				}
			}
		}

		[TestMethod]
		public void CompileSimpleObject()
		{
			const string dllName = "CompileSimpleObject";
			var path = TestUtils.GetExecutionPath();
			var source = ResourceContentLoader.LoadText("SimpleObject.cs", Assembly.GetExecutingAssembly());
			
			var sc = new SourceCompiler(dllName, path);

			sc.AddFile("SharpTemplate.Test.Resources.Compilers", "SimpleObject", source);
			sc.LoadCurrentAssemblies();
			var loadedAssembly = sc.Compile();

			Assert.IsNotNull(loadedAssembly);
			Assert.IsFalse(sc.HasErrors);
            
			AddFile(loadedAssembly);

			var content = File.ReadAllBytes(loadedAssembly);
			var compileSimpleObjectAsm = Assembly.Load(content);
			var instance = Activator.CreateInstance(compileSimpleObjectAsm.FullName, "SharpTemplate.Test.Resources.Compilers.SimpleObject").Unwrap();
			Assert.IsNotNull(instance);
			var method = instance.GetType().GetMethod("GetAssemblyName");
			Assert.IsNotNull(method);

			var result = method.Invoke(instance, new object[] {});
			Assert.AreEqual(dllName,result);
		}


		[TestMethod]
		public void CompileRetryIfErrors()
		{
			const string dllName = "CompileRetryIfErrors";
			var path = TestUtils.GetExecutionPath();
			var sourceCorrect = ResourceContentLoader.LoadText("CorrectObject.cs", Assembly.GetExecutingAssembly());
			var sourceFail = ResourceContentLoader.LoadText("FailObject.cs", Assembly.GetExecutingAssembly());

			var sc = new SourceCompiler(dllName, path);

			sc.AddFile("SharpTemplate.Test.Resources.Compilers", "CorrectObject", sourceCorrect);
			sc.AddFile("SharpTemplate.Test.Resources.Compilers", "FailObject", sourceFail);
			sc.LoadCurrentAssemblies();
			var loadedAssembly = sc.Compile(2);

			Assert.IsNotNull(loadedAssembly);
			Assert.IsTrue(sc.HasErrors);
			Assert.AreEqual(1, sc.Errors.Count);
			Assert.AreEqual(1, sc.Errors[0].Count);
			Assert.IsTrue(sc.Errors[0][0].Contains("not all code paths return a value"));

			AddFile(loadedAssembly);

			var content = File.ReadAllBytes(loadedAssembly);
			var compileSimpleObjectAsm = Assembly.Load(content);
			var instance = Activator.CreateInstance(compileSimpleObjectAsm.FullName, "SharpTemplate.Test.Resources.Compilers.CorrectObject").Unwrap();
			Assert.IsNotNull(instance);
			var method = instance.GetType().GetMethod("GetAssemblyName");
			Assert.IsNotNull(method);

			var result = method.Invoke(instance, new object[] { });
			Assert.AreEqual(dllName, result);
		}

		[TestMethod]
		public void CompileSignedAssembly()
		{
			const string dllName = "CompileSignedAssembly";
			var path = TestUtils.GetExecutionPath();
			var keyTemp = Path.Combine(path, "TestKey.snk");
			var keyContent = ResourceContentLoader.LoadBytes("TestKey.snk", Assembly.GetExecutingAssembly());
			File.WriteAllBytes(keyTemp,keyContent);
			AddFile(keyTemp);

			var sc = new SourceCompiler(dllName, path);

			sc.Key = keyTemp;
			sc.LoadCurrentAssemblies();
			var loadedAssembly = sc.Compile();

			Assert.IsNotNull(loadedAssembly);
			Assert.IsFalse(sc.HasErrors);

			AddFile(loadedAssembly);

			var content = File.ReadAllBytes(loadedAssembly);
			var compileSimpleObjectAsm = Assembly.Load(content);
			Assert.IsNotNull(compileSimpleObjectAsm);

			var fullName = compileSimpleObjectAsm.FullName;
			Assert.IsTrue(fullName.Contains("3e48d4993a05c8f5"));
		}
	}
}