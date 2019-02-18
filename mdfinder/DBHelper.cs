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

        /// <summary> Gets the file records. </summary>
        /// <value> The file records. </value>
        public LiteCollection<FileRecord> FileRecords
        {
            get
            {
                return this.Database.GetCollection<FileRecord>("FileRecords");
            }
        }

        public IEnumerable<FileRecord> ASDF
        {
            get
            {
                return this.FileRecords.FindAll();
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
            this.FileRecords.Insert(new FileRecord() { Path = path, Size = size, Hash = hash, HashProvider = hashProvider });
            OnPropertyChanged("ASDF");
        }

        #endregion
    }
}
