using System.Diagnostics;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class ObjectExtension
    {
        public static bool IsCalledInside(this object instance, int skipFrames = 1)
        {
            var stackFrame = new StackFrame(skipFrames);
            var calledType = stackFrame.GetMethod().DeclaringType;
            return instance.GetType() == calledType;
        }
    }
}