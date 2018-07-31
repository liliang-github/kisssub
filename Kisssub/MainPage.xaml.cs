using Kisssub.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Kisssub
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ObservableCollection<ItemModel> items;
        public MainPage()
        {
            this.InitializeComponent();
            items = new ObservableCollection<ItemModel>();
        }
        /// <summary>
        /// 左侧汉堡菜单开关
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            leftSplitView.IsPaneOpen = !leftSplitView.IsPaneOpen;
        }
        /// <summary>
        /// 首次加载获取第一页数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            loading.IsActive = true;
            await ItemManager.GetAllItems(items,1);
            loading.IsActive = false;
        }
        /// <summary>
        /// 当窗体大小发生改变时执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //获取当前窗体大小
            double width = e.NewSize.Width;
            //判断是否是小窗体
            if (600 > width)
            {
                //小窗体
                //左侧宽度放大到100%
                leftSplitView.Width = width;
                //显示右侧的返回按钮
                back.Visibility = Visibility.Visible;
            }
            else
            {
                //大窗体
                //左侧宽度修改为窗体宽度的百分之33
                leftSplitView.Width = width * 0.33;
                //右侧宽度修改为窗体宽度的百分之66
                right.Width = width * 0.66;
                //隐藏右侧的返回按钮
                back.Visibility = Visibility.Collapsed;
            }
            

        }
        /// <summary>
        /// 点击左边左侧列表时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            //获取点击的元素
            ItemModel item = e.ClickedItem as ItemModel;
            //设置右侧的标题
            Title.Text = item.Name;
            //设置右侧时间
            Date.Text = item.Date;
            //设置右侧作者
            Author.Text = item.Author;
            //显示loading图
            loading.IsActive = true;
            //清空webView数据以免回显
            webView.NavigateToString("");
            //判断当前屏幕是否是小屏
            if (600 > this.ActualWidth)
            {
                //无动画切换
                //leftSplitView.Width = 0;
                //right.Width = this.ActualWidth;

                //是用动画切换页面
                leftOpen.From = this.ActualWidth;
                rightOpen.To = this.ActualWidth;
                open.Begin();
            }
            //调用API获取返回的html代码
            string content = await ItemManager.GetHtmlContent(item.UrlId);
            //将html代码设置到webView内
            webView.NavigateToString(content);
            //关闭加载中图
            loading.IsActive = false;


        }
        /// <summary>
        /// 返回按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //无动画方式
            //leftSplitView.Width = this.ActualWidth;
            //right.Width = 0;

            //使用故事版动画显示控件
            leftClose.To = this.ActualWidth;
            rightClose.From = this.ActualWidth;
            close.Begin();
        }

        //加载数据时的标记
        private bool _isLoding;
        /// <summary>
        /// 列表到底部自动加载数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            //判断是否正在获取数据,如果是则返回
            if (_isLoding) return;
            //判断滚动是否到低,到低继续获取数据否则返回
            if (scroll.VerticalOffset <= scroll.ScrollableHeight - 100) return;
            //修改标记为获取数据中以免重复调用
            _isLoding = true;
            //显示Loading
            loading.IsActive = true;
            //异步获取数据
            await ItemManager.GetAllItems(items, items.Count / 50);
            loading.IsActive = false;
            //当数据获取完成时,修改标记为未加载数据
            _isLoding = false;
        }
    }
}
