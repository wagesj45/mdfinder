using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        #endregion

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

        #endregion

        #region Constructors

        public ScanResults()
        {
            this.duplicateFiles = Enumerable.Empty<DuplicateFileGroup>();
            this.selectedDuplicateFileGroup = new DuplicateFileGroup(Enumerable.Empty<FileRecord>());
        }

        #endregion
    }
}
