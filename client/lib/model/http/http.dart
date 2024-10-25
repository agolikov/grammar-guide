import 'dart:convert';
import 'dart:typed_data';
import 'package:flutter/cupertino.dart';
import 'package:flutter/foundation.dart';
import 'package:grammar_guide_client/model/provider/current_guide.dart';
import 'package:grammar_guide_client/util/constants.dart';
import 'package:http/http.dart' as http;
import '../all.dart';

final HostDescription _host = Constants.getHost();

const String _contentRoot = 'content';
const String _guidePath = '$_contentRoot/guide';
const String _languagesPath = '$_contentRoot/languages';
const String _user = 'user';
const String _userGuide = '$_user/guide';
const String _bookmark = 'bookmark';

const String _blob = 'blob';
const String _audio = '$_blob/audio';
const String _image = '$_blob/image';

const String _apiKey = "6a66fba8-d8ac-416a-a301-d363206a4636";

Map<String, String> getRegularHeaders() => _headers;

final _headers = {
  'x-api-key': _apiKey,
};

Future<LanguageResponse> fetchLanguages() async {
  var url = _toUri(_languagesPath);
  var response = await http.get(url, headers: _headers);

  if (response.statusCode == 200) {
    var data = response.body;
    return LanguageResponse.fromJson(jsonDecode(data));
  } else {
    print('Request failed with status: ${response.statusCode}');
    throw Exception("exception while receiving guides");
  }
}

Future<Guide> fetchGuide(String sourceId, String destinationId) async {
  var url = _toUri('$_guidePath/$sourceId/$destinationId/structure');
  var response = await http.get(url, headers: _headers);

  if (response.statusCode == 200) {
    var data = response.body;
    return Guide.fromJson(jsonDecode(data));
  } else {
    print('Request failed ${response.statusCode}');
    throw Exception("no guide");
  }
}

Future<UnitContent> fetchUnitContent(
    CurrentGuideLangKey langKey, int unitIndex) async {
  var url = _toUri(
      '$_guidePath/${langKey.sourceId}/${langKey.destinationId}/unit/$unitIndex');
  var response = await http.get(url, headers: _headers);

  if (response.statusCode == 200) {
    var data = response.body;
    return UnitContent.fromJson(jsonDecode(data), langKey);
  } else {
    print('Request failed ${response.statusCode}');
    throw Exception("no unit");
  }
}

Future<void> fetchExercises(
    CurrentGuideLangKey langKey, int unitIndex, int ruleIndex) async {
  var url = _toUri(
      '$_guidePath/${langKey.sourceId}/${langKey.destinationId}/unit/$unitIndex/rule/$ruleIndex/exercise');
  var response = await http.post(url, headers: _headers);
  if (response.statusCode == 200) {
  } else {
    print('Request failed ${response.statusCode}');
  }
}

Future<void> generateImage(
    CurrentGuideLangKey langKey, int unitIndex, int ruleIndex) async {
  var url = _toUri(
      '$_guidePath/${langKey.sourceId}/${langKey.destinationId}/unit/$unitIndex/rule/$ruleIndex/image');
  var response = await http.post(url, headers: _headers);
  if (response.statusCode == 200) {
  } else {
    print('Request failed ${response.statusCode}');
  }
}

Future<bool> setCompleteStatusForExercise(
    String userId, String guideId, int unitIndex, int exerciseIndex, bool isCompleted) async {
  var url = _toUri("$_userGuide/$guideId/unit/$unitIndex/exercise/$exerciseIndex")
      .replace(queryParameters: {'isCompleted': "$isCompleted", "userId": userId});
  var response = await http.put(url, headers: _headers);

  if (response.statusCode == 200) {
    return Future.value(isCompleted);
  } else {
    print('Request failed ${response.statusCode} ${response.body}');
    throw Exception("no guide");
  }
}

Future<bool> setCompleteStatus(
    String userId, String guideId, int unitIndex, bool isCompleted) async {
  var url = _toUri("$_userGuide/$guideId/unit/$unitIndex")
      .replace(queryParameters: {'isCompleted': "$isCompleted", "userId": userId});
  var response = await http.put(url, headers: _headers);

  if (response.statusCode == 200) {
    return Future.value(isCompleted);
  } else {
    print('Request failed ${response.statusCode} ${response.body}');
    throw Exception("no guide");
  }
}

Future<bool> setBookmarkStatus(
    String userId, String guideId, int unitIndex, bool isAdded) async {
  var url = _toUri("$_userGuide/$guideId/$_bookmark/$unitIndex")
      .replace(queryParameters: {'isAdded': "$isAdded", "userId": userId});
  var response = await http.put(url, headers: _headers);

  if (response.statusCode == 200) {
    return Future.value(isAdded);
  } else {
    print('Request failed ${response.statusCode} ${response.body}');
    throw Exception("no guide");
  }
}

Future<Uint8List> downloadAudio(String audioId) async {
  final base = _toUri(_audio);
  final url = base.replace(queryParameters: {'blobId': audioId});

  var response = await http.get(url, headers: _headers);

  if (response.statusCode == 200) {
    return Future.value(response.bodyBytes);
  } else {
    print('Request failed ${response.toString()}');
    throw Exception("no guide");
  }
}

Future<UserResponse> downloadUser(String userId) async {
  final url = _toUri("$_user/$userId");

  final response = await http.get(url, headers: _headers);
  if (response.statusCode == 200) {
    return UserResponse.fromJson(jsonDecode(response.body));
  } else {
    throw Exception("can't create user ${response.body}");
  }
}

Future<UserResponse> createUser(String name) async {
  final base = _toUri(_user);

  final url = base.replace(queryParameters: {
    "userName": name,
  });

  final response = await http.post(url, headers: _headers);

  if (response.statusCode == 200) {
    return UserResponse.fromJson(jsonDecode(response.body));
  } else {
    throw Exception("can't create user ${response.body}");
  }
}

String getImageUrl(String imageId) {
  final base = _toUri(_image);
  return base.replace(queryParameters: {'blobId': imageId}).toString();
}

String getFlagUrl(String countryCode) {
  return _toUri("/content/flag/$countryCode").toString();
}

Uri _toUri(String handle) => Uri.parse('${_host.scheme}://${_host.host}/$handle');

NetworkImage getNetworkImage(String? imageId) =>
    NetworkImage(getImageUrl(imageId!), headers: getRegularHeaders());
