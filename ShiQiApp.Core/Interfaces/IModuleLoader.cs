/// <summary>
/// 模块加载器接口
/// 定义模块发现、实例化和卸载的标准流程
/// </summary>

using System.Windows;

namespace ShiQiApp.Core.Interfaces
{
    public interface IModuleLoader
    {
        /// <summary>
        /// 在后台线程中扫描模块目录
        /// 发现 IModule 实现类（不创建实例）
        /// </summary>
        List<(string AssemblyPath, Type ModuleType)> DiscoverModulesInBackground();

        /// <summary>
        /// 在 UI 线程中实例化模块视图
        /// 必须在 STA 线程调用！
        /// </summary>
        IEnumerable<UIElement> InstantiateViews(IGlobalState globalState);

        /// <summary>
        /// 卸载所有已加载的模块程序集
        /// </summary>
        void UnloadAll();

        /// <summary>
        /// 获取所有已成功初始化的模块（快照）
        /// </summary>
        IModule[] GetInitializedModules();
    }
}
