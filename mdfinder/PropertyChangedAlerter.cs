﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace mdfinder
{
    /// <summary> A class responsible for implementing the <see cref="INotifyPropertyChanged"/> interface and helper functions. </summary>
    public abstract class PropertyChangedAlerter : INotifyPropertyChanged
    {
        #region Members

        /// <summary> Occurs when a property value changes. </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary> A list of properties to always call as updated. Generally used for composite properties. </summary>
        private List<string> alwaysCall = new List<string>();

        #endregion

        #region Properties

        //

        #endregion

        #region Methods

        /// <summary>
        /// Executes the property changed action. This alerts subscribers to its change in value.
        /// </summary>
        /// <param name="name"> (Optional) The name of the property. </param>
        /// <example>
        /// This will automatically pass in "SomeProperty" as the property name, derived useing the
        /// <see cref="CallerMemberNameAttribute" /> attribute.
        /// <code lang="cs" title="Automatic Property Detection">
        /// public bool SomeProperty
        /// {
        ///     get
        ///     {
        ///         return this.someProperty;
        ///     }
        ///     set
        ///     {
        ///         this.someProperty = value;
        ///         OnPropertyChanged();
        ///     }
        /// }
        /// </code>
        /// </example>
        protected virtual void OnPropertyChanged([CallerMemberName]string name = null)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
                foreach (var updatedProperty in this.alwaysCall)
                {
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(updatedProperty));
                }
            }
        }

        /// <summary> Executes when all properties are changed and should be updated. </summary>
        protected virtual void OnAllPropertiesChanged()
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }

        /// <summary> Adds a property that will always be called when any property is updated.. </summary>
        /// <param name="name"> The name of the property. </param>
        public void AddConstantCallProperty(string name)
        {
            if (this.alwaysCall == null)
            {
                // This item has been deserialized and the list needs to be reinitialized.
                this.alwaysCall = new List<string>();
            }

            if (!this.alwaysCall.Any(c => c.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                this.alwaysCall.Add(name);
            }
        }

        #endregion
    }
}
