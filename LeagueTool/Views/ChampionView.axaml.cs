using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using LeagueTool.Models;
using LeagueTool.ViewModels;

namespace LeagueTool.Views;

public partial class ChampionView : UserControl
{
    public ChampionView()
    {
        InitializeComponent();
    }

    private void ToggleShowCompletedFilter(object? sender, RoutedEventArgs e)
    {
        ((ChampionViewModel)DataContext!).ToggleShowCompletedFilter();
    }

    private void ToggleFilter_Top(object? sender, RoutedEventArgs e)
    {
        ((ChampionViewModel)DataContext!).ToggleFilter(Lane.Top);
    }
    private void ToggleFilter_Mid(object? sender, RoutedEventArgs e)
    {
        ((ChampionViewModel)DataContext!).ToggleFilter(Lane.Mid);
    }
    private void ToggleFilter_Jungle(object? sender, RoutedEventArgs e)
    {
        ((ChampionViewModel)DataContext!).ToggleFilter(Lane.Jungle);
    }
    private void ToggleFilter_Bot(object? sender, RoutedEventArgs e)
    {
        ((ChampionViewModel)DataContext!).ToggleFilter(Lane.Bot);
    }
    private void ToggleFilter_Support(object? sender, RoutedEventArgs e)
    {
        ((ChampionViewModel)DataContext!).ToggleFilter(Lane.Support);
    }

    private void InputElement_OnKeyUp(object? sender, KeyEventArgs e)
    {
        ((ChampionViewModel)DataContext!).FilterText(((TextBox)sender!).Text);
    }
}