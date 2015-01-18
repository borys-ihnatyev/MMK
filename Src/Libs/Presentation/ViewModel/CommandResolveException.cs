using System;
using System.Reflection;

namespace MMK.Presentation.ViewModel
{
    public class CommandResolveException : Exception
    {
        public CommandResolveException(Type targeType, PropertyInfo command, int methodCount) : base(
            String.Format("Can't resolve  Command '{0}' in '{1}' type between {2} methods in ", command.Name,
                targeType.Name, methodCount))
        {
            TargetTypeFullName = targeType.FullName;
            CommandName = command.Name;
        }

        public string CommandName { get; private set; }

        public string TargetTypeFullName { get; private set; }
    }
}