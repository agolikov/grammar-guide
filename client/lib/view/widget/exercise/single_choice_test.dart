import 'package:flutter/material.dart';
import 'package:grammar_guide_client/model/exercise.dart';
import 'package:grammar_guide_client/view/widget/exercise/validator.dart';
import 'package:provider/provider.dart';

class SingleChoiceTestWidget extends StatefulWidget {
  final SingleChoiceTestOption test;

  const SingleChoiceTestWidget(this.test, {super.key});

  @override
  State<SingleChoiceTestWidget> createState() => _SingleChoiceTestWidgetState();
}

class _SingleChoiceTestWidgetState extends State<SingleChoiceTestWidget> {
  int _selectedOption = -1;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    Validator validator = context.read<Validator>();
    return Padding(
        padding: const EdgeInsets.only(top: 10, bottom: 10),
        child: SingleChildScrollView(
            child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: widget.test.options.indexed.map((option) {
            return Column(children: [
              InkWell(
                onTap: () {
                  setState(() {
                    _selectedOption = option.$1;
                    validator.setCorrectness(
                        widget.test.options[_selectedOption].correct);
                  });
                },
                child: Container(
                  padding: const EdgeInsets.symmetric(
                      vertical: 12.0, horizontal: 16.0),
                  decoration: BoxDecoration(
                    border: Border(
                      bottom: BorderSide(color: Colors.grey.shade300),
                    ),
                    color: _selectedOption == option.$1
                        ? theme.colorScheme.primary.withOpacity(0.5)
                        : theme.splashColor,
                  ),
                  child: Row(
                    children: [
                      Text(
                        String.fromCharCode('A'.codeUnitAt(0) + option.$1),
                        style: const TextStyle(
                          fontSize: 18.0,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      const SizedBox(width: 12.0),
                      Flexible(
                          child: Text(
                        option.$2.text,
                        softWrap: true,
                        style: const TextStyle(
                          fontSize: 18.0,
                          fontWeight: FontWeight.normal,
                        ),
                      )),
                    ],
                  ),
                ),
              ),
              if (option.$1 != widget.test.options.length - 1)
                const SizedBox(
                  height: 10,
                )
            ]);
          }).toList(),
        )));
  }
}
