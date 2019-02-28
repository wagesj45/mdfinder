using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mdfinder
{
    public class DuplicateFileGroup : PropertyChangedAlerter
    {
        #region Properties

        /// <summary> Gets the hash. </summary>
        /// <value> The hash. </value>
        public string Hash { get; }

        /// <summary> Gets the number of.  </summary>
        /// <value> The count. </value>
        public int Count { get; }

        /// <summary> Gets the total size of the file group. </summary>
        /// <value> The total number of size. </value>
        public long TotalSize { get; }

        /// <summary> Gets the potential size saving. </summary>
        /// <value> The potential size saving. </value>
        public long PotentialSizeSaving { get; }

        /// <summary> Gets or sets the file records. </summary>
        /// <value> The file records. </value>
        public List<FileRecord> FileRecords { get; set; }

        #endregion

        /// <summary> Constructor. </summary>
        /// <param name="fileRecords"> The file records. </param>
        /// <param name="savingsType"> (Optional) Type of the size savings to calculate. </param>
        public DuplicateFileGroup(IEnumerable<FileRecord> fileRecords, SavingsType savingsType = SavingsType.SaveBiggest)
        {
            this.FileRecords = new List<FileRecord>(fileRecords);

            //Precalculate stats.
            this.Hash = this.FileRecords.Select(fr => fr.Hash).FirstOrDefault();
            this.Count = this.FileRecords.Count();

            this.TotalSize = this.FileRecords.Sum(fr => fr.Size);

            switch (savingsType)
            {
                case SavingsType.SaveBiggest:
                    this.PotentialSizeSaving = this.FileRecords.OrderByDescending(fr => fr.Size).Skip(1).Sum(fr => fr.Size); 
                    break;
                case SavingsType.SaveSmallest:
                    this.PotentialSizeSaving = this.FileRecords.OrderBy(fr => fr.Size).Skip(1).Sum(fr => fr.Size);
                    break;
                case SavingsType.SaveMedian:
                    //This is kind of hacky, but good enough for our purposes here. CLOSE ENOUGH
                    var medianFileRecord = this.FileRecords.OrderBy(fr => fr.Size).ElementAt(this.Count / 2);
                    this.PotentialSizeSaving = this.FileRecords.Except(new[] { medianFileRecord }).Sum(fr => fr.Size);
                    break;
                default:
                    break;
            }
        }

        /// <summary> Values that represent the ways of saving space. </summary>
        public enum SavingsType
        {
            /// <summary> Saves the biggest, and presumably highest quality, file. </summary>
            SaveBiggest,
            /// <summary> Saves the smallest file. </summary>
            SaveSmallest,
            /// <summary> . </summary>
            SaveMedian
        }
    }
}
