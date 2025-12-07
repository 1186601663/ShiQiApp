using ShiQiApp.Core.Interfaces;
using System.Windows;

namespace ShiQiApp
{
    public class NavigationItem
    {
        public string DisplayName { get; set; }
        public string? IconGlyph { get; set; }
        public Func<UIElement> ViewFactory { get; set; }
        public IModule Module { get; set; } // 可选，用于调试或扩展

        // 👇 缓存视图（懒加载）
        private UIElement? _cachedView;
        public UIElement GetOrCreateView()
        {
            return _cachedView ??= ViewFactory();
        }
    }
}
