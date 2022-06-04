using NHibernate.Metadata;
using NHibernate.Type;

namespace SincronizacaoServico.Util
{
    public static class SincronizarExtensionMethods
    {
        public static bool ContainsType(this IType[] types, Type tipo)
        {
            return types.Any(a => a.GetType().Equals(tipo));
        }

        public static List<int> GetManyToOneIndexes(this IType[] types)
        {
            return types.Select((value, index) => new { Tipo = value, Posicao = index })
                .Where(w => w.Tipo.GetType().Equals(typeof(ManyToOneType)))
                .Select(s => s.Posicao).ToList();
        }

        public static IList<string> GetManyToOnePropertyNames(this string[] propNames, IClassMetadata persister)
        {
            return propNames.Where(w => persister.GetPropertyType(w).GetType().Equals(typeof(ManyToOneType))).ToList();
        }

        public static string GetPropertyTypeSimpleName(this IClassMetadata persister, string property)
        {
            string fullName = persister.GetPropertyType(property).Name;
            return fullName.Substring(fullName.LastIndexOf('.') + 1);
        }

        public static int PropertyIndex(this string[] propNames, string nome)
        {
            return propNames.Select((name, index) => new { Nome = name, Posicao = index })
                .Where(w => w.Nome.Equals(nome))
                .Select(s => s.Posicao)
                .FirstOrDefault();
        }
    }
}
