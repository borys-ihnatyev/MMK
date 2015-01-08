using System.Diagnostics;

namespace MMK.Utils.Extensions
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