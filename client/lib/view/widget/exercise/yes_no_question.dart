import 'package:flutter/cupertino.dart';
import 'package:grammar_guide_client/model/exercise.dart';
import 'package:grammar_guide_client/view/widget/exercise/single_choice_test.dart';

class YesNoQuestionWidget extends StatefulWidget {
  final YesNoQuestion question;

  const YesNoQuestionWidget(this.question, {super.key});

  @override
  State<YesNoQuestionWidget> createState() => _YesNoQuestionWidgetState();
}

class _YesNoQuestionWidgetState extends State<YesNoQuestionWidget> {
  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Text(
          widget.question.text.text,
          style: const TextStyle(fontSize: 20),
        ),
        SingleChoiceTestWidget(widget.question.singeChoice),
      ],
    );
  }
}
