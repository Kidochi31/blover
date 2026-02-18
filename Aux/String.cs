
namespace Blover.Aux
{
    public static class StringExtension
    {
        public static string Indent(this string s)
        {
            return "    " + s.ReplaceLineEndings("\n    ");
        }
    }
}