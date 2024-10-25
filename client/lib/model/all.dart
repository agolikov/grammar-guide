import 'dart:core';

import 'package:grammar_guide_client/model/exercise.dart';
import 'package:grammar_guide_client/model/provider/current_guide.dart';

class GuideBrief {
  final String id;
  final String sourceLanguage;
  final String destinationLanguage;
  final String title;

  GuideBrief(this.id, this.sourceLanguage, this.destinationLanguage)
      : title = "$sourceLanguage to $destinationLanguage";

  factory GuideBrief.fromJson(Map<String, dynamic> json) {
    return GuideBrief(
      json['id'],
      json['sourceLanguage'],
      json['destinationLanguage'],
    );
  }
}

class Guide {
  final GuideBrief brief;
  final List<UnitGroup> unitGroups;

  Guide(this.brief, this.unitGroups);

  factory Guide.fromJson(Map<String, dynamic> json) {
    return Guide(
      GuideBrief.fromJson(json),
      (json['unitGroups'] as List)
          .map((unitGroupJson) => UnitGroup.fromJson(unitGroupJson))
          .toList(),
    );
  }
}

class UnitGroup {
  final int id;
  final String title;
  final List<Unit> units;

  UnitGroup(this.id, this.title, this.units);

  factory UnitGroup.fromJson(Map<String, dynamic> json) {
    return UnitGroup(
        json['index'],
        json['title'],
        (json['units'] as List)
            .map((unitJson) => Unit.fromJson(unitJson))
            .toList());
  }
}

class Unit {
  final int index;
  final String guideId;
  final String title;
  final bool completed;

  Unit(this.index, this.guideId, this.title, this.completed);

  factory Unit.fromJson(Map<String, dynamic> json) {
    return Unit(
      json['index'],
      json['guideId'],
      json['title'],
      json['completed'] ?? false,
    );
  }
}

class UnitContent {
  final int index;

  // final String guideId;
  //todo tmp workaround
  final CurrentGuideLangKey langKey;
  final String title;
  final List<Rule> explanations;
  final List<Exercise> exercises;

  UnitContent(
    this.index,
    // this.guideId,
    this.langKey,
    this.title,
    this.explanations,
    this.exercises,
  );

  factory UnitContent.fromJson(
      Map<String, dynamic> json, CurrentGuideLangKey langKey) {
    return UnitContent(
        json['index'],
        // json['guideId'],
        langKey,
        json['title'],
        (json['rules'] as List).map((e) => Rule.fromJson(e)).toList(),
        json['exercises'] == null
            ? List.empty()
            : List<dynamic>.from(json['exercises'])
                .map((e) {
                  final ex = Exercise.fromJson(e);
                  return ex.actionParts.isEmpty ? null : ex;
                })
                .whereType<Exercise>()
                .toList());
  }
}

class Rule {
  final int index;
  final String title;
  final String content;
  final String? example;
  final String? exampleTranslation;
  final String? exampleAudioId;
  final String? exampleTranslationAudioId;
  final String? imageId;

  Rule(
      this.index,
      this.title,
      this.content,
      this.example,
      this.exampleTranslation,
      this.exampleAudioId,
      this.exampleTranslationAudioId,
      [this.imageId]);

  factory Rule.fromJson(Map<String, dynamic> json) {
    return Rule(
      json['index'] as int,
      json['title'] as String,
      json['content'] as String? ?? "",
      json['example'],
      json['exampleTranslation'] as String?,
      json['exampleAudioId'] as String?,
      json['exampleTranslationAudioId'] as String?,
      json['imageId'] as String?, // It's nullable
    );
  }
}

class Explanation {
  final int index;
  final String kind;
  final String text;

  Explanation(this.index, this.kind, this.text);

  factory Explanation.fromJson(Map<String, dynamic> json) {
    return Explanation(
      json['index'],
      json['kind'],
      json['text'],
    );
  }
}

class Language {
  final String langId;
  final String title;
  final String countryCode;

  Language(this.langId, this.title, this.countryCode);

  factory Language.fromJson(Map<String, dynamic> json) =>
      Language(json['id'], json['title'], json['countryCode']);
}

class LanguageResponse {
  final List<Language> source;
  final List<Language> destination;

  LanguageResponse(this.source, this.destination);

  factory LanguageResponse.fromJson(Map<String, dynamic> json) {
    var languages = json['languages'];
    return LanguageResponse(
        List<Map<String, dynamic>>.from(languages)
            .map((lang) => Language.fromJson(lang))
            .toList(),
        List<Map<String, dynamic>>.from(languages)
            .map((lang) => Language.fromJson(lang))
            .toList());
  }
}

class UserProgresses {
  final String guideId;
  final List<int> bookmarks;
  final List<int> completed;

  UserProgresses(this.guideId, this.bookmarks, this.completed);

  factory UserProgresses.fromJson(Map<String, dynamic> json) => UserProgresses(
        json["guideId"],
        List<int>.from(json["bookmarks"].map((v) => v as int)),
        List<int>.from(json["completedUnitIndexes"].map((v) => v as int)),
      );
}

class UserResponse {
  final String id;
  final String name;
  final String theme;
  final int balance;
  final Map<String, UserProgresses> progresses;

  //"progresses":[]}

  UserResponse(this.id, this.name, this.theme, this.balance, this.progresses);

  factory UserResponse.fromJson(Map<String, dynamic> json) => UserResponse(
      json["id"],
      json["name"],
      json["theme"],
      json["balance"],
      json.containsKey("progresses")
          ? {
              for (var it in List<Map<String, dynamic>>.from(json["progresses"])
                  .map((elem) => UserProgresses.fromJson(elem)))
                it.guideId: it
            }
          : <String, UserProgresses>{});
}
