using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TimothyK.Data.UnitOfWork
{
    public class LocalDbAttacher
    {
        /// <summary>
        /// Name of the Catalog (i.e. database name) on the local DB SQL Server
        /// </summary>
        public string DatabaseName { get; }
        public override string ToString() => DatabaseName;

        private readonly Assembly Assembly;
        private DirectoryInfo AssemblyPath => new FileInfo(Assembly.Location).Directory;

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
        public LocalDbAttacher(Type workerClass, string methodName = null)
        {
            DatabaseName = "Test_"
                + workerClass.FullName
                + (string.IsNullOrWhiteSpace(methodName) ? string.Empty : "_" + methodName);

            Assembly = workerClass.Assembly;

        }

        public LocalDbAttacher(Assembly workerAssembly, string workContext = null) 
        {
            DatabaseName = "Test_"
                + workerAssembly.GetName().Name
                + (string.IsNullOrWhiteSpace(workContext) ? string.Empty : "_" + workContext);

            Assembly = workerAssembly;
        }

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
        /// var attachedDatabase = new LocalDbAttacher(typeof(UnitTest1))
        ///     .AttachDatabase(@"Databases\MyProduct.mdf", @"Databases\MyProduct_log.ldf");
        /// //...
        /// attachedDatabase.DropDatabase();
        /// </code>
        /// </example>
        public LocalDbAttacher AttachDatabase(params string[] databaseFiles)
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

            using (var dbMaster = new MasterDbContext())
            {
                dbMaster.DropDatabase(DatabaseName);

                //Copy the database files
                foreach (var map in renameMap)
                    map.Key.CopyTo(map.Value.FullName, overwrite: true);

                dbMaster.AttachDatabase(DatabaseName, renameMap.Values.ToArray());
            }

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
            using (var dbMaster = new MasterDbContext())
                dbMaster.DropDatabase(DatabaseName);
        }


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
