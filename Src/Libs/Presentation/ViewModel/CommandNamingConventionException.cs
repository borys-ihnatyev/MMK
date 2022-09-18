using System;
using System.Collections.Generic;

namespace MMK.Presentation.ViewModel
{
    public class CommandNamingConventionException : Exception
    {
        public CommandNamingConventionException(Type targetType, IEnumerable<string> commandNames ) 
            : base(String.Format("Class '{0}' has unconventional Command Names : '{1}' ", targetType.FullName, String.Join(", ",commandNames)))
        {
            
        }
    }
}