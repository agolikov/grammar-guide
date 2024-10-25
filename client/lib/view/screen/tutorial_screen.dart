import 'package:grammar_guide_client/model/provider/current_guide.dart';
import 'package:grammar_guide_client/model/provider/language_selection.dart';
import 'package:grammar_guide_client/model/provider/theme_provider.dart';
import 'package:grammar_guide_client/model/provider/tutorial_completed.dart';
import 'package:grammar_guide_client/view/screen/language_pick.dart';
import 'package:grammar_guide_client/view/screen/util.dart';
import 'package:flutter/material.dart';
import 'package:flutter_onboarding_slider/flutter_onboarding_slider.dart';
import 'package:provider/provider.dart';

import 'routes.dart';

class OnboardingScreen extends StatefulWidget {
  const OnboardingScreen({super.key});

  @override
  State<StatefulWidget> createState() => _OnboardingScreenState();
}

class _OnboardingScreenState extends State<OnboardingScreen> {
  ThemeMode _selectedTheme = ThemeMode.system;

  @override
  Widget build(BuildContext context) {
    return MultiProvider(
        providers: [
          ChangeNotifierProvider(create: (_) => LanguageSelectionProvider())
        ],
        builder: (context, child) => PopScope(
            canPop: false,
            child: OnBoardingSlider(
              headerBackgroundColor: Colors.white,
              finishButtonText: 'Start Learning!',
              onFinish: () {
                final langProvider = context.read<LanguageSelectionProvider>();
                if (langProvider.completed) {
                  context.read<TutorialCompletedProvider>().markCompleted();
                  context
                      .read<CurrentGuide>()
                      .update(langProvider.source, langProvider.destination);
                  Navigator.pushNamedAndRemoveUntil(
                    context,
                    Routes.guide,
                    (k) => false,
                  );
                } else {
                  ScaffoldMessenger.of(context).showSnackBar(const SnackBar(
                    content: Text("No guide selected!"),
                  ));
                }
              },
              finishButtonStyle: const FinishButtonStyle(
                backgroundColor: Colors.black,
              ),
              skipTextButton: const Text('Skip'),
              trailing: const Text('Login'),
              background: [
                Container(
                  color: getAppBarBackgroundColor(context),
                ),
                Container(
                  color: getAppBarBackgroundColor(context),
                ),
                Container(
                  color: getAppBarBackgroundColor(context),
                ),
              ],
              totalPage: 3,
              speed: 1.8,
              pageBodies: [
                Container(
                  padding: const EdgeInsets.symmetric(horizontal: 40),
                  child: const Column(
                    children: <Widget>[
                      SizedBox(
                        height: 480,
                      ),
                      Text(
                          'Welcome to our grammar Guide! Please choose you language set!'),
                    ],
                  ),
                ),
                Column(children: [
                  const Padding(
                    padding: EdgeInsets.all(16),
                    child: Text("Choose app theme"),
                  ),
                  Expanded(
                      child: ListView(
                    children: availableThemes
                        .map((theme) => RadioListTile(
                              value: theme.$1,
                              groupValue: _selectedTheme,
                              onChanged: (k) {
                                context.read<ThemeProvider>().update(theme.$1);
                                setState(() {
                                  _selectedTheme = theme.$1;
                                });
                              },
                              title: Row(children: [
                                Icon(theme.$2),
                                const SizedBox(width: 16),
                                Text(theme.$1.name)
                              ]),
                            ))
                        .toList(),
                  ))
                ]),
                Column(children: [
                  const LanguagePickWidget(),
                  Container(
                    height: 90,
                  )
                ]),
              ],
            )));
  }
}
