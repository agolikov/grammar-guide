import 'package:audioplayers/audioplayers.dart';
import 'package:flutter_gen/gen_l10n/app_localizations.dart';
import 'package:grammar_guide_client/model/exercise.dart';
import 'package:grammar_guide_client/model/http/http.dart';
import 'package:grammar_guide_client/model/provider/current_guide.dart';
import 'package:grammar_guide_client/model/provider/user_data_provider.dart';
import 'package:grammar_guide_client/model/provider/user_provider.dart';
import 'package:grammar_guide_client/view/widget/exercise/yes_no_question.dart';
import 'package:grammar_guide_client/view/widget/exercise/complete_sentence_widget.dart';
import 'package:grammar_guide_client/view/widget/exercise/construct_sentence.dart';
import 'package:grammar_guide_client/view/widget/exercise/single_choice_test.dart';
import 'package:grammar_guide_client/view/widget/exercise/validator.dart';
import 'package:grammar_guide_client/view/widget/exercise/write_word_to_complete_sentence.dart';
import 'package:markdown_widget/widget/all.dart';

import '../../model/all.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import 'util.dart';

final Map<Result, MaterialColor> _colorMap = {
  Result.empty: Colors.grey,
  Result.success: Colors.green,
  Result.failed: Colors.red,
};

class UnitScreen extends StatefulWidget {
  const UnitScreen({super.key});

  @override
  State<UnitScreen> createState() => _UnitScreenState();
}

class _UnitScreenState extends State<UnitScreen> with TickerProviderStateMixin {
  late TabController _tabController;
  int _currentPageIndex = 0;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 3, vsync: this);
  }

  @override
  void dispose() {
    super.dispose();
    _tabController.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final unit = ModalRoute.of(context)!.settings.arguments as Unit;
    final userProvider = context.read<UserProvider>();
    final currentGuide = context.read<CurrentGuide>();
    return ChangeNotifierProvider(
        create: (context) {
          final prov = _UnitScreenModel(unit);
          prov.fetchData(currentGuide.langKey);
          return prov;
        },
        child: Scaffold(
            appBar: AppBar(
              backgroundColor: getAppBarBackgroundColor(context),
              title: Text(unit.title),
              actions: [
                Consumer<_UnitScreenModel>(
                  builder: (context, value, child) => value._load
                      ? const SizedBox.shrink()
                      : IconButton(
                          onPressed: () {
                            value.fetchData(
                                context.read<CurrentGuide>().langKey);
                          },
                          icon: const Icon(Icons.refresh)),
                ),
                Consumer2<UserDataProvider, _UnitScreenModel>(
                    builder: (contextUser, valueUser, valueScreen, childUser) =>
                        valueUser.load || valueScreen.load
                            ? const SizedBox.shrink()
                            : IconButton(
                                onPressed: () {
                                  final newStatus =
                                      !valueUser.isCompleted(unit);
                                  valueScreen.complete(
                                      userProvider.userId, newStatus);
                                  valueUser.changeCompleteStatus(
                                      unit, newStatus);
                                },
                                icon: valueUser.isCompleted(unit)
                                    ? const Icon(Icons.check_box)
                                    : const Icon(
                                        Icons.check_box_outline_blank))),
                Consumer2<UserDataProvider, _UnitScreenModel>(
                    builder: (contextUser, valueUser, valueScreen, childUser) =>
                        valueUser.load || valueScreen.load
                            ? const SizedBox.shrink()
                            : IconButton(
                                onPressed: () {
                                  final newStatus =
                                      !valueUser.isBookmarked(unit);
                                  valueScreen.bookmark(
                                      userProvider.userId, newStatus);
                                  valueUser.changeBookmarkStatus(
                                      unit, newStatus);
                                },
                                icon: valueUser.isBookmarked(unit)
                                    ? const Icon(Icons.bookmark)
                                    : const Icon(Icons.bookmark_add_outlined))),
              ],
              bottom: PreferredSize(
                preferredSize: const Size.fromHeight(10),
                child: Consumer<_UnitScreenModel>(
                    builder: (context, value, child) {
                  if (value.load) {
                    return const LinearProgressIndicator();
                  } else {
                    return const SizedBox.shrink();
                  }
                }),
              ),
            ),
            body: Consumer<_UnitScreenModel>(
                builder: (context, value, child) => value.load
                    ? const SizedBox.shrink()
                    : IndexedStack(index: _currentPageIndex, children: <Widget>[
                        _ExplanationPage(value.unitContent.explanations),
                        if (value.unitContent.exercises.isNotEmpty)
                          // ? List.empty()
                          _ExercisesPage(value.unitContent.exercises
                              .expand((e) => e.flatten())
                              .toList())
                      ])),
            bottomNavigationBar: Consumer<_UnitScreenModel>(
              builder: (context, value, child) =>
                  value.load || value.unitContent.exercises.isEmpty
                      ? const SizedBox.shrink()
                      : BottomNavigationBar(
                          currentIndex: _currentPageIndex,
                          onTap: (index) {
                            setState(() {
                              _currentPageIndex = index;
                            });
                          },
                          items: const [
                              BottomNavigationBarItem(
                                icon: Icon(Icons.text_fields),
                                label: 'Explanations',
                              ),
                              BottomNavigationBarItem(
                                icon: Icon(Icons.checklist),
                                label: 'Exercises',
                              ),
                            ]),
            )));
  }
}

class _ExplanationPage extends StatefulWidget {
  final List<Rule> explanations;

  const _ExplanationPage(this.explanations);

  @override
  State<_ExplanationPage> createState() => _ExplanationPageState();
}

abstract class _GeneratorNotifier with ChangeNotifier {
  bool _load = false;

  final CurrentGuideLangKey langKey;
  final _UnitScreenModel unitModel;
  final int ruleIndex;

  _GeneratorNotifier(this.langKey, this.unitModel, this.ruleIndex);

  bool get load => _load;

  void runRequest() async {
    _load = true;
    notifyListeners();
    await _doWork();
    _load = false;
    notifyListeners();
  }

  Future<void> _doWork();
}

class _CreateExerciseNotifier extends _GeneratorNotifier {
  _CreateExerciseNotifier(super.langKey, super.unitModel, super.ruleIndex);

  @override
  Future<void> _doWork() async =>
      fetchExercises(langKey, unitModel._unit.index, ruleIndex);
}

class _CreateImageNotifier extends _GeneratorNotifier {
  _CreateImageNotifier(super.langKey, super.unitModel, super.ruleIndex);

  @override
  Future<void> _doWork() async =>
      generateImage(langKey, unitModel._unit.index, ruleIndex);
}

class _ExplanationPageState extends State<_ExplanationPage> {
  String? _playedAudioAid;

  Icon getIconForAudioPlayerButton(String? id) {
    return Icon(id == _playedAudioAid ? Icons.pause : Icons.play_arrow);
  }

  Widget getExample(AudioPlayer audioPlayer, String example, String? audioId,
      String sectionTitle) {
    return Column(children: [
      Align(alignment: Alignment.centerLeft, child: Text(sectionTitle)),
      Row(
        children: [
          if (audioId != null)
            Padding(
                padding: const EdgeInsets.only(right: 10),
                child: ElevatedButton(
                  onPressed: () async {
                    setState(() {
                      _playedAudioAid = audioId;
                    });

                    await audioPlayer
                        .play(BytesSource(await downloadAudio(audioId)));
                    setState(() {
                      _playedAudioAid = null;
                    });
                  },
                  child: getIconForAudioPlayerButton(audioId),
                )),
          Expanded(
              child: MarkdownBlock(
            data: example,
          ))
        ],
      )
    ]);
  }

  @override
  Widget build(BuildContext context) {
    final audioPlayer = AudioPlayer();
    return ListView(
      children: widget.explanations
          .map((rule) => ListTile(
              title: MarkdownBlock(
                data: rule.title,
              ),
              subtitle: Column(children: [
                Row(
                  children: [
                    Expanded(
                        child: Padding(
                      padding: const EdgeInsets.all(16),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: <Widget>[
                          MarkdownBlock(
                            data: rule.content,
                          ),
                          if (rule.example != null)
                            getExample(
                                audioPlayer,
                                rule.example!,
                                rule.exampleAudioId,
                                AppLocalizations.of(context)!.exampleText),
                          if (rule.exampleTranslation != null)
                            getExample(
                                audioPlayer,
                                rule.exampleTranslation!,
                                rule.exampleTranslationAudioId,
                                AppLocalizations.of(context)!
                                    .exampleTranslationText),
                          Padding(
                              padding: const EdgeInsets.all(16),
                              child: Column(
                                children: [
                                  ChangeNotifierProvider<
                                      _CreateExerciseNotifier>(
                                    create: (context) =>
                                        _CreateExerciseNotifier(
                                            context
                                                .read<CurrentGuide>()
                                                .langKey,
                                            context.read<_UnitScreenModel>(),
                                            rule.index),
                                    child: Consumer<_CreateExerciseNotifier>(
                                      builder: (context, value, child) {
                                        return value._load
                                            ? const CircularProgressIndicator()
                                            : ElevatedButton(
                                                onPressed: () {
                                                  value.runRequest();
                                                },
                                                child: const Text(
                                                    "Generate exercise"));
                                      },
                                    ),
                                  ),
                                  ChangeNotifierProvider<_CreateImageNotifier>(
                                    create: (context) => _CreateImageNotifier(
                                        context.read<CurrentGuide>().langKey,
                                        context.read<_UnitScreenModel>(),
                                        rule.index),
                                    child: Consumer<_CreateImageNotifier>(
                                      builder: (context, value, child) {
                                        return value._load
                                            ? const CircularProgressIndicator()
                                            : ElevatedButton(
                                                onPressed: () {
                                                  value.runRequest();
                                                },
                                                child: const Text(
                                                    "Generate image"));
                                      },
                                    ),
                                  ),
                                ],
                              )),
                        ],
                      ),
                    )),
                  ],
                ),
                if (rule.imageId != null)
                  Image(
                    image: getNetworkImage(rule.imageId),
                  ),
              ])))
          .toList(),
    );
  }
}

class _ExercisesPage extends StatefulWidget {
  final List<FlatExercise> exercises;

  _ExercisesPage(this.exercises);

  @override
  State<_ExercisesPage> createState() => _ExercisesPageState();
}

class _ExercisesPageState extends State<_ExercisesPage>
    with SingleTickerProviderStateMixin {
  late List<Result> _status;
  late TabController _tabController;

  bool _completed = false;

  @override
  void initState() {
    super.initState();
    _tabController =
        TabController(length: widget.exercises.length, vsync: this);
    _status = _generateBlankResults();
  }

  List<Result> _generateBlankResults() =>
      List.generate(widget.exercises.length, (index) => Result.empty);

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  void _nextPage(Result currentResult) {
    _status[_tabController.index] = currentResult;
    if (_tabController.index < _status.length - 1) {
      _tabController.animateTo(_tabController.index + 1);
    }
    setState(() {
      if (_status.where((item) => item == Result.empty).isEmpty) {
        _completed = true;
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    return _completed ? buildResultView() : buildTestView();
  }

  Stack buildTestView() {
    return Stack(
      children: [
        Align(
          alignment: Alignment.topCenter,
          child: TabBarView(
            controller: _tabController,
            children: widget.exercises.indexed
                .map((item) => _TestPage(item.$2, _nextPage, item.$1))
                .toList(),
          ),
        ),
        Align(
            alignment: Alignment.bottomCenter,
            child: TabBar(
              controller: _tabController,
              tabAlignment: TabAlignment.center,
              isScrollable: true,
              labelPadding: const EdgeInsets.only(left: 1, right: 1),
              tabs: List.generate(
                  widget.exercises.length,
                  (index) => Tab(
                      iconMargin: EdgeInsets.zero,
                      icon: DecoratedBox(
                        decoration: BoxDecoration(
                            borderRadius: BorderRadius.circular(4),
                            color: _colorMap[_status[index]]),
                        child: SizedBox(
                            width: 50,
                            child: Center(
                                child: Text(
                              "${index + 1}",
                              style: const TextStyle(fontSize: 20),
                            ))),
                      ))),
            )),
      ],
    );
  }

  Container buildResultView() {
    return Container(
        padding: const EdgeInsets.all(10.0),
        child: ListView(
            children: <Widget>[
                  ListTile(
                    title: const Text(
                      'Result',
                      style: TextStyle(fontSize: 25),
                    ),
                    subtitle: Text(
                        '${_status.where((item) => item == Result.success).length}/${_status.length}',
                        style: const TextStyle(fontSize: 20)),
                  )
                ] +
                _status.indexed
                    .map((item) => ListTile(
                          title: Text('Question ${item.$1 + 1}'),
                          subtitle: Text(
                            item.$2.name,
                            style: TextStyle(
                              color: _colorMap[item.$2],
                            ),
                          ),
                        ))
                    .toList() +
                <Widget>[
                  InkWell(
                      onTap: () {
                        setState(() {
                          _completed = false;
                          _status = _generateBlankResults();
                          _tabController.animateTo(0);
                        });
                      },
                      child: const ListTile(
                        title: Center(
                            child: Text(
                          'Retake',
                          style: TextStyle(fontSize: 25),
                        )),
                      )),
                ]));
  }
}

class _UnitScreenModel with ChangeNotifier {
  final Unit _unit;

  bool _load = true;
  UnitContent? _content;

  UnitContent get unitContent => _content!;

  bool get load => _load;

  bool get hasUnit => _content != null;

  _UnitScreenModel(this._unit);

  void fetchData(CurrentGuideLangKey langKey) async {
    _load = true;
    notifyListeners();
    _content = await fetchUnitContent(langKey, _unit.index);
    _load = false;
    notifyListeners();
  }

  void bookmark(String userId, bool marked) async {
    if (!_load) {
      await setBookmarkStatus(userId, _unit.guideId, _unit.index, marked);
      notifyListeners();
    }
  }

  void complete(String userId, bool marked) async {
    if (!_load) {
      await setCompleteStatus(userId, _unit.guideId, _unit.index, marked);
      notifyListeners();
    }
  }

  void completeExercise(String userId, int exerciseIndex) async {
    if (!load) {
      await setCompleteStatusForExercise(
          userId, _unit.guideId, _unit.index, exerciseIndex, true);
    }
  }
}

class _TestPage extends StatefulWidget {
  final FlatExercise _exercise;
  final void Function(Result) _submit;
  final int _index;

  const _TestPage(this._exercise, this._submit, this._index);

  @override
  State<_TestPage> createState() => _TestPageState();
}

class _TestPageState extends State<_TestPage>
    with AutomaticKeepAliveClientMixin {
  Widget _buildWidgetForExercise(FlatExercise exercise) {
    switch (exercise.action.type) {
      case ExerciseKind.completeSentence:
        return CompleteSentenceWidget(exercise.action as CompleteSentence);
      case ExerciseKind.yesNoQuestion:
        return YesNoQuestionWidget(exercise.action as YesNoQuestion);
      case ExerciseKind.singleChoiceTest:
        return SingleChoiceTestWidget(
            (exercise.action as SingleChoiceTest).option);
      case ExerciseKind.writeWordToCompleteSentence:
        return WriteWordToCompleteSentenceWidget(
            exercise.action as WriteWordToCompleteSentence);
      case ExerciseKind.constructSentence:
        return ConstructSentenceWidget(exercise.action as ConstructSentence);
      case ExerciseKind.enterCorrectWordForm:
      // TODO: Handle this case.
      case ExerciseKind.blank:
      // TODO: Handle this case.
    }
    return const SizedBox.shrink();
  }

  @override
  Widget build(BuildContext context) {
    super.build(context);
    return ListenableProvider<Validator>(
        create: (context) => Validator(),
        child: Padding(
            padding: const EdgeInsets.all(20),
            child: Column(
              children: [
                Container(
                  padding: const EdgeInsets.all(16.0),
                  alignment: Alignment.center,
                  child: Text(
                    'Question ${widget._index + 1}',
                    style: const TextStyle(
                        fontSize: 20, fontWeight: FontWeight.bold),
                  ),
                ),
                Text(
                  widget._exercise.title,
                  style: const TextStyle(fontSize: 20),
                ),
                Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: <Widget>[
                    _buildWidgetForExercise(widget._exercise),
                    if (widget._exercise.imageId != null)
                      Image(image: getNetworkImage(widget._exercise.imageId)),
                    Consumer<Validator>(
                      builder: (context, value, child) => Container(
                        padding: const EdgeInsets.all(16.0),
                        alignment: Alignment.center,
                        child: ElevatedButton(
                          onPressed: value.result == Result.empty
                              ? null
                              : () {
                                  context
                                      .read<_UnitScreenModel>()
                                      .completeExercise(
                                          context.read<UserProvider>().userId,
                                          widget._index);
                                  widget._submit(value.result);
                                },
                          child: const Text('Submit'),
                        ),
                      ),
                    )
                  ],
                ),
                const SizedBox(
                  height: 20,
                )
              ],
            )));
  }

  @override
  bool get wantKeepAlive => true;
}
