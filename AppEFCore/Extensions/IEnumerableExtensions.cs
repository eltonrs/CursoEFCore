using System.Collections.Generic;
using System.Linq;

namespace AppEFCore.Extensions
{
    public static class IEnumerableExtensions
    {
        public static bool ENuloOuVazio<T>(this IEnumerable<T> origem)
        {
            return origem == null || !origem.Any();
        }
    }
}