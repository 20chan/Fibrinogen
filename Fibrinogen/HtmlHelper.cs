namespace Fibrinogen
{
    public static class HtmlHelper
    {
        public static string WrapWithTag(string tag, string content)
            => $"<{tag}>{content}</{tag}>";
    }
}
