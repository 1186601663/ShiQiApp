/// <summary>
/// 示例模块的视图代码
/// 接收 AppName 并设置为 DataContext
/// </summary>

using System.Windows.Controls;


namespace ShiQiApp.Modules.Sample
{
    /// <summary>
    /// SampleView.xaml 的交互逻辑
    /// </summary>
    public partial class SampleView : UserControl
    {
        /// <summary>
        /// 应用名称（从主程序传入）
        /// </summary>
        public string AppName { get; }

        /// <summary>
        /// 构造函数：接收 AppName 并初始化 UI
        /// 注意：此方法必须在 UI 线程调用！
        /// </summary>
        public SampleView(string appName)
        {
            AppName = appName;
            InitializeComponent();
            DataContext = this; // 简单绑定（实际项目建议用 ViewModel）
        }
    }
}
