import 'package:flutter/material.dart';
import 'package:grammar_guide_client/model/exercise.dart';
import 'package:grammar_guide_client/view/widget/exercise/validator.dart';
import 'package:provider/provider.dart';

class ConstructSentenceWidget extends StatefulWidget {
  final ConstructSentence exercise;

  const ConstructSentenceWidget(this.exercise, {super.key});

  @override
  State<ConstructSentenceWidget> createState() =>
      _ConstructSentenceWidgetState();
}

class _ConstructSentenceWidgetState extends State<ConstructSentenceWidget> {
  late List<ConstructionPart> _answer;
  late List<ConstructionPart> _bottomList;

  @override
  void initState() {
    super.initState();
    _bottomList = List.of(widget.exercise.parts as List<ConstructionPart>);
    _answer = List<ConstructionPart>.empty(growable: true);
  }

  @override
  Widget build(BuildContext context) {
    Validator validator = context.read<Validator>();
    return Column(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Wrap(
          alignment: WrapAlignment.start,
          children: _answer.indexed
              .map((part) => Padding(
                  padding: const EdgeInsets.only(left: 10, right: 10),
                  child: ElevatedButton(
                      onPressed: () {
                        _bottomList.add(part.$2);
                        setState(() {
                          _answer.removeAt(part.$1);
                          validator.markEmpty();
                        });
                      },
                      child: Text(part.$2.text))))
              .toList(),
        ),
        const Divider(),
        Wrap(
          alignment: WrapAlignment.start,
          runAlignment: WrapAlignment.start,
          verticalDirection: VerticalDirection.up,
          children: _bottomList.indexed
              .map((part) => Padding(
                  padding: const EdgeInsets.only(left: 10, right: 10),
                  child: ElevatedButton(
                      onPressed: () {
                        _answer.add(part.$2);
                        setState(() {
                          _bottomList.removeAt(part.$1);
                          if (_bottomList.isEmpty) {
                            validator.setCorrectness(_answer
                                .asMap()
                                .entries
                                .where((ent) => ent.key != ent.value.rightIndex)
                                .isEmpty);
                          }
                        });
                      },
                      child: Text(part.$2.text))))
              .toList(),
        ),
      ],
    );
  }
}
