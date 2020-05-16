using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Retail.Data.SqlDb.Tests.TestRecordFactory
{
    internal static class IdFactory
    {
        private static int _id;

        public static int Next() => Interlocked.Increment(ref _id);
    }
}
