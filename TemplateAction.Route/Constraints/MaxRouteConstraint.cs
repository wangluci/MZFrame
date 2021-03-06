using System;
using System.Collections.Generic;
using TemplateAction.Core;

namespace TemplateAction.Route
{
    /// <summary>
    /// Constrains a route parameter to be an integer with a maximum value.
    /// </summary>
    public class MaxRouteConstraint : IRouteConstraint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaxRouteConstraint" /> class.
        /// </summary>
        /// <param name="max">The maximum value allowed for the route parameter.</param>
        public MaxRouteConstraint(long max)
        {
            Max = max;
        }

        /// <summary>
        /// Gets the maximum allowed value of the route parameter.
        /// </summary>
        public long Max { get; private set; }

        /// <inheritdoc />
        public bool Match(ITAContext context, IRouter route, string routeKey, IDictionary<string, object> values)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (route == null)
            {
                throw new ArgumentNullException(nameof(route));
            }

            if (routeKey == null)
            {
                throw new ArgumentNullException(nameof(routeKey));
            }

            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            object value;
            if (values.TryGetValue(routeKey, out value) && value != null)
            {
                long longValue;
                var valueString = Convert.ToString(value);
                if (long.TryParse(valueString, out longValue))
                {
                    return longValue <= Max;
                }
            }

            return false;
        }
    }
}
