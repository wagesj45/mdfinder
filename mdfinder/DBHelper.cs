using LiteDB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mdfinder
{
    /// <summary> A database helper class. </summary>
    public class DBHelper : PropertyChangedAlerter
    {
        #region Members

        /// <summary> The default database file name. </summary>
        private const string DEFAULT_DB_FILE_NAME = "mdfinder.db";

        #endregion

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

        #endregion

        #region Constructors

        /// <summary> Default constructor. </summary>
        public DBHelper()
        {
            this.Database = new LiteDatabase(DEFAULT_DB_FILE_NAME);
        }

        /// <summary> Constructor. </summary>
        /// <param name="database"> The database. </param>
        public DBHelper(string database)
        {
            this.Database = new LiteDatabase(database);
        }

        #endregion

        #region Methods

        /// <summary> Inserts a file record. </summary>
        /// <param name="path">         Full pathname of the file. </param>
        /// <param name="size">         The size. </param>
        /// <param name="hash">         The hash. </param>
        /// <param name="hashProvider"> The hash provider. </param>
        public void InsertFileRecord(string path, long size, string hash, string hashProvider)
        {
            var fileRecord = new FileRecord() { Path = new Uri(path), Size = size, Hash = hash, HashProvider = hashProvider };
            this.FileRecordCollection.Insert(fileRecord);
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
        }

        #endregion
    }
}
