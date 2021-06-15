
using System.Collections.Generic;

using MyAccess.Json.Mapping;


namespace MyAccess.Json.Processing
{
    /// <summary>
    /// Base class for both the EncodingProcess and DecodingProcess class.
    /// </summary>
    internal class Process
    {
        /// <summary>
        /// The reference store used while encoding or decoding.
        /// </summary>
        internal ReferenceStore References { get; private set; }

        /// <summary>
        /// Indicates if any of the configured mappings requires referencing.
        /// </summary>
        internal bool RequiresReferencing { get; private set; }

        /// <summary>
        /// Indicates if parallel processing is desired.
        /// </summary>
        internal bool IsParallel { get; private set; }

        internal Process(ICollection<JsonTypeMappingBase> mappings, bool parallel)
        {
            this.RequiresReferencing = false;
            this.IsParallel = parallel;
            foreach (JsonTypeMappingBase mapping in mappings)
            {
                if (mapping.UsesReferencing)
                {
                    this.RequiresReferencing = true;
                    break;
                }
            }

            if (this.RequiresReferencing)
            {
                this.References = new ReferenceStore();
            }
        }

    }
}
