import 'package:grammar_guide_client/model/provider/current_guide.dart';
import 'package:grammar_guide_client/model/provider/language_selection.dart';
import 'package:grammar_guide_client/view/screen/language_pick.dart';
import 'package:grammar_guide_client/view/screen/util.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import 'routes.dart';

class GuideChooseScreen extends StatefulWidget {
  final String title = 'Select your Grammar Guide';

  const GuideChooseScreen({super.key});

  @override
  State<StatefulWidget> createState() => _GuideChooseState();
}

class _GuideChooseState extends State<GuideChooseScreen> {
  @override
  Widget build(BuildContext context) {
    return MultiProvider(
        providers: [
          ChangeNotifierProvider(create: (context) {
            final LanguageChooseModel model = LanguageChooseModel();
            model.loadData();
            return model;
          }),
          ChangeNotifierProvider(
              create: (context) => LanguageSelectionProvider())
        ],
        child: Scaffold(
            appBar: AppBar(
              backgroundColor: getAppBarBackgroundColor(context),
              title: Text(widget.title),
              bottom: PreferredSize(
                preferredSize: const Size.fromHeight(10),
                child: Consumer<LanguageChooseModel>(
                    builder: (context, value, child) {
                  if (value.load) {
                    return const LinearProgressIndicator();
                  } else {
                    return const SizedBox.shrink();
                  }
                }),
              ),
            ),
            body:
                Consumer<LanguageChooseModel>(builder: (context, value, child) {
              return value.load
                  ? const SizedBox.shrink()
                  : Column(
                      children: [
                        const LanguagePickWidget(),
                        Padding(
                            padding: const EdgeInsets.all(16),
                            child: Center(child:
                                Consumer<LanguageSelectionProvider>(
                                    builder: (context, langChoose, child) {
                              return ElevatedButton(
                                  onPressed: langChoose.completed
                                      ? () {
                                          context.read<CurrentGuide>().update(
                                              langChoose.source,
                                              langChoose.destination);
                                          Navigator.pushNamedAndRemoveUntil(
                                              context,
                                              Routes.guide,
                                              (r) => false);
                                        }
                                      : null,
                                  child: const Text("Select"));
                            }))),
                      ],
                    );
            })));
  }
}
