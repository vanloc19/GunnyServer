//using System;
//using System.CodeDom.Compiler;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using log4net;
//using Microsoft.CSharp;
//using Microsoft.VisualBasic;

//namespace Game.Server.Managers
//{
//	public class ScriptMgr
//	{
//		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

//		private static readonly Dictionary<string, Assembly> m_scripts = new Dictionary<string, Assembly>();

//		public static Assembly[] Scripts
//		{
//			get
//			{
//				lock (m_scripts)
//				{
//					return m_scripts.Values.ToArray();
//				}
//			}
//		}

//		public static bool InsertAssembly(Assembly ass)
//		{
//			lock (m_scripts)
//			{
//				if (!m_scripts.ContainsKey(ass.FullName))
//				{
//					m_scripts.Add(ass.FullName, ass);
//					return true;
//				}
//				return false;
//			}
//		}

//		public static bool RemoveAssembly(Assembly ass)
//		{
//			lock (m_scripts)
//			{
//				return m_scripts.Remove(ass.FullName);
//			}
//		}

//		public static bool CompileScripts(bool compileVB, string path, string dllName, string[] asm_names)
//		{
//			if (!path.EndsWith("\\") && !path.EndsWith("/"))
//			{
//				path += "/";
//			}
//			ArrayList files = ParseDirectory(new DirectoryInfo(path), compileVB ? "*.vb" : "*.cs", deep: true);
//			if (files.Count == 0)
//			{
//				return true;
//			}
//			if (File.Exists(dllName))
//			{
//				File.Delete(dllName);
//			}
//			CompilerResults res = null;
//			try
//			{
//				CodeDomProvider compiler = null;
//				compiler = ((!compileVB) ? ((CodeDomProvider)new CSharpCodeProvider()) : ((CodeDomProvider)new VBCodeProvider()));
//				CompilerParameters param = new CompilerParameters(asm_names, dllName, includeDebugInformation: true);
//				param.GenerateExecutable = false;
//				param.GenerateInMemory = false;
//				param.WarningLevel = 2;
//				param.CompilerOptions = "/lib:.";
//				string[] filepaths = new string[files.Count];
//				for (int i = 0; i < files.Count; i++)
//				{
//					filepaths[i] = ((FileInfo)files[i]).FullName;
//				}
//				res = compiler.CompileAssemblyFromFile(param, filepaths);
//				GC.Collect();
//				if (res.Errors.HasErrors)
//				{
//					foreach (CompilerError err in res.Errors)
//					{
//						if (!err.IsWarning)
//						{
//							StringBuilder builder = new StringBuilder();
//							builder.Append("   ");
//							builder.Append(err.FileName);
//							builder.Append(" Line:");
//							builder.Append(err.Line);
//							builder.Append(" Col:");
//							builder.Append(err.Column);
//							if (log.IsErrorEnabled)
//							{
//								log.Error("Script compilation failed because: ");
//								log.Error(err.ErrorText);
//								log.Error(builder.ToString());
//							}
//						}
//					}
//					return false;
//				}
//			}
//			catch (Exception e)
//			{
//				if (log.IsErrorEnabled)
//				{
//					log.Error("CompileScripts", e);
//				}
//			}
//			if (res != null && !res.Errors.HasErrors)
//			{
//				InsertAssembly(res.CompiledAssembly);
//			}
//			return true;
//		}

//		private static ArrayList ParseDirectory(DirectoryInfo path, string filter, bool deep)
//		{
//			ArrayList files = new ArrayList();
//			if (!path.Exists)
//			{
//				return files;
//			}
//			files.AddRange(path.GetFiles(filter));
//			if (deep)
//			{
//				DirectoryInfo[] directories = path.GetDirectories();
//				foreach (DirectoryInfo subdir in directories)
//				{
//					files.AddRange(ParseDirectory(subdir, filter, deep));
//				}
//			}
//			return files;
//		}

//		public static Type GetType(string name)
//		{
//			Assembly[] scripts = Scripts;
//			for (int i = 0; i < scripts.Length; i++)
//			{
//				Type t = scripts[i].GetType(name);
//				if (t != null)
//				{
//					return t;
//				}
//			}
//			return null;
//		}

//		public static object CreateInstance(string name)
//		{
//			Assembly[] scripts = Scripts;
//			for (int i = 0; i < scripts.Length; i++)
//			{
//				Type t = scripts[i].GetType(name);
//				if (t != null && t.IsClass)
//				{
//					return Activator.CreateInstance(t);
//				}
//			}
//			Console.WriteLine(name);
//			return null;
//		}

//		public static object CreateInstance(string name, Type baseType)
//		{
//			Assembly[] scripts = Scripts;
//			for (int i = 0; i < scripts.Length; i++)
//			{
//				Type t = scripts[i].GetType(name);
//				if (t != null && t.IsClass && baseType.IsAssignableFrom(t))
//				{
//					return Activator.CreateInstance(t);
//				}
//			}
//			return null;
//		}

//		public static Type[] GetDerivedClasses(Type baseType)
//		{
//			if (baseType == null)
//			{
//				return new Type[0];
//			}
//			ArrayList types = new ArrayList();
//			foreach (Assembly item in new ArrayList(Scripts))
//			{
//				Type[] types2 = item.GetTypes();
//				foreach (Type t in types2)
//				{
//					if (t.IsClass && baseType.IsAssignableFrom(t))
//					{
//						types.Add(t);
//					}
//				}
//			}
//			return (Type[])types.ToArray(typeof(Type));
//		}

//		public static Type[] GetImplementedClasses(string baseInterface)
//		{
//			ArrayList types = new ArrayList();
//			foreach (Assembly item in new ArrayList(Scripts))
//			{
//				Type[] types2 = item.GetTypes();
//				foreach (Type t in types2)
//				{
//					if (t.IsClass && t.GetInterface(baseInterface) != null)
//					{
//						types.Add(t);
//					}
//				}
//			}
//			return (Type[])types.ToArray(typeof(Type));
//		}
//	}
//}

using log4net;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Game.Server.Managers
{
	public class ScriptMgr
	{
		private readonly static ILog log;

		private readonly static Dictionary<string, Assembly> m_scripts;

		public static Assembly[] Scripts
		{
			get
			{
				Assembly[] array;
				lock (ScriptMgr.m_scripts)
				{
					array = ScriptMgr.m_scripts.Values.ToArray<Assembly>();
				}
				return array;
			}
		}

		static ScriptMgr()
		{
			ScriptMgr.log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
			ScriptMgr.m_scripts = new Dictionary<string, Assembly>();
		}

		public ScriptMgr()
		{
		}

		public static bool CompileScripts(bool compileVB, string path, string dllName, string[] asm_names)
		{
			if (!path.EndsWith("\\") && !path.EndsWith("/"))
			{
				path = string.Concat(path, "/");
			}
			ArrayList files = ScriptMgr.ParseDirectory(new DirectoryInfo(path), (compileVB ? "*.vb" : "*.cs"), true);
			if (files.Count == 0)
			{
				return true;
			}
			if (File.Exists(dllName))
			{
				File.Delete(dllName);
			}
			CompilerResults res = null;
			try
			{
				CodeDomProvider compiler = null;
				if (!compileVB)
				{
					compiler = new CSharpCodeProvider();
				}
				else
				{
					compiler = new VBCodeProvider();
				}
				CompilerParameters param = new CompilerParameters(asm_names, dllName, true)
				{
					GenerateExecutable = false,
					GenerateInMemory = false,
					WarningLevel = 2,
					CompilerOptions = "/lib:."
				};
				string[] filepaths = new string[files.Count];
				for (int i = 0; i < files.Count; i++)
				{
					filepaths[i] = ((FileInfo)files[i]).FullName;
				}
				res = compiler.CompileAssemblyFromFile(param, filepaths);
				GC.Collect();
				if (res.Errors.HasErrors)
				{
					foreach (CompilerError err in res.Errors)
					{
						if (err.IsWarning)
						{
							continue;
						}
						StringBuilder builder = new StringBuilder();
						builder.Append("   ");
						builder.Append(err.FileName);
						builder.Append(" Line:");
						builder.Append(err.Line);
						builder.Append(" Col:");
						builder.Append(err.Column);
						if (!ScriptMgr.log.IsErrorEnabled)
						{
							continue;
						}
						ScriptMgr.log.Error("Script compilation failed because: ");
						ScriptMgr.log.Error(err.ErrorText);
						ScriptMgr.log.Error(builder.ToString());
					}
					return false;
				}
			}
			catch (Exception exception)
			{
				Exception e = exception;
				if (ScriptMgr.log.IsErrorEnabled)
				{
					ScriptMgr.log.Error("CompileScripts", e);
				}
			}
			if (res != null && !res.Errors.HasErrors)
			{
				ScriptMgr.InsertAssembly(res.CompiledAssembly);
			}
			return true;
		}

		public static object CreateInstance(string name)
		{
			Assembly[] scripts = ScriptMgr.Scripts;
			for (int i = 0; i < (int)scripts.Length; i++)
			{
				Type t = scripts[i].GetType(name);
				if (t != null && t.IsClass)
				{
					return Activator.CreateInstance(t);
				}
			}
			return null;
		}

		public static Type[] GetDerivedClasses(Type baseType)
		{
			if (baseType == null)
			{
				return new Type[0];
			}
			ArrayList types = new ArrayList();
			foreach (Assembly asm in new ArrayList(ScriptMgr.Scripts))
			{
				Type[] typeArray = asm.GetTypes();
				for (int i = 0; i < (int)typeArray.Length; i++)
				{
					Type t = typeArray[i];
					if (t.IsClass && baseType.IsAssignableFrom(t))
					{
						types.Add(t);
					}
				}
			}
			return (Type[])types.ToArray(typeof(Type));
		}

		public static bool InsertAssembly(Assembly ass)
		{
			bool flag;
			lock (ScriptMgr.m_scripts)
			{
				if (ScriptMgr.m_scripts.ContainsKey(ass.FullName))
				{
					flag = false;
				}
				else
				{
					ScriptMgr.m_scripts.Add(ass.FullName, ass);
					flag = true;
				}
			}
			return flag;
		}

		private static ArrayList ParseDirectory(DirectoryInfo path, string filter, bool deep)
		{
			ArrayList files = new ArrayList();
			if (!path.Exists)
			{
				return files;
			}
			files.AddRange(path.GetFiles(filter));
			if (deep)
			{
				DirectoryInfo[] directories = path.GetDirectories();
				for (int i = 0; i < (int)directories.Length; i++)
				{
					DirectoryInfo subdir = directories[i];
					files.AddRange(ScriptMgr.ParseDirectory(subdir, filter, deep));
				}
			}
			return files;
		}

		public static bool RemoveAssembly(Assembly ass)
		{
			bool flag;
			lock (ScriptMgr.m_scripts)
			{
				flag = ScriptMgr.m_scripts.Remove(ass.FullName);
			}
			return flag;
		}
	}
}