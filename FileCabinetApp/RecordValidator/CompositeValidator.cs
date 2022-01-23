using System.Collections.Generic;

namespace FileCabinetApp.RecordValidator
{
    /// <summary>
    /// The Composite class represents the complex components.
    /// </summary>
    public class CompositeValidator : IRecordValidator
    {
        private readonly List<IRecordValidator> validators = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValidator"/> class.
        /// </summary>
        /// <param name="validators">List of validators.</param>
        public CompositeValidator(IEnumerable<IRecordValidator> validators)
        {
            foreach (var validator in validators)
            {
                this.validators.Add(validator);
            }
        }

        /// <summary>
        /// Validates parameters.
        /// </summary>
        /// <param name="v">Pararmeter object.</param>
        public void ValidateParameters(ParameterObject v)
        {
            foreach (var validator in this.validators)
            {
                validator.ValidateParameters(v);
            }
        }
    }
}