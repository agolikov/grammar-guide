import 'package:flutter/widgets.dart';
import 'package:grammar_guide_client/model/all.dart';

class LanguageSelectionProvider extends ChangeNotifier {
  Language? _source;
  Language? _destination;

  Language get source => _source!;
  Language? get sourceNullable => _source;

  set source(Language val) {
    _source = val;
    notifyListeners();
  }

  Language get destination => _destination!;
  Language? get destinationNullable => _destination;

  set destination(Language val) {
    _destination = val;
    notifyListeners();
  }

  bool get completed => _source != null && _destination != null;
}
