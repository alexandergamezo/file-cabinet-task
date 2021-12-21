using System;

namespace FileCabinetGenerator
{      
    /// <summary>
    /// Provides access to values of variables by using properties.
    /// </summary>
    public class FileCabinetRecord
    {        
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value>
        /// Id.
        /// </value>
        public int Id { get; set; }
                        
        /// <summary>
        /// Gets or sets First name.
        /// </summary>
        /// <value>
        /// FirstName.
        /// </value>
        public string FirstName { get; set; }
                
        /// <summary>
        /// Gets or sets Last name.
        /// </summary>
        /// <value>
        /// LastName.
        /// </value>
        public string LastName { get; set; }
        
        /// <summary>
        /// Gets or sets Date of birth.
        /// </summary>
        /// <value>
        /// DateOfBirth.
        /// </value>
        public DateTime DateOfBirth { get; set; }
        
        /// <summary>
        /// Gets or sets Property in "short" format.
        /// </summary>
        /// <value>
        /// Property1.
        /// </value>
        public short Property1 { get; set; }
        
        /// <summary>
        /// Gets or sets Property in "decimal" format.
        /// </summary>
        /// <value>
        /// Property2.
        /// </value>
        public decimal Property2 { get; set; }
       
        /// <summary>
        /// Gets or sets Property in "char" format.
        /// </summary>
        /// <value>
        /// Property3.
        /// </value>
        public char Property3 { get; set; }
    }
}
