import 'package:flutter/material.dart';
import 'package:grammar_guide_client/model/all.dart';
import 'package:grammar_guide_client/model/http/http.dart';
import 'package:grammar_guide_client/model/provider/language_selection.dart';
import 'package:provider/provider.dart';

class LanguagePickWidget extends StatefulWidget {
  const LanguagePickWidget({super.key});

  @override
  State<LanguagePickWidget> createState() => _LanguagePickWidgetState();
}

class _LanguagePickWidgetState extends State<LanguagePickWidget> {
  Language? _srcLanguage;
  Language? _dstLanguage;

  @override
  Widget build(BuildContext context) {
    final langModel = context.read<LanguageSelectionProvider>();
    return Center(
        child: ChangeNotifierProvider<LanguageChooseModel>(
            create: (context) {
              final LanguageChooseModel model = LanguageChooseModel();
              model.loadData();
              return model;
            },
            child: Consumer<LanguageChooseModel>(
                builder: (context, value, child) => value.load
                    ? const LinearProgressIndicator()
                    : Column(
                        children: [
                          const Padding(
                              padding: EdgeInsets.all(8),
                              child: Text(
                                "Pick two languages",
                                style: TextStyle(fontSize: 26),
                              )),
                          const Padding(
                              padding: EdgeInsets.all(8),
                              child: Text(
                                "I know",
                                style: TextStyle(fontSize: 20),
                              )),
                          DropdownButton<Language>(
                              padding: const EdgeInsets.only(left: 5, right: 5),
                              value: _srcLanguage,
                              hint: const Text("pick language you know"),
                              onChanged: (nv) {
                                if (nv != null) {
                                  langModel.source = nv;
                                }
                                setState(() {
                                  _srcLanguage = nv;
                                });
                              },
                              items: value._languages.source
                                  .map((value) => DropdownMenuItem(
                                        value: value,
                                        child: Row(children: [
                                          Image(
                                              image: NetworkImage(
                                                  getFlagUrl(value.countryCode),
                                                  headers:
                                                      getRegularHeaders())),
                                          SizedBox.fromSize(
                                            size: const Size.fromWidth(10),
                                          ),
                                          Text(value.title)
                                        ]),
                                      ))
                                  .toList()),
                          const Padding(
                              padding: EdgeInsets.all(8),
                              child: Text(
                                "And I want to learn",
                                style: TextStyle(fontSize: 20),
                              )),
                          DropdownButton<Language>(
                              padding: const EdgeInsets.only(left: 5, right: 5),
                              value: _dstLanguage,
                              hint: const Text("pick language you learn"),
                              onChanged: (nv) {
                                if (nv != null) {
                                  langModel.destination = nv;
                                }
                                setState(() {
                                  _dstLanguage = nv;
                                });
                              },
                              items: value._languages.source
                                  .map((value) => DropdownMenuItem(
                                        value: value,
                                        child: Row(children: [
                                          Image(
                                              image: NetworkImage(
                                                  getFlagUrl(value.countryCode),
                                                  headers:
                                                      getRegularHeaders())),
                                          SizedBox.fromSize(
                                            size: const Size.fromWidth(10),
                                          ),
                                          Text(value.title)
                                        ]),
                                      ))
                                  .toList()),
                        ],
                      ))));
  }
}

class LanguageChooseModel with ChangeNotifier {
  bool _load = true;
  late LanguageResponse _languages;

  bool get load => _load;

  LanguageResponse get languages => _languages;

  void loadData() async {
    _languages = await fetchLanguages();
    _load = false;
    notifyListeners();
  }
}
