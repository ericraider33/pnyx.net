namespace pnyx.net.test.util
{
    public static class EqualsHelper
    {
        public static bool areArraysEquals<T>(T[] a, T[] b)
        {
            if (a == null && b == null)
                return true;

            if (a == null || b == null)
                return false;

            if (a.Length != b.Length)
                return false;

            for (int i = 0; i < a.Length; i++)
            {
                if (!Equals(a[i], b[i]))
                    return false;
            }

            return true;
        }
    }
}