import 'package:flutter/material.dart';
import 'package:grammar_guide_client/model/exercise.dart';
import 'package:grammar_guide_client/view/widget/exercise/validator.dart';
import 'package:provider/provider.dart';

class CompleteSentenceWidget extends StatefulWidget {
  final CompleteSentence exercise;

  const CompleteSentenceWidget(this.exercise, {super.key});

  @override
  State<CompleteSentenceWidget> createState() => _CompleteSentenceWidgetState();
}

class _CompleteSentenceWidgetState extends State<CompleteSentenceWidget> {
  OptionWithCorrectness? _current;

  @override
  Widget build(BuildContext context) {
    Validator validator = context.read<Validator>();
    return Wrap(
      direction: Axis.horizontal,
      crossAxisAlignment: WrapCrossAlignment.center,
      children: widget.exercise.parts.map((part) {
        if (PartKind.text == part.kind) {
          return Text((part as TextPart).text);
        } else {
          return DropdownButton<OptionWithCorrectness>(
              padding: const EdgeInsets.only(left: 5, right: 5),
              value: _current,
              hint: const Text("pick the right word"),
              onChanged: (nv) {
                if (nv!.correct) {
                  validator.markCorrect();
                } else {
                  validator.markFailed();
                }
                setState(() {
                  _current = nv;
                });
              },
              items: (part as PickOneWordPart)
                  .options
                  .map((value) => DropdownMenuItem(
                        value: value,
                        child: Text(value.text),
                      ))
                  .toList());
        }
      }).toList(),
    );
  }
}
