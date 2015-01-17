using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using MMK.Marking.Representation;
using MMK.Notify.Observer.Tasking.Common.Base;

namespace MMK.Notify.Observer.Tasking.Common
{
    [Serializable]
    public class ChangeHashTagModelTask : Mp3FileChangeTask
    {
        protected readonly HashTagModel AddHashTagModel;
        protected readonly HashTagModel RemoveHashTagModel;

        public static IEnumerable<Task> Many(IEnumerable<string> paths, HashTagModel add, HashTagModel remove)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");
            Contract.EndContractBlock();

            return paths.Select(p => new ChangeHashTagModelTask(p, add, remove));
        }  

        public ChangeHashTagModelTask(string oldPath, HashTagModel add, HashTagModel remove)
            : base(oldPath)
        {
            AddHashTagModel = add;
            RemoveHashTagModel = remove;
        }

        public HashTagModel OldHashTagModel { get; private set; }

        public HashTagModel NewHashTagModel { get; private set; }

        protected virtual HashTagModel MakeNewHashTagModel()
        {
            var newHashTagModel = new HashTagModel(NameModel.HashTagModel);
            newHashTagModel += AddHashTagModel;
            newHashTagModel -= RemoveHashTagModel;
            return newHashTagModel;
        }

        protected override void Initialize()
        {
            base.Initialize();

            NewHashTagModel = MakeNewHashTagModel();
            OldHashTagModel = NameModel.HashTagModel;
            
            if(NewHashTagModel.SetEquals(OldHashTagModel))
                throw new Cancel("Hash tag model not changed");

            NameModel.HashTagModel = NewHashTagModel;

            var directory = OldFile.DirectoryName;
            Contract.Assume(directory != null);
            var newFilePath = Path.Combine(directory, NameModel.FullName + OldFile.Extension);
            NewFile = new FileInfo(newFilePath);
        }

        protected override string CommonDescription
        {
            get { return "Change hash tag model"; }
        }

        protected override string DetailedDescription
        {
            get
            {
                if (OldHashTagModel.IsEmpty())
                    return string.Format("Add hash tag model : {0}", NewHashTagModel);

                if (NewHashTagModel.IsEmpty())
                    return string.Format("Remove hash tag model : {0}", OldHashTagModel);

                return string.Format("Changed hash tag model\nFrom : {0}\nTo : {1}", OldHashTagModel, NewHashTagModel);
            }
        }

        #region Serialization

        protected const string AddHashTagsVar = "AddHashTagModel";
        protected const string RemoveHashTagsVar = "RemoveHashTagModel";

        protected ChangeHashTagModelTask(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            AddHashTagModel = info.GetValue(AddHashTagsVar,typeof(HashTagModel)) as HashTagModel;
            RemoveHashTagModel = info.GetValue(RemoveHashTagsVar, typeof(HashTagModel)) as HashTagModel;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(AddHashTagsVar,AddHashTagModel,typeof(HashTagModel));
            info.AddValue(AddHashTagsVar,RemoveHashTagModel,typeof(HashTagModel));
        }

        #endregion
    }
}