import 'package:grammar_guide_client/model/http/http.dart';
import 'package:grammar_guide_client/model/provider/current_guide.dart';
import 'package:grammar_guide_client/model/provider/theme_provider.dart';
import 'package:grammar_guide_client/model/provider/user_provider.dart';
import 'package:grammar_guide_client/view/screen/unit_group.dart';
import '../../model/all.dart';
import 'routes.dart';
import 'util.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

class UnitGroupListPage extends StatefulWidget {
  final String title = 'Grammar Guide';

  const UnitGroupListPage({super.key});

  @override
  State<UnitGroupListPage> createState() => _UnitGroupListPageState();
}

class _UnitGroupListPageState extends State<UnitGroupListPage> {
  @override
  Widget build(BuildContext context) {
    final currentGuide = context.read<CurrentGuide>();
    return ChangeNotifierProvider(
        create: (context) {
          final prov = _MainScreenModel();
          prov.fetchData(currentGuide.sourceId, currentGuide.destinationId);
          return prov;
        },
        child: Scaffold(
          appBar: AppBar(
            backgroundColor: getAppBarBackgroundColor(context),
            title: Consumer<_MainScreenModel>(
                builder: (context, value, child) => value.load
                    ? const Text("Loading your guide")
                    : Text(value.responseData.brief.title)),
            bottom: PreferredSize(
              preferredSize: const Size.fromHeight(10),
              child:
                  Consumer<_MainScreenModel>(builder: (context, value, child) {
                if (value.load) {
                  return const LinearProgressIndicator();
                } else {
                  return const SizedBox.shrink();
                }
              }),
            ),
          ),
          drawer: Drawer(
            child: ListView(
              children: [
                Consumer<UserProvider>(
                  builder: (context, value, child) => UserAccountsDrawerHeader(
                    accountName: const Text("Happy language learner"),
                    currentAccountPicture: const Icon(Icons.android),
                    decoration: BoxDecoration(
                      color: getAppBarBackgroundColor(context),
                    ), accountEmail: null,
                  ),
                ),
                ListTile(
                  leading: const Icon(Icons.home),
                  title: const Text("Home"),
                  onTap: () {
                    Navigator.pop(context);
                  },
                ),
                ListTile(
                  leading: const Icon(Icons.bookmarks),
                  title: const Text("Bookmarks"),
                  onTap: () {
                    Navigator.of(context).pushNamed(Routes.bookmarks);
                  },
                ),
                ListTile(
                  leading: const Icon(Icons.check_box_outlined),
                  title: const Text("Select guide"),
                  onTap: () {
                    Navigator.pop(context);
                    Navigator.pushNamed(context, Routes.guideSelection);
                  },
                ),
                Consumer<ThemeProvider>(
                  builder: (BuildContext context, ThemeProvider value,
                      Widget? child) {
                    return ListTile(
                      leading: Icon(value.currentIcon),
                      title: Text("Theme: ${value.mode.name}"),
                      onTap: () {
                        value.cycle();
                      },
                    );
                  },
                ),
              ],
            ),
          ),
          body: _MainList(),
        ));
  }
}

class _MainScreenModel with ChangeNotifier {
  bool _load = true;
  late Guide _responseData;

  _MainScreenModel();

  Guide get responseData => _responseData;

  bool get load => _load;

  void fetchData(String source, String destination) async {
    _responseData = await fetchGuide(source, destination);
    _load = false;
    notifyListeners();
  }
}

class _MainList extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Consumer<_MainScreenModel>(builder: (context, value, child) {
      if (value.load) {
        return const SizedBox.shrink();
      } else {
        final lst = ListView.builder(
          itemCount: value.responseData.unitGroups.length,
          itemBuilder: (context, index) {
            final topic = value.responseData.unitGroups[index];
            return ListTile(
              key: key,
              title: Text(topic.title),
              onTap: () {
                Navigator.of(context).pushNamed(
                  Routes.guideDetailed,
                  arguments: UnitGroupPageData(value.responseData.unitGroups,
                      index, value.responseData.brief.title),
                );
              },
            );
          },
        );
        return lst;
      }
    });
  }
}
