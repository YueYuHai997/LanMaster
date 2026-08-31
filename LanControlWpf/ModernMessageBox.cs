using MaterialDesignThemes.Wpf;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace LanControlWpf
{
    static class MessageBox
    {
        public static MessageBoxResult Show(string message, string title)
        {
            return Show(message, title, MessageBoxButton.OK, MessageBoxImage.None);
        }

        public static MessageBoxResult Show(string message, string title, MessageBoxButton buttons, MessageBoxImage image)
        {
            var owner = Application.Current == null
                ? null
                : Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive)
                  ?? Application.Current.MainWindow;
            var dialog = new ModernDialogWindow(message, title, buttons, image);
            if (owner != null && owner.IsVisible) dialog.Owner = owner;
            return dialog.ShowResult();
        }
    }

    sealed class ModernDialogWindow : Window
    {
        readonly MessageBoxButton buttons;
        MessageBoxResult result = MessageBoxResult.None;

        public ModernDialogWindow(string message, string title, MessageBoxButton buttons, MessageBoxImage image)
        {
            this.buttons = buttons;
            Title = title;
            Width = 460;
            MinHeight = 210;
            MaxHeight = 680;
            SizeToContent = SizeToContent.Height;
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            AllowsTransparency = true;
            Background = Brushes.Transparent;
            ShowInTaskbar = false;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            FontFamily = new FontFamily("Microsoft YaHei UI");
            SnapsToDevicePixels = true;

            var root = new Border
            {
                Margin = new Thickness(18),
                Background = Brushes.White,
                BorderBrush = BrushFrom("#D9DEE8"),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(12),
                Effect = new DropShadowEffect { BlurRadius = 28, ShadowDepth = 7, Opacity = 0.24, Color = ColorFrom("#344054") }
            };
            var layout = new Grid();
            layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            layout.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            root.Child = layout;

            var header = new Border
            {
                Background = BrushFrom("#F8F7FC"),
                CornerRadius = new CornerRadius(12, 12, 0, 0),
                Padding = new Thickness(22, 15, 12, 13)
            };
            var headerGrid = new Grid();
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition());
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            header.Child = headerGrid;
            headerGrid.Children.Add(new TextBlock
            {
                Text = string.IsNullOrWhiteSpace(title) ? "提示" : title,
                FontSize = 17,
                FontWeight = FontWeights.SemiBold,
                Foreground = BrushFrom("#253044"),
                VerticalAlignment = VerticalAlignment.Center
            });
            var close = new Button
            {
                Width = 38,
                Height = 38,
                Padding = new Thickness(8),
                Foreground = BrushFrom("#667085"),
                Content = new PackIcon { Kind = PackIconKind.Close, Width = 20, Height = 20 },
                ToolTip = "关闭"
            };
            close.SetResourceReference(FrameworkElement.StyleProperty, "MaterialDesignIconButton");
            close.Click += delegate { result = DefaultResult(); Close(); };
            Grid.SetColumn(close, 1);
            headerGrid.Children.Add(close);
            layout.Children.Add(header);

            var content = new Grid { Margin = new Thickness(28, 25, 30, 26) };
            content.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(48) });
            content.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            var icon = CreateIcon(image);
            content.Children.Add(icon);
            var messageText = new TextBlock
            {
                Text = message ?? string.Empty,
                FontSize = 15,
                LineHeight = 24,
                TextWrapping = TextWrapping.Wrap,
                Foreground = BrushFrom("#273142"),
                VerticalAlignment = VerticalAlignment.Center,
                MaxWidth = 340
            };
            Grid.SetColumn(messageText, 1);
            content.Children.Add(messageText);
            var contentScroll = new ScrollViewer
            {
                Content = content,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
            };
            Grid.SetRow(contentScroll, 1);
            layout.Children.Add(contentScroll);

            var footer = new Border
            {
                Background = BrushFrom("#F6F7F9"),
                CornerRadius = new CornerRadius(0, 0, 12, 12),
                Padding = new Thickness(20, 12, 20, 14)
            };
            var actions = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            footer.Child = actions;
            if (buttons == MessageBoxButton.YesNo)
            {
                actions.Children.Add(CreateButton("取消", false, false, MessageBoxResult.No));
                actions.Children.Add(CreateButton("确认", true, title != null && title.Contains("删除"), MessageBoxResult.Yes));
            }
            else
            {
                actions.Children.Add(CreateButton("确定", true, false, MessageBoxResult.OK));
            }
            Grid.SetRow(footer, 2);
            layout.Children.Add(footer);
            Content = root;

            KeyDown += delegate(object sender, KeyEventArgs e)
            {
                if (e.Key != Key.Escape) return;
                result = DefaultResult();
                Close();
            };
        }

        public MessageBoxResult ShowResult()
        {
            ShowDialog();
            return result == MessageBoxResult.None ? DefaultResult() : result;
        }

        Button CreateButton(string text, bool primary, bool danger, MessageBoxResult clickResult)
        {
            var button = new Button
            {
                Content = text,
                MinWidth = 96,
                Height = 38,
                Margin = new Thickness(5, 0, 0, 0),
                Padding = new Thickness(18, 7, 18, 7),
                IsDefault = primary,
                IsCancel = !primary
            };
            button.SetResourceReference(FrameworkElement.StyleProperty, primary ? "MaterialDesignRaisedButton" : "MaterialDesignOutlinedButton");
            if (danger)
            {
                button.Background = BrushFrom("#C62828");
                button.Foreground = Brushes.White;
            }
            button.Click += delegate { result = clickResult; Close(); };
            return button;
        }

        static PackIcon CreateIcon(MessageBoxImage image)
        {
            var kind = PackIconKind.InformationOutline;
            var color = "#3F51B5";
            if (image == MessageBoxImage.Warning) { kind = PackIconKind.AlertOutline; color = "#F59E0B"; }
            else if (image == MessageBoxImage.Error) { kind = PackIconKind.AlertCircleOutline; color = "#C62828"; }
            else if (image == MessageBoxImage.Question) { kind = PackIconKind.HelpCircleOutline; color = "#3F51B5"; }
            return new PackIcon
            {
                Kind = kind,
                Width = 30,
                Height = 30,
                Foreground = BrushFrom(color),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(1, 1, 0, 0)
            };
        }

        MessageBoxResult DefaultResult()
        {
            return buttons == MessageBoxButton.YesNo ? MessageBoxResult.No : MessageBoxResult.OK;
        }

        static SolidColorBrush BrushFrom(string value) { return new SolidColorBrush(ColorFrom(value)); }
        static Color ColorFrom(string value) { return (Color)ColorConverter.ConvertFromString(value); }
    }
}
