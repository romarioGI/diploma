using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ProcessorsSubsystem
{
    public class TarskiTable
    {
        private class Column : IEnumerable<Sign>
        {
            private readonly List<Sign> _signs;

            public int NumberOfLastZero { get; private set; }

            public int Count => _signs.Count;

            public Column()
            {
                _signs = new List<Sign>();
                NumberOfLastZero = -1;
            }

            public void Push(Sign sign)
            {
                if (sign == Sign.EqualZero)
                    NumberOfLastZero = _signs.Count;
                _signs.Add(sign);
            }

            public Sign this[int index] => _signs[index];

            public Sign Last => _signs[^1];

            public IEnumerator<Sign> GetEnumerator()
            {
                return _signs.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private class PolynomialCollection : IEnumerable<Polynomial>
        {
            private readonly List<Polynomial> _polynomials;
            private readonly Dictionary<Polynomial, int> _polynomialNums;

            public PolynomialCollection()
            {
                _polynomials = new List<Polynomial>();
                _polynomialNums = new Dictionary<Polynomial, int>();
            }

            public void Add(Polynomial polynomial)
            {
                _polynomialNums.Add(polynomial, _polynomials.Count);
                _polynomials.Add(polynomial);
            }

            public int this[Polynomial polynomial] => _polynomialNums[polynomial];

            public Polynomial this[int number] => _polynomials[number];

            public IEnumerator<Polynomial> GetEnumerator()
            {
                return _polynomials.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private readonly LinkedList<Column> _columns;
        private readonly Column _firstColumn;
        private readonly Column _lastColumn;
        private readonly PolynomialCollection _polynomialCollection;

        public TarskiTable(IEnumerable<Polynomial> polynomials)
        {
            _columns = new LinkedList<Column>();
            _firstColumn = new Column();
            _lastColumn = new Column();
            _polynomialCollection = new PolynomialCollection();

            foreach (var p in polynomials.OrderBy(p => p.Degree))
            {
                AddPolynomial(p);
                UpdateColumns();
            }
        }

        public IEnumerable<Sign> this[Polynomial polynomial]
        {
            get
            {
                var polyNum = _polynomialCollection[polynomial];

                yield return _firstColumn[polyNum];
                foreach (var column in _columns)
                    yield return column[polyNum];
                yield return _lastColumn[polyNum];
            }
        }

        public IEnumerable<Polynomial> Polynomials => _polynomialCollection;

        public int Width => 2 + _columns.Count;

        public Dictionary<Polynomial, List<Sign>> GetTableDictionary()
        {
            return Polynomials.ToDictionary(p => p, p => this[p].ToList());
        }

        private void AddPolynomial(Polynomial polynomial)
        {
            switch (polynomial.Degree)
            {
                case -1:
                    AddZeroPolynomial(polynomial);
                    break;
                case 0:
                    AddZeroDegreePolynomial(polynomial);
                    break;
                default:
                    AddMoreZeroDegreePolynomial(polynomial);
                    break;
            }
        }

        private void AddZeroPolynomial(Polynomial polynomial)
        {
            _polynomialCollection.Add(polynomial);

            _firstColumn.Push(Sign.EqualZero);
            foreach (var column in _columns)
                column.Push(Sign.EqualZero);
            _lastColumn.Push(Sign.EqualZero);
        }

        private void AddZeroDegreePolynomial(Polynomial polynomial)
        {
            _polynomialCollection.Add(polynomial);

            _firstColumn.Push(polynomial.Leading.Sign);
            foreach (var column in _columns)
                column.Push(polynomial.Leading.Sign);
            _lastColumn.Push(polynomial.Leading.Sign);
        }

        private void AddMoreZeroDegreePolynomial(Polynomial polynomial)
        {
            _polynomialCollection.Add(polynomial);

            _firstColumn.Push(CalcSignFirstColumn(polynomial));
            foreach (var column in _columns)
                column.Push(CalcSign(polynomial, column));
            _lastColumn.Push(polynomial.Leading.Sign);
        }

        private static Sign CalcSignFirstColumn(Polynomial polynomial)
        {
            return polynomial.Degree % 2 == 0 ? polynomial.Leading.Sign : polynomial.Leading.Sign.Invert();
        }

        private Sign CalcSign(Polynomial polynomial, Column column)
        {
            var lastZeroSignPoly = _polynomialCollection[column.NumberOfLastZero];
            polynomial %= lastZeroSignPoly;

            if (polynomial.IsZero)
                return Sign.EqualZero;
            return column[_polynomialCollection[polynomial]];
        }

        private void UpdateColumns()
        {
            if (_columns.Count == 0)
            {
                UpdateEmptyTable();
                return;
            }

            var start = _columns.First;
            var finish = _columns.Last;
            var current = start;
            while (current != finish)
            {
                var next = current.Next;
                // ReSharper disable once PossibleNullReferenceException
                if (!CheckColumns(current.Value, next.Value))
                    _columns.AddAfter(current, GetNewColumn(current.Value, next.Value));

                current = next;
            }

            if (!CheckColumns(_firstColumn, start.Value))
                _columns.AddFirst(GetNewColumn(_firstColumn, start.Value));

            if (!CheckColumns(finish.Value, _lastColumn))
                _columns.AddLast(GetNewColumn(finish.Value, _lastColumn));
        }

        private void UpdateEmptyTable()
        {
            if (!CheckColumns(_firstColumn, _lastColumn))
                _columns.AddFirst(GetNewColumn(_firstColumn, _lastColumn));
        }

        private static bool CheckColumns(Column left, Column right)
        {
            return !(left.Last == Sign.LessZero && right.Last == Sign.MoreZero ||
                     left.Last == Sign.MoreZero && right.Last == Sign.LessZero);
        }

        private static Column GetNewColumn(Column left, Column right)
        {
            var newColumn = new Column();

            for (var i = 0; i < left.Count; i++)
            {
                var sign = left[i] switch
                {
                    Sign.LessZero => right[i] switch
                    {
                        Sign.LessZero => Sign.LessZero,
                        Sign.EqualZero => Sign.LessZero,
                        Sign.MoreZero => Sign.EqualZero,
                        _ => throw new ArgumentException()
                    },
                    Sign.EqualZero => right[i] switch
                    {
                        Sign.LessZero => Sign.LessZero,
                        Sign.MoreZero => Sign.MoreZero,
                        Sign.EqualZero => Sign.EqualZero,
                        _ => throw new ArgumentException()
                    },
                    Sign.MoreZero => right[i] switch
                    {
                        Sign.LessZero => Sign.EqualZero,
                        Sign.EqualZero => Sign.MoreZero,
                        Sign.MoreZero => Sign.MoreZero,
                        _ => throw new ArgumentException()
                    },
                    _ => throw new ArgumentException()
                };
                newColumn.Push(sign);
            }

            return newColumn;
        }
    }
}