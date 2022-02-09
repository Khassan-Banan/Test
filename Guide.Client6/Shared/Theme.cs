using MudBlazor;

namespace Guide.Client6.Shared
{
    public static class ThemeCollection
    {
        public static MudTheme Theme = new MudTheme
        {
            Palette = new Palette
            {
                Primary = Colors.Purple.Darken3,
                AppbarBackground = Colors.Purple.Darken3,
                Background = Colors.Grey.Lighten4,
                Divider = Colors.Grey.Darken1

            },
            LayoutProperties = new LayoutProperties
            {
                DrawerWidthLeft = "300px",
                DrawerWidthRight = "300px",
            }
        };
    }
}