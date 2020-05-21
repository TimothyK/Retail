using System;
using System.Collections.Generic;
using System.Text;

namespace Retail.Data.SqlDb.TestRecordFactory
{
    public static class LinqExtentions
    {
        /// <summary>
        /// Provides additional setup for a object in a Fluent API.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T With<T>(this T obj, Action<T> action)
        {
            action(obj);
            return obj;
        }
    }
}
