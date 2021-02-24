namespace AppEFCore.Extensions
{
    public static class StringExtencions
    {
        public static string Truncar(this string s, int tamanho)
        {
            if (tamanho >= s.Length)
                return s;

            return s.Substring(0, tamanho);
        }

        public static string ReplaceMany(this string textoOrigem, string[] textosAntigos, string textoNovo)
        {
            foreach(var textoAntigo in textosAntigos)
            {
                textoOrigem = textoOrigem.Replace(textoAntigo, textoNovo);
            }

            return textoOrigem;
        }
    }
}
