using mdfinder.hashprovider;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace mdfinder
{
    public class MD5HashProvider : IHashProvider
    {
        /// <summary> The file extensions this hash provider works with. </summary>
        private readonly string[] fileExtensions = new [] { "*" };

        /// <summary> The MD5 instance. </summary>
        private MD5 md5;

        /// <summary> Gets the name of the hash provider. </summary>
        /// <value> The name of the hash provider. </value>
        public string Name
        {
            get
            {
                return Localization.Localization.DefaultHashProviderName;
            }
        }

        /// <summary> Gets the description of the hash provider. </summary>
        /// <value> The description. </value>
        public string Description
        {
            get
            {
                return Localization.Localization.DefaultHashProviderDescription;
            }
        }

        /// <summary> Gets a value indicating if this hash provider is valid to use. </summary>
        /// <remarks>
        /// This value can be used by hash providers to limit use of the provider. This can be used to
        /// create paid hash providers.
        /// </remarks>
        /// <value> The value indicating if the hash provider is valid. </value>
        public bool IsValid
        {
            get
            {
                return true;
            }
        }

        public IEnumerable<string> FileExtensions
        {
            get
            {
                return this.fileExtensions;
            }
        }

        /// <summary>
        /// Gets or sets the priority this provider has when multiple providers support the same
        /// extension.
        /// </summary>
        /// <exception cref="NotSupportedException"> Thrown when the requested operation is not supported. </exception>
        /// <value> The priority of the hash provider. </value>
        public int Priority
        {
            get
            {
                return 1;
            }
            set
            {
                throw new NotSupportedException(Localization.Localization.MD5ProviderSetPriorityException);
            }
        }

        /// <summary> Default constructor. </summary>
        public MD5HashProvider()
        {
            this.md5 = MD5.Create();
        }

        /// <summary> Gets a hash from a file. </summary>
        /// <param name="file"> The file to hash. </param>
        /// <returns> The hash. </returns>
        public string GetHash(FileInfo file)
        {
            try
            {
                return BitConverter.ToString(this.md5.ComputeHash(file.OpenRead()));
            }
            catch
            {
                //
            }

            return string.Empty;
        }
    }
}
