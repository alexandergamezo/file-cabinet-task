using Newtonsoft.Json;

namespace FileCabinetApp.Models
{
    /// <summary>
    /// Validation.
    /// </summary>
    public static class Validation
    {
        /// <summary>
        /// First name.
        /// </summary>
        [JsonObject("firstName")]
        public class FirstName
        {
            /// <summary>
            /// Gets or sets min value.
            /// </summary>
            /// <value>
            /// Min value.
            /// </value>
            [JsonProperty("min")]
            public int Min { get; set; }

            /// <summary>
            /// Gets or sets max value.
            /// </summary>
            /// <value>
            /// Max value.
            /// </value>
            [JsonProperty("max")]
            public int Max { get; set; }
        }

        /// <summary>
        /// Last name.
        /// </summary>
        [JsonObject("lastName")]
        public class LastName
        {
            /// <summary>
            /// Gets or sets min value.
            /// </summary>
            /// <value>
            /// Min value.
            /// </value>
            [JsonProperty("min")]
            public int Min { get; set; }

            /// <summary>
            /// Gets or sets max value.
            /// </summary>
            /// <value>
            /// Max value.
            /// </value>
            [JsonProperty("max")]
            public int Max { get; set; }
        }

        /// <summary>
        /// Date of birth.
        /// </summary>
        [JsonObject("dateOfBirth")]
        public class DateOfBirth
        {
            /// <summary>
            /// Gets or sets min value.
            /// </summary>
            /// <value>
            /// Min value.
            /// </value>
            [JsonProperty("from")]
            public string From { get; set; }

            /// <summary>
            /// Gets or sets max value.
            /// </summary>
            /// <value>
            /// Max value.
            /// </value>
            [JsonProperty("to")]
            public string To { get; set; }
        }

        /// <summary>
        /// Property 1.
        /// </summary>
        [JsonObject("property1")]
        public class Property1
        {
            /// <summary>
            /// Gets or sets min value.
            /// </summary>
            /// <value>
            /// Min value.
            /// </value>
            [JsonProperty("min")]
            public short Min { get; set; }

            /// <summary>
            /// Gets or sets max value.
            /// </summary>
            /// <value>
            /// Max value.
            /// </value>
            [JsonProperty("max")]
            public short Max { get; set; }
        }

        /// <summary>
        /// Property 2.
        /// </summary>
        [JsonObject("property2")]
        public class Property2
        {
            /// <summary>
            /// Gets or sets min value.
            /// </summary>
            /// <value>
            /// Min value.
            /// </value>
            [JsonProperty("min")]
            public decimal Min { get; set; }

            /// <summary>
            /// Gets or sets max value.
            /// </summary>
            /// <value>
            /// Max value.
            /// </value>
            [JsonProperty("max")]
            public decimal Max { get; set; }
        }
    }
}
