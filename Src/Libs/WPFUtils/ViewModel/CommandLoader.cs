using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace MMK.Wpf.ViewModel
{
    /// <summary>
    /// Bind ICommand Properties with Name 'CommandName'Command
    /// to method with name 'CommandName' upwards inherit tree
    /// </summary>
    internal class CommandLoader
    {
        private readonly ViewModel rootViewModel;

        private readonly Dictionary<PropertyInfo, MethodInfo> commandBindings; 

        public CommandLoader(ViewModel rootViewModel)
        {
            this.rootViewModel = rootViewModel;
            commandBindings = new Dictionary<PropertyInfo, MethodInfo>();
        }

        public void BindCommands()
        {
            var unresolvedCommandProperties = new List<PropertyInfo>();

            GetRootClassHierarchy()
                .Reverse()
                .Select(type => new CommandResolver(type, unresolvedCommandProperties))
                .SelectMany(resolver => resolver.CommandBindings)
                .Where(b =>!IsCommandInitialized(b.Key))
                .ForEach(b => commandBindings.Add(b.Key, b.Value));

            commandBindings.ForEach(binding => BindCommandHandler(binding.Key, binding.Value));
        }

        private bool IsCommandInitialized(PropertyInfo commandProperty)
        {
            return commandProperty.GetValue(rootViewModel) != null;
        }

        private IEnumerable<Type> GetRootClassHierarchy()
        {
            var rootType = rootViewModel.GetType();

            while (rootType != typeof(ViewModel))
            {
                yield return rootType;

                Contract.Assert(rootType != null);
                rootType = rootType.BaseType;
            }
        }

        private void BindCommandHandler(PropertyInfo commandPropertyInfo, MethodInfo commandHandlerInfo)
        {
            commandPropertyInfo.SetValue(rootViewModel, CreateCommand(commandHandlerInfo));
        }

        private ICommand CreateCommand(MethodInfo commandHandlerInfo)
        {
            var parameters = commandHandlerInfo.GetParameters();

            if(parameters.Length == 0)
                return CreateSimpleCommand(commandHandlerInfo);

            if (parameters.Length == 1)
                return CreateParametrizedCommand(commandHandlerInfo);
            
            throw new NotImplementedException();
        }

        private ICommand CreateSimpleCommand(MethodInfo commandHandlerInfo)
        {
            return new Command(() => commandHandlerInfo.Invoke(rootViewModel, null));
        }

        private ICommand CreateParametrizedCommand(MethodInfo methodInfo)
        {
            return new Command<object>(o => methodInfo.Invoke(rootViewModel, new []{o}));
        }
    }
}