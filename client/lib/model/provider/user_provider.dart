import 'package:flutter/foundation.dart';
import 'package:grammar_guide_client/model/http/http.dart';
import 'package:grammar_guide_client/model/persistence/prefs.dart';
import 'package:shared_preferences/shared_preferences.dart';

class UserProvider with ChangeNotifier {
  final SharedPreferences _prefs;

  bool _load = false;

  late String _userId;

  bool get load => _load;

  String get userId => _userId;

  UserProvider(this._prefs) {
    if (_prefs.containsKey(PrefsKeys.keyUserId)) {
      _load = false;
      _userId = _prefs.getString(PrefsKeys.keyUserId)!;
    } else {
      loadUserId();
      _load = true;
    }
  }

  void loadUserId() async {
    final user = await createUser("stubName");
    _userId = user.id;
    _prefs.setString(PrefsKeys.keyUserId, _userId);
    _load = false;
    notifyListeners();
  }
}
