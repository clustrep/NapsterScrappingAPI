using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace NapsterScrappingInfrastructure.Cache
{
    public static class CacheExtensions
    {
        /// <summary>
        /// Returns an entry from the cache, or creates a new cache entry using the
        /// specified asynchronous factory method. Concurrent invocations are prevented,
        /// unless the entry is evicted before the completion of the delegate. The errors
        /// of failed invocations are not cached.
        /// </summary>
        public static Task<T> GetOrCreateExclusiveAsync<T>(this IMemoryCache cache, object key,
            Func<Task<T>> factory, MemoryCacheEntryOptions options = null)
        {
            if (!cache.TryGetValue(key, out Task<T> task))
            {
                var entry = cache.CreateEntry(key);
                if (options != null) entry.SetOptions(options);
                var cts = new CancellationTokenSource();
                var newTaskTask = new Task<Task<T>>(async () =>
                {
                    try { return await factory().ConfigureAwait(false); }
                    catch { cts.Cancel(); throw; }
                    finally { cts.Dispose(); }
                });
                var newTask = newTaskTask.Unwrap();
                entry.ExpirationTokens.Add(new CancellationChangeToken(cts.Token));
                entry.Value = newTask;
                entry.Dispose(); // The Dispose actually inserts the entry in the cache
                if (!cache.TryGetValue(key, out task)) task = newTask;
                if (task == newTask)
                    newTaskTask.RunSynchronously(TaskScheduler.Default);
                else
                    cts.Dispose();
            }
            return task;
        }
    }
}
