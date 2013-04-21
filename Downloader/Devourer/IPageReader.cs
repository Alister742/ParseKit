using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ParseKit.Interfaces
{
    public delegate void ReadCompleteDel(DevourTarget self);
    public delegate void NewTargetsDel(List<DevourTarget> targets);

    public interface IPageReader
    {
        //event ResultsDel OnPageReaded;
        event NewTargetsDel OnNewTargets;
        event ReadCompleteDel OnReadComplete;

        void ReadData(string data, DevourTarget target);
    }
}
