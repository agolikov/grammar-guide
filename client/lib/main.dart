import 'package:flutter_gen/gen_l10n/app_localizations.dart';
import 'package:grammar_guide_client/model/provider/current_guide.dart';
import 'package:grammar_guide_client/model/provider/theme_provider.dart';
import 'package:grammar_guide_client/model/provider/tutorial_completed.dart';
import 'package:grammar_guide_client/model/provider/user_data_provider.dart';
import 'package:grammar_guide_client/model/provider/user_provider.dart';
import 'package:grammar_guide_client/view/screen/bookmarks_screen.dart';
import 'package:grammar_guide_client/view/screen/guide_choose.dart';
import 'package:grammar_guide_client/view/screen/tutorial_screen.dart';
import 'package:grammar_guide_client/view/screen/unit_group_list.dart';
import 'package:grammar_guide_client/view/screen/unit_group.dart';
import 'package:provider/provider.dart';
import 'package:shared_preferences/shared_preferences.dart';

import 'view/screen/routes.dart';
import 'view/screen/unit.dart';
import 'package:flutter/material.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  final prefs = (await SharedPreferences.getInstance());
  runApp(MultiProvider(providers: [
    ChangeNotifierProvider(create: (_) => CurrentGuide(prefs)),
    ChangeNotifierProvider(create: (_) => TutorialCompletedProvider(prefs)),
    ChangeNotifierProvider(create: (_) => ThemeProvider(prefs)),
    ChangeNotifierProvider(create: (_) => UserProvider(prefs)),
    ChangeNotifierProxyProvider2<UserProvider, CurrentGuide, UserDataProvider>(
        create: (context) => UserDataProvider(context.read(), context.read()),
        update: (_, userProv, guideProv, result) {
          if (!userProv.load) {
            result!.update(
                userProv.userId, guideProv.sourceId, guideProv.destinationId);
          }
          return result!;
        }),
  ], child: const MyApp()));
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return Consumer<ThemeProvider>(
        builder: (BuildContext context, ThemeProvider value, Widget? child) {
      return MaterialApp(
        title: 'Grammar Guide',
        theme: ThemeData(
          colorScheme: ColorScheme.fromSeed(seedColor: Colors.yellow),
          useMaterial3: true,
          brightness: Brightness.light,
        ),
        darkTheme: ThemeData(
          colorScheme: ColorScheme.fromSeed(
              seedColor: Colors.orange, brightness: Brightness.dark),
          brightness: Brightness.dark,
          useMaterial3: true,
        ),
        themeMode: value.mode,
        home: context.read<TutorialCompletedProvider>().isCompleted
            ? const UnitGroupListPage()
            : const OnboardingScreen(),
        routes: {
          Routes.tutorial: (context) => const OnboardingScreen(),
          Routes.bookmarks: (context) => const BookmarksScreen(),
          Routes.guideDetailed: (context) => const UnitGroupPage(),
          Routes.guide: (context) => const UnitGroupListPage(),
          Routes.unit: (context) => const UnitScreen(),
          Routes.guideSelection: (context) => const GuideChooseScreen(),
        },
        localizationsDelegates: AppLocalizations.localizationsDelegates,
        supportedLocales: AppLocalizations.supportedLocales,
      );
    });
  }
}
