using System.Collections.Generic;

namespace Domain
{
    public class ValueComparer : IComparer<Value>
    {
        public int Compare(Value x, Value y)
        {
            if (x == null && y == null)
                return 0;

            if (x == null)
                return -1;

            if (y == null)
                return 1;

            if (x.Revision != y.Revision)
                return x.Revision.CompareTo(y.Revision);

            return Comparer<string>.Default.Compare(x.Content, y.Content);
        }
    }
}