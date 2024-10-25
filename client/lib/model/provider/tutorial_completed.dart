import 'package:flutter/widgets.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../persistence/prefs.dart';

class TutorialCompletedProvider with ChangeNotifier {
  final SharedPreferences _prefs;

  bool get isCompleted =>
      _prefs.getBool(PrefsKeys.keyTutorialCompleted) ?? false;

  TutorialCompletedProvider(this._prefs);

  void markCompleted() {
    _prefs.setBool(PrefsKeys.keyTutorialCompleted, true);
  }
}
