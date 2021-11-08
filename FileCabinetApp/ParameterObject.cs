using System;

namespace FileCabinetApp
{
        /// <summary>
        /// Contains method for pass parameters.
        /// </summary>
        public class ParameterObject
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ParameterObject"/> class.
            /// Passes parameters.
            /// </summary>
            /// <param name="firstName">First name.</param>
            /// <param name="lastName">Last name.</param>
            /// <param name="dateOfBirth">Date of birth.</param>
            /// <param name="property1">Property in "short" format.</param>
            /// <param name="property2">Property in "decimal" format.</param>
            /// <param name="property3">Property in "char" format.</param>
            public ParameterObject(string firstName, string lastName, DateTime dateOfBirth, short property1, decimal property2, char property3)
            {
                this.FirstName = firstName;
                this.LastName = lastName;
                this.DateOfBirth = dateOfBirth;
                this.Property1 = property1;
                this.Property2 = property2;
                this.Property3 = property3;
            }

            /// <summary>
            /// Gets First name.
            /// </summary>
            /// <value>
            /// FirstName.
            /// </value>
            public string FirstName { get; private set; }

            /// <summary>
            /// Gets Last name.
            /// </summary>
            /// <value>
            /// LastName.
            /// </value>
            public string LastName { get; private set; }

            /// <summary>
            /// Gets Date of birth.
            /// </summary>
            /// <value>
            /// DateOfBirth.
            /// </value>
            public DateTime DateOfBirth { get; private set; }

            /// <summary>
            /// Gets Property in "short" format.
            /// </summary>
            /// <value>
            /// Property1.
            /// </value>
            public short Property1 { get; private set; }

            /// <summary>
            /// Gets Property in "decimal" format.
            /// </summary>
            /// <value>
            /// Property2.
            /// </value>
            public decimal Property2 { get; private set; }

            /// <summary>
            /// Gets Property in "char" format.
            /// </summary>
            /// <value>
            /// Property3.
            /// </value>
            public char Property3 { get; private set; }
        }
}