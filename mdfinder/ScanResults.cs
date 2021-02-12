using System.Collections.Generic;
using System.Linq;

namespace mdfinder
{
    /// <summary> A scan results model providing bindable properties. </summary>
    public class ScanResults : PropertyChangedAlerter
    {
        #region Members

        /// <summary> The duplicate files. </summary>
        private IEnumerable<DuplicateFileGroup> duplicateFiles;

        /// <summary> The selected duplicate file group. </summary>
        private DuplicateFileGroup selectedDuplicateFileGroup;

        #endregion Members

        #region Properties

        /// <summary> Gets or sets the duplicate files. </summary>
        /// <value> The duplicate files. </value>
        public IEnumerable<DuplicateFileGroup> DuplicateFiles
        {
            get
            {
                return this.duplicateFiles;
            }
            set
            {
                this.duplicateFiles = value;
                OnPropertyChanged();
            }
        }

        /// <summary> Gets or sets the selected duplicate file group. </summary>
        /// <value> The selected duplicate file group. </value>
        public DuplicateFileGroup SelectedDuplicateFileGroup
        {
            get
            {
                return this.selectedDuplicateFileGroup;
            }
            set
            {
                this.selectedDuplicateFileGroup = value;
                OnPropertyChanged();
            }
        }

        /// <summary> Gets the statistics string. </summary>
        /// <value> The statistics string. </value>
        public string Statistics
        {
            get
            {
                return string.Format("{0} Duplicates", this.duplicateFiles.Sum(df => df.Count - 1));
            }
        }

        #endregion Properties

        #region Constructors

        public ScanResults()
        {
            this.duplicateFiles = Enumerable.Empty<DuplicateFileGroup>();
            this.selectedDuplicateFileGroup = new DuplicateFileGroup(Enumerable.Empty<FileRecord>());

            this.AddConstantCallProperty("Statistics");
        }

        #endregion Constructors
    }
}