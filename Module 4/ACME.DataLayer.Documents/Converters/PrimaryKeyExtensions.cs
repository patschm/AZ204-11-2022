namespace ACME.DataLayer.Documents.Converters;

public static class PrimaryKeyExtensions
{
    public static string ToDocumentId(this long id, string? prefix)
    {
        return $"{prefix?.ToLower()}_{id}";
    }
    public static long ToPrimaryKey(this string docid)
    {
        if (!string.IsNullOrEmpty(docid) &&
           long.TryParse(docid.Substring(docid.LastIndexOf("_") + 1), out long id))
        {
            return id;
        }
        throw new Exception($"Invalid document Id: {docid}");
    }
}
