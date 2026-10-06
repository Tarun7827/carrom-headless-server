using System;
using Carrom.Headless.DTO;
using Carrom.Headless.Simulation;
namespace Carrom.Headless.Core
{
    public sealed class SimulationJob { public SimulationRequest Request; public Action<SimulationResponse> Complete; public CarromBoard Board; }
}
