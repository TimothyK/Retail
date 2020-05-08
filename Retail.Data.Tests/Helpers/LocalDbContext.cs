using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Retail.Data.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Retail.Data.Tests.Extensions
{

    /// <summary>
    /// Manages creation of a SQL Server database for a given Test Class instance or test methods.
    /// Databases are created from copies of the MDF/LDF/NDF files.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="LocalDbContext"/> holds a context of a unit test class or test method.
    /// It can create a create a new copy (see <see cref="AttachDatabase(string[])"/>) 
    /// of a database using the context as the new database name.
    /// Because each test context is uniquely named, the unit tests are isolated from each other.
    /// The database copy it thrown away (see <see cref="DropDatabase"/>) after the test ends, there the test environment
    /// is rest to a clean state.
    /// </para>
    /// <para>
    /// The database is attached to the LocalDb database server.  
    /// This can be overridden with <see cref="MasterDbContext.ConnectionString"/>.
    /// </para>
    /// </remarks>
    public class LocalDbContext
    {
        public readonly Type TestClassType;
        public readonly DirectoryInfo AssemblyPath;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="testClassType">Type of the Test Class.  
        /// This will be used to uniquely identify the database copy to create.
        /// The Full Name of this type is used as the database name.
        /// </param>
        /// <param name="testMethodName">Optional test name to add to the context.  
        /// Should be used if each test within a class creates its own database.
        /// </param>
        /// <remarks>
        /// <para>
        /// It is on the honor system that testMethodName includes legal file name characters (e.g. no * or ?)
        /// and also does not contain any SQL injection code for the Create/Drop Database commands.
        /// This class should only be used from a test project, not through a public API.  
        /// Therefore the attack vector is quite low. 
        /// </para>
        /// </remarks>
        public LocalDbContext(Type testClassType, string testMethodName = null)
        {
            TestClassType = testClassType;

            DatabaseName = "Test_"
                + TestClassType.FullName
                + (string.IsNullOrWhiteSpace(testMethodName) ? string.Empty : " " + testMethodName);                              

            var assembly = TestClassType.Assembly;
            var file = new FileInfo(assembly.Location);
            AssemblyPath = file.Directory;
        }

        /// <summary>
        /// Name of the database copy to create
        /// </summary>
        public string DatabaseName { get; }

        /// <summary>
        /// Attaches a database from MDF, NDF, and LDF files to a new database on the LocalDb SQL Server.
        /// </summary>
        /// <param name="databaseFiles">Relative path to database file(s).  
        /// Path is relative to the local of the test assembly DLL, which contains <see cref="TestClassType"/>.
        /// </param>
        /// <returns></returns>
        /// <remarks>
        /// <para>
        /// The database is attached to the LocalDb database server.  
        /// This can be overridden with <see cref="MasterDbContext.ConnectionString"/>.
        /// </para>
        /// <para>
        /// The MDF and LDF (and NDF) files should be added to the Test project.
        /// Open the file properties from the Solution Explorer and mark the files to Copy to Output Always.
        /// </para>
        /// <para>
        /// You will need to detach the database from the existing SQL Server instance then copy the files to the test project folder.
        /// You may attach these files to a server right from the test project.  
        /// Then you can make modifications to be included in the test project.
        /// Be sure to detach the database before committing the changes to source control or running the tests;
        /// the files need to be detached in order to be safely copied.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// var localDb = new LocalDbContext(typeof(UnitTest1));
        /// localDb.AttachDatabase(@"Databases\Retail.mdf", @"Databases\Retail_log.ldf");
        /// </code>
        /// </example>
        public LocalDbContext AttachDatabase(params string[] databaseFiles)
        {
            if (databaseFiles == null || databaseFiles.Length < 1)
                throw new ArgumentException("At least 1 MDF file must be specified", nameof(databaseFiles));

            var renameMap = databaseFiles
                .Select(file => AssemblyPath.Combine(file))
                .ToDictionary(
                    file => file
                    , file => new FileInfo($"{file.DirectoryName}\\{DatabaseName}_{file.Name}")
                );
            GuardFilesExist(renameMap.Keys);

            using var dbMaster = new MasterDbContext();
            
            dbMaster.DropDatabase(DatabaseName);

            //Copy the database files
            foreach (var map in renameMap)
                map.Key.CopyTo(map.Value.FullName, overwrite: true);

            dbMaster.AttachDatabase(DatabaseName, renameMap.Values.ToArray());
            
            return this;
        }

        private static void GuardFilesExist(IEnumerable<FileInfo> files)
        {
            var badFile = files.FirstOrDefault(file => !file.Exists);
            if (badFile != null)
                throw new FileNotFoundException("Database file to attach was not found", badFile.FullName);
        }

        /// <summary>
        /// Drop the database from the server.  This will implicitly remove the MDF/LDF file copies.
        /// </summary>
        public void DropDatabase()
        {
            using var dbMaster = new MasterDbContext();
            dbMaster.DropDatabase(DatabaseName);
        }

        /// <summary>
        /// Connection options for EF Core DbContext
        /// </summary>
        /// <typeparam name="T">DbContext sub class</typeparam>
        /// <returns></returns>
        public DbContextOptions<T> GetDbConnectionOptions<T>() where T : DbContext
        {
            var contextBuilder = new DbContextOptionsBuilder<T>();
            contextBuilder
                .UseSqlServer(ConnectionString.ConnectionString);

            return contextBuilder.Options;
        }

        /// <summary>
        /// Connection String to the database for this text context.
        /// </summary>
        /// <seealso cref="DatabaseName"/>
        public SqlConnectionStringBuilder ConnectionString
        {
            get
            {
                var connStr = MasterDbContext.ConnectionString;
                connStr.InitialCatalog = DatabaseName;
                return connStr;
            }
        }

    }

}
