using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mdfinder
{
    public class FileRecord
    {
        #region Properties

        /// <summary> Gets or sets the identifier. </summary>
        /// <value> The identifier. </value>
        public Int64 Id { get; set; }

        /// <summary> Gets or sets the full pathname of the file. </summary>
        /// <value> The full pathname of the file. </value>
        public string Path { get; set; }

        /// <summary> Gets or sets the size. </summary>
        /// <value> The size. </value>
        public long Size { get; set; }

        /// <summary> Gets or sets the hash. </summary>
        /// <value> The hash. </value>
        public string Hash { get; set; }

        /// <summary> Gets or sets the hash provider. </summary>
        /// <value> The hash provider. </value>
        public string HashProvider { get; set; }

        #endregion
    }
}
