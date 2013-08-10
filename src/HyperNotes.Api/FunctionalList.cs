using System.Collections.Generic;
using System.Linq;

namespace HyperNotes.Api {
    public class FunctionalList<TModel> {
        public FunctionalList(IEnumerable<TModel> sequence) {
            _sequence = sequence;
        }

        public bool IsEmpty {
            get { return !_sequence.Any(); }
        }

        public TModel First {
            get { return _sequence.FirstOrDefault(); }
        }

        public IEnumerable<TModel> Rest {
            get { return _sequence.Skip(1); }
        } 

        public IEnumerable<TModel> All {
            get { return _sequence; }
        } 

        private readonly IEnumerable<TModel> _sequence;
    }
}