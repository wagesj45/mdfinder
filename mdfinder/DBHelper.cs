using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;

namespace mdfinder
{
    /// <summary> A database helper class. </summary>
    public class DBHelper : PropertyChangedAlerter
    {
        #region Members

        /// <summary> The default database file name. </summary>
        private const string DEFAULT_DB_FILE_NAME = "mdfinder.db";

        #endregion Members

        #region Properties

        /// <summary> Gets or sets the database. </summary>
        /// <value> The database. </value>
        private LiteDatabase Database { get; set; }

        /// <summary> Gets a collection of file records. </summary>
        /// <value> A collection of file records. </value>
        private LiteCollection<FileRecord> FileRecordCollection
        {
            get
            {
                return this.Database.GetCollection<FileRecord>("FileRecords");
            }
        }

        /// <summary> Gets the database statistics string. </summary>
        /// <value> The database statistics. </value>
        public string DbStatistics
        {
            get
            {
                return string.Format("{0} Files In Database.", this.FileRecordCollection.Count());
            }
        }

        #endregion Properties

        #region Constructors

        /// <summary> Default constructor. </summary>
        public DBHelper()
        {
            this.Database = new LiteDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), DEFAULT_DB_FILE_NAME));
        }

        /// <summary> Constructor. </summary>
        /// <param name="database"> The database. </param>
        public DBHelper(string database)
        {
            this.Database = new LiteDatabase(database);
        }

        #endregion Constructors

        #region Methods

        /// <summary> Inserts a file record. </summary>
        /// <param name="path">         Full pathname of the file. </param>
        /// <param name="size">         The size. </param>
        /// <param name="hash">         The hash. </param>
        /// <param name="hashProvider"> The hash provider. </param>
        public void InsertFileRecord(string path, long size, string hash, string hashProvider)
        {
            var fileRecord = new FileRecord(path, size, hash, hashProvider);
            this.FileRecordCollection.Upsert(fileRecord);

            this.OnPropertyChanged("DbStatistics");
        }

        /// <summary> Removes the file record described by its path. </summary>
        /// <param name="id"> The <see cref="Guid"/> ID of the file. </param>
        public void RemoveFileRecord(string id)
        {
            this.FileRecordCollection.Delete(fr => fr.Id == id);

            this.OnPropertyChanged("DbStatistics");
        }

        /// <summary> Gets the file records in this collection. </summary>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the file records in this collection.
        /// </returns>
        public IEnumerable<FileRecord> GetFileRecords()
        {
            return this.FileRecordCollection.FindAll();
        }

        /// <summary> Gets the file records in this collection. </summary>
        /// <param name="predicate"> The predicate. </param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the file records in this collection.
        /// </returns>
        public IEnumerable<FileRecord> GetFileRecords(Func<FileRecord, bool> predicate)
        {
            return this.FileRecordCollection.Find(fr => predicate(fr));
        }

        /// <summary> Clears the database to its blank/initial state. </summary>
        public void Clear()
        {
            this.FileRecordCollection.Delete(Query.All());

            this.OnPropertyChanged("DbStatistics");
        }

        #endregion Methods
    }
}