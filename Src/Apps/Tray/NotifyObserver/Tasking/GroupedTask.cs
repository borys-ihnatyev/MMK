using System.Reflection;
using System.Runtime.CompilerServices;

namespace MMK.Notify.Observer.Tasking
{
    public sealed class GroupedTask : Task
    {
        private readonly Task origin;

        public GroupedTask(Task origin, TaskGroup @group)
        {
            this.origin = origin;
            Group = @group;
        }

        protected override void Initialize()
        {
            try
            {
                InvokeOrigin();
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

        private void InvokeOrigin([CallerMemberName]string methodName = "")
        {
            var originTaskProc = origin.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            originTaskProc.Invoke(origin, new object[] {});
        }

        protected override void OnRun()
        {
            try
            {
                InvokeOrigin();
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

        public TaskGroup Group { get; private set; }

        protected override string CommonDescription
        {
            get { return ((INotifyable) origin).CommonDescription; }
        }

        protected override string DetailedDescription
        {
            get { return ((INotifyable) origin).DetailedDescription; }
        }

        protected override string TargetObject
        {
            get { return ((INotifyable) origin).TargetObject; }
        }
    }
}