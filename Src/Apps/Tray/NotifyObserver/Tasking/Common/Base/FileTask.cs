using System.IO;
using MMK.Notify.Observer.Tasking.Contexts;

namespace MMK.Notify.Observer.Tasking.Common.Base
{
    public abstract class FileTask : Task
    {
        protected FileTask()
        {
        }

        protected FileTask(ITaskContext context) : base(context)
        {
        }

        protected FileTask(string filePath) : base(FileContext.Build(filePath))
        {
        }

        protected FileContext FileContext
        {
            get { return (FileContext) Context; }
        }

        protected override void CheckContext()
        {
            base.CheckContext();
            if (!(Context is FileContext))
                throw new InvalidTaskContextException("unsupported context type");
        }

        protected override sealed INotifyable OnRun()
        {
            try
            {
                return OnFileChange();
            }
            catch (FileNotFoundException ex)
            {
                throw new Cancel("Cancel while TryFileChange", ex);
            }
            catch (IOException ex)
            {
                throw NotifyableException(ex, true);
            }
        }

        protected abstract INotifyable OnFileChange();
    }
}