﻿using System;
using ServiceStack.Redis.Generic;

namespace ServiceStack.Redis
{
	/// <summary>
	/// Pipeline for redis typed client
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class RedisTypedPipeline<T> : RedisTypedCommandQueue<T>, IRedisTypedPipeline<T>
	{
		internal RedisTypedPipeline(RedisTypedClient<T> redisClient)
			: base(redisClient)
		{
            if (redisClient.Transaction != null)
                throw new InvalidOperationException("A transaction is already in use");

			if (redisClient.Pipeline != null)
				throw new InvalidOperationException("A pipeline is already in use");

			redisClient.Pipeline = this;
		}
		public void Flush()
		{
			// flush send buffers
			RedisClient.FlushSendBuffer();

			//receive expected results
			foreach (var queuedCommand in QueuedCommands)
			{
				queuedCommand.ProcessResult();
			}
		}

		protected void ClosePipeline()
		{
			RedisClient.ResetSendBuffer();
			RedisClient.Pipeline = null;
		}

		public void Dispose()
		{
			ClosePipeline();
		}
	}
}