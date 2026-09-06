using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework2
{
    public class ScenarioContext
    {
        public long UserId { get; set; }

        public ScenarioType CurrentScenario { get; set; }

        public string? CurrentStep { get; set; }

        public string? TemporaryTaskName { get; set; }

        public Dictionary<string, object> Data = new Dictionary<string, object>();

        public ScenarioContext(ScenarioType scenario)
        {
            CurrentScenario = scenario;
        }
    }
}
