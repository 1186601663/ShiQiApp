using System.Reflection;
using System.Runtime.Loader;
using ShiQiApp.Core.Interfaces;
using System.IO;
using System.Diagnostics;
using System.Windows;

namespace ShiQiApp.Services
{
    /// <summary>
    /// 模块加载器实现
    /// 职责：
    /// 1. 在后台线程扫描 DLL 并发现 IModule 类型
    /// 2. 在 UI 线程安全地创建 UserControl
    /// 3. 支持程序集卸载（热更新）
    /// </summary>
    public class ModuleLoader : IModuleLoader
    {
        
        // 线程同步锁，保护内部状态
        private readonly object _lock = new();

        // 存储已发现的模块：上下文 + 类型
        // 注意：不存储实例，避免跨线程访问 UI
        private List<(AssemblyLoadContext Context, Type ModuleType)> _discoveredModules = [];

        private List<IModule> _initializedModules = new();

        public IModule[] GetInitializedModules()
        {
            lock (_lock)
            {
                return _initializedModules.ToArray();
            }
        }

        /// <summary>
        /// 卸载所有已加载的模块程序集
        /// 触发垃圾回收以释放内存
        /// </summary>
        public void UnloadAll()
        {
            lock (_lock)
            {
                foreach (var (context, _) in _discoveredModules)
                {
                    try
                    {
                        context.Unload(); // 标记为可回收
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Unload error: {ex}");
                    }
                }
                _discoveredModules.Clear(); // 清空引用
                _initializedModules.Clear();
            }

            // 强制 GC 回收（确保 Collectible Assembly 被释放）
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// 在后台线程中扫描 Modules 目录
        /// 加载程序集并发现 IModule 实现类
        /// 注意：此方法不创建任何 UI 元素！
        /// </summary>
        /// <returns>发现的模块信息列表（用于日志）</returns>
        public List<(string AssemblyPath, Type ModuleType)> DiscoverModulesInBackground()
        {
            var result = new List<(string AssemblyPath, Type ModuleType)>(); // 👈 关键：命名元组
            var contexts = new List<AssemblyLoadContext>();

            string modulesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules");
            if (!Directory.Exists(modulesPath))
                return result;

            foreach (string dll in Directory.GetFiles(modulesPath, "*.dll"))
            {
                try
                {
                    var context = new AssemblyLoadContext(
                        $"Module_{Path.GetFileNameWithoutExtension(dll)}",
                        isCollectible: true
                    );
                    contexts.Add(context);

                    Assembly asm = context.LoadFromAssemblyPath(dll);

                    foreach (Type type in asm.GetTypes())
                    {
                        if (typeof(IModule).IsAssignableFrom(type) &&
                            !type.IsAbstract &&
                            !type.IsInterface)
                        {
                            result.Add((dll, type)); // 自动映射到 AssemblyPath 和 ModuleType
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Discovery error in {dll}: {ex}");
                }
            }

            lock (_lock)
            {
                UnloadAll();
                _discoveredModules = contexts.Zip(result, (ctx, item) => (ctx, item.ModuleType)).ToList();
            }

            return result; // ✅ 类型匹配：List<(string AssemblyPath, Type ModuleType)>
        }

        /// <summary>
        /// 在 UI 线程中实例化模块视图
        /// 必须在 STA 线程（WPF 主线程）调用！
        /// 记录已初始化模块
        /// </summary>
        /// <param name="globalState">全局状态，注入到模块</param>
        /// <returns>生成的 UIElement 集合</returns>
        public IEnumerable<UIElement> InstantiateViews(IGlobalState globalState)
        {
            var views = new List<UIElement>();
            var initializedModules = new List<IModule>();

            lock (_lock)
            {
                foreach (var (_, moduleType) in _discoveredModules)
                {
                    try
                    {
                        // 创建模块实例（非 UI）
                        var module = (IModule)Activator.CreateInstance(moduleType)!;

                        // 初始化模块（传入全局状态）
                        module.Initialize(globalState);
                        initializedModules.Add(module);

                        // 调用工厂方法创建 UI（必须在 UI 线程！）
                        var view = module.CreateViewFactory();
                        views.Add(view);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"View creation failed for {moduleType}: {ex}");
                    }
                }

                _initializedModules = initializedModules;

            }

            return views.ToArray();
        }
    }
}
