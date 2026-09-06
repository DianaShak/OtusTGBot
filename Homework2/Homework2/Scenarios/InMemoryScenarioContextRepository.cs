using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Homework2
{
    internal class InMemoryScenarioContextRepository : IScenarioContextRepository
    {
        ConcurrentDictionary<long, ScenarioContext> scenarioDictionary = new ConcurrentDictionary<long, ScenarioContext>();
        public async Task<ScenarioContext?> GetContext(long userId, CancellationToken ct)
        {
            return scenarioDictionary.FirstOrDefault(x => x.Key == userId).Value;
        }

        public async Task ResetContext(long userId, CancellationToken ct)
        {
            var pair = scenarioDictionary.First(x => x.Key == userId);
            scenarioDictionary.TryRemove(pair);
        }

        public async Task SetContext(long userId, ScenarioContext context, CancellationToken ct)
        {
            scenarioDictionary[userId] = context;
        }
    }
}
