class PrefsKeys {
  static const String keyTutorialCompleted = "key_tutorial_completed";
  static const String keyCurrentGuideSource = "key_current_guide_source";
  static const String keyCurrentGuideDestination =
      "key_current_guide_destination";
  static const String keyTheme = "key_theme";

  static const String keyUserId = "key_user_id";
}

class PrefsThemeKey {
  final String key;

  PrefsThemeKey(this.key);

  static const system = "system";
  static const light = "light";
  static const dark = "dark";
}
