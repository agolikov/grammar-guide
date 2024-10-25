import 'package:flutter/material.dart';
import 'package:grammar_guide_client/model/exercise.dart';
import 'package:grammar_guide_client/view/widget/exercise/validator.dart';
import 'package:provider/provider.dart';

class WriteWordToCompleteSentenceWidget extends StatefulWidget {
  final WriteWordToCompleteSentence exercise;

  const WriteWordToCompleteSentenceWidget(this.exercise, {super.key});

  @override
  State<WriteWordToCompleteSentenceWidget> createState() =>
      _WriteWordToCompleteSentenceWidgetState();
}

class _WriteWordToCompleteSentenceWidgetState
    extends State<WriteWordToCompleteSentenceWidget> {
  @override
  Widget build(BuildContext context) {
    Validator validator = context.read<Validator>();
    return Wrap(
      direction: Axis.horizontal,
      crossAxisAlignment: WrapCrossAlignment.center,
      children: widget.exercise.parts.map((part) {
        if (PartKind.text == part.kind) {
          return Text(
            (part as TextPart).text,
            style: const TextStyle(fontSize: 16),
          );
        } else {
          return Container(
            width: 150,
            padding: const EdgeInsets.only(left: 10, right: 10),
            child: TextField(onChanged: (txt) {
              if (txt.isEmpty) {
                validator.markEmpty();
              } else {
                validator.setCorrectness(txt == (part as RightWordPart).word);
              }
            }),
          );
        }
      }).toList(),
    );
  }
}
