using System.Reflection;
using Application.Sdk;
using Jurassic;
using Jurassic.Library;
using SamplePlugins;

namespace Application;

public class Program
{
    private readonly List<IPlugin> _plugins;
    public Program()
    {
        _plugins = LoadExtensions();
        Console.WriteLine($"Found {_plugins.Count} plugins.");
    }
    
    public async Task RunAsync(string scriptContent)
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
                var getDelegate = plugin.GetType().GetMethod("GetDelegate");
                if (getDelegate is null)
                {
                    throw new InvalidOperationException($"Unable to find the GetDelegate method on IPlugin instance '{plugin.Name}' with Output attribute.");
                }

                engine.SetGlobalFunction(plugin.Name, (Delegate) getDelegate.Invoke(plugin, null));
            }
            else
            {
                engine.SetGlobalFunction(plugin.Name, new Action(() => plugin.Execute()));                
            }
        }
        
        engine.Evaluate(scriptContent);
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