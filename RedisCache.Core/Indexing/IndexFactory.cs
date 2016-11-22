﻿using System;
using PEL.Framework.Redis.Extractors;
using PEL.Framework.Redis.Serialization;

namespace PEL.Framework.Redis.Indexing
{
    internal class IndexFactory<TValue>
    {
        private readonly TimeSpan? _expiry;
        private readonly string _masterCollectionRootName;
        //private readonly IKeyExtractor<TValue> _masterKeyResolver;
        private readonly ISerializer _serializer;

        internal IndexFactory(
            string masterCollectionRootName,
            TimeSpan? expiry,
            ISerializer serializer
            //IKeyExtractor<TValue> masterKeyResolver,
        )
        {
            _serializer = serializer;
            //_masterKeyResolver = masterKeyResolver;
            _expiry = expiry;
            _masterCollectionRootName = masterCollectionRootName;
        }

        internal IIndex<TValue> CreateIndex<TExtractor>(
            bool unique,
            bool withPayload,
            TExtractor indexValueExtractor,
            string name = null)
            where TExtractor : IKeyExtractor<TValue>
        {
            if (withPayload && unique)
            {
                return new UniquePayloadIndex<TValue>(
                    name ?? indexValueExtractor.GetType().Name,
                    indexValueExtractor,
                    //  _masterKeyResolver,
                    _masterCollectionRootName,
                    _serializer,
                    _expiry);
            }

            // TODO: (trais, 21 Nov 2016) - implement other type of possible indexes
            throw new NotImplementedException($"Index of this type : unique:'{unique}' and with payload: '{withPayload}') not supported yet.");
        }
    }
}