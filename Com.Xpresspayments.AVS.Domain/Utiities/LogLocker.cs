using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Xpresspayments.AVS.Common.Utiities
{
    public class LogLocker : log4net.Appender.FileAppender.MinimalLock
    {
            public override Stream AcquireLock()
            {
                if (CurrentAppender.Threshold == log4net.Core.Level.Off)
                    return null;

                return base.AcquireLock();
            }
        
    }
}
