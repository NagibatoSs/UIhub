namespace UIhub.Analyze
{
    public static class ElementClassNames
    {
        public static readonly Dictionary<string, string> Names = new()
    {
        { "button",   "Кнопка" },
        { "menuItem", "Пункт меню" },
        { "input",    "Поле ввода" },
        { "checkbox", "Чекбокс" },
        { "header",   "Заголовок" },
        { "text",     "Текст" },
        { "label",    "Подпись" }
    };

        public static string Translate(string className)
        {
            return Names.TryGetValue(className, out var name) ? name : className;
        }
    }
}
