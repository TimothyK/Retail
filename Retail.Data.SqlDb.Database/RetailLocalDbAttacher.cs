using System;
using TimothyK.Data;

namespace Retail.Data.SqlDb.Database
{
    public class RetailLocalDbAttacher : LocalDbAttacher
    {
        public RetailLocalDbAttacher(Type workerClass) 
            : base(workerClass)
        {
            AttachDatabase(@"Databases\Retail.mdf", @"Databases\Retail_log.ldf");
        }


    }
}
