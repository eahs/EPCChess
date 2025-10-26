
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Helpers
{
    /// <summary>
    /// Provides utility methods for controllers.
    /// </summary>
    public static class ControllerUtil
    {
        /// <summary>
        /// Removes any model state errors that don't pertain to the actual fields being bound.
        /// </summary>
        /// <param name="ModelState">The model state dictionary.</param>
        /// <param name="bindingFields">A comma-separated string of fields that are being bound.</param>
        public static void Scrub(this ModelStateDictionary ModelState, string bindingFields)
        {
            string[] bindingKeys = bindingFields.Split(",");
            foreach (string key in ModelState.Keys)
            {
                if (!bindingKeys.Contains(key))
                    ModelState.Remove(key);
            }
        }
    }
}