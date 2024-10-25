import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../persistence/prefs.dart';

final _prefsToMode = {
  PrefsThemeKey.system: ThemeMode.system,
  PrefsThemeKey.light: ThemeMode.light,
  PrefsThemeKey.dark: ThemeMode.dark,
};

final _modeToPrefs = {
  ThemeMode.system: PrefsThemeKey.system,
  ThemeMode.light: PrefsThemeKey.light,
  ThemeMode.dark: PrefsThemeKey.dark,
};

final _modeIcon = {
  ThemeMode.light: Icons.sunny,
  ThemeMode.dark: Icons.nightlight,
  ThemeMode.system: Icons.mobile_friendly,
};

final List<(ThemeMode, IconData)> availableThemes =
    _modeIcon.entries.map((e) => (e.key, e.value)).toList();

class ThemeProvider extends ChangeNotifier {
  final _cycleMap = {
    ThemeMode.light: ThemeMode.dark,
    ThemeMode.dark: ThemeMode.system,
    ThemeMode.system: ThemeMode.light,
  };

  ThemeMode _mode;

  final SharedPreferences _prefs;

  ThemeProvider(this._prefs)
      : _mode = _prefsToMode[
            _prefs.getString(PrefsKeys.keyTheme) ?? PrefsThemeKey.system]!;

  ThemeMode get mode => _mode;

  IconData get currentIcon => _modeIcon[_mode]!;

  void update(ThemeMode theme) {
    _update(theme);
  }

  void cycle() {
    _update(_cycleMap[_mode]!);
  }

  void _update(ThemeMode theme) {
    _mode = theme;
    _prefs.setString(PrefsKeys.keyTheme, _modeToPrefs[_mode]!);
    notifyListeners();
  }
}
