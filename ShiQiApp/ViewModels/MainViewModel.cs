using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShiQiApp.Core.Interfaces;
using ShiQiApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShiQiApp
{
    /// <summary>
    /// 主窗口 ViewModel
    /// 使用 CommunityToolkit.Mvvm 自动生成 INotifyPropertyChanged 和 RelayCommand
    /// </summary>
    public partial class MainViewModel : ObservableObject
    {
        private readonly IModuleLoader _moduleLoader;
        private readonly IGlobalState _globalState;

        /// <summary>
        /// 构造函数：通过 DI 注入依赖
        /// </summary>
        public MainViewModel(IModuleLoader moduleLoader, IGlobalState globalState)
        {
            _moduleLoader = moduleLoader;
            _globalState = globalState;
            _loadedViews = new ObservableCollection<object>();
            NavigationItems = new ObservableCollection<NavigationItem>();
        }

        /// <summary>
        /// 当前操作状态（绑定到 UI）
        /// </summary>
        [ObservableProperty]
        private string _status = "准备好了";

        /// <summary>
        /// 已加载的模块视图集合（绑定到 ItemsControl）
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<object> _loadedViews;

        // 导航项集合（绑定到左侧 ListBox）
        [ObservableProperty]
        private ObservableCollection<NavigationItem> _navigationItems;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNavigationVisible))]
        private NavigationItem? _selectedNavigationItem;

        // 👇 新增：导航栏是否展开
        [ObservableProperty]
        private bool _isNavigationExpanded = true;

        // 控制是否显示导航栏（当无模块时不显示）
        public bool IsNavigationVisible => NavigationItems.Count > 0;

        [RelayCommand]
        private void ToggleNavigation()
        {
            IsNavigationExpanded = !IsNavigationExpanded;
        }

        // 当前选中的视图（绑定到右侧 ContentControl）
        [ObservableProperty]
        private UIElement? _currentView;

        /// <summary>
        /// 异步加载模块命令
        /// 1. 后台线程加载模块并更新导航栏
        /// 2. UI 线程创建视图
        /// </summary>
        [RelayCommand]
        private async Task LoadModulesAsync()
        {
            try
            {
                Status = "🔍 正在发现模块（后台）...";
                await Task.Run(() => _moduleLoader.DiscoverModulesInBackground());

                Status = "🎨 在 UI 线程上实例化视图...";

                // 获取模块实例（已初始化）
                var modules = _moduleLoader.GetInitializedModules(); // ← 需要新增此方法

                // 清空旧数据
                NavigationItems.Clear();

                foreach (var module in modules)
                {
                    NavigationItems.Add(new NavigationItem
                    {
                        DisplayName = module.Name,
                        IconGlyph = module.IconGlyph,
                        ViewFactory = module.CreateViewFactory,
                        Module = module
                    });
                }

                // 自动选中第一个
                if (NavigationItems.Count > 0)
                {
                    SelectedNavigationItem = NavigationItems[0];
                }

                Status = $"✅ 加载成功 {NavigationItems.Count} 模块.";
            }
            catch (Exception ex)
            {
                Status = "❌ 模块重载失败.";
                MessageBox.Show(ex.ToString(), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 👇 当用户点击导航项时，创建并显示视图
        partial void OnSelectedNavigationItemChanged(NavigationItem? value)
        {
            if (value != null)
            {
                try
                {
                    // 👇 使用缓存视图
                    CurrentView = value.GetOrCreateView(); // 在 UI 线程调用
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"创建视图失败: {ex.Message}", "错误",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    CurrentView = null;
                }
            }
            else
            {
                CurrentView = null;
            }
        }
    }
}
