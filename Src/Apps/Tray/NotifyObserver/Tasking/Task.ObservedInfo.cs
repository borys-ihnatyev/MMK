using System;

namespace MMK.Notify.Observer.Tasking
{
    public partial class Task
    {
        public class ObservedInfo : INotifyable
        {
            public const int FailedTaskRerunCount = 10;

            private readonly NotifyType originType;
            private string additionalDescription;
            private string detailedDescription;
            private readonly int runCount;

            public ObservedInfo(int runCount)
            {
                this.runCount = runCount;
            }

            public static ObservedInfo Build(INotifyable notifyable, int taskRunCount)
            {
                var canContinue = false;

                var exception = notifyable as NotifyableException;
                if (exception != null)
                    canContinue = exception.CanContinue;

                return new ObservedInfo(taskRunCount)
                {
                    CanContinue = canContinue,
                    CommonDescription = notifyable.CommonDescription,
                    DetailedDescription = notifyable.DetailedDescription,
                    TargetObject = notifyable.TargetObject,
                    Type = notifyable.Type
                };
            }

            public bool IsFailed { get; private set; }

            public bool IsOk
            {
                get { return !IsFailed; }
            }

            public bool IsRerunFailed
            {
                get { return IsFailed && runCount > 1; }
            }

            public bool IsNeedRerun
            {
                get { return IsFailed && CanContinue && !IsReachRerunLimit; }
            }

            private bool IsReachRerunLimit
            {
                get { return runCount >= FailedTaskRerunCount; }
            }

            public bool CanContinue { get; internal set; }

            public NotifyType Type { get; internal set; }

            public string CommonDescription { get; internal set; }

            public string DetailedDescription
            {
                get { return detailedDescription + additionalDescription; }
                internal set { detailedDescription = value; }
            }

            public string TargetObject { get; internal set; }

            internal void MarkRerun()
            {
                additionalDescription = "\n Retry once.";
                Type = NotifyType.Warning;
            }

            internal void UnmarkRerun()
            {
                additionalDescription = String.Empty;
                Type = originType;
            }
        }
    }
}