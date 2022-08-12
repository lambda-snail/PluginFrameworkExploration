
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;
using Application.Sdk;
using Jurassic;
using Jurassic.Library;

namespace Application;

public class Program
{
    private readonly List<IPlugin> _plugins;

    public Program()
    {
        _plugins = LoadExtensions();
        Console.WriteLine($"Found {_plugins.Count} plugins.");
    }
    
    public async Task RunAsync()
    {
        // TODO: Consider jint https://github.com/sebastienros/jint
        var engine = new ScriptEngine();
        engine.SetGlobalValue("console", new FirebugConsole(engine));
        
        foreach (var plugin in _plugins)
        {
            var outputProperty =
                plugin
                    .GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .FirstOrDefault(prop => Attribute.IsDefined(prop, typeof(OutputAttribute)));
            if (outputProperty is not null)
            {
                var outputAttribute = Attribute.GetCustomAttribute(outputProperty, typeof(OutputAttribute)) as OutputAttribute;

                switch (outputAttribute!.Type)
                {
                    case ReturnType.String:
                        RegisterDelegate<string>(engine, plugin, outputProperty);
                        break;
                    case ReturnType.Int:
                        RegisterDelegate<int>(engine, plugin, outputProperty);
                        break;
                    case ReturnType.Bool:
                        RegisterDelegate<bool>(engine, plugin, outputProperty);
                        break;
                    case ReturnType.Double:
                        RegisterDelegate<double>(engine, plugin, outputProperty);
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown return type: {outputAttribute.Type}");
                }

            }
            else
            {
                engine.SetGlobalFunction(plugin.Name, new Action(() => plugin.Execute()));                
            }
        }
        
        engine.Evaluate("hello()");
        engine.Evaluate("let x = returnString(); console.log(x);");
        engine.Evaluate("let integer = getInt(); console.log(`Today's random int is ${integer}`);");

    }

    private static void RegisterDelegate<T>(ScriptEngine engine, IPlugin plugin, PropertyInfo outputProperty)
    {
        engine.SetGlobalFunction(plugin.Name, new
            Func<T>(() =>
            {
                plugin.Execute();
                return (T) outputProperty.GetValue(plugin);
            }));
    }

    public class jsfunction
    {
        public string Execute()
        {
            return DateTime.Now.ToShortDateString();
        }
    }
    
    List<IPlugin> LoadExtensions()
    {
        var plugins = new List<IPlugin>();
        var files = Directory.GetFiles("extensions", "*.dll");

        foreach (var file in files)
        {
            var assembly = Assembly.LoadFile(Path.Combine(Directory.GetCurrentDirectory(), file));
            
            var types = assembly.GetTypes().Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsInterface).ToArray();
            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type) as IPlugin;
                plugins.Add(instance);
            }
        }

        return plugins;
    }
}