using System.Collections.Concurrent;
using System.Collections.Generic;
using Carrom.Headless.Simulation;
namespace Carrom.Headless.Core
{
    public sealed class BoardPool
    {
        readonly ConcurrentQueue<CarromBoard> _available=new ConcurrentQueue<CarromBoard>();
        readonly HashSet<int> _leased=new HashSet<int>(); readonly object _gate=new object();
        public BoardPool(IReadOnlyList<CarromBoard> boards){foreach(var b in boards)_available.Enqueue(b);}
        public bool TryAcquire(out CarromBoard board){lock(_gate){if(_available.TryDequeue(out board)){_leased.Add(board.BoardId);return true;}board=null;return false;}}
        public void Release(CarromBoard board){if(board==null)return;lock(_gate){if(_leased.Remove(board.BoardId))_available.Enqueue(board);}}
        public int AvailableCount{get{lock(_gate)return _available.Count;}}
    }
}
