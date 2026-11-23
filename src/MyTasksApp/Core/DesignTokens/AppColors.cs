using Microsoft.Maui.Graphics;

namespace MyTasksApp.Core.DesignTokens;

/// <summary>
/// Nguồn chân lý duy nhất (Single Source of Truth) cho bảng màu của toàn bộ ứng dụng.
/// Mọi thay đổi màu chủ đạo (Primary) hay màu nền chỉ cần thực hiện tại đây.
/// </summary>
public static class AppColors
{
    // ==========================================
    // 1. PRIMARY BRAND COLORS (MÀU CHỦ ĐẠO)
    // ==========================================
    // Để đổi màu chủ đạo (VD: sang Teal #0D9488), chỉ cần sửa 2 dòng dưới đây:
    public static readonly Color Primary = Color.FromArgb("#4F46E5");            // Light Primary (Indigo-600)
    public static readonly Color PrimaryDark = Color.FromArgb("#6366F1");        // Dark Primary (Indigo-500)

    public static readonly Color PrimaryVariant = Color.FromArgb("#4338CA");     // Indigo-700
    public static readonly Color PrimaryVariantDark = Color.FromArgb("#818CF8"); // Indigo-400

    public static readonly Color PrimaryLight = Color.FromArgb("#EEF2FF");       // Indigo-50
    public static readonly Color PrimaryLightDark = Color.FromArgb("#1E1B4B");   // Indigo-950

    // ==========================================
    // 2. SURFACES & BACKGROUNDS
    // ==========================================
    public static readonly Color Background = Color.FromArgb("#F8FAFC");         // Slate-50
    public static readonly Color BackgroundDark = Color.FromArgb("#0F172A");     // Slate-900

    public static readonly Color Surface = Color.FromArgb("#FFFFFF");            // White
    public static readonly Color SurfaceDark = Color.FromArgb("#1E293B");        // Slate-800

    public static readonly Color SurfaceVariant = Color.FromArgb("#F1F5F9");     // Slate-100
    public static readonly Color SurfaceVariantDark = Color.FromArgb("#334155"); // Slate-700

    public static readonly Color BorderColor = Color.FromArgb("#E2E8F0");        // Slate-200
    public static readonly Color BorderColorDark = Color.FromArgb("#334155");    // Slate-700

    // ==========================================
    // 3. TYPOGRAPHY TEXT COLORS
    // ==========================================
    public static readonly Color TextPrimary = Color.FromArgb("#0F172A");        // Slate-900
    public static readonly Color TextPrimaryDark = Color.FromArgb("#F8FAFC");    // Slate-50

    public static readonly Color TextSecondary = Color.FromArgb("#475569");      // Slate-600
    public static readonly Color TextSecondaryDark = Color.FromArgb("#94A3B8");  // Slate-400

    public static readonly Color TextMuted = Color.FromArgb("#94A3B8");          // Slate-400
    public static readonly Color TextMutedDark = Color.FromArgb("#64748B");      // Slate-500

    // ==========================================
    // 4. SEMANTIC STATUS COLORS
    // ==========================================
    public static readonly Color Danger = Color.FromArgb("#DC2626");             // Red-600
    public static readonly Color DangerDark = Color.FromArgb("#EF4444");         // Red-500
    public static readonly Color DangerSurface = Color.FromArgb("#FEF2F2");      // Red-50
    public static readonly Color DangerSurfaceDark = Color.FromArgb("#450A0A");  // Red-950

    public static readonly Color Warning = Color.FromArgb("#D97706");            // Amber-600
    public static readonly Color WarningDark = Color.FromArgb("#F59E0B");        // Amber-500
    public static readonly Color WarningSurface = Color.FromArgb("#FFFBEB");     // Amber-50
    public static readonly Color WarningSurfaceDark = Color.FromArgb("#451A03"); // Amber-950

    public static readonly Color Success = Color.FromArgb("#16A34A");            // Green-600
    public static readonly Color SuccessDark = Color.FromArgb("#22C55E");        // Green-500
    public static readonly Color SuccessSurface = Color.FromArgb("#F0FDF4");     // Green-50
    public static readonly Color SuccessSurfaceDark = Color.FromArgb("#052E16"); // Green-950

    // ==========================================
    // 5. COMPONENT SPECIFIC
    // ==========================================
    public static readonly Color ProgressTrack = Color.FromArgb("#E0E7FF");      // Indigo-100
    public static readonly Color ProgressTrackDark = Color.FromArgb("#312E81");  // Indigo-900
    public static readonly Color ProgressFill = Color.FromArgb("#4F46E5");       // Primary
    public static readonly Color ProgressFillDark = Color.FromArgb("#6366F1");   // PrimaryDark

    public static readonly Color SnackbarBg = Color.FromArgb("#FFFFFF");         // White
    public static readonly Color SnackbarBgDark = Color.FromArgb("#020617");     // Slate-950
    public static readonly Color SnackbarUndoBg = Color.FromArgb("#EEF2FF");     // PrimaryLight
    public static readonly Color SnackbarUndoBgDark = Color.FromArgb("#1E1B4B"); // PrimaryLightDark
    public static readonly Color SnackbarUndoText = Color.FromArgb("#4F46E5");   // Primary
    public static readonly Color SnackbarUndoTextDark = Color.FromArgb("#818CF8");// PrimaryVariantDark
}
