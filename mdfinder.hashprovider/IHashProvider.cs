using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mdfinder.hashprovider
{
    public interface IHashProvider
    {
        /// <summary> Gets the name of the hash provider. </summary>
        /// <value> The name of the hash provider. </value>
        string Name { get; }

        /// <summary> Gets the description of the hash provider. </summary>
        /// <value> The description. </value>
        string Description { get; }

        /// <summary> Gets a value indicating if this hash provider is valid to use. </summary>
        /// <value> The value indicating if the hash provider is valid. </value>
        /// <remarks> This value can be used by hash providers to limit use of the provider. This can be used to create paid hash providers. </remarks>
        bool IsValid { get; }

        /// <summary> Gets or sets the priority this provider has when multiple providers support the same extension. </summary>
        /// <value> The priority of the hash provider. </value>
        int Priority { get; set; }

        /// <summary> Gets the file extensions that this hash provider can work with. </summary>
        /// <value> The file extensions the hash provider works with. </value>
        IEnumerable<string> FileExtensions { get; }

        /// <summary> Gets a hash from a file. </summary>
        /// <param name="file"> The file to hash. </param>
        /// <returns> The hash. </returns>
        string GetHash(FileInfo file);
    }
}
