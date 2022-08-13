using System.Reflection;
using Application.Sdk;
using Jurassic;
using Jurassic.Library;
using SamplePlugins;

namespace Application;

public class Program
{
    private readonly List<IJSOperation> _plugins;
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
            
            var inputProperty =
                plugin
                    .GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .FirstOrDefault(prop => Attribute.IsDefined(prop, typeof(InputAttribute)));
            if (outputProperty is not null || inputProperty is not null)
            {
                var getDelegate = plugin.GetType().GetMethod("GetDelegate");
                if (getDelegate is null)
                {
                    throw new InvalidOperationException($"Unable to find the GetDelegate method on IJSOperation instance '{plugin.Name}' with Output attribute.");
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

    private static void RegisterDelegate<T>(ScriptEngine engine, IJSOperation ijsOperation, PropertyInfo outputProperty)
    {
        engine.SetGlobalFunction(ijsOperation.Name, new
            Func<T>(() =>
            {
                ijsOperation.Execute();
                return (T) outputProperty.GetValue(ijsOperation);
            }));
    }

    public class jsfunction
    {
        public string Execute()
        {
            return DateTime.Now.ToShortDateString();
        }
    }
    
    List<IJSOperation> LoadExtensions()
    {
        var plugins = new List<IJSOperation>();
        var files = Directory.GetFiles("extensions", "*.dll");

        foreach (var file in files)
        {
            var assembly = Assembly.LoadFile(Path.Combine(Directory.GetCurrentDirectory(), file));
            
            var types = assembly.GetTypes().Where(t => typeof(IJSOperation).IsAssignableFrom(t) && !t.IsInterface).ToArray();
            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type) as IJSOperation;
                plugins.Add(instance);
            }
        }

        return plugins;
    }
}