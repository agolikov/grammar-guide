import 'package:grammar_guide_client/model/all.dart';
import 'package:grammar_guide_client/model/persistence/prefs.dart';
import 'package:flutter/foundation.dart';
import 'package:shared_preferences/shared_preferences.dart';

class CurrentGuideLangKey {
  final String sourceId;
  final String destinationId;

  CurrentGuideLangKey(this.sourceId, this.destinationId);
}

class CurrentGuide with ChangeNotifier {
  final SharedPreferences _prefs;

  String get sourceId => _prefs.getString(PrefsKeys.keyCurrentGuideSource)!;

  String get destinationId =>
      _prefs.getString(PrefsKeys.keyCurrentGuideDestination)!;

  CurrentGuideLangKey get langKey =>
      CurrentGuideLangKey(sourceId, destinationId);

  bool get complete =>
      _prefs.containsKey(PrefsKeys.keyCurrentGuideSource) &&
      _prefs.containsKey(PrefsKeys.keyCurrentGuideDestination);

  CurrentGuide(this._prefs);

  void update(Language source, Language destination) async {
    updateByIds(source.langId, destination.langId);
    notifyListeners();
  }

  void updateByIds(String sourceId, String destinationId) async {
    _prefs.setString(PrefsKeys.keyCurrentGuideSource, sourceId);
    _prefs.setString(PrefsKeys.keyCurrentGuideDestination, destinationId);
  }
}
