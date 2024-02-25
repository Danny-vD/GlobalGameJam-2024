using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Ink.Parsed;
using Path = System.IO.Path;

namespace Ink
{
    public class PluginManager
    {
        private readonly List<IPlugin> _plugins;

        public PluginManager(List<string> pluginDirectories)
        {
            _plugins = new List<IPlugin>();

            foreach (var pluginName in pluginDirectories)
            foreach (var file in Directory.GetFiles(pluginName, "*.dll"))
            foreach (var type in Assembly.LoadFile(Path.GetFullPath(file)).GetExportedTypes())
                if (typeof(IPlugin).IsAssignableFrom(type))
                    _plugins.Add((IPlugin)Activator.CreateInstance(type));
        }

        public string PreParse(string storyContent)
        {
            object[] args = { storyContent };

            foreach (var plugin in _plugins)
                typeof(IPlugin).InvokeMember("PreParse", BindingFlags.InvokeMethod, null, plugin, args);

            return (string)args[0];
        }

        public Story PostParse(Story parsedStory)
        {
            object[] args = { parsedStory };

            foreach (var plugin in _plugins)
                typeof(IPlugin).InvokeMember("PostParse", BindingFlags.InvokeMethod, null, plugin, args);

            return (Story)args[0];
        }

        public Runtime.Story PostExport(Story parsedStory, Runtime.Story runtimeStory)
        {
            object[] args = { parsedStory, runtimeStory };

            foreach (var plugin in _plugins)
                typeof(IPlugin).InvokeMember("PostExport", BindingFlags.InvokeMethod, null, plugin, args);

            return (Runtime.Story)args[1];
        }
    }
}