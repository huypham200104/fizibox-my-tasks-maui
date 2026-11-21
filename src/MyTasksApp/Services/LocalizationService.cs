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

        // Floating Snackbar & Undo
        { "SnackbarTaskCompleted", "Đã hoàn thành công việc" },
        { "SnackbarTaskReopened", "Đã mở lại công việc" },
        { "SnackbarTaskDeleted", "Đã xóa công việc" },
        { "SnackbarTaskCreated", "Đã tạo công việc mới thành công" },
        { "SnackbarTaskUpdated", "Đã cập nhật công việc thành công" },
        { "SnackbarTaskRestored", "Đã hoàn tác: " },
        { "SnackbarUndoButton", "Hoàn tác" },
        { "SnackbarCannotUpdate", "Không thể cập nhật công việc" },
        { "SnackbarUnsavedChanges", "Bạn có thay đổi chưa được lưu" },
        { "SnackbarEnterTitle", "Vui lòng nhập tên công việc" },
        { "SnackbarTestSuccess", "Thử nghiệm: Thao tác đã thành công!" },
        { "SnackbarTestDanger", "Thử nghiệm: Không thể cập nhật công việc!" },
        { "SnackbarTestWarning", "Thử nghiệm: Bạn có thay đổi chưa được lưu!" },
        { "SnackbarTestUndo", "Thử nghiệm: Đã xóa công việc mẫu!" },

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
        { "HistorySubtitle", "Những công việc bạn đã hoàn thành" },
        { "HistoryCompletedBadge", "Đã hoàn thành" },
        { "HistoryCompletedSection", "Công việc đã hoàn thành" },
        { "HistoryAchievementTitle", "Công việc đã hoàn thành" },
        { "HistorySearchPlaceholder", "Tìm theo tiêu đề hoặc tên dự án..." },
        { "HistorySearchEmptyMessage", "Không tìm thấy kết quả" },
        { "HistorySearchEmptySubMessage", "Thử tìm kiếm với từ khóa khác hoặc xóa bộ lọc." },
        { "HistoryClearSearch", "Xóa tìm kiếm" },
        { "HistoryBackToToday", "Quay lại hôm nay" },
        { "HistoryFilterAll", "Tất cả" },
        { "HistoryFilterToday", "Hôm nay" },
        { "HistoryFilterWeek", "Tuần này" },
        { "HistoryFilterMonth", "Tháng này" },
        { "HistoryPeriodAll", "Tất cả thời gian" },
        { "HistoryPeriodToday", "Trong ngày hôm nay" },
        { "HistoryPeriodWeek", "Trong tuần này" },
        { "HistoryPeriodMonthFormat", "Trong tháng {0}, {1}" },
        { "HistorySortTooltip", "Đổi thứ tự sắp xếp" },
        { "HistoryReopenTitle", "Mở lại công việc?" },
        { "HistoryReopenMessage", "Bạn có chắc chắn muốn mở lại công việc này không? Trạng thái sẽ được chuyển về Chưa hoàn thành." },
        { "HistoryReopenConfirm", "Xác nhận" },
        { "HistoryReopenCancel", "Hủy" },
        { "HistoryDateToday", "Hôm nay" },
        { "HistoryDateYesterday", "Hôm qua" },
        { "AppSubtitle", "Việc của tôi" },

        // Màn hình Chi tiết công việc (TaskDetailPage) & Modals
        { "DetailTitle", "Chi tiết công việc" },
        { "DetailMenuEdit", "Chỉnh sửa thông tin" },
        { "DetailMenuDelete", "Xóa công việc" },
        { "DetailSectionDesc", "Mô tả công việc" },
        { "DetailSectionInfo", "Thông tin công việc" },
        { "DetailDueDate", "Hạn hoàn thành" },
        { "DetailDueTime", "Thời gian" },
        { "DetailProject", "Dự án" },
        { "DetailStatus", "Trạng thái" },
        { "DetailSectionNotes", "Ghi chú" },
        { "DetailActionComplete", "Hoàn thành công việc" },
        { "DetailActionReopen", "Mở lại công việc" },
        { "DetailCompletedAt", "Hoàn thành lúc {0}, {1}" },
        { "StatusPendingTitle", "Chưa hoàn thành" },
        { "StatusPendingDesc", "Công việc chưa được thực hiện" },
        { "StatusInProgressTitle", "Đang thực hiện" },
        { "StatusInProgressDesc", "Công việc đang được xử lý" },
        { "StatusCompletedTitle", "Đã hoàn thành" },
        { "StatusCompletedDesc", "Công việc đã hoàn tất" },
        { "SheetStatusTitle", "Chọn trạng thái" },
        { "SheetStatusSubtitle", "Cập nhật trạng thái công việc" },
        { "SheetDateTitle", "Chọn ngày đến hạn" },
        { "SheetDateSubtitle", "Chọn thời gian hoàn thành công việc" },
        { "SheetTimeTitle", "Chọn giờ đến hạn" },
        { "SheetTimeSubtitle", "Thiết lập thời gian hoàn thành công việc" },
        { "SheetProjectTitle", "Chọn dự án" },
        { "SheetProjectSubtitle", "Gán công việc cho dự án" },
        { "ModalEditTitle", "Chỉnh sửa công việc" },
        { "ModalEditSubtitle", "Cập nhật thông tin công việc của bạn" },
        { "ModalCreateTitle", "Tạo công việc mới" },
        { "ModalCreateSubtitle", "Nhập thông tin công việc cần hoàn thành hôm nay" },
        { "FieldTaskTitle", "Tên công việc" },
        { "FieldTaskTitlePlaceholder", "Ví dụ: Thiết kế giao diện Home" },
        { "FieldTaskProject", "Dự án" },
        { "FieldTaskProjectPlaceholder", "Tên dự án (Ví dụ: MyTasks App)" },
        { "FieldTaskDesc", "Mô tả chi tiết" },
        { "FieldTaskDescPlaceholder", "Nhập ghi chú thêm cho công việc..." },
        { "FieldTaskNotes", "Ghi chú" },
        { "BtnCancel", "Hủy" },
        { "BtnSave", "Lưu thay đổi" },
        { "BtnSaveTask", "Lưu công việc" },
        { "BtnConfirm", "Xác nhận" },
        { "BtnDelete", "Xóa" },
        { "DefaultProjectPersonal", "Công việc cá nhân" },
        { "DialogReopenTitle", "Mở lại công việc?" },
        { "DialogReopenMsg", "Bạn có chắc chắn muốn mở lại công việc này không? Trạng thái sẽ được chuyển về Chưa hoàn thành." },
        { "DialogDeleteTitle", "Xóa công việc?" },
        { "DialogDeleteMsg", "Bạn có chắc chắn muốn xóa công việc này không? Hành động này có thể hoàn tác trong vòng 5 giây." },
        { "ShortcutToday", "Hôm nay" },
        { "NotFoundDetailTitle", "Không tìm thấy công việc" },
        { "NotFoundDetailDesc", "Công việc này không tồn tại trong bộ dữ liệu hoặc đã bị xóa khỏi hệ thống." },
        { "BtnBackToList", "Quay lại danh sách" }
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

        // Floating Snackbar & Undo
        { "SnackbarTaskCompleted", "Task completed" },
        { "SnackbarTaskReopened", "Task reopened" },
        { "SnackbarTaskDeleted", "Task deleted" },
        { "SnackbarTaskCreated", "New task created successfully" },
        { "SnackbarTaskUpdated", "Task updated successfully" },
        { "SnackbarTaskRestored", "Restored: " },
        { "SnackbarUndoButton", "Undo" },
        { "SnackbarCannotUpdate", "Cannot update task" },
        { "SnackbarUnsavedChanges", "You have unsaved changes" },
        { "SnackbarEnterTitle", "Please enter a task title" },
        { "SnackbarTestSuccess", "Test: Action completed successfully!" },
        { "SnackbarTestDanger", "Test: Cannot update task!" },
        { "SnackbarTestWarning", "Test: You have unsaved changes!" },
        { "SnackbarTestUndo", "Test: Sample task deleted!" },

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
        { "HistorySubtitle", "Tasks that you have completed" },
        { "HistoryCompletedBadge", "Completed" },
        { "HistoryCompletedSection", "Completed tasks" },
        { "HistoryAchievementTitle", "Tasks completed" },
        { "HistorySearchPlaceholder", "Search by title or project name..." },
        { "HistorySearchEmptyMessage", "No matching tasks found" },
        { "HistorySearchEmptySubMessage", "Try searching with a different keyword or reset filters." },
        { "HistoryClearSearch", "Clear search" },
        { "HistoryBackToToday", "Back to today" },
        { "HistoryFilterAll", "All" },
        { "HistoryFilterToday", "Today" },
        { "HistoryFilterWeek", "This Week" },
        { "HistoryFilterMonth", "This Month" },
        { "HistoryPeriodAll", "All time" },
        { "HistoryPeriodToday", "Today" },
        { "HistoryPeriodWeek", "This week" },
        { "HistoryPeriodMonthFormat", "In {0} {1}" },
        { "HistorySortTooltip", "Toggle sort order" },
        { "HistoryReopenTitle", "Reopen task?" },
        { "HistoryReopenMessage", "Are you sure you want to reopen this task? Its status will be changed to Incomplete." },
        { "HistoryReopenConfirm", "Confirm" },
        { "HistoryReopenCancel", "Cancel" },
        { "HistoryDateToday", "Today" },
        { "HistoryDateYesterday", "Yesterday" },
        { "AppSubtitle", "My tasks" },

        // Task Detail Page & Modals
        { "DetailTitle", "Task Details" },
        { "DetailMenuEdit", "Edit Task Info" },
        { "DetailMenuDelete", "Delete Task" },
        { "DetailSectionDesc", "Task Description" },
        { "DetailSectionInfo", "Task Information" },
        { "DetailDueDate", "Due Date" },
        { "DetailDueTime", "Time" },
        { "DetailProject", "Project" },
        { "DetailStatus", "Status" },
        { "DetailSectionNotes", "Notes" },
        { "DetailActionComplete", "Complete Task" },
        { "DetailActionReopen", "Reopen Task" },
        { "DetailCompletedAt", "Completed at {0}, {1}" },
        { "StatusPendingTitle", "Pending" },
        { "StatusPendingDesc", "Task has not been started" },
        { "StatusInProgressTitle", "In Progress" },
        { "StatusInProgressDesc", "Task is being handled" },
        { "StatusCompletedTitle", "Completed" },
        { "StatusCompletedDesc", "Task has been finished" },
        { "SheetStatusTitle", "Change Status" },
        { "SheetStatusSubtitle", "Update task status" },
        { "SheetDateTitle", "Select Due Date" },
        { "SheetDateSubtitle", "Choose task deadline" },
        { "SheetTimeTitle", "Select Due Time" },
        { "SheetTimeSubtitle", "Set task deadline time" },
        { "SheetProjectTitle", "Select Project" },
        { "SheetProjectSubtitle", "Assign task to project" },
        { "ModalEditTitle", "Edit Task" },
        { "ModalEditSubtitle", "Update your task information" },
        { "ModalCreateTitle", "Create New Task" },
        { "ModalCreateSubtitle", "Enter task details to complete today" },
        { "FieldTaskTitle", "Task Title" },
        { "FieldTaskTitlePlaceholder", "e.g. Design Home UI" },
        { "FieldTaskProject", "Project" },
        { "FieldTaskProjectPlaceholder", "Project name (e.g. MyTasks App)" },
        { "FieldTaskDesc", "Detailed Description" },
        { "FieldTaskDescPlaceholder", "Enter additional notes for task..." },
        { "FieldTaskNotes", "Notes" },
        { "BtnCancel", "Cancel" },
        { "BtnSave", "Save Changes" },
        { "BtnSaveTask", "Save Task" },
        { "BtnConfirm", "Confirm" },
        { "BtnDelete", "Delete" },
        { "DefaultProjectPersonal", "Personal Task" },
        { "DialogReopenTitle", "Reopen task?" },
        { "DialogReopenMsg", "Are you sure you want to reopen this task? Its status will be changed to Pending." },
        { "DialogDeleteTitle", "Delete task?" },
        { "DialogDeleteMsg", "Are you sure you want to delete this task? You can undo this action within 5 seconds." },
        { "ShortcutToday", "Today" },
        { "NotFoundDetailTitle", "Task Not Found" },
        { "NotFoundDetailDesc", "This task does not exist in the dataset or has been deleted." },
        { "BtnBackToList", "Back to List" }
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