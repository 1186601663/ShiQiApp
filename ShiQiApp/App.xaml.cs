using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShiQiApp.Core.Interfaces;
using ShiQiApp.Services;
using System.Configuration;
using System.Data;
using System.Windows;

namespace ShiQiApp
{
    /// <summary>
    /// 应用程序入口类
    /// 负责 DI 容器初始化和主窗口显示
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider Services { get; }
        public static new App Current => (App)Application.Current;
        /// <summary>
        /// 构造函数：配置服务
        /// </summary>
        public App()
        {
            Services = ConfigureServices();
        }

        private static IServiceProvider ConfigureServices()
        {
        
            // 创建 DI 容器
            var services = new ServiceCollection();

            // 注册全局状态（单例）
            services.AddSingleton<IGlobalState>(_ => new GlobalState("Modular WPF App"));

            // 注册模块加载器（单例）
            services.AddSingleton<IModuleLoader, ModuleLoader>();

            // 注册主视图模型（每次请求新建）
            services.AddSingleton(sp => new MainWindow { DataContext = sp.GetRequiredService<MainViewModel>() }); 
            
            services.AddTransient<MainViewModel>();

            return services.BuildServiceProvider();
        }


        /// <summary>
        /// 应用启动时创建主窗口并设置 DataContext
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
        {
            // 从 DI 容器解析 ViewModel
            MainWindow = Services.GetRequiredService<MainWindow>();
            MainWindow.Show();
            base.OnStartup(e);
        }
    }

    /// <summary>
    /// 全局状态实现类
    /// 用于向模块传递应用级信息
    /// </summary>
    internal class GlobalState : IGlobalState
    {
        public string AppName { get; }
        public GlobalState(string name) => AppName = name;
    }
}
