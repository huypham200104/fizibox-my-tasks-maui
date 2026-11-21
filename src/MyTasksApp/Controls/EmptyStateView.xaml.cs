using FluentIcons.Common;

namespace MyTasksApp.Controls;

public partial class EmptyStateView : ContentView
{
    public static readonly BindableProperty SymbolProperty =
        BindableProperty.Create(
            nameof(Symbol),
            typeof(Symbol),
            typeof(EmptyStateView),
            Symbol.CalendarEmpty);

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(
            nameof(Title),
            typeof(string),
            typeof(EmptyStateView),
            string.Empty);

    public static readonly BindableProperty SubtitleProperty =
        BindableProperty.Create(
            nameof(Subtitle),
            typeof(string),
            typeof(EmptyStateView),
            string.Empty,
            propertyChanged: OnSubtitleChanged);

    public static readonly BindableProperty HasSubtitleProperty =
        BindableProperty.Create(
            nameof(HasSubtitle),
            typeof(bool),
            typeof(EmptyStateView),
            false);

    public static readonly BindableProperty IconSizeProperty =
        BindableProperty.Create(
            nameof(IconSize),
            typeof(double),
            typeof(EmptyStateView),
            42.0);

    public static readonly BindableProperty IconForegroundColorProperty =
        BindableProperty.Create(
            nameof(IconForegroundColor),
            typeof(Color),
            typeof(EmptyStateView),
            null,
            propertyChanged: OnIconForegroundColorChanged);

    public static readonly BindableProperty IconBackgroundColorProperty =
        BindableProperty.Create(
            nameof(IconBackgroundColor),
            typeof(Color),
            typeof(EmptyStateView),
            null,
            propertyChanged: OnIconBackgroundColorChanged);

    public Symbol Symbol
    {
        get => (Symbol)GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public bool HasSubtitle
    {
        get => (bool)GetValue(HasSubtitleProperty);
        private set => SetValue(HasSubtitleProperty, value);
    }

    public double IconSize
    {
        get => (double)GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    public Color? IconForegroundColor
    {
        get => (Color?)GetValue(IconForegroundColorProperty);
        set => SetValue(IconForegroundColorProperty, value);
    }

    public Color? IconBackgroundColor
    {
        get => (Color?)GetValue(IconBackgroundColorProperty);
        set => SetValue(IconBackgroundColorProperty, value);
    }

    public EmptyStateView()
    {
        InitializeComponent();
    }

    private static void OnSubtitleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is EmptyStateView view)
        {
            view.HasSubtitle = !string.IsNullOrWhiteSpace(newValue as string);
        }
    }

    private static void OnIconForegroundColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is EmptyStateView view && newValue is Color color)
        {
            view.IconElement.ForegroundColor = color;
        }
    }

    private static void OnIconBackgroundColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is EmptyStateView view && newValue is Color color)
        {
            view.IconContainer.BackgroundColor = color;
        }
    }
}
