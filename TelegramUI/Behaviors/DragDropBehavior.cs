using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TelegramUI.Models;

namespace TelegramUI.Behaviors;

public static class DragDropBehavior
{
    public static readonly DependencyProperty EnableChatReorderProperty =
        DependencyProperty.RegisterAttached(
            "EnableChatReorder", typeof(bool), typeof(DragDropBehavior),
            new PropertyMetadata(false, OnEnableChanged));

    public static bool GetEnableChatReorder(DependencyObject obj) => (bool)obj.GetValue(EnableChatReorderProperty);
    public static void SetEnableChatReorder(DependencyObject obj, bool value) => obj.SetValue(EnableChatReorderProperty, value);

    private static void OnEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ListBox list && e.NewValue is true)
        {
            list.AllowDrop = true;
            list.PreviewMouseMove += OnMouseMove;
            list.DragOver += OnDragOver;
            list.Drop += OnDrop;
        }
    }

    private static Point _startPoint;
    private static bool _isDragging;

    private static void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (_isDragging) return;
        if (e.LeftButton != MouseButtonState.Pressed) { _startPoint = e.GetPosition(null); return; }

        var diff = e.GetPosition(null) - _startPoint;
        if (Math.Abs(diff.X) < 6 && Math.Abs(diff.Y) < 6) return;

        if (sender is ListBox { SelectedItem: Chat chat } && chat.IsPinned)
        {
            _isDragging = true;
            var data = new DataObject("chat", chat);
            DragDrop.DoDragDrop((ListBox)sender, data, DragDropEffects.Move);
            _isDragging = false;
        }
    }

    private static void OnDragOver(object sender, DragEventArgs e)
    {
        // Only allow pinned chat reorder
        e.Effects = e.Data.GetDataPresent("chat") ? DragDropEffects.Move : DragDropEffects.None;
        e.Handled = true;
    }

    private static void OnDrop(object sender, DragEventArgs e)
    {
        if (sender is not ListBox list) return;
        if (!e.Data.GetDataPresent("chat")) return;
        if (list.ItemsSource is not System.Collections.Generic.IList<Chat> items) return;

        var dragged = (Chat)e.Data.GetData("chat");
        if (!dragged.IsPinned) return;

        // Find target index relative to item under mouse
        var targetItem = GetItemAt(list, e.GetPosition(list));
        if (targetItem == null || targetItem == dragged) return;
        if (!targetItem.IsPinned) return; // only reorder among pinned

        var oldIndex = items.IndexOf(dragged);
        var newIndex = items.IndexOf(targetItem);

        items.RemoveAt(oldIndex);
        items.Insert(newIndex, dragged);
        e.Handled = true;
    }

    private static Chat? GetItemAt(ListBox list, Point pos)
    {
        var element = list.InputHitTest(pos) as DependencyObject;
        while (element != null && element is not ListBoxItem)
            element = System.Windows.Media.VisualTreeHelper.GetParent(element);
        return (element as ListBoxItem)?.Content as Chat;
    }
}
