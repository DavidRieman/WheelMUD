
namespace NCalc
{
    using System;

    /// <summary>
    /// Parameter Arguments
    /// </summary>
    public class ParameterArgs : EventArgs
    {
        /// <summary>
        /// Backing member variable for the Result property.
        /// </summary>
        private object result;

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>The result.</value>
        public object Result
        {
            get
            {
                return this.result;
            }

            set
            {
                this.result = value;
                this.HasResult = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has result.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has result; otherwise, <c>false</c>.
        /// </value>
        public bool HasResult { get; set; }
    }
}
