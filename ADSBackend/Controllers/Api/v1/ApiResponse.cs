
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using ADSBackend.Controllers.Api.v1;

namespace ADSBackend.Controllers.Api.v1
{
    /// <summary>
    /// Represents a generic API response.
    /// </summary>
    [DataContract]
    public class ApiResponse
    {
        /// <summary>
        /// Gets the version of the API.
        /// </summary>
        [DataMember]
        public string Version { get { return "1"; } }

        /// <summary>
        /// Gets or sets the status code of the response.
        /// </summary>
        [DataMember]
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the error message, if any.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the count of errors.
        /// </summary>
        [DataMember(EmitDefaultValue = true)]
        public int ErrorCount { get; set; } = 0;

        /// <summary>
        /// Gets or sets the list of API errors.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public List<ApiError> Errors { get; set; }

        /// <summary>
        /// Gets or sets the result object of the response.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public object Result { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResponse"/> class.
        /// </summary>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="result">The result object.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="state">The model state dictionary containing errors.</param>
        public ApiResponse(HttpStatusCode statusCode, object result = null, string errorMessage = null, ModelStateDictionary state = null)
        {
            StatusCode = (int)statusCode;
            Result = result;

            if (errorMessage?.Length > 0)
            {
                ErrorCount = 1;
                ErrorMessage = errorMessage;
            }

            if (state != null)
            {
                Errors = new List<ApiError>();

                foreach (var modelStateKey in state.Keys)
                {
                    var modelStateVal = state[modelStateKey];

                    if (modelStateVal.Errors.Count > 0)
                    {
                        Errors.Add(new ApiError
                        {
                            Key = modelStateKey,
                            Errors = modelStateVal.Errors.Select(m => m.ErrorMessage).ToList()
                        });

                    }
                }

            }
        }

    }
}