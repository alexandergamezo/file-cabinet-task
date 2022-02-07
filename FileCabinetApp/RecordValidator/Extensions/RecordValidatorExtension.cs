using System;
using FileCabinetApp.Models;
using Microsoft.Extensions.Configuration;

namespace FileCabinetApp.RecordValidator.Extensions
{
    /// <summary>
    /// Class for extension methods.
    /// </summary>
    public static class RecordValidatorExtension
    {
        /// <summary>
        /// Checks record with default parameters.
        /// </summary>
        /// <param name="validatorBuilder">Object reference.</param>
        /// <param name="builder">Configuration settings.</param>
        /// <returns>A new reference with default validators.</returns>
        public static IRecordValidator CreateDefault(this ValidatorBuilder validatorBuilder, IConfiguration builder)
        {
            var appConfig = builder.GetSection("default");

            IRecordValidator result = validatorBuilder.ValidateFirstName(appConfig.GetSection("firstName").Get<Validation.FirstName>().Min, appConfig.GetSection("firstName").Get<Validation.FirstName>().Max)
                                                      .ValidateLastName(appConfig.GetSection("lastName").Get<Validation.LastName>().Min, appConfig.GetSection("lastName").Get<Validation.LastName>().Max)
                                                      .ValidateDateOfBirth(
                                                                           DateTime.Parse(appConfig.GetSection("dateOfBirth").Get<Validation.DateOfBirth>().From),
                                                                           DateTime.Parse(appConfig.GetSection("dateOfBirth").Get<Validation.DateOfBirth>().To))
                                                      .ValidateProperty1(appConfig.GetSection("property1").Get<Validation.Property1>().Min, appConfig.GetSection("property1").Get<Validation.Property1>().Max)
                                                      .ValidateProperty2(appConfig.GetSection("property2").Get<Validation.Property2>().Min, appConfig.GetSection("property2").Get<Validation.Property2>().Max)
                                                      .ValidateProperty3()
                                                      .Create();

            return result;
        }

        /// <summary>
        /// Checks record with custom parameters.
        /// </summary>
        /// <param name="validatorBuilder">Object reference.</param>
        /// /// <param name="builder">Configuration settings.</param>
        /// <returns>A new reference with custom validators.</returns>
        public static IRecordValidator CreateCustom(this ValidatorBuilder validatorBuilder, IConfiguration builder)
        {
            var appConfig = builder.GetSection("custom");

            IRecordValidator result = validatorBuilder.ValidateFirstName(appConfig.GetSection("firstName").Get<Validation.FirstName>().Min, appConfig.GetSection("firstName").Get<Validation.FirstName>().Max)
                                                      .ValidateLastName(appConfig.GetSection("lastName").Get<Validation.LastName>().Min, appConfig.GetSection("lastName").Get<Validation.LastName>().Max)
                                                      .ValidateDateOfBirth(
                                                                           DateTime.Parse(appConfig.GetSection("dateOfBirth").Get<Validation.DateOfBirth>().From),
                                                                           DateTime.Parse(appConfig.GetSection("dateOfBirth").Get<Validation.DateOfBirth>().To))
                                                      .ValidateProperty1(appConfig.GetSection("property1").Get<Validation.Property1>().Min, appConfig.GetSection("property1").Get<Validation.Property1>().Max)
                                                      .ValidateProperty2(appConfig.GetSection("property2").Get<Validation.Property2>().Min, appConfig.GetSection("property2").Get<Validation.Property2>().Max)
                                                      .ValidateProperty3()
                                                      .Create();

            return result;
        }
    }
}
