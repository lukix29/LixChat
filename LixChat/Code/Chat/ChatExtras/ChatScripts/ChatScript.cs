using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace LX29_ChatClient.Addons.Scripts
{
    public delegate object BaseFunc(ChatMessage msg);

    public class ScriptClass : IEnumerable<BaseFunc>
    {
        public ScriptClass(Type type)
        {
            Class = type;
            Scripts = new List<BaseFunc>();
            foreach (var m in type.GetMethods(BindingFlags.Static | BindingFlags.Public))
            {
                var parm = m.GetParameters();
                if (m.ReturnType.Equals(typeof(object)) && parm.Length == 1 && parm[0].ParameterType.Equals(typeof(ChatMessage)))
                {
                    Scripts.Add((BaseFunc)Delegate.CreateDelegate(typeof(BaseFunc), m));
                }
                else
                {
                }
            }
        }

        public Type Class
        {
            get;
            private set;
        }

        public string Name
        {
            get { return Class.Name; }
        }

        public string[] PropertyNames
        {
            get { return Class.GetProperties(BindingFlags.Static | BindingFlags.Public).Select(t => t.Name).ToArray(); }
        }

        public PropertyInfo[] Propertys
        {
            get { return Class.GetProperties(); }
        }

        public List<BaseFunc> Scripts
        {
            get;
            private set;
        }

        public IEnumerator<BaseFunc> GetEnumerator()
        {
            return Scripts.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public object GetPorperty(string Name)
        {
            var prop = Class.GetProperty(Name);
            if (prop != null)
            {
                return prop.GetValue(null);
            }
            return null;
        }

        public bool SetPorperty(string Name, object Value)
        {
            var prop = Class.GetProperty(Name);
            if (prop != null)
            {
                prop.SetValue(null, Value);
                return true;
            }
            return false;
        }
    }

    public class ScriptClassCollection
    {
        private static bool isLoading = false;
        private static Dictionary<string, ScriptClass> scriptClasses = new Dictionary<string, ScriptClass>();

        public ScriptClass this[string Class]
        {
            get { return scriptClasses[Class]; }
        }

        public static List<object> ForEach(string Class, ChatMessage message)
        {
            if (isLoading) return null;
            try
            {
                List<object> output = new List<object>();
                foreach (var func in scriptClasses[Class])
                {
                    output.Add(func(message));
                }
                return output;
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        return ForEach(Class, message);
                }
            }
            return null;
        }

        public static List<object> ForEachAll(ChatMessage message)
        {
            if (isLoading) return null;
            try
            {
                List<object> output = new List<object>();
                foreach (string key in scriptClasses.Keys)
                {
                    output.AddRange(ForEach(key, message));
                }
                return output;
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        return ForEachAll(message);
                }
            }
            return null;
        }

        public static void LoadScripts()
        {
            isLoading = true;
            scriptClasses.Clear();
            LoadInternalScripts();
            LoadExternalScripts();
            isLoading = false;
        }

        private static ScriptClass CompileCode(string finalCode, string scriptName)
        {
            //string finalCode = code.Replace("[REPLACE]", function);

            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();
            var assemblies = AppDomain.CurrentDomain
                            .GetAssemblies()
                //.Where(a => !a.IsDynamic)
                            .Select(a => a.Location).ToArray();

            parameters.ReferencedAssemblies.AddRange(assemblies);
            parameters.ReferencedAssemblies.Add(
                System.IO.Path.GetFullPath(typeof(LX29_TwitchChat.Program).Assembly.CodeBase.Replace("file:///", "")));

            parameters.GenerateInMemory = true;
            parameters.GenerateExecutable = false;

            CompilerResults results = provider.CompileAssemblyFromSource(parameters, finalCode);

            if (results.Errors.HasErrors)
            {
                StringBuilder sb = new StringBuilder();

                foreach (CompilerError error in results.Errors)
                {
                    string err = error.ToString().Replace(error.FileName, scriptName)
                        .Replace(": error", ":\r\nerror").Replace(", ", ",\r\n");
                    sb.AppendLine(err);
                    sb.AppendLine();
                }

                LX29_MessageBox.Show(sb.ToString(), "Script Compilation Error!");
            }

            var type = results.CompiledAssembly.ExportedTypes.First();

            var meth = new ScriptClass(type);
            //MethodInfo func = binaryFunction.GetMethod("Function");
            return meth;// (Func<ChatMessage, object>)Delegate.CreateDelegate(typeof(Func<ChatMessage, object>), func);
        }

        private static void LoadExternalScripts()
        {
            try
            {
                var names = Directory.GetFiles(Settings.scriptDir, "*.cs", SearchOption.AllDirectories);
                if (names.Length > 0)
                {
                    foreach (var name in names)//
                    {
                        string read = "";
                        using (StreamReader sr = new StreamReader(Path.GetFullPath(name)))
                        {
                            read = sr.ReadToEnd();
                        }
                        var d = CompileCode(read, Path.GetFileName(name));
                        scriptClasses.Add(d.Name, d);
                    }
                }
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        x.Handle();
                        break;

                    case MessageBoxResult.Abort:
                        Application.Exit();
                        break;
                }
            }
        }

        private static void LoadInternalScripts()
        {
            try
            {
                var names = Assembly.GetExecutingAssembly()
                    .GetManifestResourceNames().Where(t => t.Contains("BaseScripts")).ToList();
                foreach (var name in names)//
                {
                    string read = "";
                    using (StreamReader sr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(name)))
                    {
                        read = sr.ReadToEnd();
                    }
                    var d = CompileCode(read, name);
                    scriptClasses.Add(d.Name, d);
                }
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        LoadInternalScripts();
                        break;

                    case MessageBoxResult.Abort:
                        Application.Exit();
                        break;
                }
            }
        }
    }
}