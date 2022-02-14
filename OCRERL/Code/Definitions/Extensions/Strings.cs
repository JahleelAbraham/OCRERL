namespace OCRERL.Code.Definitions.Extensions;

public static class Strings
{
    public static string Capitalize(this string text)
        => string.Join(" ", text.Split(' ').ToList()
            .ConvertAll(word =>
                word[..1].ToUpper() + word[1..].ToLower()
            )
        );
}