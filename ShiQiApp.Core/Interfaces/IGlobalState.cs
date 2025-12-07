/// <summary>
/// 全局状态接口
/// 用于向模块传递应用级共享数据
/// </summary>
namespace ShiQiApp.Core.Interfaces
{
    public interface IGlobalState
    {
        string AppName { get; }
    }
}
