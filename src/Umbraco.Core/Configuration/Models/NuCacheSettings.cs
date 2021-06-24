// Copyright (c) Umbraco.
// See LICENSE for more details.

namespace Umbraco.Cms.Core.Configuration.Models
{
    /// <summary>
    /// Typed configuration options for NuCache settings.
    /// </summary>
    public class NuCacheSettings
    {
        /// <summary>
        /// Gets or sets a value defining the BTree block size.
        /// </summary>
        public int? BTreeBlockSize { get; set; }

        /// <summary>
        /// The serializer type that nucache uses to persist documents in the database.
        /// </summary>
        public NuCacheSerializerType NuCacheSerializerType { get; set; } = NuCacheSerializerType.MessagePack;

        /// <summary>
        /// The paging size to use for nucache SQL queries.
        /// </summary>
        public int SqlPageSize { get; set; } = 1000;
    }
}
