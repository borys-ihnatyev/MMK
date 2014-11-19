using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MMK.Notify.Observer.Tasking
{
    public partial class Task
    {
        [Serializable]
        public class Cancel : Exception
        {
        }
    }
}
