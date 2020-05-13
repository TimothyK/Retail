using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;

namespace TimothyK.Testing
{
    /// <summary>
    /// Base class for creating a <see cref="DbContext"/> for testing
    /// </summary>
    /// <typeparam name="TContext">Must have a constructor that takes <see cref="DbContextOptions{TContext}"/> as a parameter</typeparam>
    public abstract class DbContextFactory<TContext> : IDisposable where TContext : DbContext
    {
        /// <summary>
        /// Name of the test database instance to be created by this factory
        /// </summary>
        /// <remarks>
        /// <para>
        /// Each instance of this class logically represents a different database that can be used to run a set of tests.
        /// This property gives a logical name to that database.
        /// In some subclasses this DatabaseName may be used to uniquely create the instance of the test database.
        /// </para>
        /// </remarks>
        public string DatabaseName { get; }
        public override string ToString() => DatabaseName;

        protected readonly Assembly Assembly;

        #region constructors

        public DbContextFactory(Type testClassType, string testMethodName = null)
        {
            DatabaseName = "Test_"
                + testClassType.FullName
                + (string.IsNullOrWhiteSpace(testMethodName) ? string.Empty : " " + testMethodName);

            Assembly = testClassType.Assembly;
        }

        public DbContextFactory(Assembly assembly, string testContext = null)
        {
            DatabaseName = "Test_"
                + assembly.GetName().Name
                + (string.IsNullOrWhiteSpace(testContext) ? string.Empty : " " + testContext);

            Assembly = assembly;
        }
        public virtual void Dispose()
        {
        }
        
        #endregion

        #region Create Context

        public virtual TContext CreateContext()
        {
            var options = CreateOptions();
            return (TContext)Activator.CreateInstance(typeof(TContext), options);
        }

        protected abstract DbContextOptions<TContext> CreateOptions();

        #endregion
    }
}
