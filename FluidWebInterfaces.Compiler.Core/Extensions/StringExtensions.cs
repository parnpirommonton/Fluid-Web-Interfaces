namespace FluidWebInterfaces.Compiler.Core;

public static class StringExtensions
{
    public static string RemoveWhitespace(this string str)
    {
        return str.Replace(" ", "").Replace("\t", "").Replace("\n", "");
    }
    public static bool IsNextSequence(this string str, string sequence, int i, bool caseSensitive=true)
    {
        if (str.Length >= i+sequence.Length && (str[i..(i+sequence.Length)] == sequence
                                                || (!caseSensitive && str[i..(i+sequence.Length)].ToLower() == sequence)))
        {
            return true;
        }

        return false;
    }
}