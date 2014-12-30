using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace MMK.Wpf.ViewModel
{
    public class CommandResolver
    {
        private const string CommandSuffix = "Command";
        private readonly Type rootType;

        private readonly List<PropertyInfo> unresolvedCommandProperties;
        private List<PropertyInfo> commandProperties;
        private Dictionary<PropertyInfo, MethodInfo> commandBindings;


        public CommandResolver(Type rootType) : this(rootType, new List<PropertyInfo>())
        {
        }

        public CommandResolver(Type rootType, List<PropertyInfo> unresolvedCommandProperties)
        {
            this.rootType = rootType;
            this.unresolvedCommandProperties = unresolvedCommandProperties;
        }

        public IReadOnlyList<PropertyInfo> CommandProperties
        {
            get { return commandProperties ?? (commandProperties = GetCommandProperties().ToList()); }
        }

        private IEnumerable<PropertyInfo> GetCommandProperties()
        {
            return rootType
                .GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
                .Where(p => typeof (ICommand).IsAssignableFrom(p.PropertyType));
        }


        public IReadOnlyDictionary<PropertyInfo, MethodInfo> CommandBindings
        {
            get
            {
                if (commandBindings == null)
                {
                    commandBindings = new Dictionary<PropertyInfo, MethodInfo>();
                    Resolve();
                }
                return commandBindings;
            }
        }


        private void Resolve()
        {
            CheckCommandNamingConventions();

            TryResolveUnresolvedCommands();
            TryResolveCommandProperties();
        }

        private void CheckCommandNamingConventions()
        {
            var invaidNames = GetUnconventionalCommandNames().ToList();
            
            if(invaidNames.Count == 0)
                return;
            throw new CommandNamingConventionException(rootType,invaidNames);   
        }


        public IEnumerable<string> GetUnconventionalCommandNames()
        {
            return CommandProperties.Select(p => p.Name).Where(name => !IsValidCommandName(name));
        }

        private static bool IsValidCommandName(string commandName)
        {
            if (string.IsNullOrWhiteSpace(commandName))
                throw new ArgumentException("must be not empty", "commandName");
            Contract.EndContractBlock();

            return commandName.EndsWith(CommandSuffix);
        }

        private void TryResolveUnresolvedCommands()
        {
            if(unresolvedCommandProperties.Count == 0)
                return;

            var bindings = unresolvedCommandProperties
                .Where(p => IsValidCommandName(p.Name))
                .Select(c => new Tuple<PropertyInfo, MethodInfo>(c, FindCommandHandler(c)))
                .Where(b => !ReferenceEquals(b.Item2, null))
                .ToList();

            bindings.ForEach(b => unresolvedCommandProperties.Remove(b.Item1));
            bindings.ForEach(b => commandBindings.Add(b.Item1, b.Item2));
        }

        private void TryResolveCommandProperties()
        {
            if(CommandProperties.Count == 0)
                return;

            CommandProperties
                .Where(p => IsValidCommandName(p.Name))
                .Select(c => new Tuple<PropertyInfo, MethodInfo>(c, FindCommandHandler(c)))
                .ForEach(ResolveBinding);
        }

        private void ResolveBinding(Tuple<PropertyInfo, MethodInfo> binding)
        {
            if (binding.Item2 == null)
                unresolvedCommandProperties.Add(binding.Item1);
            else
                commandBindings.Add(binding.Item1, binding.Item2);
        }


        private MethodInfo FindCommandHandler(PropertyInfo p)
        {
            var commandHandlerName = GetCommandHandlerName(p.Name);
            var commandHandlers = GetCommandHandlers(commandHandlerName).ToList();

            if (commandHandlers.Count == 0)
                return null;

            if (commandHandlers.Count == 1)
                return commandHandlers[0];

            throw new CommandResolveException(rootType, p, commandHandlers.Count);
        }

        private static string GetCommandHandlerName(string commandName)
        {
            Contract.Assert(IsValidCommandName(commandName));
            return commandName.Substring(0, commandName.Length - CommandSuffix.Length);
        }

        private IEnumerable<MethodInfo> GetCommandHandlers(string commandHandlerName)
        {
            return rootType
                .GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.Name.Equals(commandHandlerName, StringComparison.Ordinal));
        }
    }
}