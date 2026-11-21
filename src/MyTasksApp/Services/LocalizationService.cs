using MyTasksApp.Core.Enums;
using MyTasksApp.Services.Interfaces;

namespace MyTasksApp.Services;

/// <summary>
/// Quản lý từ điển bản địa hóa song ngữ (Tiếng Việt và English)
/// </summary>
public class LocalizationService : ILocalizationService
{
    private AppLanguage _currentLanguage = AppLanguage.Vietnamese;

    public AppLanguage CurrentLanguage => _currentLanguage;

    public event EventHandler? LanguageChanged;

    private static readonly Dictionary<string, string> ViStrings = new()
    {
        // Bottom Tab Bar
        { "TabToday", "Hôm nay" },
        { "TabWeek", "Tuần này" },
        { "TabHistory", "Lịch sử" },
        { "TabSettings", "Cài đặt" },

        // Màn hình Cài đặt (Settings)
        { "SettingsTitle", "Cài đặt" },
        { "SettingsSubtitle", "Cá nhân hóa trải nghiệm của bạn" },
        { "SectionAppearance", "Giao diện" },
        { "ThemeMode", "Chế độ giao diện" },
        { "ThemeModeDesc", "Tùy chỉnh giao diện ứng dụng" },
        { "LightTheme", "Sáng" },
        { "DarkTheme", "Tối" },
        { "FontSize", "Cỡ chữ" },
        { "FontSizeDesc", "Điều chỉnh kích thước văn bản" },
        { "FontSizeSmall", "Nhỏ" },
        { "FontSizeNormal", "Thường" },
        { "FontSizeLarge", "Lớn" },
        { "SectionLanguage", "Ngôn ngữ" },
        { "AppLanguage", "Ngôn ngữ ứng dụng" },
        { "AppLanguageDesc", "Chọn ngôn ngữ hiển thị" },
        { "LangVietnamese", "Tiếng Việt" },
        { "LangEnglish", "English" },
        { "SectionPreview", "Xem trước" },
        { "PreviewTitle", "Công việc của tôi" },
        { "PreviewDesc", "Hoàn thành công việc quan trọng hôm nay" },
        { "PreviewTaskTitle", "Review Pull Request" },
        { "PreviewTaskProject", "MyTasks App" },
        { "SectionReviewerTools", "Công cụ kiểm thử" },
        { "ReviewerToolsDesc", "Giả lập các trạng thái để kiểm tra giao diện" },
        { "StateLoading", "Trạng thái đang tải" },
        { "StateEmpty", "Danh sách trống" },
        { "StateNotFound", "Không tìm thấy công việc" },
        { "StateResetData", "Đặt lại dữ liệu mẫu" },
        { "ActionPreview", "Xem thử" },
        { "ActionReset", "Đặt lại" },
        { "SectionAppInfo", "Thông tin ứng dụng" },
        { "AppDescription", "Ứng dụng quản lý công việc cá nhân" },
        { "AppVersion", "Phiên bản 1.0.0" },
        { "ConfirmResetTitle", "Xác nhận đặt lại" },
        { "ConfirmResetMessage", "Bạn có chắc muốn đặt lại toàn bộ dữ liệu mẫu in-memory về trạng thái ban đầu?" },
        { "ConfirmOk", "Đặt lại" },
        { "ConfirmCancel", "Hủy" },
        { "ResetSuccess", "Đã khôi phục dữ liệu mẫu thành công!" },

        // Snackbar Feedback khi đổi Cài đặt
        { "FeedbackLangChangedVi", "Đã đổi ngôn ngữ sang Tiếng Việt" },
        { "FeedbackLangChangedEn", "Đã đổi ngôn ngữ sang English" },
        { "FeedbackFontSmall", "Cỡ chữ: Nhỏ" },
        { "FeedbackFontNormal", "Cỡ chữ: Thường" },
        { "FeedbackFontLarge", "Cỡ chữ: Lớn" },
        { "FeedbackThemeLight", "Đã chuyển sang giao diện Sáng" },
        { "FeedbackThemeDark", "Đã chuyển sang giao diện Tối" },

        // Màn hình Hôm nay (TodayPage)
        { "TodayEmptyMessage", "Hôm nay bạn không có việc cần làm!" },
        { "TodayEmptySubMessage", "Hãy nghỉ ngơi hoặc lên kế hoạch cho những ngày tiếp theo." },
        { "SectionOverdue", "QUÁ HẠN" },
        { "SectionToday", "HÔM NAY" },
        { "SectionCompleted", "XONG" },
        { "ProgressTodayTitle", "Tiến độ hôm nay" },
        { "ProgressCompletedFormat", "{0}/{1} việc hoàn thành" },

        // Màn hình Tuần này (WeekPage)
        { "WeekEmptyMessage", "Không có công việc" },
        { "WeekEmptySubMessage", "Ngày này chưa có công việc nào được lên lịch." },
        { "WeekNavigationTitle", "Tuần này" },
        { "DayProgressTitle", "Tiến độ ngày" },
        { "DayOfWeekMon", "T2" },
        { "DayOfWeekTue", "T3" },
        { "DayOfWeekWed", "T4" },
        { "DayOfWeekThu", "T5" },
        { "DayOfWeekFri", "T6" },
        { "DayOfWeekSat", "T7" },
        { "DayOfWeekSun", "CN" },

        // Màn hình Lịch sử (HistoryPage)
        { "HistoryEmptyMessage", "Chưa có công việc hoàn thành" },
        { "HistoryEmptySubMessage", "Những công việc bạn hoàn thành sẽ xuất hiện tại đây." },
        { "HistoryTitle", "Lịch sử" },
        { "HistoryCompletedBadge", "Đã hoàn thành" },
        { "AppSubtitle", "Việc của tôi" }
    };

    private static readonly Dictionary<string, string> EnStrings = new()
    {
        // Bottom Tab Bar
        { "TabToday", "Today" },
        { "TabWeek", "This Week" },
        { "TabHistory", "History" },
        { "TabSettings", "Settings" },

        // Màn hình Cài đặt (Settings)
        { "SettingsTitle", "Settings" },
        { "SettingsSubtitle", "Personalize your experience" },
        { "SectionAppearance", "Appearance" },
        { "ThemeMode", "Theme Mode" },
        { "ThemeModeDesc", "Customize application appearance" },
        { "LightTheme", "Light" },
        { "DarkTheme", "Dark" },
        { "FontSize", "Font Size" },
        { "FontSizeDesc", "Adjust display text scaling" },
        { "FontSizeSmall", "Small" },
        { "FontSizeNormal", "Regular" },
        { "FontSizeLarge", "Large" },
        { "SectionLanguage", "Language" },
        { "AppLanguage", "App Language" },
        { "AppLanguageDesc", "Select user interface language" },
        { "LangVietnamese", "Tiếng Việt" },
        { "LangEnglish", "English" },
        { "SectionPreview", "Preview" },
        { "PreviewTitle", "My Tasks" },
        { "PreviewDesc", "Accomplish important tasks today" },
        { "PreviewTaskTitle", "Review Pull Request" },
        { "PreviewTaskProject", "MyTasks App" },
        { "SectionReviewerTools", "Reviewer Tools" },
        { "ReviewerToolsDesc", "Simulate states to verify interface" },
        { "StateLoading", "Loading State" },
        { "StateEmpty", "Empty List" },
        { "StateNotFound", "Task Not Found" },
        { "StateResetData", "Reset Mock Data" },
        { "ActionPreview", "Preview" },
        { "ActionReset", "Reset" },
        { "SectionAppInfo", "App Information" },
        { "AppDescription", "Personal task management application" },
        { "AppVersion", "Version 1.0.0" },
        { "ConfirmResetTitle", "Reset Confirmation" },
        { "ConfirmResetMessage", "Are you sure you want to reset all mock data to its initial state?" },
        { "ConfirmOk", "Reset" },
        { "ConfirmCancel", "Cancel" },
        { "ResetSuccess", "Mock data successfully restored!" },

        // Snackbar Feedback khi đổi Cài đặt
        { "FeedbackLangChangedVi", "Language switched to Vietnamese" },
        { "FeedbackLangChangedEn", "Language switched to English" },
        { "FeedbackFontSmall", "Font size: Small" },
        { "FeedbackFontNormal", "Font size: Regular" },
        { "FeedbackFontLarge", "Font size: Large" },
        { "FeedbackThemeLight", "Switched to Light mode" },
        { "FeedbackThemeDark", "Switched to Dark mode" },

        // Màn hình Hôm nay (TodayPage)
        { "TodayEmptyMessage", "You have no tasks to do today!" },
        { "TodayEmptySubMessage", "Take a break or plan ahead for the upcoming days." },
        { "SectionOverdue", "OVERDUE" },
        { "SectionToday", "TODAY" },
        { "SectionCompleted", "DONE" },
        { "ProgressTodayTitle", "Today's progress" },
        { "ProgressCompletedFormat", "{0}/{1} tasks completed" },

        // Màn hình Tuần này (WeekPage)
        { "WeekEmptyMessage", "No tasks scheduled" },
        { "WeekEmptySubMessage", "There are no tasks scheduled for this day." },
        { "WeekNavigationTitle", "This Week" },
        { "DayProgressTitle", "Daily progress" },
        { "DayOfWeekMon", "Mon" },
        { "DayOfWeekTue", "Tue" },
        { "DayOfWeekWed", "Wed" },
        { "DayOfWeekThu", "Thu" },
        { "DayOfWeekFri", "Fri" },
        { "DayOfWeekSat", "Sat" },
        { "DayOfWeekSun", "Sun" },

        // Màn hình Lịch sử (HistoryPage)
        { "HistoryEmptyMessage", "No completed tasks yet" },
        { "HistoryEmptySubMessage", "Tasks that you complete will show up here." },
        { "HistoryTitle", "History" },
        { "HistoryCompletedBadge", "Completed" },
        { "AppSubtitle", "My tasks" }
    };

    public string GetString(string key)
    {
        var dict = _currentLanguage == AppLanguage.English ? EnStrings : ViStrings;
        if (dict.TryGetValue(key, out var val))
        {
            return val;
        }

        return ViStrings.TryGetValue(key, out var fallback) ? fallback : key;
    }

    public string this[string key] => GetString(key);

    public void SetLanguage(AppLanguage language)
    {
        if (_currentLanguage != language)
        {
            _currentLanguage = language;
            LanguageChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}