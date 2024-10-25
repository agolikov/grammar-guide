import 'package:flutter/foundation.dart';

enum Result {
  failed,
  success,
  empty,
}

class Validator extends ChangeNotifier {

  Result _result = Result.empty;

  Result get result => _result;

  void setCorrectness(bool cor) {
    if (cor) {
      markCorrect();
    } else {
      markFailed();
    }
  }

  void markEmpty() {
    _result = Result.empty;
    notifyListeners();
  }

  void markCorrect() {
    _result = Result.success;
    notifyListeners();
  }

  void markFailed() {
    _result = Result.failed;
    notifyListeners();
  }

}