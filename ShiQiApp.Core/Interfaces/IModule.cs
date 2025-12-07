/// <summary>
/// 模块接口
/// 所有热加载模块必须实现此接口
/// 注意：CreateViewFactory 必须返回工厂委托，而非直接创建 UI
/// </summary>

using System.Windows;

namespace ShiQiApp.Core.Interfaces
{
    public interface IModule
    {
        /// <summary>
        /// 模块名称（用于导航显示）
        /// </summary>
        string Name { get; }

        
        /// <summary>
        /// 图标字符（如 "\uE70F"）或图标键名（用于 ResourceDictionary）
        /// 推荐使用 Unicode 字符（Segoe MDL2 Assets）
        /// </summary>
        string? IconGlyph { get; }

        /// <summary>
        /// 创建视图实例的工厂方法（必须在 UI 线程调用）
        /// </summary>
        Func<UIElement> CreateViewFactory { get; }

        /// <summary>
        /// 初始化模块（注入全局状态等）
        /// </summary>
        void Initialize(IGlobalState globalState);
    }
}
