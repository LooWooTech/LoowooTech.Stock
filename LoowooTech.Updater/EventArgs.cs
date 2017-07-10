using LoowooTech.Updater.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoowooTech.Updater
{
    public enum UpdateStateChangeStateEnum
    {
        InProgress,Done,Fail
    }

    public class UpdateProgressChangedEventArgs:EventArgs
    {
        public string Message { get; set; }

        public long TotalProgress { get; set; }

        public long CurrentProgress { get; set; }

        public UpdateStateChangeStateEnum State { get; set; }

        public bool NeedCancel { get; set; }
    }

}
