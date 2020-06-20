using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mdfinder
{
    public class FileRecord : PropertyChangedAlerter
    {
        #region Members

        /// <summary> The identifier. </summary>
        private string id;

        /// <summary> Full pathname of the file. </summary>
        private Uri path;

        /// <summary> The size. </summary>
        private long size;

        /// <summary> The hash. </summary>
        private string hash;

        /// <summary> The hash provider. </summary>
        private string hashProvider;

        /// <summary> True to keep. </summary>
        private bool keep;

        #endregion Members

        #region Properties

        /// <summary> Gets or sets the identifier. </summary>
        /// <value> The identifier. </value>
        public string Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
                OnPropertyChanged();
            }
        }

        /// <summary> Gets or sets the full pathname of the file. </summary>
        /// <value> The full pathname of the file. </value>
        public Uri Path
        {
            get
            {
                return this.path;
            }
            set
            {
                this.path = value;
                OnPropertyChanged();
            }
        }

        /// <summary> Gets or sets the size. </summary>
        /// <value> The size. </value>
        public long Size
        {
            get
            {
                return this.size;
            }
            set
            {
                this.size = value;
                OnPropertyChanged();
            }
        }

        /// <summary> Gets or sets the hash. </summary>
        /// <value> The hash. </value>
        public string Hash
        {
            get
            {
                return this.hash;
            }
            set
            {
                this.hash = value;
                OnPropertyChanged();
            }
        }

        /// <summary> Gets or sets the hash provider. </summary>
        /// <value> The hash provider. </value>
        public string HashProvider
        {
            get
            {
                return this.hashProvider;
            }
            set
            {
                this.hashProvider = value;
                OnPropertyChanged();
            }
        }

        /// <summary> Gets or sets a value indicating whether to keep the file when processing duplicates. </summary>
        /// <value> True if keep, false if not. </value>
        public bool Keep
        {
            get
            {
                return this.keep;
            }
            set
            {
                this.keep = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region Constructors

        public FileRecord()
        {
            this.Id = string.Empty;
            this.Path = default(Uri);
            this.Size = 0;
            this.Hash = string.Empty;
            this.HashProvider = string.Empty;
        }

        /// <summary> Constructor. </summary>
        /// <param name="path">         Full pathname of the file. </param>
        /// <param name="size">         The size. </param>
        /// <param name="hash">         The hash. </param>
        /// <param name="hashProvider"> The hash provider. </param>
        public FileRecord(string path, long size, string hash, string hashProvider)
        {
            this.Id = Guid.NewGuid().ToString();
            this.Path = new Uri(path);
            this.Size = size;
            this.Hash = hash;
            this.HashProvider = hashProvider;
        }

        #endregion Constructors
    }
}