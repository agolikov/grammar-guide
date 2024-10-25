import 'package:flutter/widgets.dart';
import 'package:grammar_guide_client/model/http/http.dart';
import 'package:grammar_guide_client/model/all.dart';
import 'package:grammar_guide_client/model/provider/current_guide.dart';
import 'package:grammar_guide_client/model/provider/user_provider.dart';
import 'package:quiver/collection.dart';

class UserDataProvider with ChangeNotifier {
  bool _load = true;

  bool get load => _load;

  late UserResponse userResponse;

  UserDataProvider(UserProvider userProv, CurrentGuide guideProv) {
    if (!userProv.load && guideProv.complete) {
      update(userProv.userId, guideProv.sourceId, guideProv.destinationId);
    }
  }

  TreeSet<Unit> _bookmarked = TreeSet();
  TreeSet<Unit> _completed = TreeSet();

  Set<Unit> get units => _bookmarked;

  void update(String userId, String sourceId, String destinationId) async {
    final userFut = downloadUser(userId);
    final guideFut = fetchGuide(sourceId, destinationId);
    userResponse = await userFut;
    final guide = await guideFut;

    final progress = userResponse.progresses[guide.brief.id];

    _bookmarked = createSet(guide, progress?.bookmarks);
    _completed = createSet(guide, progress?.completed);

    _load = false;
    notifyListeners();
  }

  TreeSet<Unit> createSet(Guide guide, List<int>? items) {
    final Set<int> responseSet = Set.of(items ?? List.empty());
    final List<Unit> tmp = List.empty(growable: true);
    for (var value in guide.unitGroups) {
      for (final unit in value.units) {
        if (responseSet.contains(unit.index)) {
          tmp.add(unit);
        }
      }
    }
    final returnSet =
        TreeSet(comparator: (Unit a, Unit b) => a.index.compareTo(b.index));
    returnSet.addAll(tmp);
    return returnSet;
  }

  void changeBookmarkStatus(Unit unit, bool status) {
    if (status) {
      _bookmarked.add(unit);
    } else {
      _bookmarked.remove(unit);
    }
    notifyListeners();
  }

  void changeCompleteStatus(Unit unit, bool status) {
    if (status) {
      _completed.add(unit);
    } else {
      _completed.remove(unit);
    }
    notifyListeners();
  }

  bool isBookmarked(Unit unit) {
    return _bookmarked.contains(unit);
  }

  bool isCompleted(Unit unit) {
    return _completed.contains(unit);
  }
}
