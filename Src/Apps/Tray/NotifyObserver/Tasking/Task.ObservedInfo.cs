using System;
using MMK.Notify.Observer.Tasking.Common;

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

            public ObservedInfo(INotifyable notifyable)
            {
                if (!TryInitialize(notifyable))
                    throw new ArgumentException("Unsupported type " + notifyable.GetType().FullName, "notifyable");
                
                originType = Type;
                CommonDescription = notifyable.CommonDescription;
                DetailedDescription = notifyable.DetailedDescription;
                TargetObject = notifyable.TargetObject;
            }

            private bool TryInitialize(INotifyable notifyable)
            {
                return TryInitializeAsTask(notifyable) 
                    || TryInitalizeAsException(notifyable) 
                    || TryInitializeAsSelf(notifyable);
            }

            private bool TryInitializeAsTask(INotifyable notifyable)
            {
                var task = notifyable as Task;
                if (task == null) return false;

                Task = task;
                Type = ((INotifyable)task).Type;
                return true;
            }

            private bool TryInitalizeAsException(INotifyable notifyable)
            {
                var failure = notifyable as NotifyableException;
                if (failure == null) return false;

                FailureReason = failure;
                CanContinue = failure.CanContinue;
                Type = NotifyType.Error;
                return true;
            }

            private bool TryInitializeAsSelf(INotifyable notifyable)
            {
                var info = notifyable as ObservedInfo;
                if (info == null) return false;

                Task = info.Task;
                FailureReason = info.FailureReason;
                CanContinue = info.CanContinue;
                return true;
            }

            
            internal Task Task { get; set; }

            public Exception FailureReason { get; private set; }


            public bool IsFailed
            {
                get { return FailureReason != null; }
            }

            public bool IsOk
            {
                get { return FailureReason == null; }
            }

            public bool IsRerunFailed
            {
                get { return IsFailed && Task.RunCount > 1; }
            }

            public bool IsNeedRerun
            {
                get { return IsFailed && CanContinue && !IsReachRerunLimit; }
            }

            private bool IsReachRerunLimit
            {
                get { return Task.RunCount >= FailedTaskRerunCount; }
            }

            public bool CanContinue { get; private set; }

            public NotifyType Type { get; internal set; }

            public string CommonDescription { get; private set; }

            public string DetailedDescription
            {
                get { return detailedDescription + additionalDescription; }
                protected internal set { detailedDescription = value; }
            }

            public string TargetObject { get; protected internal set; }


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


            public bool IsSimilarTo(ObservedInfo info)
            {
                var isTypeSimilar = ((Task is ChangeHashTagModelTask) && (info.Task is ChangeHashTagModelTask))
                                    || (Task.GetType() == info.Task.GetType());

                var isStateSimilar = IsOk && info.IsOk;

                return isTypeSimilar && isStateSimilar;
            }
        }
    }
}