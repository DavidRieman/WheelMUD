
namespace NCalc
{
    using System;

    /// <summary>
    /// Function Arguments
    /// </summary>
    public class FunctionArgs : EventArgs
    {
        /// <summary>
        /// Member variable that holds the value for the Result property.
        /// </summary>
        private object result;

        /// <summary>
        /// Backing member variable.
        /// </summary>
        private Expression[] parameters = new Expression[0];

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
        ///       <c>true</c> if this instance has result; otherwise, <c>false</c>.
        /// </value>
        public bool HasResult { get; set; }

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public Expression[] Parameters
        {
            get { return this.parameters; }
            set { this.parameters = value; }
        }

        /// <summary>
        /// Evaluate parameters.
        /// </summary>
        /// <returns>Object array</returns>
        public object[] _EvaluateParameters()
        {
            var values = new object[this.parameters.Length];

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = this.parameters[i].Evaluate();
            }

            return values;
        }
    }
}
