namespace ACME.Frontend.GremlinConsole;

internal static class GremlinExtensions
{
    public static string Parse(this string s)
    {
        return s.Replace("'", "\\'");
    }
}
