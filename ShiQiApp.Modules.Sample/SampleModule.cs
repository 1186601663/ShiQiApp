
/// <summary>
/// 示例模块实现
/// 实现 IModule 接口
/// </summary>
using ShiQiApp.Core.Interfaces;
using System.Windows;

namespace ShiQiApp.Modules.Sample
{
    public class SampleModule : IModule
    {
       private IGlobalState? _state;

        public string Name => "hjhj";
        public string? IconGlyph => "\uE70F";
        /// <summary>
        /// 返回视图工厂委托
        /// 在调用时创建 SampleView（必须在 UI 线程！）
        /// </summary>
        public Func<UIElement> CreateViewFactory => () => new SampleView(_state!.AppName);

        /// <summary>
        /// 初始化模块，保存全局状态引用
        /// </summary>
        public void Initialize(IGlobalState globalState) => _state = globalState;
    }
}
