enum ExerciseKind {
  blank(0),
  singleChoiceTest(1),
  completeSentence(2),
  writeWordToCompleteSentence(3),
  constructSentence(7),
  yesNoQuestion(8),
  enterCorrectWordForm(8),
  ;

  const ExerciseKind(this.exerciseType);

  final int exerciseType;

  factory ExerciseKind._fromString(String value) {
    ExerciseKind? kind = _mapping[value];
    if (kind != null) {
      return kind;
    } else {
      return ExerciseKind.blank;
    }
    // throw Exception("unknown value $value for enum ExerciseKind");
  }

  static Map<String, ExerciseKind> _mapping = {
    'YesNoQuestion': ExerciseKind.yesNoQuestion,
    'PickAnOption': ExerciseKind.singleChoiceTest,
    'TextWithOptions': ExerciseKind.completeSentence,
    'TextWithoutOptions': ExerciseKind.writeWordToCompleteSentence,
    'PickAnOrder': ExerciseKind.constructSentence,
    'EnterTheCorrectWordForm': ExerciseKind.enterCorrectWordForm,
  };
}

enum PartKind {
  text(1),
  rightWord(2),
  pickOneWord(3),
  singleChoiceTestOption(4),
  wordConstructionPart(7),
  ;

  const PartKind(this._numericKind);

  final int _numericKind;

  factory PartKind._fromString(String value) {
    PartKind? kind = _mapping[value];
    if (kind != null) {
      return kind;
    }
    throw Exception("unknown value $value for enum PartKind");
  }

  static Map<String, PartKind> _mapping = {
    'PlainText': PartKind.text,
    'InputText': PartKind.rightWord,
    'InputTextWithOptions': PartKind.pickOneWord,
    'Radio': PartKind.singleChoiceTestOption,
  };
}

class YesNoQuestion extends ActionPart {
  final TextPart text;
  final SingleChoiceTestOption singeChoice;

  YesNoQuestion(super.type, super.imageId, this.text, this.singeChoice);

  factory YesNoQuestion.fromJson(Map<String, dynamic> json) {
    final parts = (json['parts'] as List<dynamic>).map((item) {
      final kind = PartKind._fromString(item['kind']);
      if (kind == PartKind.text) {
        return TextPart(item['text']);
      } else {
        return SingleChoiceTestOption.fromJson(item);
      }
    }).toList();

    return YesNoQuestion(ExerciseKind.yesNoQuestion, json['imageId'],
        parts[0] as TextPart, parts[1] as SingleChoiceTestOption);
  }
}

class SingleChoiceTest extends ActionPart {
  final SingleChoiceTestOption option;

  SingleChoiceTest(String? imageId, this.option)
      : super(ExerciseKind.singleChoiceTest, imageId);

  factory SingleChoiceTest.fromJson(Map<String, dynamic> json) {
    return SingleChoiceTest(
        json['imageId'],
        (json['parts'] as List<dynamic>)
            .where((item) => item['kind'] == 'Radio')
            .map((item) => SingleChoiceTestOption.fromJson(item))
            .first);
  }
}

class WriteWordToCompleteSentence extends ActionPart {
  final String question;
  final List<ExercisePart> parts;

  WriteWordToCompleteSentence(this.question, this.parts, String? imageId)
      : super(ExerciseKind.writeWordToCompleteSentence, imageId);

  factory WriteWordToCompleteSentence.fromJson(Map<String, dynamic> json) {
    return WriteWordToCompleteSentence(
        json['question'],
        (json['parts'] as List<dynamic>).map((item) {
          final kind = PartKind._fromString(item['kind']);
          if (kind case PartKind.text) {
            return TextPart(item['text']);
          } else if (kind case PartKind.rightWord) {
            return RightWordPart(item['correctValue']);
          }
          throw Exception("unexpected kind $kind");
        }).toList(),
        json['imageId']);
  }
}

class CompleteSentence extends ActionPart {
  final String question;
  final List<ExercisePart> parts;

  CompleteSentence(this.question, this.parts, String? imageId)
      : super(ExerciseKind.completeSentence, imageId);

  factory CompleteSentence.fromJson(Map<String, dynamic> json) {
    return CompleteSentence(
      json['question'],
      (json['parts'] as List<dynamic>).map((item) {
        final kind = PartKind._fromString(item['kind']);
        if (kind case PartKind.text) {
          return TextPart(item['text']);
        } else if (kind case PartKind.pickOneWord) {
          return PickOneWordPart(
              List<String>.from(item['options']), item['correctOptionIndex']);
        }
        throw Exception("unexpected kind $kind");
      }).toList(),
      json['imageId'],
    );
  }
}

class ConstructSentence extends ActionPart {
  final List<ExercisePart> parts;

  ConstructSentence(this.parts, String? imageId)
      : super(ExerciseKind.constructSentence, imageId) {
    parts.shuffle();
  }

  factory ConstructSentence.fromJson(Map<String, dynamic> json) {
    return ConstructSentence(
        (json['parts'] as List<dynamic>).indexed.map((item) {
          final kind = PartKind._fromString(item.$2['kind']);
          if (kind case PartKind.pickOneWord) {
            return ConstructionPart(
                item.$2['options'][item.$2['correctOptionIndex']], item.$1);
          }
          throw Exception("unexpected kind $kind");
        }).toList(),
        json['imageId']);
  }
}

class ActionPart {
  final ExerciseKind type;

  final String? imageId;

  ActionPart(this.type, this.imageId);

  static ActionPart fromJson(Map<String, dynamic> json, ExerciseKind kind) {
    switch (kind) {
      case ExerciseKind.completeSentence:
        return CompleteSentence.fromJson(json);
      case ExerciseKind.singleChoiceTest:
        return SingleChoiceTest.fromJson(json);
      case ExerciseKind.writeWordToCompleteSentence:
        return WriteWordToCompleteSentence.fromJson(json);
      case ExerciseKind.constructSentence:
        return ConstructSentence.fromJson(json);
      case ExerciseKind.yesNoQuestion:
        return YesNoQuestion.fromJson(json);
      case ExerciseKind.enterCorrectWordForm:
      // TODO: Handle this case.
      case ExerciseKind.blank:
      // TODO: Handle this case.
    }
    return ActionPart(kind, json['imageId']);
  }
}

class FlatExercise {
  final int index;
  final int unitIndex;
  final String title;
  final String? imageId;
  final ActionPart action;

  FlatExercise(
      this.index, this.unitIndex, this.title, this.imageId, this.action);
}

class Exercise {
  final int index;
  final int unitIndex;
  final String title;
  final List<ActionPart> actionParts;

  Exercise(this.index, this.unitIndex, this.title, this.actionParts);

  factory Exercise.fromJson(Map<String, dynamic> json) {
    return Exercise(
      json['index'],
      json['unitIndex'],
      json['title'] ?? "",
      (json['parts'] as List<dynamic>)
          .map((item) {
            try {
              return ActionPart.fromJson(
                  item, ExerciseKind._fromString(json['type']));
            } catch (e) {
              return null;
            }
          })
          .whereType<ActionPart>()
          .toList(),
    );
  }

  Iterable<FlatExercise> flatten() => actionParts.map((action) =>
      FlatExercise(index, unitIndex, title, action.imageId, action));
}

class ExercisePart {
  final PartKind kind;

  ExercisePart(this.kind);
}

class TextPart extends ExercisePart {
  final String text;

  TextPart(this.text) : super(PartKind.text);
}

class RightWordPart extends ExercisePart {
  final String word;

  RightWordPart(this.word) : super(PartKind.rightWord);
}

class OptionWithCorrectness {
  final String text;
  final bool correct;

  OptionWithCorrectness(this.text, this.correct);
}

class PickOneWordPart extends ExercisePart {
  final List<OptionWithCorrectness> options;

  PickOneWordPart(List<String> items, int correctOptionIndex)
      : options = items.indexed
            .map((item) =>
                OptionWithCorrectness(item.$2, correctOptionIndex == item.$1))
            .toList(),
        super(PartKind.pickOneWord);
}

class SingleChoiceTestOption extends ExercisePart {
  final List<OptionWithCorrectness> options;

  SingleChoiceTestOption(this.options) : super(PartKind.singleChoiceTestOption);

  factory SingleChoiceTestOption.fromJson(Map<String, dynamic> json) {
    int correctOptionIndex = json['correctOptionIndex'];
    final options = List<String>.from(json['options'])
        .indexed
        .map((item) =>
            OptionWithCorrectness(item.$2, correctOptionIndex == item.$1))
        .toList();
    options.shuffle();
    return SingleChoiceTestOption(options);
  }
}

class ConstructionPart extends ExercisePart {
  final String text;
  final int rightIndex;

  ConstructionPart(this.text, this.rightIndex)
      : super(PartKind.wordConstructionPart);
}
