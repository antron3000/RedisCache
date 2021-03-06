﻿using System;
using PEL.Framework.Redis.Database;
using StackExchange.Redis;

namespace PEL.Framework.Redis.Publishing
{
    public class RedisExpiryMessageSubscriber : IDisposable
    {
        private readonly string _keySpaceChannel;
        private readonly ISubscriber _subscriber;

        protected RedisExpiryMessageSubscriber(
            IRedisDatabaseConnector connection,
            string collectionName
        )
        {
            var db = connection.GetConnectedDatabase();
            _subscriber = db.Multiplexer.GetSubscriber();
            _keySpaceChannel = $"__keyspace@{db.Database}__:{collectionName}*";
        }

        public void Dispose()
        {
            _subscriber.UnsubscribeAll();
        }

        public static RedisExpiryMessageSubscriber CreateForCollectionType<TValue>(
            IRedisDatabaseConnector connection
        )
        {
            return new RedisExpiryMessageSubscriber(connection, typeof(TValue).Name.ToLowerInvariant());
        }

        public void SubscribeExpiry(Action<string> onExpiryMessage)
        {
            _subscriber.Subscribe(_keySpaceChannel, (ctx, message) =>
            {
                if ((string) message == "expired")
                    onExpiryMessage(message);
            });
        }
    }
}