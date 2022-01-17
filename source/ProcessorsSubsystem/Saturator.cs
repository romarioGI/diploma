using System.Collections.Generic;
using System.Linq;

namespace ProcessorsSubsystem
{
    /// <summary>
    /// saturator of a polynomial system
    /// </summary>
    public static class Saturator
    {
        public static IEnumerable<Polynomial> Saturate(IEnumerable<Polynomial> polynomials)
        {
            var result = new HashSet<Polynomial>();

            var system = polynomials.Distinct().ToList();

            if (system.All(p => p.IsZero))
                return system;

            var queue = new Queue<Polynomial>();

            foreach (var p in system.Where(p => !p.IsZero))
                queue.Enqueue(p);

            var multiplication = queue
                .Aggregate((res, nxt) => res * nxt);
            queue.Enqueue(multiplication.GetDerivative());

            while (queue.Count != 0)
            {
                var cur = queue.Dequeue();
                if (result.Contains(cur))
                    continue;

                if (cur.Degree > 0)
                {
                    queue.Enqueue(cur.GetDerivative());
                    foreach (var r in GetRemainders(result, cur))
                        queue.Enqueue(r);
                }

                result.Add(cur);
            }


            return result;
        }

        private static IEnumerable<Polynomial> GetRemainders(IEnumerable<Polynomial> system, Polynomial polynomial)
        {
            foreach (var p in system)
            {
                if (p.Degree < 1)
                    continue;

                if (p.Degree <= polynomial.Degree)
                    yield return polynomial % p;

                if (polynomial.Degree <= p.Degree)
                    yield return p % polynomial;
            }
        }
    }
}